using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI HighScore;
    public void OnEnable()
    {
        
    }
    public void Update()
    {
        HighScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }
}
