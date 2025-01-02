using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollison : MonoBehaviour
{
    PlayerHandler playerRefrence;
    private void Start()
    {
        playerRefrence = GetComponent<PlayerHandler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IngameKit>() != null)
        {
            IngameKit ingameKit = other.GetComponent<IngameKit>();
            if (ingameKit.isBullet)
            {
            playerRefrence.gun.GetMoreBullets(ingameKit.bullets);
            }
            else
            {
                playerRefrence.ChangeHealthStatus(ingameKit.health);
            }
            Destroy(other.gameObject);
        }
    }
}
