using UnityEngine;

namespace Balatro
{
    public class CardSlot : MonoBehaviour
    {
        public Vector2 CardPosition;
        public Vector3 CardLocalRotation;

        public void SetCardTransform(Vector2 pos, Vector3 rot)
        {
            CardPosition = pos;
            CardLocalRotation = rot;
        }
    }
}