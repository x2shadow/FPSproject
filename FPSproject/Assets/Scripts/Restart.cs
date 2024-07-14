using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //if (Input.GetKeyDown(KeyCode.Q)) Application.Quit();
    }
}
