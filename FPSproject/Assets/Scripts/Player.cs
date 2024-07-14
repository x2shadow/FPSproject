using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodScreen;
    public TextMeshProUGUI playerHealthUI;
    public GameObject youDiedUI;

    public bool isDead;

    private void Start()
    {
        playerHealthUI.text = "Health: 100";
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            print("Player Dead");
            StartCoroutine(BloodyScreenEffect());
            PlayerDead();
        }
        else
        {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHit);
        }
    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDeath);

        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;
        SoundManager.Instance.playerChannel.PlayDelayed(0.1f);

        isDead = true;
        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        // Dying animation
        GetComponentInChildren<Animator>().enabled = true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenBlackout>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        youDiedUI.SetActive(true);

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene("Main Menu");
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodScreen.activeInHierarchy == false)
        {
            bloodScreen.SetActive(true);
        }

        var image = bloodScreen.GetComponentInChildren<Image>();
 
        // Set the initial alpha value to 1 (fully visible)
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;
 
        float duration = 2f;
        float elapsedTime = 0f;
 
        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
 
            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
 
            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
 
            yield return null; ; // Wait for the next frame
        }

        if (bloodScreen.activeInHierarchy == true)
        {
            bloodScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamageSphere"))
        {
            if(isDead == false)
            TakeDamage(25);
        }
    }
}
