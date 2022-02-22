using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;

public class Board : MonoBehaviour
{
    public BallSlot ballSlotPrefab;
    public GameObject ballSlotsContainer;

    private PathCreator pathCreator;
    private BallFactory ballFactory;

    private BallSlot[] ballSlots;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        ballFactory = FindObjectOfType<BallFactory>();
        
        InitBallSlots();
    }

    private void InitBallSlots()
    {
        float pathLength = pathCreator.path.length;
        int slotsCount = (int) pathLength;
        float step = pathLength / slotsCount;
        ballSlots = new BallSlot[slotsCount];

        for (int i = 0; i < slotsCount; i++)
        {
            float distanceTraveled = i * step;
            Vector3 slotPos = pathCreator.path.GetPointAtDistance(distanceTraveled);
            BallSlot ballSlot = Instantiate(ballSlotPrefab, slotPos, Quaternion.identity);
            ballSlot.distanceTraveled = distanceTraveled;
            ballSlot.transform.parent = ballSlotsContainer.transform;
            ballSlots[i] = ballSlot;
        }
    }
    
    void Update()
    {
        BallSlot zeroSlot = ballSlots.OrderBy(bs => bs.distanceTraveled).ToArray()[0];
        if (!zeroSlot.ball)
        {
            Ball ball = ballFactory.CreateRandomBallAt(zeroSlot.transform.position);
            zeroSlot.ball = ball;
            ball.transform.parent = zeroSlot.transform;
            ball.transform.localScale = Vector3.zero;
            ball.state = BallState.Spawning;
        }
    }
}
