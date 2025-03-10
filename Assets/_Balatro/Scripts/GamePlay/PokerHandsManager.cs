using System;
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

        private string _savePath;

        public List<PokerHand> GetPokerHands()
        {
            return _pokerHands;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _savePath = Path.Combine(Application.persistentDataPath, "PokerHands.json");
                Debug.Log(_savePath);
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
            if (resetProgress || !File.Exists(_savePath))
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
            string json = File.ReadAllText(_savePath);
            _pokerHands = JsonUtility.FromJson<PokerHandsConfig>(json).hands;
        }

        public void SaveProgress()
        {
            string json = JsonUtility.ToJson(new PokerHandsConfig { hands = _pokerHands });
            File.WriteAllText(_savePath, json);
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

        public static List<CardData> GenerateHandByType(PokerHandType type, List<CardData> availableCards, int numberCards)
        {
            List<CardData> hand = new List<CardData>();
            List<CardData> remainingCards = new List<CardData>(availableCards);
            System.Random rng = new System.Random();

            void AddCardsWithSameValue(int value, int count)
            {
                var matchingCards = remainingCards.Where(card => card.Value == value).Take(count).ToList();
                hand.AddRange(matchingCards);
                remainingCards.RemoveAll(card => matchingCards.Contains(card));
            }

            if (type == PokerHandType.TwoPair)
            {
                var valueGroups = remainingCards.GroupBy(c => c.Value).Where(g => g.Count() >= 2).ToList();
                if (valueGroups.Count >= 2)
                {
                    AddCardsWithSameValue(valueGroups[0].Key, 2);
                    AddCardsWithSameValue(valueGroups[1].Key, 2);
                }
            }
            else if (type == PokerHandType.ThreeOfAKind)
            {
                var triplet = remainingCards.GroupBy(c => c.Value).FirstOrDefault(g => g.Count() >= 3);
                if (triplet != null) AddCardsWithSameValue(triplet.Key, 3);
            }
            else if (type == PokerHandType.FullHouse)
            {
                var triplet = remainingCards.GroupBy(c => c.Value).FirstOrDefault(g => g.Count() >= 3);
                var pair = remainingCards.GroupBy(c => c.Value).FirstOrDefault(g => g.Count() >= 2 && g.Key != triplet?.Key);

                if (triplet != null && pair != null)
                {
                    AddCardsWithSameValue(triplet.Key, 3);
                    AddCardsWithSameValue(pair.Key, 2);
                }
            }
            else if (type == PokerHandType.FourOfAKind)
            {
                var quad = remainingCards.GroupBy(c => c.Value).FirstOrDefault(g => g.Count() >= 4);
                if (quad != null) AddCardsWithSameValue(quad.Key, 4);
            }
            else if (type == PokerHandType.Straight)
            {
                var sortedValues = remainingCards.Select(c => c.Value).Distinct().OrderBy(v => v).ToList();
                for (int i = 0; i <= sortedValues.Count - 5; i++)
                {
                    if (sortedValues[i + 4] - sortedValues[i] == 4)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            hand.Add(remainingCards.First(c => c.Value == sortedValues[i + j]));
                        }
                        break;
                    }
                }
            }
            else if (type == PokerHandType.Flush)
            {
                var suitGroup = remainingCards.GroupBy(c => c.Type).FirstOrDefault(g => g.Count() >= 5);
                if (suitGroup != null) hand.AddRange(suitGroup.Take(5));
            }

            while (hand.Count < numberCards && remainingCards.Count > 0)
            {
                hand.Add(remainingCards[rng.Next(remainingCards.Count)]);
                remainingCards.Remove(hand.Last());
            }

            return hand;
        }

        public static List<Card> GetHandCards(List<Card> playerHandCards, PokerHandType handType)
        {
            var result = new List<Card>();
            var cardValues = playerHandCards.Select(card => card.GetCardData().Value).ToList();
            var cardSuits = playerHandCards.Select(card => (CardType)card.GetCardData().Type).ToList();

            var valueGroups = playerHandCards
                .GroupBy(card => card.GetCardData().Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var suitGroups = playerHandCards
                .GroupBy(card => (CardType)card.GetCardData().Type)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<Card> GetCardsForCount(int count)
            {
                return valueGroups
                    .Where(pair => pair.Value.Count == count)
                    .SelectMany(pair => pair.Value)
                    .ToList();
            }

            switch (handType)
            {
                case PokerHandType.Pair:
                    return GetCardsForCount(2);
                case PokerHandType.TwoPair:
                    return valueGroups
                        .Where(pair => pair.Value.Count == 2)
                        .OrderByDescending(pair => pair.Key)
                        .Take(2)
                        .SelectMany(pair => pair.Value)
                        .ToList();
                case PokerHandType.ThreeOfAKind:
                    return GetCardsForCount(3);
                case PokerHandType.FourOfAKind:
                    return GetCardsForCount(4);
                case PokerHandType.FiveOfAKind:
                    return GetCardsForCount(5);
                case PokerHandType.FullHouse:
                    var three = valueGroups.FirstOrDefault(g => g.Value.Count == 3).Value;
                    var pair = valueGroups.FirstOrDefault(g => g.Value.Count == 2).Value;
                    if (three != null && pair != null)
                        return three.Concat(pair).ToList();
                    break;
                case PokerHandType.Flush:
                    return suitGroups.Values.FirstOrDefault(g => g.Count == 5);
                case PokerHandType.Straight:
                case PokerHandType.StraightFlush:
                    var sorted = playerHandCards.OrderBy(c => c.GetCardData().Value).ToList();
                    if (IsStraight(sorted.Select(c => c.GetCardData().Value).ToList()))
                        return sorted;
                    break;
                case PokerHandType.FlushFullHouse:
                    // Prioritize flush, then full house inside flush
                    var flushCards = suitGroups.Values.FirstOrDefault(g => g.Count == 5);
                    if (flushCards != null)
                    {
                        var flushValueGroups = flushCards.GroupBy(c => c.GetCardData().Value)
                                                         .ToDictionary(g => g.Key, g => g.ToList());
                        var threeFlush = flushValueGroups.FirstOrDefault(g => g.Value.Count == 3).Value;
                        var pairFlush = flushValueGroups.FirstOrDefault(g => g.Value.Count == 2).Value;
                        if (threeFlush != null && pairFlush != null)
                            return threeFlush.Concat(pairFlush).ToList();
                    }
                    break;
                case PokerHandType.FlushFiveOfAKind:
                    foreach (var group in suitGroups)
                    {
                        var flushValueGroups = group.Value.GroupBy(c => c.GetCardData().Value)
                                                          .Where(g => g.Count() == 5)
                                                          .FirstOrDefault();
                        if (flushValueGroups != null)
                            return flushValueGroups.ToList();
                    }
                    break;
                case PokerHandType.HighCard:
                case PokerHandType.None:
                default:
                    return playerHandCards.OrderByDescending(c => c.GetCardData().Value).Take(1).ToList();
            }

            return result;
        }
    }

}
