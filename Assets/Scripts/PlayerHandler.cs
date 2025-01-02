using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public Gun gun;

    [SerializeField] float maxHealth;

    [SerializeField] GameObject[] models;
    [SerializeField] GameObject activeModel;
     
    private float currentHealth;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        currentHealth = maxHealth;
    }
    public void ChangeHealthStatus(float damage)
    {
        currentHealth += damage;
        UiManager.Instance.UpdateHealthBar(currentHealth.ToString());
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.OnGameLose();
        }

    }

    public void ResetMyHealth()
    {
        currentHealth = maxHealth;
        gun.ResetMyBulletsForFirstWave();
        UiManager.Instance.UpdateHealthBar(currentHealth.ToString());
    }
    public void ChangeMyModel(int i)
    {
        if(activeModel != null)
            DestroyImmediate(activeModel);

        activeModel = Instantiate(models[i],transform);
        gun = activeModel.GetComponentInChildren<Gun>();
    }

    public void ResetMyPosition()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
