using System.IO;
using UnityEngine;
using EasyButtons;
using System;
using System.Collections.Generic;

namespace Meangpu.Scoreboard
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private int maxScoreBoardEntries = 5;
        [SerializeField] private Transform highScoreHolderTransform;
        [SerializeField] private GameObject scoreboardEntryObject;
        [SerializeField] bool isZeroIsHighScore;
        [SerializeField] bool isDoPrintSavePath;
        [SerializeField] string _thisScoreboardName = "highScore.json";

        private string SavePath => $"{Application.persistentDataPath}/{_thisScoreboardName}";
        public List<ScoreboardEntryData> highScores = new();

        private void Start()
        {
            if (isDoPrintSavePath) print(SavePath);
            ScoreboardSaveData oldScore = GetSavedScores();
            UpdateUI(oldScore);
            SaveScores(oldScore);
        }

        public string GetNameDateTime()
        {
            string date = DateTime.Now.ToString("d-MMM-yyy");
            string time = DateTime.Now.ToString("t");
            return $"{date} {time}";
        }

        [Button]
        public void UserGetNewScore(int newScore, string userName = "")
        {
            if (string.IsNullOrEmpty(userName))
            {
                AddEntry(new ScoreboardEntryData()
                {
                    entryName = GetNameDateTime(),
                    entryScore = newScore,
                    isNewAdd = true,
                    entryTime = GetNameDateTime()
                });
            }
            else
            {
                AddEntry(new ScoreboardEntryData()
                {
                    entryName = userName,
                    entryScore = newScore,
                    isNewAdd = true,
                    entryTime = GetNameDateTime()
                });
            }
        }

        public ScoreboardEntryData MakeScoreOld(ScoreboardEntryData score)
        {
            return new ScoreboardEntryData()
            {
                entryName = score.entryName,
                entryScore = score.entryScore,
                isNewAdd = false
            };
        }

        public void AddEntry(ScoreboardEntryData newScoreData)
        {
            ScoreboardSaveData oldScore = GetSavedScores();
            bool scoreAdded = false;

            for (int i = 0; i < oldScore.highScores.Count; i++)
            {
                oldScore.highScores[i] = MakeScoreOld(oldScore.highScores[i]);
            }

            for (int i = 0; i < oldScore.highScores.Count; i++)
            {
                if (isZeroIsHighScore)
                {
                    if (newScoreData.entryScore < oldScore.highScores[i].entryScore)
                    {
                        scoreAdded = AddHighScore(newScoreData, oldScore, i);
                        break;
                    }
                }
                else
                {
                    if (newScoreData.entryScore > oldScore.highScores[i].entryScore)
                    {
                        scoreAdded = AddHighScore(newScoreData, oldScore, i);
                        break;
                    }
                }
            }

            if (!scoreAdded && oldScore.highScores.Count < maxScoreBoardEntries)
            {
                oldScore.highScores.Add(newScoreData);
            }

            if (oldScore.highScores.Count > maxScoreBoardEntries)
            {
                oldScore.highScores.RemoveRange(maxScoreBoardEntries, oldScore.highScores.Count - maxScoreBoardEntries);
            }

            UpdateUI(oldScore);
            SaveScores(oldScore);
        }

        private static bool AddHighScore(ScoreboardEntryData newScoreData, ScoreboardSaveData oldScore, int i)
        {
            oldScore.highScores.Insert(i, newScoreData);
            return true;
        }

        private void UpdateUI(ScoreboardSaveData oldScore)
        {
            foreach (Transform child in highScoreHolderTransform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < oldScore.highScores.Count; i++)
            {
                GameObject newScore = Instantiate(scoreboardEntryObject, highScoreHolderTransform);
                ScoreboardEntryUI uiScpt = newScore.GetComponent<ScoreboardEntryUI>();
                uiScpt.Initialize(oldScore.highScores[i], i + 1);

                if (oldScore.highScores[i].isNewAdd) uiScpt.MakeJustAdd();
                if (i == 0) uiScpt.MakeHighScore();
            }
        }

        [Button]
        public void DeleteScoreJson()
        {
            File.Delete(SavePath);
            ScoreboardSaveData oldScore = GetSavedScores();
            UpdateUI(oldScore);
        }

        private ScoreboardSaveData GetSavedScores()
        {
            if (!File.Exists(SavePath))
            {
                File.Create(SavePath).Dispose();
                return new ScoreboardSaveData();
            }
            using StreamReader stream = new(SavePath);
            string json = stream.ReadToEnd();

            if (json?.Length == 0) return new ScoreboardSaveData();
            else return JsonUtility.FromJson<ScoreboardSaveData>(json);
        }

        private void SaveScores(ScoreboardSaveData scoreboardSaveData)
        {
            using StreamWriter stream = new(SavePath);
            string json = JsonUtility.ToJson(scoreboardSaveData, true);
            stream.Write(json);
        }
    }
}