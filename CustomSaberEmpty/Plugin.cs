using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace CustomSaber
{
    public class Plugin
    {
        public string Name
        {
            get { return "Saber Mod"; }
        }

        public string Version
        {
            get { return "1.1"; }
        }

        private static List<string> _saberPaths;
        private static AssetBundle _currentSaber;
        private static string _currentSaberPath;

        private bool _init;

        public void OnApplicationStart()
        {
        }

        public void OnApplicationQuit()
        {
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
        }

        public static List<string> RetrieveCustomSabers()
        {
            _saberPaths = Directory.GetFiles(Path.Combine(Application.dataPath, "../Plugin Content/Custom Saber/"),
                "*.saber", SearchOption.AllDirectories).ToList();
            Console.WriteLine("Found " + _saberPaths.Count + " sabers");
            return _saberPaths;
        }

        public void OnUpdate()
        {
        }

        public static void LoadNewSaber(string path)
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }
    }
}
