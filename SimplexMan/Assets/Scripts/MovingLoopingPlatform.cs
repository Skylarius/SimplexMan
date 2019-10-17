using UnityEngine;

public class MovingLoopingPlatform : MonoBehaviour {

    public int speed;
    public int maxDistance;
    
    private Vector3 startPosition;
    private short int direction = 1;


    void Start() {
        speed = (speed) ? speed : 10;
        maxDistance = (maxDistance) ? maxDistance : 100;
        startPosition = transform.position;      
    }

    void FixedUpdate() {
        transform.position+=transform.forward*direction*speed*Time.deltaTime; 
        if (Vector3.Distance(startPosition, transform.position) > maxDistance) {
            direction*=-1;
        }
    }

    
}
