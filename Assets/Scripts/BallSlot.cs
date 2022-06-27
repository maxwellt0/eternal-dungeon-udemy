using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    private PathCreator pathCreator;
    private GameProperties gameProperties;

    public Ball ball;
    public float distanceTraveled;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        gameProperties = FindObjectOfType<GameProperties>();
    }
    
    void Update()
    {
        distanceTraveled += gameProperties.ballSlotsSpeed * Time.deltaTime;
        if (distanceTraveled > pathCreator.path.length)
        {
            distanceTraveled = 0;
        }
        transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
    }

    public void AssignBall(Ball newBall)
    {
        ball = newBall;
        if (ball)
        {
            ball.slot = this;
        }
    }
}
