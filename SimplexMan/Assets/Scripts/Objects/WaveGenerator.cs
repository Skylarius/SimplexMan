// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class WaveGenerator : MonoBehaviour {

//     public GameObject hexWave;
//     public float secondsBetweenWaves;
//     public float secondsBetweenUnits;

//     [Header("Drop Wave")]
//     public int waveLength;
//     [Range(0, 1)]
//     public float dropProbability;

//     [Header("Wave Unit")]
//     public float waveHeight;
//     public float waveSpeed;

//     [Header("Flash Tile")]
//     public float flashTime;
//     public float flashSpeed;
//     public Color flashColor;

//     MapGenerator mapGenerator;
//     Vector2Int mapSize;
//     float currentTime;

//     Vector2Int xConstraint;
//     Vector2Int yConstraint;
    
//     void Start() {
//         mapGenerator = FindObjectOfType<MapGenerator>();
//         mapSize = mapGenerator.maps[mapGenerator.mapIndex].mapSize;
//         currentTime = Time.time;
//         secondsBetweenWaves -= flashTime;

//         xConstraint = new Vector2Int(waveLength, mapSize.x - waveLength);
//         yConstraint = new Vector2Int(waveLength, mapSize.y - waveLength);
//     }

//     void Update() {
//         if (Time.time - currentTime >= secondsBetweenWaves) {
//             currentTime = Time.time;
//             Vector2Int origin = mapGenerator.GetRandomOpenCoord(xConstraint, yConstraint);
//             if (Random.Range(0f, 1f) < dropProbability) {
//                 StartCoroutine(DropWave(origin));
//             } else {
//                 StartCoroutine(LongWave(origin, Random.Range(0, 6)));
//             }
//         }
//     }

//     IEnumerator DropWave (Vector2Int center) {
//         yield return StartCoroutine(FlashTile(center));
//         GameObject newWave = Instantiate(hexWave, 
//                                          mapGenerator.CoordToHexPosition(center), 
//                                          Quaternion.identity, 
//                                          transform);
//         StartCoroutine(WaveUnit(newWave.transform));
//         yield return new WaitForSeconds(secondsBetweenUnits);
        
//         for (int i = 0; i < waveLength; i++) {
//             Vector2Int[] neighbourhoodCoords = Utility.HexNeighbourhood(center, i+1);
//             for (int j = 0; j < neighbourhoodCoords.Length; j++) {
//                 newWave = Instantiate(hexWave, 
//                                       mapGenerator.CoordToHexPosition(neighbourhoodCoords[j]), 
//                                       Quaternion.identity, 
//                                       transform);
//                 StartCoroutine(WaveUnit(newWave.transform));
//             }
//             yield return new WaitForSeconds(secondsBetweenUnits);
//         }
//     }

//     IEnumerator LongWave (Vector2Int origin, int direction) {
//         yield return StartCoroutine(FlashTile(origin));
//         Vector2Int position = origin;
//         while (true) {
//             GameObject newHex = Instantiate(hexWave, 
//                                             mapGenerator.CoordToHexPosition(position), 
//                                             Quaternion.identity,
//                                             transform);
//             StartCoroutine(WaveUnit(newHex.transform));
//             if (IsBorder(position, mapSize)) {
//                 break;
//             }                           
//             position = Utility.HexNext(position, direction);
//             yield return new WaitForSeconds(secondsBetweenUnits);
//         }
//     }

//     IEnumerator WaveUnit(Transform hex) {
//         hex.localScale -= Vector3.up * hex.localScale.y;
//         Vector3 scale = hex.localScale;
//         float percent = 0;
//         float increment = 0;
//         while (percent <= 1) {
//             percent += Time.deltaTime * waveSpeed;
//             increment = - 4 * percent * (percent - 1);
//             scale.y = waveHeight * increment;
//             scale.y = Mathf.Clamp(scale.y, 0, waveHeight);
//             hex.localScale = scale;
//             yield return null;
//         }
//         Destroy(hex.gameObject);
//     }

//     IEnumerator FlashTile(Vector2Int tileCoords) {
//         Transform origin = mapGenerator.GetTileFromCoord(tileCoords);
//         Material tileMat = origin.GetComponentInChildren<Renderer>().material;
//         Color initialColor = tileMat.color;
//         float timer = 0;

//         while (timer < flashTime) {
//             tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(timer * flashSpeed, 1));
//             timer += Time.deltaTime;
//             yield return null;
//         }
//         tileMat.color = initialColor;
//     }

//     bool IsBorder(Vector2Int pos, Vector2Int mapSize) {
//         return (pos.x == 0 || pos.y == 0 || pos.x == mapSize.x - 1 || pos.y == mapSize.y - 1);
//     }
// }
