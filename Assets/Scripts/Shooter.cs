using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform shootPoint;
    
    private Camera mainCamera;
    private BallFactory ballFactory;
    private Board board;

    public Ball nextShootBall;

    private void Start()
    {
        ballFactory = FindObjectOfType<BallFactory>();
        board = FindObjectOfType<Board>();
        
        mainCamera = Camera.main;
    }

    private void Update()
    {
        FaceMouse();

        if (!nextShootBall)
        {
            nextShootBall = ballFactory.CreateRandomBallAt(shootPoint.position);
            nextShootBall.transform.parent = shootPoint;
        }

        if (Input.GetMouseButtonDown(0) 
            && !board.isDestroyingMatchingBalls && !board.isReverse)
        {
            ShootNextBall();
        }
    }
    
    private void ShootNextBall()
    {
        Vector3 shootDirection = (GetMousePos() - transform.position).normalized;
        
        nextShootBall.Shoot(shootDirection);
        nextShootBall.transform.parent = null;
        nextShootBall = null;
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
