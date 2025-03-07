using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class PokerHandsContentController : MonoBehaviour
    {
        [SerializeField] private Transform contentPanel;
        [SerializeField] private GameObject pokerHandPrefab;
        private List<PokerHand> pokerHands;

        void Start()
        {

            pokerHands = PokerHandManager.GetInstance().GetPokerHands();
            PopulatePokerHandList();
        }

        public void PopulatePokerHandList()
        {
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            int count = Mathf.Min(pokerHands.Count, 9);

            for (int i = 0; i < count; i++)
            {
                PokerHand hand = pokerHands[i];

                GameObject newItem = Instantiate(pokerHandPrefab, contentPanel);
                PokerHandsInfoItem itemUI = newItem.GetComponent<PokerHandsInfoItem>();

                if (itemUI != null)
                {
                    itemUI.SetPokerHandData(hand);
                }
            }
        }
    }
}
