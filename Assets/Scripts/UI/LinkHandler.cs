using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maihem.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class LinkHandler : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ClickOnLinkEvent(string keyword);

        [SerializeField] private Camera cameraToUse;
        private Canvas _canvas;
        private TMP_Text _textBox;

        private void Awake()
        {
            _textBox = GetComponent<TMP_Text>();
            _canvas = GetComponentInParent<Canvas>();

            cameraToUse = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
            var linkTaggedText = TMP_TextUtilities.FindIntersectingLink(_textBox, mousePosition, cameraToUse);
            if (linkTaggedText == -1) return;
            var linkInfo = _textBox.textInfo.linkInfo[linkTaggedText];

            var linkID = linkInfo.GetLinkID();
            if (linkID.Contains("https"))
            {
                Application.OpenURL(linkID);
                return;
            }

            Debug.Log("Okay");
            OnClickedOnLinkEvent?.Invoke(linkInfo.GetLinkText());
        }

        public static event ClickOnLinkEvent OnClickedOnLinkEvent;
    }
}