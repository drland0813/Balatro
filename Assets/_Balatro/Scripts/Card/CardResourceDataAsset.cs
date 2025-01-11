using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balatro
{
    [CreateAssetMenu(fileName = "Normal Card Data", menuName = "Sprite Data/Card Data", order = 1)]
    public class CardResourceDataAsset : ScriptableObject
    {
        public List<Sprite> Spades;
        public List<Sprite> Clubs;
        public List<Sprite> Diamonds;
        public List<Sprite> Hearts;


        public Sprite GetCardSprite(int type, string id)
        {
            var sprite = (CardType)type switch
            {
                CardType.Spades => Spades.FirstOrDefault(s => s.name == id),
                CardType.Clubs => Clubs.FirstOrDefault(s => s.name == id),
                CardType.Diamonds => Diamonds.FirstOrDefault(s => s.name == id),
                CardType.Hearts => Hearts.FirstOrDefault(s => s.name == id),
                _ => null
            };
            return sprite;
        }
    }
}