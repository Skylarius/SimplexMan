using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Material underFloorMat;

    [Range(0, 1)]
    public float outlinePercent;

    public float tileSize = 1;
    public int borderLayers;
    public Vector2 borderHeightRange;
    public float borderLayerOffset;
    public bool interactiveFloor;

    List<Vector2Int> allTileCoords;
    Queue<Vector2Int> shuffledTileCoords;
    Queue<Vector2Int> shuffledOpenTileCoords;
    Transform[,] tileMap;

    Map currentMap;
    Transform player;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);

        // Generating coordinates
        allTileCoords = new List<Vector2Int>();
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                allTileCoords.Add(new Vector2Int(x, y));
            }
        }
        shuffledTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Creating Map Holder
        string holderName = "generated Map";
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        Transform floorHolder = new GameObject("Ground").transform;
        floorHolder.parent = mapHolder;
        Transform borderHolder = new GameObject("Border").transform;
        borderHolder.parent = mapHolder;
        Transform obstaclesHolder = new GameObject("Obstacles").transform;
        obstaclesHolder.parent = mapHolder;

        // Spawning Tiles
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                Vector3 tilePosition = CoordToHexPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = floorHolder;
                tileMap[x, y] = newTile;
                if (!interactiveFloor) {
                    DestroyImmediate(newTile.GetComponentInChildren<FloorTile>());
                }
                
                // Spawn under floor
                Transform underTile = Instantiate(tilePrefab, tilePosition + Vector3.down * 0.05f, Quaternion.identity) as Transform;
                underTile.localScale = Vector3.one * tileSize;
                underTile.parent = newTile;
                underTile.GetComponentInChildren<Renderer>().sharedMaterial = underFloorMat;
            }
        }

        // Spawning Border
        if (borderLayers > 0) {
            for (int b = 1; b <= borderLayers; b++) {
                for (int x = -b; x < currentMap.mapSize.x + b; x++) {
                    for (int y = -b; y < currentMap.mapSize.y + b; y++) {
                        if (x == -b || x == currentMap.mapSize.x - 1 + b || y == -b || y >= currentMap.mapSize.y - 1 + b) {
                            Vector3 obstaclePosition = CoordToHexPosition(x, y);
                            Transform newObstacle = Instantiate(obstaclePrefab, 
                                                                obstaclePosition,// + Vector3.up * obstacleHeight / 2, 
                                                                Quaternion.identity) as Transform;
                            newObstacle.parent = borderHolder;
                            newObstacle.localScale = new Vector3(tileSize, 
                                                                 Random.Range(borderHeightRange.x, borderHeightRange.y) + borderLayerOffset * (b-1), 
                                                                 tileSize);

                            Renderer obstacleRenderer = newObstacle.GetComponentInChildren<Renderer>();
                            Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                            float colorPercent = Random.Range(0f, 1);//randomCoord.y / (float) currentMap.mapSize.y;
                            obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                            obstacleRenderer.sharedMaterial = obstacleMaterial;
                        }
                    }
                }
            }
        }

        // Spawning Obstacles
        bool[,] obstacleMap = new bool[(int) currentMap.mapSize.x, (int) currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        List<Vector2Int> allOpenCoords = new List<Vector2Int>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++) {
            Vector2Int randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCenter && Utility.FloodFill(obstacleMap, currentObstacleCount, currentMap.mapSize)) {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) prng.NextDouble());
                Vector3 obstaclePosition = CoordToHexPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, 
                                                    obstaclePosition,// + Vector3.up * obstacleHeight / 2, 
                                                    Quaternion.identity) as Transform;
                newObstacle.parent = obstaclesHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponentInChildren<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = Random.Range(0f, 1);//randomCoord.y / (float) currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);

            } else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
    }

    public Vector3 CoordToHexPosition(int x, int y) {
        return new Vector3(Mathf.Sqrt(3) * (-currentMap.mapSize.x/2 + x + (y%2==0?0:0.5f)), 
                           0, 
                           1.5f * (-currentMap.mapSize.y/2 + y)) * tileSize;
    }

    public Vector3 CoordToHexPosition(Vector2Int coord) {
        return new Vector3(Mathf.Sqrt(3) * (-currentMap.mapSize.x/2 + coord.x + (coord.y%2==0?0:0.5f)), 
                           0, 
                           1.5f * (-currentMap.mapSize.y/2 + coord.y)) * tileSize;
    }

    public Vector2Int GetRandomCoord() {
        Vector2Int randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Vector2Int GetRandomOpenCoord(Vector2Int xConstraint, Vector2Int yConstraint) {
        Vector2Int randomCoord = shuffledOpenTileCoords.Dequeue();
        while (randomCoord.x < xConstraint.x || randomCoord.x >= xConstraint.y ||
               randomCoord.y < yConstraint.x || randomCoord.y >= yConstraint.y) {
            shuffledOpenTileCoords.Enqueue(randomCoord);
            randomCoord = shuffledOpenTileCoords.Dequeue();
        }
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetTileFromCoord(Vector2Int coord) {
        return tileMap[coord.x, coord.y];
    }

    [System.Serializable]
    public class Map {

        public Vector2Int mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;


        public Vector2Int mapCenter {
            get {
                return new Vector2Int(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }

}