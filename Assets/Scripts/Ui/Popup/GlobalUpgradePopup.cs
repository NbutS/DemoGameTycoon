using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class GlobalUpgradePopup : BasePopup
    {
        [SerializeField] private List<UpgradeOptionView> optionViews;
        [SerializeField] private Button closeButton;

        public override void OnInit()
        {
            closeButton.onClick.AddListener(PopupManager.Instance.HideCurrent);
            foreach (var option in optionViews)
                option.OnInit();
        }

        public void Setup()
        {
            ShowFullscreen();
        }
    }
}