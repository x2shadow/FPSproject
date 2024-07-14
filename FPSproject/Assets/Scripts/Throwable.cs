using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    private bool hasExploded   = false;
    public  bool hasBeenThrown = false;
    
    public enum ThrowableType
    {
        None,
        Grenade,
        Smoke
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = false;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke:
                SmokeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Visual Effect
        GameObject explosionEffect = GlobalReferences.Instance.greandeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwableChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if (objectInRange.gameObject.GetComponent<Enemy>())
            {
                if (objectInRange.gameObject.GetComponent<Enemy>().isDead == false)
                    objectInRange.gameObject.GetComponent<Enemy>().TakeDamage(100);
            }
        }
    }

    private void SmokeEffect()
    {
        // Visual Effect
        GameObject smokeEffect = GlobalReferences.Instance.smokeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwableChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply blindness to enemies
            }
        }
    }
}
