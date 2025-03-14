using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public enum RewardType
    {
        RemainingHands = 1,
        RemainingDiscards = 2,
        TypeImage = 3 //Default for type image
    }
    
    public class Reward
    {
        public RewardType Type;
        public Sprite Icon;           
        public string LeadingText;    
        public string Description;    
        public int Value;             

        public Reward(RewardType rewardType, Sprite icon, string leadingText, string description, int value)
        {
            this.Type = rewardType;
            this.Icon = icon;
            this.LeadingText = leadingText;
            this.Description = description;
            this.Value = value;
        }
    }
    
    public class StageReward
    {
        public Sprite Icon;
        public string Score;      
        public int Value;      

        public StageReward(Sprite icon, string score, int value)
        {
            this.Icon = icon;
            this.Score = score;
            this.Value = value;
        }
    }
}
