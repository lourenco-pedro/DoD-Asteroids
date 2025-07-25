using UnityEngine;

namespace Core
{
    public class UIElement : MonoBehaviour
    {
        public string Id => _id;
        public GameObject GameObject => gameObject;
        [SerializeField] private string _id;
        [SerializeField] private bool _disableOnAwake;
        [SerializeField] private bool _persistEvenDisabled;

        void Awake()
        {
            UITable.RegisterUIElement(this);
            gameObject.SetActive(!_disableOnAwake);
        }

        internal virtual void OnEnable()
        {
            UITable.RegisterUIElement(this);
        }

        internal virtual void OnDisable()
        {
            if(!_persistEvenDisabled)
                UITable.RemoveUIElement(this);
        }        
    }
    public abstract class UIElement<T> : UIElement
        where T: Object
    {
        [SerializeField] protected T _element;
    }
}
