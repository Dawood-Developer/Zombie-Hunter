using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs : MonoBehaviour
{
    public static int SoundToggle
    {
        get => PlayerPrefs.GetInt(nameof(SoundToggle), 1);
        set => PlayerPrefs.SetInt(nameof(SoundToggle), value);
    }
    public static int TotalZombiesKilled
    {
        get => PlayerPrefs.GetInt(nameof(TotalZombiesKilled), 0);
        set => PlayerPrefs.SetInt(nameof(TotalZombiesKilled), value);
    }
    public static int NameInit
    {
        get => PlayerPrefs.GetInt(nameof(NameInit), 0);
        set => PlayerPrefs.SetInt(nameof(NameInit), value);
    }
    public static string PlayerName
    {
        get => PlayerPrefs.GetString(nameof(PlayerName), "");
        set => PlayerPrefs.SetString(nameof(PlayerName), value);
    }
    public static int PlayerID
    {
        get => PlayerPrefs.GetInt(nameof(PlayerID), 0);
        set => PlayerPrefs.SetInt(nameof(PlayerID), value);
    }
}
