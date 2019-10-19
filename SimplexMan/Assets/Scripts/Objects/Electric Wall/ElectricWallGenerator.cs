using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWallGenerator : MonoBehaviour {

    public GameObject wallBlock;
    public int wallSize;
    public float tileSize = 4;

    public void GenerateWall() {
        GameObject holder = new GameObject("Electric Wall");
        holder.transform.position = transform.position;
        Vector2Int position = Vector2Int.zero;
        for (int i = 0; i < wallSize; i++) {
            GameObject newBlock = Instantiate(wallBlock, 
                                              Utility.CoordToHexPosition(position, tileSize) + transform.position, 
                                              transform.rotation);
            newBlock.transform.parent = holder.transform;
            position.x++;
        }
    }

}
