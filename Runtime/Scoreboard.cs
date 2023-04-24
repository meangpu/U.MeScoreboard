using System.IO;
using UnityEngine;
using EasyButtons;
using System;

namespace Meangpu
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private int maxScoreBoardEntries = 5;
        [SerializeField] private Transform highScoreHolderTransform = null;
        [SerializeField] private GameObject scoreboardEntryObject = null;
        private string SavePath => $"{Application.persistentDataPath}/highScore.json";

        private void Start()
        {
            print(SavePath);
            ScoreboardSaveData oldScore = GetSavedScores();
            UpdateUI(oldScore);
            SaveScores(oldScore);
        }

        public string GetNameDateTime()
        {
            string date = DateTime.Now.ToString("d-MMM-yyy");
            string time = DateTime.Now.ToString("t");
            string finalDateTime = $"{date} {time}";
            return finalDateTime;
        }

        [Button]
        public void UserGetNewScore(int newScore)
        {
            AddEntry(new ScoreboardEntryData()
            {
                entryName = GetNameDateTime(),
                entryScore = newScore,
                isNewAdd = true
            });
        }

        public ScoreboardEntryData MakeScoreOld(ScoreboardEntryData score)
        {
            ScoreboardEntryData scoreWithOldData = new ScoreboardEntryData()
            {
                entryName = score.entryName,
                entryScore = score.entryScore,
                isNewAdd = false
            };
            return scoreWithOldData;
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
                if (newScoreData.entryScore < oldScore.highScores[i].entryScore)
                {
                    oldScore.highScores.Insert(i, newScoreData);
                    scoreAdded = true;
                    break;
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

        private void UpdateUI(ScoreboardSaveData oldScore)
        {

            foreach (Transform child in highScoreHolderTransform)
            {
                DestroyImmediate(child.gameObject);
            }

            for (int i = 0; i < oldScore.highScores.Count; i++)
            {
                GameObject newScore = Instantiate(scoreboardEntryObject, highScoreHolderTransform);
                ScoreboardEntryUI uiScpt = newScore.GetComponent<ScoreboardEntryUI>();
                uiScpt.Initialize(oldScore.highScores[i], i + 1);
                if (oldScore.highScores[i].isNewAdd)
                {
                    uiScpt.MakeJustAdd();
                }

                if (i == 0)
                {
                    uiScpt.MakeHighScore();
                }
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
            using (StreamReader stream = new StreamReader(SavePath))
            {
                string json = stream.ReadToEnd();
                if (json == "")
                {
                    return new ScoreboardSaveData();
                }
                else
                {
                    return JsonUtility.FromJson<ScoreboardSaveData>(json);
                }
            }

        }

        private void SaveScores(ScoreboardSaveData scoreboardSaveData)
        {
            using (StreamWriter stream = new StreamWriter(SavePath))
            {
                string json = JsonUtility.ToJson(scoreboardSaveData, true);
                stream.Write(json);
            }
        }

    }
}