using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace CustomSaber
{
    public class Plugin : IPlugin
    {
        public string Name
        {
            get { return "Saber Mod"; }
        }

        public string Version
        {
            get { return "2.0"; }
        }

        private static List<string> _saberPaths;
        private static AssetBundle _currentSaber;
        public static string _currentSaberPath;

        private bool _init;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;
            
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            var sabers = RetrieveCustomSabers();
            if (sabers.Count == 0)
            {
                Console.WriteLine("No custom sabers found.");
                return;
            }
            
            _currentSaberPath = PlayerPrefs.GetString("lastSaber", null);
            if (_currentSaberPath == null || !sabers.Contains(_currentSaberPath))
            {
                _currentSaberPath = sabers[0];
            }
            
            //LoadNewSaber(_currentSaberPath);
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.name == "NiceEnvironment" || scene.name == "DefaultEnvironment" || scene.name == "BigMirrorEnvironment" || scene.name == "TriangleEnvironment")
            {
                LoadNewSaber(_currentSaberPath);
                SaberScript.LoadAssets();
            }

            if (scene.name == "Menu")
            {
                if(_currentSaber != null)
                {
                    _currentSaber.Unload(true);
                }
                CustomSaberUI.OnLoad();
            }
        }

        public static List<string> RetrieveCustomSabers()
        {
           
            _saberPaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomSabers/"),
                "*.saber", SearchOption.AllDirectories).ToList());
            Console.WriteLine("Found " + _saberPaths.Count + " sabers");
            return _saberPaths;
        }

        public void OnUpdate()
        {
            if (_currentSaber == null) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RetrieveCustomSabers();
                if (_saberPaths.Count == 1) return;
                var oldIndex = _saberPaths.IndexOf(_currentSaberPath);
                if (oldIndex >= _saberPaths.Count - 1)
                {
                    oldIndex = -1;
                }

                var newSaber = _saberPaths[oldIndex + 1];
                LoadNewSaber(newSaber);
                if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftAlt))
            {
                RetrieveCustomSabers();
                if (_saberPaths.Count == 1) return;
                var oldIndex = _saberPaths.IndexOf(_currentSaberPath);
                if (oldIndex <= 0)
                {
                    oldIndex = _saberPaths.Count - 1;
                }

                var newSaber = _saberPaths[oldIndex - 1];
                LoadNewSaber(newSaber);
                if (SceneManager.GetActiveScene().buildIndex != 4) return;
                SaberScript.LoadAssets();
            }
        }

        public static void LoadNewSaber(string path)
        {
            if (_currentSaber != null)
            {
                _currentSaber.Unload(true);
            }

            _currentSaberPath = path;
            
            _currentSaber =
                AssetBundle.LoadFromFile(_currentSaberPath);
            Console.WriteLine(_currentSaber.GetAllAssetNames()[0]);
            if (_currentSaber == null)
            {
                Console.WriteLine("something went wrong getting the asset bundle");
            }
            else
            {
                Console.WriteLine("Succesfully obtained the asset bundle!");
                SaberScript.CustomSaber = _currentSaber;
            }
            
            PlayerPrefs.SetString("lastSaber", _currentSaberPath);
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
