using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    private PathCreator pathCreator;
    private GameProperties gameProperties;
    private BallFactory ballFactory;

    public Ball ball;
    public float distanceTraveled;
    public float speedMultiplier = 1;

    private void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        gameProperties = FindObjectOfType<GameProperties>();
        ballFactory = FindObjectOfType<BallFactory>();
    }
    
    private void Update()
    {
        distanceTraveled += speedMultiplier * gameProperties.ballSlotsSpeed * Time.deltaTime;

        if (speedMultiplier < 0 && distanceTraveled < 1f && ball)
        {
            ballFactory.AddTypeToStack(ball.type);
            DestroyBall(distanceTraveled < 0.5f);
        }
        
        TrimDistanceTraveled();

        transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
        
        LookTowardsPathDirection();
    }

    private void LookTowardsPathDirection()
    {
        Vector3 bPos = pathCreator.path.GetPointAtDistance(distanceTraveled);
        Vector3 aPos = pathCreator.path.GetPointAtDistance(distanceTraveled - 0.1f);
        Vector3 lookDirection = aPos - bPos;
        transform.up = new Vector2(lookDirection.x, lookDirection.y);
    }

    private void TrimDistanceTraveled()
    {
        if (distanceTraveled > pathCreator.path.length)
        {
            distanceTraveled = 0;
        }
        else if (distanceTraveled < 0)
        {
            distanceTraveled = pathCreator.path.length;
        }
    }

    private void DestroyBall(bool immediately)
    {
        if (immediately)
        {
            Destroy(ball.gameObject);
        }
        else
        {
            ball.StartDestroying();
        }
        
        AssignBall(null);
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
