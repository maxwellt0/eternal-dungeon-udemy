using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    private PathCreator pathCreator;

    public Ball ball;
    public float distanceTraveled;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
    }
    
    void Update()
    {
        distanceTraveled += Time.deltaTime;
        if (distanceTraveled > pathCreator.path.length)
        {
            distanceTraveled = 0;
        }
        transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
    }
}
