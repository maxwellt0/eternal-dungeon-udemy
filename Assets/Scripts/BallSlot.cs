using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    private PathCreator pathCreator;

    public float distanceTraveled;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
    }
    
    void Update()
    {
        distanceTraveled += Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
    }
}
