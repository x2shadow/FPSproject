using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectWeHit.gameObject.name + "!");

            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("hit Wall!");

            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            print("hit a beer bottle!");
            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();

            // Dont destroy bullet, it will be destroyed after bullet lifetime
        }

        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            if (objectWeHit.gameObject.GetComponent<Enemy>().isDead == false)
            {
                objectWeHit.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }
            
            CreateBloodSpayEffect(objectWeHit);
            Destroy(gameObject);
        }
    }

    private void CreateBloodSpayEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
    
        GameObject bloodEffectPrefab = Instantiate(
            GlobalReferences.Instance.bloodEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        bloodEffectPrefab.transform.SetParent(objectWeHit.gameObject.transform);
    }

    private void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
    
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
