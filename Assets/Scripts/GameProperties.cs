using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperties : MonoBehaviour
{
    private const string KeyLastLevel = "LAST_LEVEL";
    private void Awake()
    {
        if (!PlayerPrefs.HasKey(KeyLastLevel))
        {
            PlayerPrefs.SetInt(KeyLastLevel, 1);
        }
        lastLevel = PlayerPrefs.GetInt(KeyLastLevel);
    }

    public float ballUpscaleSpeed = 2f;
    public float ballDownscaleSpeed = 2f;
    public float ballSlotsSpeed = 4f;
    public float ballShootingSpeed = 20f;
    public float ballLandingSpeed = 5f;
    public float ballSlotSwitchingSpeed = 5f;
    
    public int bombRadius = 1;
    public float reverseDuration = 2f;
    public float timeSlowDuration = 2f;
    public float levelDurationSeconds = 10f;
    public float slotSpeedUpPerLevel = 0.1f;

    private int lastLevel = 1;

    public int LastLevel => lastLevel;

    public void IncrementLastLevel()
    {
        lastLevel++;
        PlayerPrefs.SetInt(KeyLastLevel, lastLevel);
    }
    
    public float GetSlotSpeedMultiplier(float effectMultiplier)
    {
        return effectMultiplier * (1 + lastLevel * slotSpeedUpPerLevel);
    }

    public void ResetLastLevel()
    {
        lastLevel = 1;
        PlayerPrefs.SetInt(KeyLastLevel, lastLevel);
    }
}
