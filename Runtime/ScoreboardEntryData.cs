using System;

namespace Meangpu.Scoreboard
{
    [Serializable]

    public struct ScoreboardEntryData
    {
        public string entryName;
        public int entryScore;
        public bool isNewAdd;
    }
}