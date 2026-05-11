using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Customer
{
    public class CustomerAnimatorController : UnityEngine.MonoBehaviour
    {
        [SerializeField] private UnityEngine.Animator animator;

        public void OnStateChanged(CustomerStateType state)
        {
            if (animator == null) return;

            switch (state)
            {
                case CustomerStateType.MovingToCounter:
                case CustomerStateType.MoveOut:
                    animator.Play("Move");
                    break;
                case CustomerStateType.Idle:
                case CustomerStateType.Waiting:
                case CustomerStateType.ReceiveGood:
                    animator.Play("Idle");
                    break;
            }
        }
    }
}