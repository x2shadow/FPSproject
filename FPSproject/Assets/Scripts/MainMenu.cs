using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource mainChannel;
    public AudioClip backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        mainChannel.PlayOneShot(backgroundMusic);
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        mainChannel.Stop();
        SceneManager.LoadScene("Main Scene");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
