using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Wave", order = 1)]
public class WavesScriptable : ScriptableObject
{
    public int waveTime;

    public List<ZombieWaveData> zombiesInWave;
}

[System.Serializable]
public class ZombieWaveData
{
    public Zombie zombiePrefab; 
    public int quantity; 
}
