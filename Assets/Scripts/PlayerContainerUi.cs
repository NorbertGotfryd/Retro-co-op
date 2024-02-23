using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainerUi : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image healthBatFill;

    public void Initialize(Color color)
    {
        scoreText.color = color;
        healthBatFill.color = color;

        scoreText.text = "0";
        healthBatFill.fillAmount = 1.0f;
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateHealthBar(int currentHp, int maxHp)
    {
        healthBatFill.fillAmount = (float)currentHp / (float)maxHp;
    }
}
