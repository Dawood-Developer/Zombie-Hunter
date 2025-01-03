using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region Singleton
    public static UiManager Instance { get; private set; }  // Singleton instance
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
     
    [SerializeField] Text BulletCountTxt;
    [SerializeField] Text healthBar;
    [SerializeField] Text waveNumberTxt;
    [SerializeField] Image zombiesFill;
    [SerializeField] Image zombiesFillIcon;
    [SerializeField] GameObject SoundOnBtn;
    [SerializeField] EventTrigger eventTrigger;
    [SerializeField] PlayerHandler player;
    [SerializeField] Button reloadBtn;

    [Header("Game End")]
    [SerializeField] GameObject gameLosePanel;
    [SerializeField] Text waveSurviveTxt;
    [SerializeField] Text zombiesKilledInCurrentGameTxt;
    [SerializeField] Text totalZombiesKilledTxt;

    [Header("Player Name")]
    [SerializeField] GameObject EnterNameGO;
    [SerializeField] TextMeshProUGUI InputPlayerNameTxt;
    [SerializeField] Text playerNameTxt;

    [Header("Canvas")]
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject gameCanvas;

    private void OnEnable()
    {
        GameManager.OnGameStart += GetButtonRefrences;
    }
    private void OnDisable()
    {
        GameManager.OnGameStart -= GetButtonRefrences;
    }
    private void Start()
    {
        Init();
    }
    void Init()
    {
        player = GameManager.Instance.player;
        if (Prefs.NameInit == 0)
        {
            Prefs.PlayerID = GenerateRandomID();
            EnterNameGO.SetActive(true);
            Prefs.NameInit = 1;
        }
        playerNameTxt.text = Prefs.PlayerName;
    }
    void GetButtonRefrences()

    {
        reloadBtn.onClick.AddListener(player.gun.Reload);

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDownEntry.callback.AddListener((data) => { player.gun.FireButtonDown(); });
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUpEntry.callback.AddListener((data) => { player.gun.FireButtonUp(); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    public void WaveUpdate(string wave)
    {
        waveNumberTxt.text = "Wave " + wave;
    }

    public void WaveProgress(int filling, int totalZombies)
    {
        if (totalZombies <= 0)
        {
            return;
        }

        float targetFillPercentage = (float)filling / totalZombies;
        StartCoroutine(SmoothFill(targetFillPercentage));
    }

    private IEnumerator SmoothFill(float targetFillPercentage)
    {
        float currentFill = zombiesFill.fillAmount;
        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            zombiesFill.fillAmount = Mathf.Lerp(currentFill, targetFillPercentage, t);

            UpdateIconPosition(zombiesFill.fillAmount);

            yield return null;
        }

        zombiesFill.fillAmount = targetFillPercentage;
        UpdateIconPosition(targetFillPercentage);
    }

    private void UpdateIconPosition(float fillPercentage)
    {
        RectTransform zombiesFillRect = zombiesFill.rectTransform;
        RectTransform zombiesFillIconRect = zombiesFillIcon.rectTransform;

        Vector2 iconPosition = zombiesFillIconRect.anchoredPosition;

        zombiesFillIconRect.anchoredPosition = new Vector2(
            fillPercentage * zombiesFillRect.rect.width,
            iconPosition.y
        );
    }

    public void UpdateBulletUi(int cb, int tb)
    {
        BulletCountTxt.text = tb.ToString() + "/" + cb.ToString();
    }

    public void UpdateHealthBar(string health)
    {
        healthBar.text = health;
    }

    public void OnGameOverUi()
    {
        gameLosePanel.SetActive(true);
    }

    public void RestartButton()
    {
        gameLosePanel.SetActive(false);
        GameManager.Instance.ResetWave(false);
    }

    public void ExitBtn()
    {
        gameLosePanel.SetActive(false);
        GameManager.Instance.ResetWave(true);
        SwitchCanvas(0);
    }

    public void SetSoundActive(bool active)
    {
        SoundOnBtn.SetActive(active);
    }

    public void GameEndZombieInfo(string waveSurvive, string zombiesKilledInCurrentGame, string totalZombiesKilled)
    {
        waveSurviveTxt.text = "Wave survived : " + waveSurvive;
        zombiesKilledInCurrentGameTxt.text = "Zombies killed : " + zombiesKilledInCurrentGame;
        totalZombiesKilledTxt.text = "Total Zombies Killed : " + totalZombiesKilled;
    }

    public void SavePlayerName()
    {
        if (InputPlayerNameTxt.text.Length > 0)
        {
            Prefs.PlayerName = InputPlayerNameTxt.text;
            playerNameTxt.text = Prefs.PlayerName;
            EnterNameGO.SetActive(false);
        }
    }
    public void SwitchCanvas(int i)
    {
        switch (i)
        {
            case 0:
                mainMenuCanvas.SetActive(true);
                gameCanvas.SetActive(false);
                break;
            case 1:
                mainMenuCanvas.SetActive(false);
                gameCanvas.SetActive(true);
                break;
        }
    }
    public int GenerateRandomID()
    {
        System.Random random = new System.Random();
        int randomID = random.Next(1000000000, int.MaxValue);
        return randomID;
    }
}
