using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }  // Singleton instance
    private void Awake()
    {
        // Ensure that only one instance of UiManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy any duplicate UiManager
        }
    }
    #endregion
     
    BoxCollider mapCollider;

    public PlayerHandler player;
    public ZombieHandler zombieHandler;
    [HideInInspector] public UiManager uiManager;
    
    [SerializeField] GameObject bulletBox;
    [SerializeField] GameObject HealthBox;

    [SerializeField] GameObject itemParentGO;

    bool spawnHealthNext;

    public static event Action OnGameStart;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        uiManager = UiManager.Instance;
        mapCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<BoxCollider>();
    }

    public void OnGameLose()
    {
        zombieHandler.isGameStart = false;
        StopCoroutine(PlaceItemRandomlyOnMap());
        uiManager.OnGameOverUi();
    }

    #region ItemSpawn
    public IEnumerator PlaceItemRandomlyOnMap()
    {
        Bounds bounds = mapCollider.bounds;

        while (zombieHandler.isGameStart)
        {
            GameObject itemPrefab = spawnHealthNext ? HealthBox : bulletBox;

            spawnHealthNext = !spawnHealthNext;

            Vector3 randomPosition = GetRandomPosition(bounds);

            if (Physics.CheckSphere(randomPosition, itemPrefab.transform.localScale.x / 2) == false)
            {
                Instantiate(itemPrefab, randomPosition, Quaternion.identity, itemParentGO.transform);
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, 3));
        }
    }

    public void ClearItems()
    {
        foreach(Transform t in itemParentGO.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private Vector3 GetRandomPosition(Bounds bounds)
    {
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float y = 1;
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }
    #endregion


    public void ResetWave(bool isGameExit)
    {
        zombieHandler.ClearZombies();
        ClearItems();
        zombieHandler.ResetToFirstWave();
        player.ResetMyHealth();
        player.ResetMyPosition();
        if(!isGameExit)
        StartTheGame();
    }

    public void StartTheGame()
    {
        OnGameStart();
        uiManager.SwitchCanvas(1);
        zombieHandler.StartWaves();
        StartCoroutine(PlaceItemRandomlyOnMap());
    }


}
