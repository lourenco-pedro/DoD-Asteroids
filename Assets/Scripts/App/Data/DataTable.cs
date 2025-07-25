using System;
using System.Collections.Generic;
using System.Linq;
using App.Data.Definition;
using UnityEngine;

namespace App.Data
{
    public static class DataTable
    {




        public class ManagedCollection
        {
            internal Dictionary<string, IData> Data;

            public string[] Ids => Data.Keys.ToArray();

            public void RemoveId(string id)
            {
                if (Data != null && Data.ContainsKey(id))
                {
                    Data.Remove(id);
                }
                else
                {
                    Debug.LogWarning($"Attempted to remove non-existing ID '{id}' from data collection.");
                }
            }

            public void Disable(string id)
            {
                if (Data.TryGetValue(id, out IData data))
                {
                    data.IsDisabled = true;
                }
                else
                {
                    Debug.LogWarning($"Attempted to disable non-existing ID '{id}' in data collection.");
                }
            }

            public T WithId<T>(string id)
                where T : class
            {
                if (Data.TryGetValue(id, out IData data))
                {
                    try
                    {
                        return data as T;
                    }
                    catch (System.InvalidCastException)
                    {
                        Debug.LogError($"Data with ID '{id}' is not of type '{typeof(T)}'. Actual type: '{data.GetType()}'.");
                        return null;
                    }
                }

                return null;
            }

            public void Register(IData data)
            {
                if (Data == null)
                {
                    Data = new Dictionary<string, IData>();
                }

                if (!Data.ContainsKey(data.Id))
                {
                    Data.Add(data.Id, data);
                }
            }

            public bool Pool<T>(out T data)
                where T : class
            {
                if (Data == null)
                {
                    Data = new Dictionary<string, IData>();
                }

                var disabledData = Data.Values.FirstOrDefault(d => d is T && d.IsDisabled);
                if (disabledData != null)
                {
                    disabledData.IsDisabled = false;
                    data = disabledData as T;
                    return true;
                }

                // No disabled data found, create a new one
                var newData = Activator.CreateInstance(typeof(T)) as IData;
                if (newData != null)
                {
                    newData.Id = "";
                    data = newData as T;
                    return false;
                }

                Debug.LogError($"Failed to create instance of type '{typeof(T)}'.");
                data = null;
                return false;
            }

            public T[] GetAll<T>(bool includeDisabled = false)
                where T : class
            {
                return Data.Values
                    .Where(d => d is T && (!d.IsDisabled || includeDisabled))
                    .Select(d => d as T)
                    .ToArray();
            }

            public void Clear()
            {
                Data.Clear(); 
            }
        }




        static private Dictionary<DataType, ManagedCollection> _dataTable = new Dictionary<DataType, ManagedCollection>();

        public static ManagedCollection FromCollection(DataType type)
        {

            if (!_dataTable.ContainsKey(type))
            {
                _dataTable[type] = new ManagedCollection();
                _dataTable[type].Data = new Dictionary<string, IData>();
            }

            return _dataTable[type];
        }

        public static void Clear()
        {
            foreach (var collection in _dataTable.Values)
            {
                collection.Data.Clear();
            }
            _dataTable.Clear();
        }

        public static void Clear(DataType type)
        {
            if (_dataTable.TryGetValue(type, out var table))
            {
                table.Clear();
            } 
        }
    }
}