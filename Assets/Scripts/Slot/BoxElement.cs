using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class BoxElement: MonoBehaviour
    {
        [SerializeField] Animation animations;

        public void OnOpen()
        {
            gameObject.SetActive(true);
            animations.Play("BoxOpen");
        }
        public void OnClose()
        {
            gameObject.SetActive(false);
            // animator.Play("");
        }
    }
}