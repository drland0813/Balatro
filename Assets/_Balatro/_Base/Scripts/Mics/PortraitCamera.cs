using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitCamera : MonoBehaviour
{
    void Awake()
    {
        var camera = GetComponent<Camera>();
        if (camera.aspect < 9f / 16)
        {
            var h = 720f / camera.aspect;
            camera.orthographicSize = h * 0.5f / 100f;
        }
    }
}
