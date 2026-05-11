using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Worker
{
    public class WorkerAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void OnStateChanged(WorkerState state)
        {
            if (animator == null) return;

            switch (state)
            {
                case WorkerState.Idle:
                case WorkerState.MovingToSlot:
                case WorkerState.MovingToCustomer:
                    animator.Play("Move");
                    break;
                case WorkerState.Harvesting:
                case WorkerState.Delivering:
                    animator.Play("Idle");
                    break;
            }
        }

        public void PlayIdle()
        {
            animator?.Play("Idle");
        }
    }
}