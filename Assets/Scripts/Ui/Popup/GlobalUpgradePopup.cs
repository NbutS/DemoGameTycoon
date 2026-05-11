using System.Collections.Generic;
using Assembly_CSharp.Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class GlobalUpgradePopup : BasePopup
    {
        [SerializeField] private List<UpgradeOptionView> optionViews;
        [SerializeField] private Button                  closeButton;

        public override void OnInit()
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => GameController.Instance.PopupManager.HideCurrent());
        }

        public void Setup()
        {
            foreach (var option in optionViews)
                option.OnInit();
            ShowFullscreen();
        }
    }
}