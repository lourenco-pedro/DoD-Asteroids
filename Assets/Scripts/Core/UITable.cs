using System;
using System.Collections.Generic;

namespace Core
{
    public static class UITable
    {
        private static Dictionary<string, UIElement> _elements = new Dictionary<string, UIElement>();

        public static UIElement RegisterUIElement(UIElement element)
        {
            if (_elements.TryAdd(element.Id, element))
            {
                return element;
            }

            return null;
        }

        public static UIElement<T> Get<T>(string id)
            where T : UnityEngine.Object
        {
            if (_elements.TryGetValue(id, out UIElement element))
            {
                return element as UIElement<T>;
            }

            return null;
        }

        public static UIElement Get(string id)
        {
            if (_elements.TryGetValue(id, out UIElement element))
            {
                return element;
            }

            return null;
        }

        public static TUIElement Get<TUIElement, T>(string id)
            where T : UnityEngine.Object
            where TUIElement : UIElement<T>
        {
            if (_elements.TryGetValue(id, out UIElement element))
            {
                return element as TUIElement;
            }

            return null;
        }

        public static void RemoveUIElement(UIElement element, bool alsoDestroy = false)
        {
            _elements.Remove(element.Id);
            if (alsoDestroy)
                UnityEngine.Object.Destroy(element.gameObject);
        }

        public static void Clear()
        {
            _elements.Clear();
        }
    }
}