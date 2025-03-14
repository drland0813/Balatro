using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class DividerLine : MonoBehaviour
    {
        public GameObject dotPrefab;
        public int dotCount = 50;

        void Start()
        {
            for (int i = 0; i < dotCount; i++)
            {
                Instantiate(dotPrefab, transform);
            }
        }
    }
}
