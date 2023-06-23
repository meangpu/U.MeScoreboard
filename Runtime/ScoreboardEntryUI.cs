using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Meangpu.Scoreboard
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text entryNameText = null;
        [SerializeField] private TMP_Text entryScoreText = null;
        [SerializeField] private TMP_Text idText = null;
        [SerializeField] Image bgImage;
        [SerializeField] Image bgImageJustAdd;
        [SerializeField] Color highScoreCol;
        [SerializeField] Color justAddCol;

        public void Initialize(ScoreboardEntryData scoreboardEntryData, int id)
        {
            entryNameText.text = scoreboardEntryData.entryName;
            entryScoreText.text = scoreboardEntryData.entryScore.ToString();
            idText.text = id.ToString();
        }
        public void MakeHighScore()
        {
            bgImage.color = highScoreCol;
        }

        public void MakeJustAdd()
        {
            bgImageJustAdd.color = justAddCol;
        }
    }
}