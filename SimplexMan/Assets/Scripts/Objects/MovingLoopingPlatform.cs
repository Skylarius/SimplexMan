using UnityEngine;

public class MovingLoopingPlatform : RecordablePhysics {

    public int speed;
    public int maxDistance;
    
    private Vector3 startPosition;
    private int direction = 1;


    public override void Start() {
        base.Start();
        startPosition = transform.position;      
    }

    public override void Update() {
        base.Update();
    }

    void FixedUpdate() {
        transform.position+=transform.right*direction*speed*Time.deltaTime; 
        if (Vector3.Distance(startPosition, transform.position) > maxDistance) {
            direction*=-1;
        }
    }

    
}
