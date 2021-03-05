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
        }
    }
}
