using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameProperties gameProperties;
    
    public BallState state;
    private float upscaleCounter;
    private float downscaleCounter = 1f;
    private Vector3 shootDirection;

    private void Start()
    {
        gameProperties = FindObjectOfType<GameProperties>();
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
        }
    }

    public void Shoot(Vector3 direction)
    {
        shootDirection = direction;
        state = BallState.Shooting;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball Slot"))
        {
            BallSlot ballSlot = other.GetComponent<BallSlot>();

            if (ballSlot.ball && state == BallState.Shooting)
            {
                Debug.Log("Boo!!");
            }
        }
    }
}
