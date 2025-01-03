using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZombieVariant
{
    public GameObject zombiePrefab; 
    public int spawnWaveIndex; // Kis wave mei ye zombie ayega
    [Range(0, 100)] public int spawnChance; // 100 mei sei kitna percent chance hai spawn hone ka
}

public class ZombieHandler : MonoBehaviour
{
    #region Singleton
    public static ZombieHandler Instance { get; private set; }  // Singleton instance
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

    public List<ZombieVariant> zombieVariants; // Inspector mein show hone wali list

    [SerializeField] Zombie normalZombies;
     
    private List<BoxCollider> waypoint = new List<BoxCollider>();
    private Transform zombieParentGO;
    private PlayerHandler player;
    private int spawnPointIndex;
    private int orignalWaveIndex;
    private int orignalWaveTime;
    private int orignalWaveZombies;

    [Header("Wave Setting")]
    [SerializeField] int waveIndex;
    [SerializeField] int waveTime;
    [SerializeField] int waveZombies;

    [HideInInspector] public int zombiesKilledInCurrentGame;
    [HideInInspector] public int zombiesKilled;
    [HideInInspector] public bool isGameStart;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (Transform t in transform)
        {
            BoxCollider collider = t.GetComponent<BoxCollider>();
            if (collider != null)
                waypoint.Add(collider);
        }

        orignalWaveIndex = waveIndex;
        orignalWaveTime = waveTime;
        orignalWaveZombies = waveZombies;
        
        player = GameManager.Instance.player;
        zombieParentGO = transform.GetChild(0);
    }

    public void StartWaves()
    {
        isGameStart = true;
        StartCoroutine(StartWavesCoroutine());
    }
    IEnumerator StartWavesCoroutine()
    {
        while (isGameStart)
        {
            zombiesKilled = 0;
            
            WaveTextUpdate(waveIndex.ToString());
            
            StartCoroutine(GenerateWave(waveZombies, waveTime));

            yield return ZombiesStatus();
            
            IncreaseDifficulty();
            
            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator GenerateWave(int numberOfZombies, float timeInSecondsForWave)
    {
        float spawnDelay = timeInSecondsForWave / numberOfZombies;

        for (int i = 0; i < numberOfZombies; i++)
        {
            Vector3 spawnPosition = SpawnPoint();

            GenerateZombies(spawnPosition);

            yield return new WaitForSeconds(spawnDelay);
        }

    }

    IEnumerator ZombiesStatus()
    {
        while (zombiesKilled < waveZombies)
        {
            UiManager.Instance.WaveProgress(zombiesKilled , waveZombies);
            UiManager.Instance.GameEndZombieInfo((waveIndex-1).ToString(), zombiesKilledInCurrentGame.ToString(), Prefs.TotalZombiesKilled.ToString());
            yield return null;
        }
    }

    void WaveTextUpdate(string wave)
    {
        UiManager.Instance.WaveUpdate(wave);
    }

    public void ClearZombies()
    {
        foreach (Transform t in zombieParentGO.transform)
        {
            Destroy(t.gameObject);
        } 
    }

    public Vector3 SpawnPoint()
    {
        if(spawnPointIndex >= waypoint.Count)
            spawnPointIndex = 0;

        Vector3 minBound = waypoint[spawnPointIndex].bounds.min;
        Vector3 maxBound = waypoint[spawnPointIndex].bounds.max;

        float xPos = Random.Range(minBound.x, maxBound.x);
        float zPos = Random.Range(minBound.z, maxBound.z);
        
        spawnPointIndex++;
        
        return new Vector3(xPos,1, zPos);
    }

    public void ResetToFirstWave()
    {
        waveTime = orignalWaveTime;
        waveIndex = orignalWaveIndex;
        waveZombies = orignalWaveZombies;
        zombiesKilledInCurrentGame = 0;
    }

/*    public void GenerateZombies(Vector3 spawnPosition)
    {
        Zombie zombie = Instantiate(normalZombies, spawnPosition, Quaternion.identity, zombieParentGO);

        zombie.UpgradeMe(waveIndex);

        zombie.transform.rotation = Quaternion.Euler(90, 0, 0);

        zombie.target = player;
    }*/
    public void GenerateZombies(Vector3 spawnPosition)
    {
        ZombieVariant selectedZombie = GetRandomZombieVariant(waveIndex);

        Zombie zombie = Instantiate(selectedZombie.zombiePrefab, spawnPosition, Quaternion.identity, zombieParentGO).GetComponent<Zombie>();

        zombie.UpgradeMe(waveIndex);
        //zombie.transform.rotation = Quaternion.Euler(90, 0, 0);
        zombie.target = player;
    }
    private ZombieVariant GetRandomZombieVariant(int currentWaveIndex)
    {
        var validZombies = zombieVariants.FindAll(zv => zv.spawnWaveIndex <= currentWaveIndex);
        int totalChance = 0;

        foreach (var variant in validZombies)
        {
            totalChance += variant.spawnChance;
        }

        if (validZombies.Count == 0)
        {
            return new ZombieVariant { zombiePrefab = normalZombies.gameObject, spawnWaveIndex = currentWaveIndex, spawnChance = 100 };
        }

        int randomValue = Random.Range(0, totalChance);

        int currentSum = 0;
        foreach (var variant in validZombies)
        {
            currentSum += variant.spawnChance;
            if (randomValue < currentSum)
            {
                return variant;
            }
        }

        return validZombies[0];
    }
    public void IncreaseDifficulty()
    {
        waveTime += 5;
        waveZombies += Mathf.RoundToInt(waveZombies * 0.2f);
        waveIndex++;
    }
    
}
