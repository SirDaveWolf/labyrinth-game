using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameplayStatics
    {
        public static IEnumerator MoveToPosition(Transform transform, Vector3 targetPosition, float timeToMove, System.Action OnMoveDone = null)
        {
            var currentPos = transform.position;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;
                transform.position = Vector3.Lerp(currentPos, targetPosition, t);
                yield return null;
            }

            OnMoveDone?.Invoke();
        }

        public static float RoundToNearest10(float value)
        {
            return ((int)Math.Round(value / 10.0)) * 10;
        }

        public static bool WithinXZPlane(Transform source, Transform target, float? size = null)
        {
            var sourceX = source.position.x;
            var sourceZ = source.position.z;
            var sourceW = size.HasValue ? size.Value / 2 : source.localScale.x / 2;
            var sourceH = size.HasValue ? size.Value / 2 : source.lossyScale.y / 2;

            var targetX = target.position.x;
            var targetZ = target.position.z;
            var targetW = size.HasValue ? size.Value / 2 : target.lossyScale.y;
            var targetH = size.HasValue ? size.Value / 2 : target.lossyScale.y / 2;

            if (sourceX + sourceW < targetX - targetW ||
               sourceX - sourceW > targetX + targetW ||
               sourceZ + sourceH < targetZ - targetH ||
               sourceZ - sourceH > targetZ + targetH)
                return false;

            return true;
        }
    }
}
