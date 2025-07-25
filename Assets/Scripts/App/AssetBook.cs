using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace App
{
    public abstract class AssetBook : ScriptableObject
    {
        public static T Get<T>(string id)
            where T : AssetBook

        {
            return Resources.Load<T>(id);
        }
    }

    public abstract class AssetBook<T> : AssetBook
        where T : Object
    {
        [SerializeField] SerializedDictionary<string, T> _assets = new SerializedDictionary<string, T>();

        public T GetAsset(string id)
        {
            if (_assets.TryGetValue(id, out T asset))
            {
                return asset;
            }
            else
            {
                Debug.LogError($"Asset with name {id} not found in AssetBook.");
                return default;
            }
        }
    }


}
