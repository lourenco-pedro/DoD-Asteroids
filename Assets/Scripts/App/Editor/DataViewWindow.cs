using UnityEngine;
using UnityEditor;
using App.Data;
using App.Data.Definition;

namespace App.Editor
{
    public class DataViewWindow : EditorWindow
    {

        [MenuItem("App/Data View")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<DataViewWindow>("Data View");
        }

        private Vector2 scrollPos;

        void DrawPlayer(Player player)
        {
            EditorGUILayout.LabelField($"ID: {player.Id}");
            EditorGUILayout.LabelField($"Position: {player.position}");
            EditorGUILayout.LabelField($"Life: {player.life}");
            EditorGUILayout.LabelField($"Rotation: {player.rotation}");;
            EditorGUILayout.LabelField($"Velocity: {player.currentVelocity}");
        }

        void DrawAsteroid(Asteroid asteroid)
        {
            EditorGUILayout.LabelField($"ID: {asteroid.Id}");
            EditorGUILayout.LabelField($"Position: {asteroid.position}");
            EditorGUILayout.LabelField($"Is Disabled: {asteroid.IsDisabled}");
        }

        void DrawBullet(Bullet bullet)
        {
            EditorGUILayout.LabelField($"ID: {bullet.Id}");
            EditorGUILayout.LabelField($"Position: {bullet.position}");
        }

        void Update()
        {
            if(Application.isPlaying)
                Repaint();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Data Table Entries", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (DataType type in System.Enum.GetValues(typeof(DataType)))
            {
                var collection = DataTable.FromCollection(type);
                if (collection != null && collection.Ids != null)
                {
                    EditorGUILayout.LabelField($"{type} Entries ({collection.Ids.Length})", EditorStyles.boldLabel);
                    foreach (var id in collection.Ids)
                    {
                        var data = collection.WithId<IData>(id);
                        if (data != null)
                        {
                            EditorGUILayout.LabelField($"ID: {data.Id}");
                            if (data is Player player)
                                DrawPlayer(player);
                            else if (data is Asteroid asteroid)
                                DrawAsteroid(asteroid);
                            else if (data is Bullet bullet)
                                DrawBullet(bullet);
                        }

                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($"{type} has no entries.");
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
