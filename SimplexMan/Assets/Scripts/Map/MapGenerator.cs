using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public int seed;
    public string mapName = "generated Map";

    [Header("Map size")]
    public Vector2Int mapSize;
    public int sectorSize;
    public float tileSize = 1;

    [Header("Floor")]
    [Range(0, 1)]
    public float outlinePercent;
    public bool interactiveFloor;

    [Header("Obstacles")]
    [Range(0, 1)]
    public float obstaclePercent;
    public float minObstacleHeight;
    public float maxObstacleHeight;
    public Color obstacleColor1;
    public Color obstacleColor2;
    public float monitorOffProbability;
    public float monitorOnProbability;

    [Header("Border and walls")]
    [Range(0, 1)]
    public float wallPercent;
    public Vector2 wallsHeightRange;
    public int borderLayers;
    public float borderLayerOffset;
    public Vector2 bordersHeightRange;

    [Header("Prefabs")]
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Material underFloorMat;
    public GameObject monitorOn;
    public GameObject monitorOff;

    Transform[,] sectorsMap;
    List<Vector2Int> sectorsCoords;
    Queue<Vector2Int> shuffledSectorsCoords;
    System.Random prng;

    public void GenerateMap() {
        prng = new System.Random(seed);
        sectorsMap = new Transform[mapSize.x, mapSize.y];

        // Generating coordinates for all the sectors
        sectorsCoords = new List<Vector2Int>();
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                sectorsCoords.Add(new Vector2Int(x, y));
            }
        }
        shuffledSectorsCoords = new Queue<Vector2Int>(Utility.ShuffleArray(sectorsCoords.ToArray(), seed));

        // Creating Map Holders        
        if (GameObject.Find(mapName)) {
            DestroyImmediate(GameObject.Find(mapName));
        }
        Transform mapHolder = new GameObject(mapName).transform;
        Transform floorHolder = new GameObject("Floor").transform;
        floorHolder.parent = mapHolder;
        Transform wallsHolder = new GameObject("Walls").transform;
        wallsHolder.parent = mapHolder;
        Transform obstaclesHolder = new GameObject("Obstacles").transform;
        obstaclesHolder.parent = mapHolder;
        Transform borderHolder = new GameObject("Border").transform;
        borderHolder.parent = mapHolder;
        Transform roofHolder = new GameObject("Roof").transform;
        roofHolder.parent = mapHolder;
        Transform floraHolder = new GameObject("Flora").transform;
        floraHolder.parent = mapHolder;

        // Generate wall sectors
        bool[,] wallSectorsMap = new bool[mapSize.x, mapSize.y];

        int wallSectorsCount = (int)(mapSize.x * mapSize.y * wallPercent);
        int currentWallSectorsCount = 0;

        List<Vector2Int> freeSectorsCoords = new List<Vector2Int>(sectorsCoords);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize.x * sectorSize, 
                                                   mapSize.y * sectorSize,
                                                   seed,
                                                   10,
                                                   8,
                                                   0.8f,
                                                   4,
                                                   Vector2.one);

        for (int i = 0; i < wallSectorsCount; i++) {
            Vector2Int randomCoord = GetRandomSectorCoord();
            wallSectorsMap[randomCoord.x, randomCoord.y] = true;
            currentWallSectorsCount++;

            if (randomCoord != Vector2Int.zero && Utility.FloodFill(wallSectorsMap, currentWallSectorsCount, mapSize.x*mapSize.y, Vector2Int.zero)) {
                GenerateWallSector(noiseMap, randomCoord, wallsHolder);
                freeSectorsCoords.Remove(randomCoord);
            } else {
                wallSectorsMap[randomCoord.x, randomCoord.y] = false;
                currentWallSectorsCount--;
            }
        }

        // Generate free map
        List<Vector2Int> freeTilesCoords = new List<Vector2Int>();
        for (int i = 0; i < freeSectorsCoords.Count; i++) {
            for (int x = 0; x < sectorSize; x++) {
                for (int y = 0; y < sectorSize; y++) {
                    freeTilesCoords.Add(new Vector2Int(x, y) + freeSectorsCoords[i] * sectorSize);
                }
            }
        }
        GenerateFreeMap(freeTilesCoords, floorHolder, obstaclesHolder, floraHolder);

        GenerateBorder(borderHolder);

        //GenerateRoof(roofHolder);
    }

    void GenerateRoof(Transform holder) {
        Vector2Int absoluteMapSize = mapSize * sectorSize;
        for (int x = 0; x < absoluteMapSize.x; x++) {
            for (int y = 0; y < absoluteMapSize.y; y++) {
                Vector3 roofTilePosition = Utility.CoordToHexPosition(new Vector2Int(x, y), tileSize);
                roofTilePosition += Vector3.up * wallsHeightRange.x;
                Transform roofTile = Instantiate(obstaclePrefab, 
                                                roofTilePosition,// + Vector3.up * obstacleHeight / 2, 
                                                Quaternion.identity) as Transform;
                roofTile.parent = holder;
                roofTile.localScale = new Vector3(tileSize, 
                                                 0.1f, 
                                                 tileSize);
                Renderer obstacleRenderer = roofTile.GetComponentInChildren<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = RandomRange(0f, 1);//randomCoord.y / (float) currentMap.sectorSize.y;
                obstacleMaterial.color = Color.Lerp(obstacleColor1, obstacleColor2, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
        }

    }

    void GenerateBorder(Transform holder) {
        Vector2Int absoluteMapSize = mapSize * sectorSize;

        if (borderLayers > 0) {
            for (int b = 1; b <= borderLayers; b++) {
                for (int x = -b; x < absoluteMapSize.x + b; x++) {
                    for (int y = -b; y < absoluteMapSize.y + b; y++) {
                        if (x == -b || x == absoluteMapSize.x - 1 + b || y == -b || y >= absoluteMapSize.y - 1 + b) {
                            Vector3 obstaclePosition = Utility.CoordToHexPosition(new Vector2Int(x, y), tileSize);
                            Transform newObstacle = Instantiate(obstaclePrefab, 
                                                                obstaclePosition,// + Vector3.up * obstacleHeight / 2, 
                                                                Quaternion.identity) as Transform;
                            newObstacle.parent = holder;
                            newObstacle.localScale = new Vector3(tileSize, 
                                                                 RandomRange(bordersHeightRange.x, bordersHeightRange.y) + borderLayerOffset * (b-1), 
                                                                 tileSize);

                            Renderer obstacleRenderer = newObstacle.GetComponentInChildren<Renderer>();
                            Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                            float colorPercent = RandomRange(0f, 1);//randomCoord.y / (float) currentMap.sectorSize.y;
                            obstacleMaterial.color = Color.Lerp(obstacleColor1, obstacleColor2, colorPercent);
                            obstacleRenderer.sharedMaterial = obstacleMaterial;
                        }
                    }
                }
            }
        }
    }

    Vector2Int GetRandomSectorCoord() {
        Vector2Int sectorCoord = shuffledSectorsCoords.Dequeue();
        shuffledSectorsCoords.Enqueue(sectorCoord);
        return sectorCoord;
    }

    void GenerateWallSector(float[,] noiseMap, Vector2Int sectorCoord, Transform holder) {
        for (int x = 0; x < sectorSize; x++) {
            for (int y = 0; y < sectorSize; y++) {
                Vector2Int absoluteCoord = new Vector2Int(x, y) + sectorCoord * sectorSize;
                Vector3 obstaclePosition = Utility.CoordToHexPosition(absoluteCoord, tileSize);
                Transform newObstacle = Instantiate(obstaclePrefab, 
                                                    obstaclePosition,// + Vector3.up * obstacleHeight / 2, 
                                                    Quaternion.identity) as Transform;
                newObstacle.parent = holder;
                float yScale = wallsHeightRange.x + noiseMap[absoluteCoord.x, absoluteCoord.y] * (wallsHeightRange.y - wallsHeightRange.x);
                newObstacle.localScale = new Vector3(tileSize, 
                                                     yScale, //RandomRange(wallsHeightRange.x, wallsHeightRange.y), 
                                                     tileSize);
                if (noiseMap[absoluteCoord.x, absoluteCoord.y] < 0) {
                    print(noiseMap[absoluteCoord.x, absoluteCoord.y]);
                }

                Renderer obstacleRenderer = newObstacle.GetComponentInChildren<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                obstacleMaterial.color = Color.Lerp(obstacleColor1, obstacleColor2, RandomRange(0, 1));
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
        }
    }

    float RandomRange(float min, float max) {
        return (float)prng.Next((int) min * 100, (int) max * 100) / 100;
    }

    public void GenerateFreeMap(List<Vector2Int> freeTilesCoords, Transform floorHolder, Transform obstaclesHolder, Transform floraHolder) {

        // List<Vector2Int> allTileCoords;
        Queue<Vector2Int> shuffledTileCoords;
        // Queue<Vector2Int> shuffledOpenTileCoords;
        // Transform[,] tileMap;
        

        //tileMap = new Transform[currentMap.sectorSize.x, currentMap.sectorSize.y];

        shuffledTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(freeTilesCoords.ToArray(), seed));

        // Spawning Obstacles
        bool[,] obstacleMap = new bool[sectorSize * mapSize.x, sectorSize * mapSize.y];
        for (int x = 0; x < sectorSize * mapSize.x; x++) {
            for (int y = 0; y < sectorSize * mapSize.y; y++) {
                obstacleMap[x, y] = true;                
            }
        }

        for (int i = 0; i < freeTilesCoords.Count; i++) {
            obstacleMap[freeTilesCoords[i].x, freeTilesCoords[i].y] = false;
        }

        int currentObstacleCount = sectorSize * mapSize.x * sectorSize * mapSize.y - freeTilesCoords.Count;
        int obstacleCount = (int)(freeTilesCoords.Count * obstaclePercent);

        List<Vector2Int> floorCoords = new List<Vector2Int>(freeTilesCoords);

        for (int i = 0; i < obstacleCount; i++) {
            Vector2Int randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != Vector2Int.zero && Utility.FloodFill(obstacleMap, currentObstacleCount, obstacleMap.GetLength(0) * obstacleMap.GetLength(1), Vector2Int.zero)) {
                float obstacleHeight = Mathf.Lerp(minObstacleHeight, maxObstacleHeight, (float) prng.NextDouble());
                Vector3 obstaclePosition = Utility.CoordToHexPosition(randomCoord, tileSize);

                Transform newObstacle = Instantiate(obstaclePrefab, 
                                                    obstaclePosition,// + Vector3.up * obstacleHeight / 2, 
                                                    Quaternion.identity) as Transform;
                newObstacle.parent = obstaclesHolder;
                //newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
                newObstacle.localScale = new Vector3(tileSize, obstacleHeight, tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponentInChildren<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = RandomRange(0, 1);//randomCoord.y / (float) currentMap.sectorSize.y;
                obstacleMaterial.color = Color.Lerp(obstacleColor1, obstacleColor2, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                float incentive = 0;
                for (int a = 0; a < 3; a++) {
                    float p = RandomRange(0, 1) - incentive;
                    if (p < monitorOnProbability) {
                        Transform newFlora = Instantiate(monitorOn, 
                                                        obstaclePosition + Vector3.up * obstacleHeight, 
                                                        Quaternion.Euler(new Vector3(0, 120*a, 0))).transform;
                        newFlora.parent = floraHolder;
                        incentive += 0.2f;
                    } else if (p < monitorOffProbability) {
                        Transform newFlora = Instantiate(monitorOff, 
                                                        obstaclePosition + Vector3.up * obstacleHeight, 
                                                        Quaternion.Euler(new Vector3(0, 120*a, 0))).transform;
                        newFlora.parent = floraHolder;
                        incentive += 0.2f;
                    }
                }

                floorCoords.Remove(randomCoord);

            } else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        //shuffledOpenTileCoords = new Queue<Vector2Int>(Utility.ShuffleArray(floorCoords.ToArray(), seed));

        // Spawning Tiles
        for (int i = 0; i < floorCoords.Count; i++) {
            Vector3 tilePosition = Utility.CoordToHexPosition(floorCoords[i], tileSize);
            Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as Transform;
            newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            newTile.parent = floorHolder;
            //tileMap[x, y] = newTile;
            if (!interactiveFloor) {
                DestroyImmediate(newTile.GetComponentInChildren<FloorTile>());
            }
            
            // Spawn under floor
            Transform underTile = Instantiate(tilePrefab, tilePosition + Vector3.down * 0.05f, Quaternion.identity) as Transform;
            underTile.localScale = Vector3.one * tileSize;
            underTile.parent = newTile;
            underTile.GetComponentInChildren<Renderer>().sharedMaterial = underFloorMat;
        }

        Vector2Int GetRandomCoord() {
            Vector2Int randomCoord = shuffledTileCoords.Dequeue();
            shuffledTileCoords.Enqueue(randomCoord);
            return randomCoord;
        }        
    }

    // public Vector3 CoordToHexPosition(int x, int y) {
    //     return new Vector3(Mathf.Sqrt(3) * (-currentMap.sectorSize.x/2 + x + (y%2==0?0:0.5f)), 
    //                        0, 
    //                        1.5f * (-currentMap.sectorSize.y/2 + y)) * tileSize;
    // }

    // public Vector3 CoordToHexPosition(Vector2Int coord) {
    //     return new Vector3(Mathf.Sqrt(3) * (-currentMap.sectorSize.x/2 + coord.x + (coord.y%2==0?0:0.5f)), 
    //                        0, 
    //                        1.5f * (-currentMap.sectorSize.y/2 + coord.y)) * tileSize;
    // }

    

    // public Vector2Int GetRandomOpenCoord(Vector2Int xConstraint, Vector2Int yConstraint) {
    //     Vector2Int randomCoord = shuffledOpenTileCoords.Dequeue();
    //     while (randomCoord.x < xConstraint.x || randomCoord.x >= xConstraint.y ||
    //            randomCoord.y < yConstraint.x || randomCoord.y >= yConstraint.y) {
    //         shuffledOpenTileCoords.Enqueue(randomCoord);
    //         randomCoord = shuffledOpenTileCoords.Dequeue();
    //     }
    //     shuffledOpenTileCoords.Enqueue(randomCoord);
    //     return randomCoord;
    // }

    // public Transform GetTileFromCoord(Vector2Int coord) {
    //     return tileMap[coord.x, coord.y];
    // }
}