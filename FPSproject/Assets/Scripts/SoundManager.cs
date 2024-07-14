using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingChannel;
    public AudioClip M1911Shot;  
    public AudioSource reloadingSoundM1911;

    public AudioClip AK74Shot;   
    public AudioSource reloadingSoundAK74;      
    public AudioSource emptyMagazineSound;

    public AudioSource throwableChannel;
    public AudioClip grenadeSound;

    public AudioClip enemyChase;
    public AudioClip enemyAttacking;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioSource enemyChannel;

    public AudioSource playerChannel;
    public AudioClip playerHit;
    public AudioClip playerDeath;
    public AudioClip gameOverMusic;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                shootingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.AK74:
                shootingChannel.PlayOneShot(AK74Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                reloadingSoundM1911.Play();
                break;
            case WeaponModel.AK74:
                reloadingSoundAK74.Play();
                break;
        }
    }
}

