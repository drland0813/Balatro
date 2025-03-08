using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balatro
{
    public class RunInfoView : MonoBehaviour
    {
        void OnEnable()
        {
            // Đưa RunInfoView lên trên cùng khi xuất hiện
            transform.SetAsLastSibling();
        }
    }
}
