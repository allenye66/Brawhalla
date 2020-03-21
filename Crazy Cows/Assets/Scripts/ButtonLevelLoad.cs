using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLevelLoad : MonoBehaviour
{
    public string gameScene;
    public void Reset()
    {
        SceneManager.LoadScene(gameScene);
        FindObjectOfType<GameManager>().P1Lives = 3;
        FindObjectOfType<GameManager>().P2Lives = 3;
    }
}
