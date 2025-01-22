// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Spine.Unity;

// public class SpineAnimLoop : MonoBehaviour
// {
//     [SerializeField] float _startDelayMax;
//     [SerializeField] float _idleDurationMin;
//     [SerializeField] float _idleDurationMax;

//     SkeletonGraphic _spineGraphic;
//     Coroutine _loopCoroutine;

//     void Awake()
//     {
//         _spineGraphic = GetComponent<SkeletonGraphic>();
//     }

//     public void Play()
//     {
//         if (_loopCoroutine == null)
//         {
//             var startDelay = Random.Range(0f, _startDelayMax);
//             _loopCoroutine = StartCoroutine(PlayLoop(startDelay));
//         }
//     }

//     public void Stop()
//     {
//         if (_loopCoroutine != null)
//         {
//             StopCoroutine(_loopCoroutine);
//             _loopCoroutine = null;
//         }
//     }

//     IEnumerator PlayLoop(float delayStart = 0f)
//     {
//         _spineGraphic.freeze = false;
//         _spineGraphic.startingLoop = false;
//         var trackEntry = _spineGraphic.AnimationState.SetAnimation(0, _spineGraphic.startingAnimation, false);
//         var animDuration = trackEntry.Animation.Duration;
//         if (delayStart > 0f)
//         {
//             trackEntry.TrackTime = animDuration;
//             yield return new WaitForSeconds(delayStart);

//             _spineGraphic.AnimationState.SetAnimation(0, _spineGraphic.startingAnimation, false);
//         }

//         var idleDuration = Random.Range(_idleDurationMin, _idleDurationMax);
//         yield return new WaitForSeconds(animDuration + idleDuration);

//         _loopCoroutine = StartCoroutine(PlayLoop());
//     }
// }
