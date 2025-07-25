using UnityEngine;

namespace Core
{
    public abstract class BaseWorldObject
    {
        public string Id => _id;
        

        public Vector2 Position
        {
            get => _gameObject.transform.position;
            set => _gameObject.transform.position = value;
        }

        public Quaternion Rotation
        {
            get => _gameObject.transform.rotation;
            set => _gameObject.transform.rotation = value;
        }

        public GameObject GameObject => _gameObject;

        public SpriteRenderer Render => _render;

        [SerializeField] internal string _id;
        [SerializeField] internal GameObject _gameObject;
        [SerializeField] internal SpriteRenderer _render;


        public BaseWorldObject(string id)
        {
            _id = id;
            _gameObject = new GameObject(id);
            _render = _gameObject.AddComponent<SpriteRenderer>();
        }

        public virtual void CheckDisabled()
        {
        }

        public virtual void Destroy()
        {
            if (_gameObject != null)
            {
                Object.Destroy(_gameObject);
                _gameObject = null;
            }
        }
    }
}