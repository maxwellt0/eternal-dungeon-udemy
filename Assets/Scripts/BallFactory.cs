using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    public Ball ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Ball CreateBallAt(Vector3 point)
    {
        Ball ball = Instantiate(ballPrefab, point, Quaternion.identity);

        return ball;
    }
}
