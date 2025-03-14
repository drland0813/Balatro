using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balatro
{
    public class RewardItem : MonoBehaviour
    {
        [Header("Leading Display")]
        public GameObject leadingImageObj;  //  Image
        public GameObject leadingTextObj;   //  Text

        [Header("Info Texts")]
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI rewardMoneyText;

        public void Setup(Reward reward)
        {
            bool useImage = reward.Type == RewardType.TypeImage;

            // Set active tương ứng
            leadingImageObj.SetActive(useImage);
            leadingTextObj.SetActive(!useImage);

            if (useImage)
            {
                Image img = leadingImageObj.GetComponent<Image>();
                if (img != null && reward.Icon != null)
                    img.sprite = reward.Icon;
            }
            else
            {
                TextMeshProUGUI text = leadingTextObj.GetComponent<TextMeshProUGUI>();
                
                if (reward.Type == RewardType.RemainingHands)
                {
                    Color targetColor;
                    if (ColorUtility.TryParseHtmlString("#1c7ac6", out targetColor))
                    {
                        text.color = targetColor;
                    }
                }
                
                if (text != null)
                    text.text = reward.LeadingText;
            }

            descriptionText.text = reward.Description;
            rewardMoneyText.text = new string('$', reward.Value);
        }
    }
}
