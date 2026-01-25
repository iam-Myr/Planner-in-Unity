using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Planning;

namespace BlocksWorld
{
    public class Block : PlanObject, IObservableHolder
    {
        private string blockName;
        private Block above;
        private Block below;

        // Raycast parameters
        public float rayDistance = 1f;
        public float rayOffset = 0.6f; //offset so we don't hit ourselves                                
        private LayerMask blockLayer; // Only detect objects in this layer 


        protected virtual void Awake()
        {
            Register();
            SetPredicateConditions();

            // Raycast stuff
            blockLayer = LayerMask.GetMask("Block");
            if (blockLayer == 0)
                Debug.LogError("Error: Layer 'Block' does not exist! Please add it in the Unity Editor.");
        }

        // Register this block with the ObservationManager
        public void Register()
        {
            ObservationManager.Register(this);
        }

        // Connect block's instance methods to domain predicates
        public void SetPredicateConditions()
        {
            BlockDomain.isClear.SetCondition(isClear);
            BlockDomain.isOn.SetCondition(isOn);
        }

        // Dynamically generate observable predicates
        public List<Predicate> GetObservablePredicates()
        {
            List<Predicate> observables = new List<Predicate>();

            // Add this block's isClear predicate
            observables.Add(new Predicate(isClear, new List<Pointer> { new Pointer(this) }));

            // Add all isOn predicates for all blocks
            foreach (Pointer p in BlockDomain.AllPointers)
            {
                if (p.value is Block other && other != this)
                {
                    observables.Add(new Predicate(isOn, new List<Pointer> { new Pointer(this), new Pointer(other) }));
                }
            }

            return observables;
        }

        /// Returns true if there is no block directly above this block.
        public bool isClear(List<object> args)
        { 
          
            RaycastHit2D up = Physics2D.Raycast(transform.position + Vector3.up * rayOffset, Vector2.up, rayDistance, blockLayer);
            return up.collider == null;
        }



        /// Returns true if this block is directly on top of the given other block.
        public bool isOn(List<object> args)
        {
            if (args.Count != 2 || !(args[1] is Block otherBlock))
                return false;
            RaycastHit2D down = Physics2D.Raycast(transform.position + Vector3.down * rayOffset, Vector2.down, rayDistance, blockLayer);
            if (down.collider != null && down.collider.gameObject == otherBlock.gameObject)
                return true;
            return false;
        }

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

        public Vector3 GetPosition() => transform.position;

        void OnDrawGizmos()
        {
            // Down ray (red)
            Vector3 downStart = transform.position + Vector3.down * rayOffset;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(downStart, downStart + Vector3.down * rayDistance);

            // Up ray (green)
            Vector3 upStart = transform.position + Vector3.up * rayOffset;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(upStart, upStart + Vector3.up * rayDistance);
        }
    }
}
