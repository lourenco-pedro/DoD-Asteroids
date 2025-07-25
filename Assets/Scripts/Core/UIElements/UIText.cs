using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Core.UIElements
{
    public class UIText : UIElement<TextMeshProUGUI>
    {
        public void SetText(string text)
        {
            _element.SetText(text);
        }

        public void Hide()
        {
            SetText("");
        }

        public void DOPunch()
        {
            _element.transform.DOPunchScale(new UnityEngine.Vector2(1.01f, 1.01f), .1f).OnComplete(()=>
            {
                transform.localScale = Vector2.one;
            });
        }

        public void DOShake()
        {
            _element.transform.DOShakePosition(.3f);
        }      

        internal override void OnEnable()
        {
            base.OnEnable();
        }
    }
}
