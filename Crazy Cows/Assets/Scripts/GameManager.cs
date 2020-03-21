using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public GameObject deathEffect;

    public int P1Health;
    public int P2Health;
    private int savedP1Health;
    private int savedP2Health;
    public int P1Lives;
    public int P2Lives;
    public float timeUntilScore;
    public float timeUntilReload;
    
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public GameObject button;

    private void Start()
    {
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        savedP1Health = P1Health;
        savedP2Health = P2Health;
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if(Time.timeSinceLevelLoad == 0)
        {
            Invoke("hideScore", 0);
            P1Health = savedP1Health;
            P2Health = savedP2Health;
            
        }
        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");
        if (P1Health <= 0)
        {
            P1Health = savedP1Health;
            P1Lives--;
            scoreText.text = P1Lives + " - " + P2Lives;
            Instantiate(deathEffect, player1.transform.position, player1.transform.rotation);
            Destroy(player1);
            if (P1Lives == 0)
            {
                Invoke("showEndScore", timeUntilScore);
            }
            else
            {
                Invoke("showScore", timeUntilScore);
                Invoke("reloadScene", timeUntilReload);
            }
        }
        else if (P2Health <= 0)
        {
            P2Health = savedP2Health;
            P2Lives--;
            scoreText.text = P1Lives + " - " + P2Lives;
            Instantiate(deathEffect, player2.transform.position, player2.transform.rotation);
            Destroy(player2);
            if(P2Lives == 0)
            {
                Invoke("showEndScore", timeUntilScore);
            }
            else
            {
                Invoke("showScore", timeUntilScore);
                Invoke("reloadScene", timeUntilReload);
            }
        }
    }

    public void DamageP1(int damage)
    {
        P1Health -= damage;
    }

    public void DamageP2(int damage)
    {
        P2Health -= damage;
    }
    
    public void showScore()
    {
        score.SetActive(true);
        button.SetActive(false);
    }

    public void showEndScore()
    {
        score.SetActive(true);
        button.SetActive(true);
    }

    public void hideScore()
    {
        score.SetActive(false);
    }

    public void reloadScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
