using System.Collections.Generic;
using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private List<PopupEntry> popupEntries;

        [System.Serializable]
        private class PopupEntry
        {
            public PopupType type;
            public BasePopup popup;
        }

        private Dictionary<PopupType, BasePopup> _popups = new();
        private BasePopup _current;

        public void OnInit()
        {
            foreach (var entry in popupEntries)
            {
                entry.popup.OnInit();
                entry.popup.Hide();
                _popups[entry.type] = entry.popup;
            }
        }

        public T Get<T>(PopupType type) where T : BasePopup
        {
            _current?.Hide();
            if (_popups.TryGetValue(type, out var popup))
            {
                _current = popup;
                return popup as T;
            }
            return null;
        }

        public void HideCurrent() => _current?.Hide();

        public void Hide(PopupType type)
        {
            if (_popups.TryGetValue(type, out var popup))
                popup.Hide();
        }
    }
}