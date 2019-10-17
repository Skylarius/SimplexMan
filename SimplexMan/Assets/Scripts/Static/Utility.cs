using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

    public static T[] ShuffleArray<T> (T[] array, int seed) {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++) {
            int randomIndex = prng.Next(i, array.Length);
            T tmp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tmp;
        }

        return array;
    }

    public static bool FloodFill(bool[,] obstacleMap, int currentObstacleCount, int freeTilesCount, Vector2Int drop) {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        
        queue.Enqueue(drop);
        mapFlags[drop.x, drop.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Vector2Int tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0) {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Vector2Int(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = freeTilesCount - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public static Vector2Int HexNext(Vector2Int coords, int direction) {
        switch(direction) {
            case 0:
                return coords + new Vector2Int(+coords.y%2, +1);
            case 1:
                return coords + new Vector2Int(+1, 0);
            case 2:
                return coords + new Vector2Int(+coords.y%2, -1);
            case 3:
                return coords + new Vector2Int(-1+coords.y%2, -1);
            case 4:
                return coords + new Vector2Int(-1, 0);
            case 5:
                return coords + new Vector2Int(-1+coords.y%2, +1);
            default:
                return coords;
        }
    }

    public static Vector2Int[] HexNeighbourhood(Vector2Int center, int distance) {
        Vector2Int[] neighbourhood = new Vector2Int[distance * 6];
        Vector2Int position = center;

        for (int i = 0; i < distance; i++) {
            position = HexNext(position, 4);
        }
        
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < distance; j++) {
                neighbourhood[i*distance+j] = HexNext(position, i);
                position = neighbourhood[i*distance+j];
            }
        }

        return neighbourhood;
    }
}