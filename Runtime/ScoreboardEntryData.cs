namespace Meangpu.Scoreboard
{
    [System.Serializable]
    public struct ScoreboardEntryData
    {
        public string entryName;
        public int entryScore;
        public string entryTime;
        public bool isNewAdd;

        public ScoreboardEntryData(string entryName, int entryScore, string entryTime, bool isNewAdd)
        {
            this.entryName = entryName;
            this.entryScore = entryScore;
            this.entryTime = entryTime;
            this.isNewAdd = isNewAdd;
        }
    }
}