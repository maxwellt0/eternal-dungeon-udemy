using System;
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

    private void Start()
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

    private void Update()
    {
        ProduceBallsOnTrack();
    }

    private void ProduceBallsOnTrack()
    {
        BallSlot zeroSlot = BallSlotsByDistance[0];
        if (!zeroSlot.ball)
        {
            Ball ball = ballFactory.CreateRandomBallAt(zeroSlot.transform.position);
            zeroSlot.AssignBall(ball);
            ball.transform.parent = zeroSlot.transform;
            ball.transform.localScale = Vector3.zero;
            ball.state = BallState.Spawning;
        }
    }

    public void LandBall(BallSlot collidedSlot, Ball landingBall)
    {
        BallSlot[] ballSlotsByDistance = BallSlotsByDistance;
        int indexOfCollidedSlot = Array.IndexOf(ballSlotsByDistance, collidedSlot);

        int firstEmptySlotIndexAfter = FirstEmptySlotIndexAfter(indexOfCollidedSlot, ballSlotsByDistance);

        for (int i = firstEmptySlotIndexAfter; i > indexOfCollidedSlot + 1; i--)
        {
            ballSlotsByDistance[i].AssignBall(ballSlotsByDistance[i - 1].ball);
        }

        if (collidedSlot.distanceTraveled <
            pathCreator.path.GetClosestDistanceAlongPath(landingBall.transform.position))
        {
            ballSlotsByDistance[indexOfCollidedSlot + 1].AssignBall(landingBall);
        }
        else
        {
            ballSlotsByDistance[indexOfCollidedSlot + 1].AssignBall(collidedSlot.ball);
            collidedSlot.AssignBall(landingBall);
        }
        
        landingBall.Land();
        foreach (BallSlot ballSlot in ballSlotsByDistance
            .Where(bs => bs.ball && bs.ball.state == BallState.InSlot))
        {
            ballSlot.ball.MoveToSlot();
        }
        
        StartCoroutine(DestroyMatchingBallsCo(landingBall.slot));
    }
    
    private IEnumerator DestroyMatchingBallsCo(BallSlot landedBallSlot)
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
            !bs.ball || bs.ball.state != BallState.Landing && bs.ball.state != BallState.SwitchingSlots));

        List<BallSlot> ballsToDestroySlots = GetSimilarBalls(landedBallSlot);

        if (ballsToDestroySlots.Count >= 3)
        {
            foreach (BallSlot ballsToDestroySlot in ballsToDestroySlots)
            {
                ballsToDestroySlot.ball.StartDestroying();
                ballsToDestroySlot.AssignBall(null);
            }
            
            yield return new WaitForSeconds(0.5f);

            MoveSeparatedBallsBack();
        }
    }
    
    private void MoveSeparatedBallsBack()
    {
        int firstEmptyIndex = Array.FindIndex(BallSlotsByDistance, bs => !bs.ball);
        int firstNonEmptyIndexAfter = Array.FindIndex(BallSlotsByDistance, firstEmptyIndex, bs => bs.ball);
        int emptySlotsCount = firstNonEmptyIndexAfter - firstEmptyIndex;

        if (firstNonEmptyIndexAfter == -1 || firstEmptyIndex == -1)
        {
            return;
        }

        for (int i = firstEmptyIndex; i < BallSlotsByDistance.Length - emptySlotsCount; i++)
        {
            BallSlotsByDistance[i].AssignBall(BallSlotsByDistance[i + emptySlotsCount].ball);
            if (BallSlotsByDistance[i].ball)
            {
                BallSlotsByDistance[i].ball.MoveToSlot();
            }
        }

        for (int i = BallSlotsByDistance.Length - emptySlotsCount; i < BallSlotsByDistance.Length; i++)
        {
            BallSlotsByDistance[i].AssignBall(null);
        }
    }

    private List<BallSlot> GetSimilarBalls(BallSlot landedBallSlot)
    {
        List<BallSlot> ballsToDestroySlots = new List<BallSlot> {landedBallSlot};
        int indexOfLandedBallSlot = Array.IndexOf(BallSlotsByDistance, landedBallSlot);

        for (int i = indexOfLandedBallSlot - 1; i >= 0; i--)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot) && ballSlot.ball.type == landedBallSlot.ball.type)
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        for (int i = indexOfLandedBallSlot + 1; i < BallSlotsByDistance.Length; i++)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot) && ballSlot.ball.type == landedBallSlot.ball.type)
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        return ballsToDestroySlots;
    }

    private static int FirstEmptySlotIndexAfter(int indexOfCollidedSlot, BallSlot[] ballSlotsByDistance)
    {
        for (int i = indexOfCollidedSlot; i < ballSlotsByDistance.Length; i++)
        {
            if (!ballSlotsByDistance[i].ball)
            {
                return i;
            }
        }

        return -1;
    }

    public BallSlot[] BallSlotsByDistance => ballSlots.OrderBy(bs => bs.distanceTraveled).ToArray();
}
