using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Balatro
{
    [System.Serializable]
    public class PokerHand
    {
        public int id;
        public string name;
        public int point;
        public int multiplier;
        public LevelBonus level_bonus;
        public int level = 1;
        public int numberOfTimes = 0;

        public void LevelUp()
        {
            level++;
            point += level_bonus.point;
            multiplier += level_bonus.multiplier;
        }

        public void ResetLevel()
        {
            level = 1;
        }
    }

    [System.Serializable]
    public class LevelBonus
    {
        public int point;
        public int multiplier;
    }

    [System.Serializable]
    public class PokerHandsConfig
    {
        public List<PokerHand> hands;
    }
}
