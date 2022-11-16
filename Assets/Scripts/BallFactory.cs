using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallFactory : MonoBehaviour
{
    private static readonly BallType[] Colors = new[]
    {
        BallType.Red,
        BallType.Green,
        BallType.Blue
    };
    
    public Ball ballPrefab;

    public Sprite redSprite;
    public Sprite greenSprite;
    public Sprite blueSprite;
    
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
        return CreateBallAt(point, GetRandomBallType());
    }

    private BallType GetRandomBallType()
    {
        return Colors[Random.Range(0, Colors.Length)];
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
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}