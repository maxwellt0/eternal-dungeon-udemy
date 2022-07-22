using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform shootPoint;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    
    private Camera mainCamera;
    private BallFactory ballFactory;
    private AudioManager audioManager;
    private Board board;
    
    private SpriteRenderer spriteRenderer;

    public Ball nextShootBall;
    public bool isShooterDisabledFromOutside;

    private void Start()
    {
        ballFactory = FindObjectOfType<BallFactory>();
        audioManager = FindObjectOfType<AudioManager>();
        board = FindObjectOfType<Board>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!board.isPaused)
        {
            FaceMouse();
            UpdateSprite();
        }

        if (!nextShootBall)
        {
            nextShootBall = ballFactory.CreateRandomBallAt(shootPoint.position);
            nextShootBall.state = BallState.SpawningToShoot;
            nextShootBall.transform.parent = shootPoint;
        }

        if (Input.GetMouseButtonUp(0) && !isShooterDisabledFromOutside && !board.isPaused)
        {
            audioManager.PlaySfx(2);
            ShootNextBall();
        }
    }

    public void UpdateSprite()
    {
        spriteRenderer.sprite = !isShooterDisabledFromOutside && IsNextBallReady ? activeSprite : inactiveSprite;
    }
    
    private void ShootNextBall()
    {
        if (!nextShootBall || nextShootBall.state != BallState.ReadyToShoot)
        {
            return;
        }
        
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

    private bool IsNextBallReady => nextShootBall && nextShootBall.state == BallState.ReadyToShoot;
}
