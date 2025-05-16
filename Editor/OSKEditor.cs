using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OSK.SoundManager
{
    public class OSKEditor : MonoBehaviour
    {
        [MenuItem("OSK-Framework/Install Dependencies/Dotween", false,4)]
        public static void InstallDependenciesDotween()
        {
            AddPackage("https://github.com/O-S-K/DOTween.git");
        }

        [MenuItem("OSK-Framework/Install Dependencies/UIFeel", false,4)]
        public static void InstallDependenciesUIFeel()
        {
            AddPackage("https://github.com/O-S-K/UIFeel.git");
        }

        [MenuItem("OSK-Framework/Install Dependencies/DevConsole", false,4)]
        public static void InstallDependenciesDevConsole()
        {
            AddPackage("https://github.com/O-S-K/DevConsole.git");
        }

        [MenuItem("OSK-Framework/Install Dependencies/Observable", false,4)]
        public static void InstallDependenciesObservable()
        {
            AddPackage("https://github.com/O-S-K/OSK-Observable");
        }
 
        [MenuItem("OSK-Framework/SO Files/List Sound")]
        public static void LoadListSound()
        {
            FindSoundSOAssets();
        }
 
        private static void FindSoundSOAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:ListSoundSO");
            if (guids.Length == 0)
            {
                Debug.LogError("No SoundSO found in the project.");
                return;
            }

            var soundData = new List<ListSoundSO>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ListSoundSO v = AssetDatabase.LoadAssetAtPath<ListSoundSO>(path);
                soundData.Add(v);
            }

            if (soundData.Count == 0)
            {
                Debug.LogError("No SoundSO found in the project.");
            }
            else
            {
                foreach (var v in soundData)
                {
                    Debug.Log("SoundSO found: " + v.name);
                    Selection.activeObject = v;
                    EditorGUIUtility.PingObject(v);
                }
            }
        }


        private static void AddPackage(string packageName)
        {
            UnityEditor.PackageManager.Client.Add(packageName);
            UnityEditor.EditorUtility.DisplayDialog("OSK-Framework", "Package added successfully", "OK");
            UnityEditor.AssetDatabase.Refresh();
        }

        private static void UpdatePackage(string packageName)
        {
            string path = System.IO.Path.Combine(Application.dataPath, "Packages", packageName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                UnityEditor.PackageManager.Client.Add(packageName);
                UnityEditor.EditorUtility.DisplayDialog("OSK-Framework", "Package updated successfully", "OK");
                UnityEditor.AssetDatabase.Refresh();
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("OSK-Framework", "Package not found", "OK");
            }
        }
    }
}