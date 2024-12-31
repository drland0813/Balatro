using UnityEngine;
using UnityEngine.UI;

namespace MagicTouch.Scripts.Mics.Common
{
    public class ContentFitterRefresh : MonoBehaviour
    {
        public void Refresh()
        {
            var rectTransform = (RectTransform)transform;
            RefreshContent(rectTransform);
        }
 
        private void RefreshContent(RectTransform transform)
        {
            // if (transform == null || !transform.gameObject.activeSelf) return;

            foreach (RectTransform child in transform)
            {
                RefreshContent(child);
            }
 
            var layoutGroup = transform.GetComponent<LayoutGroup>();
            var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
            if (layoutGroup != null)
            {
                layoutGroup.SetLayoutHorizontal();
                layoutGroup.SetLayoutVertical();
            }
 
            if (contentSizeFitter != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
            }
        }
    }
}