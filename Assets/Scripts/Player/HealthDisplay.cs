using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image[] hearts;
    public CharacterController2D player;

    private void Update()
    {
        UpdateHearts();
    }
    
    private void UpdateHearts()
    {
        int health = Mathf.Clamp(player.life, 0, hearts.Length); // health가 범위를 넘어가지 않도록 보정

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].color = Color.red;     // 체력이 남아있을 때
            }
            else
            {
                hearts[i].color = Color.black;   // 체력이 없을 때
            }
        }
    }
}
