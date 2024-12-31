using System.Collections;
using UnityEngine;

namespace MagicTouch.Scripts.Mics.Common
{
    public static class WaitUtils
    {
        public static IEnumerator Wait(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        public static IEnumerator WaitToDo(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}