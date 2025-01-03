using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float myDamage;

    public GameObject bloodEffect;
    
    public void OnBulletSpawn(float bulletSpeed, float damage)
    {
        myDamage = damage;
        StartCoroutine(bulletInit(bulletSpeed));
    }

    IEnumerator bulletInit(float bulletSpeed)
    {
        while (true)  
        {
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
            if (!IsVisibleToMainCamera())
            {
                Destroy(gameObject);
                yield break;
            }
            yield return null;  
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.GetDamage(myDamage);

            GameObject blood = Instantiate(bloodEffect, transform.position, Quaternion.identity);

            BloodDirection(transform.forward, blood);
            blood.transform.parent = zombie.transform;
            Destroy(blood,.5f);
            Destroy(gameObject);
        }
    }

    public void BloodDirection(Vector3 direction, GameObject blood)
    {
        ParticleSystem bloodParticle = blood.GetComponent<ParticleSystem>();
        if (bloodParticle != null)
        {
            var shape = bloodParticle.shape; 
            shape.rotation = -Quaternion.LookRotation(direction).eulerAngles; 
        }
    }
    private bool IsVisibleToMainCamera()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
    }
}
