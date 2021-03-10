using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameProperties gameProperties;
    private Board board;
    private PathCreator pathCreator;
    
    private CircleCollider2D circleCollider2D;
    
    public BallSlot slot;
    public BallState state;
    public BallType type;
    private float upscaleCounter;
    private float downscaleCounter = 1f;
    private Vector3 shootDirection;
    private float distanceTraveled;

    private void Start()
    {
        gameProperties = FindObjectOfType<GameProperties>();
        board = FindObjectOfType<Board>();
        pathCreator = FindObjectOfType<PathCreator>();

        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = false;
    }

    private void Update()
    {
        switch (state)
        {
            case BallState.Spawning:
            {
                upscaleCounter += gameProperties.ballUpscaleSpeed * Time.deltaTime;
            
                if (upscaleCounter >= 1)
                {
                    state = BallState.InSlot;
                    return;
                }
            
                transform.localScale = Vector3.one * upscaleCounter;
                break;
            }
            case BallState.Destroying:
                downscaleCounter -= gameProperties.ballDownscaleSpeed * Time.deltaTime;
            
                if (downscaleCounter < 0)
                {
                    Destroy(gameObject);
                    return;
                }
            
                transform.localScale = Vector3.one * downscaleCounter;
                break;
            case BallState.Shooting:
                transform.position += shootDirection * (gameProperties.ballShootingSpeed * Time.deltaTime);
                break;
            case BallState.Landing:
                transform.position =
                    Vector3.MoveTowards(transform.position, slot.transform.position, 
                        gameProperties.ballLandingSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, slot.transform.position) < 0.1f)
                {
                    state = BallState.InSlot;
                    transform.position = slot.transform.position;
                    transform.parent = slot.transform;
                }
                break;
            case BallState.SwitchingSlots:
                int direction = distanceTraveled > slot.distanceTraveled ? -1 : 1;
                distanceTraveled += direction * gameProperties.ballSlotSwitchingSpeed * Time.deltaTime;

                transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
                if (Mathf.Abs(distanceTraveled - slot.distanceTraveled) < 0.1f)
                {
                    state = BallState.InSlot;
                    transform.position = slot.transform.position;
                    transform.parent = slot.transform;
                }
                break;
        }
    }
    
    public void Shoot(Vector3 direction)
    {
        shootDirection = direction;
        state = BallState.Shooting;
        circleCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball Slot"))
        {
            return;
        }
        BallSlot ballSlot = other.GetComponent<BallSlot>();

        if (!ballSlot.ball || state != BallState.Shooting)
        {
            return;
        }
        
        board.LandBall(ballSlot, this);
        circleCollider2D.enabled = false;
    }
    
    public void Land()
    {
        state = BallState.Landing;
    }

    public void MoveToSlot()
    {
        state = BallState.SwitchingSlots;
        distanceTraveled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
    
    public void StartDestroying()
    {
        state = BallState.Destroying;
    }
}
