using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Balatro
{
    public enum PokerHandType
    {
        None = 0,
        HighCard = 1,
        Pair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        Straight = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        FiveOfAKind = 9,
        StraightFlush = 10,
        FlushFullHouse = 11,
        FlushFiveOfAKind = 12
    }

    public class PokerHandManager : MonoBehaviour
    {
        public static PokerHandManager Instance { get; private set; }

        private List<PokerHand> _pokerHands;
        private string savePath;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                savePath = Path.Combine(Application.persistentDataPath, "PokerHandsSave.json");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static PokerHandManager GetInstance()
        {
            if (Instance == null)
            {
                GameObject obj = new GameObject("PokerHandManager");
                Instance = obj.AddComponent<PokerHandManager>();

                DontDestroyOnLoad(obj);
            }
            return Instance;
        }

        public void StartNewGame(bool resetProgress = false)
        {
            if (resetProgress || !File.Exists(savePath))
            {
                LoadPokerHandsFromJson();
            }
            else
            {
                LoadPokerHandsFromSave();
            }
        }

        private void LoadPokerHandsFromJson()
        {
            _pokerHands = new List<PokerHand>();
            var pokerHandsData = Resources.Load<TextAsset>("Config/PokerHands/PokerHands");
            if (pokerHandsData != null)
            {
                _pokerHands = JsonUtility.FromJson<PokerHandsConfig>(pokerHandsData.text).hands;
                ResetAllLevels();
            }
            else
            {
                Debug.LogError("PokerHands.json not found in Resources/Config/");
            }
        }

        private void LoadPokerHandsFromSave()
        {
            string json = File.ReadAllText(savePath);
            _pokerHands = JsonUtility.FromJson<PokerHandsConfig>(json).hands;
        }

        public void SaveProgress()
        {
            string json = JsonUtility.ToJson(new PokerHandsConfig { hands = _pokerHands });
            File.WriteAllText(savePath, json);
        }

        public void LevelUpHand(int id)
        {
            PokerHand hand = GetHandById(id);
            if (hand != null)
            {
                hand.LevelUp();
                SaveProgress();
            }
        }

        public PokerHand GetHandById(int id)
        {
            if (id == (int)PokerHandType.None) return null;
            return _pokerHands.Find(hand => hand.id == id);
        }

        public void ResetAllLevels()
        {
            foreach (var hand in _pokerHands)
            {
                hand.ResetLevel();
            }
        }
    }

    public class PokerHandChecker : MonoBehaviour
    {
        public static PokerHandType CheckHand(List<Card> playerHandCards)
        {
            if (playerHandCards == null || playerHandCards.Count == 0)
            {
                return PokerHandType.None;
            }

            // Get card data
            var cardValues = playerHandCards.Select(card => card.GetCardData().Value).ToList();
            var cardSuits = playerHandCards.Select(card => card.GetCardData().Type).ToList(); // Type represents the suit

            // Count occurrences of each card value
            var valueCounts = cardValues.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());

            // Check the number of identical cards
            bool hasPair = valueCounts.ContainsValue(2);
            bool hasTwoPair = valueCounts.Values.Count(v => v == 2) == 2;
            bool hasThreeOfAKind = valueCounts.ContainsValue(3);
            bool hasFourOfAKind = valueCounts.ContainsValue(4);
            bool hasFiveOfAKind = valueCounts.ContainsValue(5);
            bool hasFullHouse = hasThreeOfAKind && hasPair;

            // Check for a Straight (5 consecutive cards, different suits)
            bool isStraight = IsStraight(cardValues);

            // Check for a Flush (5 cards of the same suit)
            bool isFlush = cardSuits.Distinct().Count() == 1 && playerHandCards.Count == 5;

            // Determine the poker hand and return the corresponding type
            if (playerHandCards.Count == 5)
            {
                if (isFlush && isStraight) return PokerHandType.StraightFlush;
                if (isFlush && hasFullHouse) return PokerHandType.FlushFullHouse;
                if (isFlush && hasFiveOfAKind) return PokerHandType.FlushFiveOfAKind;
                if (hasFiveOfAKind) return PokerHandType.FiveOfAKind;
                if (hasFourOfAKind) return PokerHandType.FourOfAKind;
                if (hasFullHouse) return PokerHandType.FullHouse;
                if (isFlush) return PokerHandType.Flush;
                if (isStraight) return PokerHandType.Straight;
                if (hasThreeOfAKind) return PokerHandType.ThreeOfAKind;
                if (hasTwoPair) return PokerHandType.TwoPair;
                if (hasPair) return PokerHandType.Pair;
            }
            else if (playerHandCards.Count == 4)
            {
                if (hasFourOfAKind) return PokerHandType.FourOfAKind;
                if (hasThreeOfAKind) return PokerHandType.ThreeOfAKind;
                if (hasTwoPair) return PokerHandType.TwoPair;
                if (hasPair) return PokerHandType.Pair;
            }
            else if (playerHandCards.Count == 3)
            {
                if (hasThreeOfAKind) return PokerHandType.ThreeOfAKind;
                if (hasPair) return PokerHandType.Pair;
            }
            else if (playerHandCards.Count == 2)
            {
                if (hasPair) return PokerHandType.Pair;
            }

            return PokerHandType.HighCard;
        }

        // Check if the list of card values forms a straight
        private static bool IsStraight(List<int> cardValues)
        {
            var sortedValues = cardValues.Distinct().OrderBy(v => v).ToList();
            if (sortedValues.Count < 5) return false;

            for (int i = 0; i <= sortedValues.Count - 5; i++)
            {
                if (sortedValues[i + 4] - sortedValues[i] == 4)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
