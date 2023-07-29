using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Meangpu.Scoreboard
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text entryNameText;
        [SerializeField] private TMP_Text entryScoreText;
        [SerializeField] private TMP_Text idText;
        [SerializeField] private TMP_Text dateText;
        [SerializeField] Image bgImage;
        [SerializeField] Image bgImageJustAdd;
        [SerializeField] Color highScoreCol;
        [SerializeField] Color justAddCol;

        public void Initialize(ScoreboardEntryData scoreboardEntryData, int id)
        {
            entryNameText.SetText(scoreboardEntryData.entryName);
            entryScoreText.SetText(scoreboardEntryData.entryScore.ToString());
            idText.SetText(id.ToString());
            dateText?.SetText(scoreboardEntryData.entryTime);
        }
        public void MakeHighScore() => bgImage.color = highScoreCol;
        public void MakeJustAdd() => bgImageJustAdd.color = justAddCol;
    }
}