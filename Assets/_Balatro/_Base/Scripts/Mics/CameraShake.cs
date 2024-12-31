// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
//
// public class CameraShake : MonoBehaviour
// {
//     public static CameraShake Instance;
//
//     private void Awake()
//     {
//         Instance = this;
//     }
//
//     public void ShakeCamera(float duration, float strength = 12, int vibrato = 15)
//     {
//         transform.DOKill(true);
//         transform.DOShakePosition(duration, strength, vibrato, 30, false, true, ShakeRandomnessMode.Harmonic);
//     }
// }