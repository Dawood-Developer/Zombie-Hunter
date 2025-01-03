using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField]float minDamage;
    [SerializeField]float maxDamage;
    [SerializeField]float minHealth;
    [SerializeField]float maxHealth;
    [SerializeField]float minSpeed;
    [SerializeField]float maxSpeed;

    float damage;
    float health;
    float speed;
     
    [SerializeField] AudioClip[] zombieGrowl;
    [SerializeField] AudioClip bulletHitSound;

    [HideInInspector]public PlayerHandler target;

    private ZombieHandler zombieHandler;
    private AudioSource audioSource;

    bool isAttacking = false;

    private void Start()
    {
        Init();
    }
    void Init()
    {
        zombieHandler = ZombieHandler.Instance;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.volume = Random.Range(0.1f, 0.5f);
        damage = Random.Range(minSpeed, maxSpeed);
        health = Random.Range(minHealth, maxHealth);
        speed = Random.Range(minSpeed, maxSpeed);
    }
    private void Update()
    {
        if (target != null)
            MoveTowardPlayer(target);
        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
            PlayRandomGrowl();
        }
    }

    public void PlayRandomGrowl()
    {
        audioSource.clip = zombieGrowl[Random.Range(0, zombieGrowl.Length)];
        audioSource.Play();
    }

    public void MoveTowardPlayer(PlayerHandler target)
    {
        if (Vector3.Distance(transform.position, target.transform.position) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            Vector3 direction = target.transform.position - transform.position; 
            Quaternion targetRotation = Quaternion.LookRotation(direction); 
            transform.rotation = Quaternion.Euler(90f, targetRotation.eulerAngles.y, 0f);
        }
        else
        {
            if (!isAttacking)
            StartCoroutine(AttackOnPlayer(target));
        }
    }

    public IEnumerator AttackOnPlayer(PlayerHandler target)
    {
        isAttacking = true; 

        while (Vector3.Distance(transform.position, target.transform.position) <= 1f)
        {
            target.ChangeHealthStatus(-damage);
            yield return new WaitForSeconds(2);
        }

        isAttacking = false;
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health < 0f)
            Die();
    }

    public void Die()
    {
        zombieHandler.zombiesKilled++;
        zombieHandler.zombiesKilledInCurrentGame++;
        Prefs.TotalZombiesKilled++;
        Destroy(gameObject);
    }

    public void UpgradeMe(int wave)
    {
        health += Mathf.RoundToInt(health * wave * 0.05f);
        speed += Mathf.RoundToInt(speed * wave * 0.03f);
        damage += Mathf.RoundToInt(damage * wave * 0.04f);
    }


}
