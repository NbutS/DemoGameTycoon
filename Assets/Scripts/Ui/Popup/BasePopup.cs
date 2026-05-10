using UnityEngine;

namespace Assembly_CSharp.Assets.Scripts.Ui.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] protected RectTransform rectTransform;
        [SerializeField] protected Canvas canvas;

        private bool _isAnchored;

        public virtual void OnInit() { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowAtWorldPosition(Vector3 worldPosition, Vector2 offset = default)
        {
            gameObject.SetActive(true);
            SetPositionAboveObject(worldPosition, offset);
        }

        public void ShowFullscreen()
        {
            gameObject.SetActive(true);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
        }
        protected void SetPositionAboveObject(Vector3 worldPosition, Vector2 offset = default)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Đảm bảo pivot popup ở center
            rectTransform.pivot = new Vector2(0.5f, 0f);

            // Chuyển screen position sang viewport (0-1)
            Vector2 viewportPos = new Vector2(
                screenPos.x / Screen.width,
                screenPos.y / Screen.height
            );

            // Chuyển viewport sang anchor position trong Canvas
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;

            rectTransform.anchorMin = viewportPos;
            rectTransform.anchorMax = viewportPos;
            rectTransform.anchoredPosition = offset;
        }
    }
    public enum PopupType
    {
        UnlockSlot,
        UpgradeSlot,
        GlobalUpgrade,
        NotEnoughCoin,
    }
}