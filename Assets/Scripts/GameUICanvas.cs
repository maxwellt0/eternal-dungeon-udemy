using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUICanvas : MonoBehaviour
{
    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI levelNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLevelTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        levelTime.text = "" + minutes + ":" + seconds.ToString("D2");
    }

    public void UpdateLevelNumber(int level)
    {
        levelNumber.text = "Level " + level;
    }
}
