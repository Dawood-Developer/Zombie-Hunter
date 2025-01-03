using UnityEngine;
using TMPro;
using UnityEngine.UI; // For managing UI components

using Dan.Main;
using System.Linq;
using System.Collections.Generic;

namespace LeaderboardCreatorDemo
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] GameObject[] playerEntry;
        [SerializeField] GameObject reloadGO;
        [SerializeField] GameObject playerStatGO;

        [SerializeField] Color myColour;

        private void OnEnable()
        {
            SubmitNameAndUploadScore();
        }
        private void OnDisable()
        {
            reloadGO.SetActive(true);
            playerStatGO.SetActive(false);  
        }

        private void LoadEntries()
        {
            Leaderboards.ZombieHunter.GetEntries(entries =>
            {
                for (int i = 0; i < Mathf.Min(entries.Length, 10); i++)
                {
                    var entry = entries[i];
                    print(entry);
                    print(entry.Username);
                    print(entry.Score.ToString());
                }
            });
        }
         
        private void SubmitNameAndUploadScore()
        {
            string username = Prefs.PlayerName; 
            int newScore = Prefs.TotalZombiesKilled;

            Leaderboards.ZombieHunter.GetEntries(entries =>
            {
                var updatedEntries = entries.Select(entry => new TemporaryEntry
                {
                    Username = entry.Username,
                    Score = entry.Score
                }).ToList();

                var existingEntry = updatedEntries.FirstOrDefault(e => e.Username == username);

                if (existingEntry != null)
                {
                    if (newScore > existingEntry.Score)
                    {
                        existingEntry.Score = newScore;
                    }
                }
                else
                {
                    updatedEntries.Add(new TemporaryEntry
                    {
                        Username = username,
                        Score = newScore
                    });
                }

                updatedEntries = updatedEntries.OrderByDescending(e => e.Score).ToList();

                LoadUpdatedEntriesToUI(updatedEntries);

                Leaderboards.ZombieHunter.UploadNewEntry(username, newScore, isSuccessful =>
                {
                    if (isSuccessful)
                    {
                        Debug.Log("Leaderboard entry uploaded successfully!");
                    }
                    else
                    {
                        Debug.LogError("Failed to upload leaderboard entry.");
                    }
                });
            });
        }


        private void LoadUpdatedEntriesToUI(List<TemporaryEntry> updatedEntries)
        {
            int entriesToShow = Mathf.Min(updatedEntries.Count, playerEntry.Length - 1); // Top 5 ke liye
            string username = Prefs.PlayerName; // Current player ka username
            bool isPlayerInTop5 = false;

            // Load top 5 entries
            for (int i = 0; i < entriesToShow; i++)
            {
                var entry = updatedEntries[i];


                GameObject rank = playerEntry[i].transform.GetChild(0).gameObject;
                Text rankTxt = rank.GetComponent<Text>();
                rankTxt.text = (i + 1).ToString();

                GameObject name = playerEntry[i].transform.GetChild(1).gameObject;
                Text nameTxt = name.GetComponent<Text>();
                nameTxt.text = entry.Username;

                GameObject kill = playerEntry[i].transform.GetChild(2).gameObject;
                Text killTxt = kill.GetComponent<Text>();
                killTxt.text = entry.Score.ToString();

                if (entry.Username == username)
                {
                    isPlayerInTop5 = true;
                    rankTxt.color = myColour;
                    nameTxt.color = myColour;
                    killTxt.color = myColour;
                }
            }

            GameObject extraEntry = playerEntry[playerEntry.Length - 1]; // playerEntry[6]

            if (isPlayerInTop5)
            {
                extraEntry.SetActive(false); // Disable if player is in top 5
            }
            else
            {
                extraEntry.SetActive(true); // Enable and display current player's rank

                var playerRank = updatedEntries.FindIndex(e => e.Username == username) + 1;
                var playerScore = updatedEntries.FirstOrDefault(e => e.Username == username)?.Score ?? 0;

                GameObject rank = extraEntry.transform.GetChild(0).gameObject;
                Text rankTxt = rank.GetComponent<Text>();
                rankTxt.text = playerRank.ToString();

                GameObject name = extraEntry.transform.GetChild(1).gameObject;
                Text nameTxt = name.GetComponent<Text>();
                nameTxt.text = username;

                GameObject kill = extraEntry.transform.GetChild(2).gameObject;
                Text killTxt = kill.GetComponent<Text>();
                killTxt.text = playerScore.ToString();
            }

            reloadGO.SetActive(false);
            playerStatGO.SetActive(true);
        }


        private class TemporaryEntry
        {
            public string Username { get; set; }
            public int Score { get; set; }
        }
    }

}