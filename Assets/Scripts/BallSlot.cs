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
    public float speedMultiplier = 1f;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        gameProperties = FindObjectOfType<GameProperties>();
    }
    
    void Update()
    {
        distanceTraveled += speedMultiplier * gameProperties.ballSlotsSpeed * Time.deltaTime;
        if (distanceTraveled > pathCreator.path.length)
        {
            distanceTraveled = 0;
        }
        
        if (speedMultiplier < 0 && distanceTraveled < 1f && ball)
        {
            if (distanceTraveled < 0.5f)
            {
                Destroy(ball.gameObject);
            }
            else
            {
                ball.StartDestroying();
            }
          
            AssignBall(null);
        }

        if (distanceTraveled < 0)
        {
            distanceTraveled = pathCreator.path.length;
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
