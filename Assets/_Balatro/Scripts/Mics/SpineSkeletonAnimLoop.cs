// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Spine.Unity;

// public class SpineSkeletonAnimLoop : MonoBehaviour
// {
//     [SerializeField] float _startDelayMax;
//     [SerializeField] float _idleDurationMin;
//     [SerializeField] float _idleDurationMax;
//     [SerializeField] string _animationName;

//     protected SkeletonAnimation _spineAnimation;
//     Coroutine _loopCoroutine;

//     void Awake()
//     {
//         _spineAnimation = GetComponent<SkeletonAnimation>();
//     }

//     public void Play()
//     {
//         if (_loopCoroutine == null)
//         {
//             var startDelay = Random.Range(0f, _startDelayMax);
//             _loopCoroutine = StartCoroutine(PlayLoop(startDelay));
//         }
//     }

//     public void OnEnable()
//     {
//         if (_loopCoroutine != null)
//         {
//             _loopCoroutine = null;
//             Play();
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
//         var trackEntry = _spineAnimation.AnimationState.SetAnimation(0, _animationName, false);
//         var animDuration = trackEntry.Animation.Duration;
//         if (delayStart > 0f)
//         {
//             trackEntry.TrackTime = animDuration;
//             yield return new WaitForSeconds(delayStart);

//             _spineAnimation.AnimationState.SetAnimation(0, _animationName, false);
//         }

//         var idleDuration = Random.Range(_idleDurationMin, _idleDurationMax);
//         yield return new WaitForSeconds(animDuration + idleDuration);

//         _loopCoroutine = StartCoroutine(PlayLoop());
//     }
// }
