using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameProperties gameProperties;
    
    public bool isSpawning;
    private float upscaleCounter;

    private void Start()
    {
        gameProperties = FindObjectOfType<GameProperties>();
    }

    private void Update()
    {
        if (isSpawning)
        {
            upscaleCounter += gameProperties.ballUpscaleSpeed * Time.deltaTime;
            
            if (upscaleCounter >= 1)
            {
                isSpawning = false;
                return;
            }
            
            transform.localScale = Vector3.one * upscaleCounter;
        }
    }
}
