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

    public bool isDestroyingMatchingBalls;
    public bool isReverse;

    private PathCreator pathCreator;
    private BallFactory ballFactory;
    private GameProperties gameProperties;

    private BallSlot[] ballSlots;

    private void Start()
    {
        pathCreator = FindObjectOfType<PathCreator>();
        ballFactory = FindObjectOfType<BallFactory>();
        gameProperties = FindObjectOfType<GameProperties>();

        InitBallSlots();
    }

    private void InitBallSlots()
    {
        float pathLength = pathCreator.path.length;
        int slotsCount = (int)pathLength;
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
        if (isReverse)
        {
            return;
        }

        BallSlot zeroSlot = BallSlotsByDistance[0];
        if (zeroSlot.ball)
        {
            return;
        }

        Ball ball = ballFactory.CreateBallAt(zeroSlot.transform.position, ballFactory.GetRandomBallType());
        zeroSlot.AssignBall(ball);
        ball.PlaceInSlotTransform();
        ball.transform.localScale = Vector3.zero;
        ball.state = BallState.Spawning;
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
        isDestroyingMatchingBalls = true;

        List<BallSlot> ballsToDestroySlots;
        BallSlot collidedBallSlot = landedBallSlot;

        do
        {
            yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
                !bs.ball || bs.ball.state != BallState.Landing && bs.ball.state != BallState.SwitchingSlots));

            ballsToDestroySlots = GetSimilarBalls(collidedBallSlot);

            if (ballsToDestroySlots.Count < 3)
            {
                break;
            }

            AddBallsIfThereIsBomb(ballsToDestroySlots);
            if (ballsToDestroySlots.FindIndex(bs => bs.ball.type == BallType.Reverse) != -1)
            {
                StartCoroutine(StartReverseCo());
            }

            if (ballsToDestroySlots.FindIndex(bs => bs.ball.type == BallType.TimeSlow) != -1)
            {
                StartCoroutine(TimeSlowCo());
            }

            foreach (BallSlot ballsToDestroySlot in ballsToDestroySlots)
            {
                ballsToDestroySlot.ball.StartDestroying();
                ballsToDestroySlot.AssignBall(null);
            }

            collidedBallSlot = ballsToDestroySlots[0];

            yield return new WaitForSeconds(0.5f);

            MoveSeparatedBallsBack();
        } while (ballsToDestroySlots.Count >= 3 && collidedBallSlot);

        yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
            !bs.ball || bs.ball.state != BallState.SwitchingSlots));
        isDestroyingMatchingBalls = false;
    }

    private IEnumerator TimeSlowCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
                isDestroyingMatchingBalls == false
                && isReverse == false
                && (!bs.ball
                    || bs.ball.state != BallState.Landing
                    && bs.ball.state != BallState.SwitchingSlots)
            )
        );

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = 0.5f;
        }

        yield return new WaitForSeconds(gameProperties.timeSlowDuration);

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = 1;
        }
    }

    private IEnumerator StartReverseCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
                isDestroyingMatchingBalls == false
                && (!bs.ball
                    || bs.ball.state != BallState.Landing
                    && bs.ball.state != BallState.SwitchingSlots)
            )
        );

        isReverse = true;

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = -1;
        }

        yield return new WaitForSeconds(gameProperties.reverseDuration);

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = 1;
        }

        isReverse = false;
    }

    private void AddBallsIfThereIsBomb(List<BallSlot> ballsToDestroySlots)
    {
        BallSlot ballSlot = ballsToDestroySlots.FirstOrDefault(bs => bs.ball.type == BallType.Bomb);
        if (ballSlot)
        {
            int indexOfBombSlot = Array.IndexOf(BallSlotsByDistance, ballSlot);
            for (int i = 1; i <= gameProperties.bombRadius; i++)
            {
                int leftIndex = indexOfBombSlot - i;
                int rightIndex = indexOfBombSlot + i;
                if (leftIndex >= 0 && BallSlotsByDistance[leftIndex].ball &&
                    !ballsToDestroySlots.Contains(BallSlotsByDistance[leftIndex]))
                {
                    ballsToDestroySlots.Add(BallSlotsByDistance[leftIndex]);
                }

                if (rightIndex < BallSlotsByDistance.Length && BallSlotsByDistance[rightIndex].ball &&
                    !ballsToDestroySlots.Contains(BallSlotsByDistance[rightIndex]))
                {
                    ballsToDestroySlots.Add(BallSlotsByDistance[rightIndex]);
                }
            }
        }
    }

    private void MoveSeparatedBallsBack()
    {
        int firstEmptyIndex = Array.FindIndex(BallSlotsByDistance, 1, bs => !bs.ball);
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
        List<BallSlot> ballsToDestroySlots = new List<BallSlot> { landedBallSlot };

        if (!landedBallSlot.ball)
        {
            return ballsToDestroySlots;
        }

        int indexOfLandedBallSlot = Array.IndexOf(BallSlotsByDistance, landedBallSlot);

        for (int i = indexOfLandedBallSlot - 1; i >= 0; i--)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot)
                              && BallUtil.GetBallColorByType(ballSlot.ball.type) ==
                              BallUtil.GetBallColorByType(landedBallSlot.ball.type))
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
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot)
                              && BallUtil.GetBallColorByType(ballSlot.ball.type) ==
                              BallUtil.GetBallColorByType(landedBallSlot.ball.type))
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        return ballsToDestroySlots.OrderBy(bs => bs.distanceTraveled).ToList();
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