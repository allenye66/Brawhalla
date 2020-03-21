using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public float maxHealth;
    public GameObject player;
    public Slider health;
    
    void Update()
    {
        if(player.tag == "Player1")
        {
            health.value = FindObjectOfType<GameManager>().P1Health / maxHealth;
        } else if(player.tag == "Player2")
        {
            health.value = FindObjectOfType<GameManager>().P2Health / maxHealth;
        }
    }
}
