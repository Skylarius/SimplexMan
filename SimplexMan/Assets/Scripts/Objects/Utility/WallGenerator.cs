using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {

    public enum Type {Line, HexLine, Hexagon};
    public Type type;
    public GameObject wallBlock;
    public int size;
    
    float tileSize = 4;

    public void GenerateWall() {
        GameObject holder = new GameObject("Generated Wall");
        if (type == Type.Line) {
            Vector2Int position = Vector2Int.zero;
            for (int i = 0; i < size; i++) {
                GenerateBlock(position, holder.transform);
                position.x++;
            }
        } else if (type == Type.HexLine) {
            Vector2Int position = Vector2Int.zero;
            for (int i = 0; i < size; i++) {
                GenerateBlock(position, holder.transform);
                position = Utility.HexNext(position, i%2);
            }
        } else {
            Vector2Int center = Vector2Int.zero;
            Vector2Int[] positions = Utility.HexNeighbourhood(center, size);
            foreach (Vector2Int position in positions) {
                GenerateBlock(position, holder.transform);
            }
        }
    }

    void GenerateBlock(Vector2Int position, Transform holder) {
        GameObject newBlock = Instantiate(wallBlock, 
                                        Utility.CoordToHexPosition(position, tileSize) + transform.position, 
                                        transform.rotation);
        newBlock.transform.parent = holder.transform;
    }

}
