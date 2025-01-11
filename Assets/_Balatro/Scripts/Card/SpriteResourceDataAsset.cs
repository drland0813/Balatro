using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balatro
{
    [Serializable]
    public class SpriteData
    {
        public string ID;
        public Sprite Sprite;
    }
    
    [CreateAssetMenu(fileName = "SpriteData", menuName = "Sprite Data/Sprite Data", order = 0)]
    public class SpriteResourceDataAsset : ScriptableObject
    {
        public List<SpriteData> SpriteData;

        public Sprite GetSpriteDataByID(string id)
        {
            var sprite = SpriteData.FirstOrDefault(s => s.ID == id).Sprite;
            return sprite;
        }
    }
}