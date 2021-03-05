using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;

public class Board : MonoBehaviour
{
    public BallSlot ballSlotPrefab;
    public Ball ballPrefab;
    public GameObject ballSlotsContainer;

    private PathCreator pathCreator;

    private BallSlot[] ballSlots;

    void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        
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
            Ball ball = Instantiate(ballPrefab, zeroSlot.transform);
            zeroSlot.ball = ball;
            ball.transform.localScale = Vector3.zero;
            ball.isSpawning = true;
        }
    }
}
