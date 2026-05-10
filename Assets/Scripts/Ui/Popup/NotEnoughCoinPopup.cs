using System.Collections;
using TMPro;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class NotEnoughCoinPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float autoDismiss = 1.5f;

        public override void OnInit()
        {
            if (messageText != null)
                messageText.text = "Not enough coins!";
        }

        public void Setup()
        {
            ShowFullscreen();
            StopAllCoroutines();
            StartCoroutine(AutoDismiss());
        }

        private IEnumerator AutoDismiss()
        {
            yield return new WaitForSeconds(autoDismiss);
            Hide();
        }
    }
}