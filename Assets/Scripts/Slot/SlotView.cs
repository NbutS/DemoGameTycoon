using System;
using System.Collections;
using Assembly_CSharp.Assets.Scripts.Ui.Popup;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private BoxElement box;
        [SerializeField] private TreeElement tree;
        [SerializeField] private SlotWorldUI slotWorldUI;

        public event Action OnReadyTriggered;

        public void Init(SlotData data)
        {
            slotWorldUI.Setup(data);
            box.gameObject.SetActive(true);
            tree.gameObject.SetActive(false);
        }

        public void OnStateChanged(SlotStateType state, SlotData data, float cooldown, float openDuration, MonoBehaviour runner)
        {
            switch (state)
            {
                case SlotStateType.Opening:
                    runner.StartCoroutine(OpenRoutine(openDuration, cooldown));
                    break;
                case SlotStateType.NotReady:
                    runner.StartCoroutine(RegrowRoutine(cooldown));
                    break;
                case SlotStateType.Ready:
                    tree.OnSpawnApple();
                    slotWorldUI.Show();
                    slotWorldUI.Refresh(data);
                    break;
            }
        }

        public void RefreshUI(SlotData data) => slotWorldUI.Refresh(data);

        private IEnumerator OpenRoutine(float openDuration, float cooldown)
        {
            box.OnOpen();
            yield return new WaitForSeconds(openDuration);
            box.OnClose();

            tree.gameObject.SetActive(true);
            tree.transform.localScale = Vector3.zero;
            yield return AnimateScale(tree.gameObject, Vector3.zero, Vector3.one, 0.4f);
            tree.OnOpen();

            yield return new WaitForSeconds(cooldown);
            OnReadyTriggered?.Invoke();
        }

        private IEnumerator RegrowRoutine(float cooldown)
        {
            tree.OnHideApple();
            yield return new WaitForSeconds(cooldown);
            OnReadyTriggered?.Invoke();
        }

        private IEnumerator AnimateScale(GameObject target, Vector3 from, Vector3 to, float duration)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                target.transform.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }
            target.transform.localScale = to;
        }
    }

    public static class SlotClickHandler
    {
        public static void Handle(BaseSlot slot, SlotStateType state, PopupManager popupManager)
        {
            switch (state)
            {
                case SlotStateType.Close:
                    popupManager.Get<UnlockSlotPopup>(PopupType.UnlockSlot).Setup(slot);
                    break;
                case SlotStateType.Ready:
                case SlotStateType.NotReady:
                    popupManager.Get<UpgradeSlotPopup>(PopupType.UpgradeSlot).Setup(slot);
                    break;
            }
        }
    }

    
    public class FaceCamera : MonoBehaviour
    {
        private Camera _cam;
        private void Start() => _cam = Camera.main;
        private void LateUpdate() => transform.rotation = _cam.transform.rotation;
    }
}