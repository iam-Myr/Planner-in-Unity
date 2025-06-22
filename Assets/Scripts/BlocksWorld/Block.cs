using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Planning;

namespace BlocksWorld
{
    public class Block : PlanObject
    {
        public string blockName;
        public Block above;
        public Block below;

        public async Task MoveToAsync(Vector3 targetPosition, float speed = 2f)
        {
            var tcs = new TaskCompletionSource<bool>();
            StartCoroutine(MoveSmoothly(targetPosition, speed, tcs));
            await tcs.Task;
        }

        private IEnumerator MoveSmoothly(Vector3 targetPosition, float speed, TaskCompletionSource<bool> tcs)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
                yield return null;
            }

            transform.position = targetPosition; // Snap to exact position
            tcs.SetResult(true);
        }


        public bool isClear() => above == null;
        public Block GetAbove() => above;
        public Block GetBelow() => below;
        public void SetAbove(Block x) => above = x;
        public void SetBelow(Block x) => below = x;
        public override string ToString() => blockName;

    }
}
