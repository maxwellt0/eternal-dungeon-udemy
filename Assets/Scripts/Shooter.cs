﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private Camera mainCamera;
    private BallFactory ballFactory;

    public Ball nextShootBall;
    
    void Start()
    {
        ballFactory = FindObjectOfType<BallFactory>();
        
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        FaceMouse();

        if (!nextShootBall)
        {
            nextShootBall = ballFactory.CreateBallAt(transform.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 shootDirection = (GetMousePos() - transform.position).normalized;
            
            nextShootBall.Shoot(shootDirection);
            nextShootBall = null;
        }
    }

    private void FaceMouse()
    {
        Vector3 mousePos = GetMousePos();
        Vector3 direction = mousePos - transform.position;

        transform.up = new Vector2(direction.x, direction.y);
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        return mousePos;
    }
}
