using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallFactory : MonoBehaviour
{
    private static readonly BallType[] Colors =
    {
        BallType.Red,
        BallType.Green,
        BallType.Blue
    };
    
    private static readonly BallType[] SpecialTypes =
    {
        BallType.Bomb,
        BallType.Reverse,
        BallType.TimeSlow,
    };
    
    public Ball ballPrefab;
    public Sprite redSprite;
    public Sprite greenSprite;
    public Sprite blueSprite;
    public Sprite bombSprite;
    public Sprite reverseSprite;
    public Sprite timeSlowSprite;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public Ball CreateBallAt(Vector3 point, BallType ballType)
    {
        Ball ball = Instantiate(ballPrefab, point, Quaternion.identity);
        ball.type = ballType;
        SpriteRenderer spriteRenderer = ball.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetSpriteByType(ballType);

        return ball;
    }

    public Ball CreateRandomBallAt(Vector3 point)
    {
        return CreateBallAt(point, GetRandomBallColor());
    }

    private BallType GetRandomBallColor()
    {
        return Colors[Random.Range(0, Colors.Length)];
    }
    
    private BallType GetRandomBallSpecialType()
    {
        return SpecialTypes[Random.Range(0, SpecialTypes.Length)];
    }
    
    public BallType GetRandomBallType()
    {
        return Random.Range(0f, 1f) > 0.2f ? GetRandomBallColor() : GetRandomBallSpecialType();
    }
    
    private Sprite GetSpriteByType(BallType type)
    {
        switch (type)
        {
            case BallType.Red:
                return redSprite;
            case BallType.Green:
                return greenSprite;
            case BallType.Blue:
                return blueSprite;
            case BallType.Bomb:
                return bombSprite;
            case BallType.Reverse:
                return reverseSprite;
            case BallType.TimeSlow:
                return timeSlowSprite;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}