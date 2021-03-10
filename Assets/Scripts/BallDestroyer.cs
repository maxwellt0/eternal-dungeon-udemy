﻿using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball Slot"))
        {
            return;
        }

        BallSlot ballSlot = other.GetComponent<BallSlot>();

        if (!ballSlot.ball)
        {
            return;
        }

        ballSlot.ball.state = BallState.Destroying;
        ballSlot.ball = null;
    }
}