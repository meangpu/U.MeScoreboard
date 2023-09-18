using System.IO;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] string _defaultName = "Anonymous";
        [SerializeField] Image _scoreboardBG;

        private string SavePath => $"{Application.persistentDataPath}/{_thisScoreboardName}";
        public List<ScoreboardEntryData> ScoreData = new();

        private void Start()
        {
            if (isDoPrintSavePath) print(SavePath);

            LoadScore();
            UpdateUI();
        }

        private void LoadScore() => ScoreData = GetSavedScores();

        public string GetNameDateTime()
        {
            string date = DateTime.Now.ToString("d-MMM-yyy");
            string time = DateTime.Now.ToString("t");
            return $"{date} {time}";
        }

        [Button]
        public void UserGetNewScore(int newScore, string userName = "")
        {
            AddEntry(new ScoreboardEntryData(
                string.IsNullOrEmpty(userName) ? _defaultName : userName,
                newScore,
                GetNameDateTime(),
                true));
        }

        [Button]
        public void UpdateUsernameOfJustGetScore(string userName = "")
        {
            if (string.IsNullOrEmpty(userName)) return;

            for (int i = 0; i < ScoreData.Count; i++)
            {
                // target just add score
                if (ScoreData[i].isNewAdd)
                {
                    ScoreData[i] = UpdateScoreName(ScoreData[i], userName);
                    break;
                }
            }

            UpdateUI();
            SaveScores();
        }

        public ScoreboardEntryData MakeScoreOld(ScoreboardEntryData score)
        {
            return new ScoreboardEntryData(score.entryName, score.entryScore, score.entryTime, false);
        }

        public ScoreboardEntryData UpdateScoreName(ScoreboardEntryData score, string newUserName = "")
        {
            return new ScoreboardEntryData(newUserName, score.entryScore, score.entryTime, score.isNewAdd);
        }

        public void AddEntry(ScoreboardEntryData newScoreData)
        {
            LoadScore();

            bool scoreAdded = false;

            for (int i = 0; i < ScoreData.Count; i++) ScoreData[i] = MakeScoreOld(ScoreData[i]);

            for (int i = 0; i < ScoreData.Count; i++)
            {
                if (isZeroIsHighScore)
                {
                    if (newScoreData.entryScore < ScoreData[i].entryScore)
                    {
                        scoreAdded = AddHighScore(newScoreData, i);
                        break;
                    }
                }
                else
                {
                    if (newScoreData.entryScore > ScoreData[i].entryScore)
                    {
                        scoreAdded = AddHighScore(newScoreData, i);
                        break;
                    }
                }
            }

            if (!scoreAdded && ScoreData.Count < maxScoreBoardEntries)
            {
                ScoreData.Add(newScoreData);
            }

            if (ScoreData.Count > maxScoreBoardEntries)
            {
                ScoreData.RemoveRange(maxScoreBoardEntries, ScoreData.Count - maxScoreBoardEntries);
            }

            UpdateUI();
            SaveScores();
        }

        private bool AddHighScore(ScoreboardEntryData newScoreData, int i)
        {
            ScoreData.Insert(i, newScoreData);
            return true;
        }

        private void UpdateUI()
        {
            _scoreboardBG.enabled = ScoreData.Count != 0;

            foreach (Transform child in highScoreHolderTransform) Destroy(child.gameObject);

            for (int i = 0; i < ScoreData.Count; i++)
            {
                GameObject newScore = Instantiate(scoreboardEntryObject, highScoreHolderTransform);
                ScoreboardEntryUI uiScpt = newScore.GetComponent<ScoreboardEntryUI>();
                uiScpt.Initialize(ScoreData[i], i + 1);

                if (ScoreData[i].isNewAdd) uiScpt.MakeJustAdd();
                if (i == 0) uiScpt.MakeHighScore();
            }
        }

        [Button]
        public void DeleteScoreJson()
        {
            File.Delete(SavePath);
            ScoreData = GetSavedScores();
            UpdateUI();
        }

        private List<ScoreboardEntryData> GetSavedScores()
        {
            if (!File.Exists(SavePath))
            {
                File.Create(SavePath).Dispose();
                return new List<ScoreboardEntryData>();
            }
            using StreamReader stream = new(SavePath);
            string json = stream.ReadToEnd();

            if (json?.Length == 0) return new List<ScoreboardEntryData>();
            else return JsonUtility.FromJson<JsonWrapper<ScoreboardEntryData>>(json).list;
        }

        private void SaveScores()
        {
            using StreamWriter stream = new(SavePath);
            string json = JsonUtility.ToJson(new JsonWrapper<ScoreboardEntryData>(ScoreData));
            stream.Write(json);
        }
    }

    [Serializable]
    public struct JsonWrapper<T>
    {
        public List<T> list;
        public JsonWrapper(List<T> list) => this.list = list;
    }
}
