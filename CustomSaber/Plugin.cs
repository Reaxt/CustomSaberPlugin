using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using IPA;
using IPA.Loader;
using IPALogger = IPA.Logging.Logger;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginVersion { get; private set; } = "Unknown version";

        private static List<string> _saberPaths;
        private static AssetBundle _currentSaber;
        public static string _currentSaberPath;
        public static Saber LeftSaber;
        public static Saber RightSaber;

        private bool _init;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            Plugin.PluginVersion = GetPluginVersion("Custom Sabers");
            Logger.log.Debug($"Custom Sabers v{Plugin.PluginVersion} loaded!");
            
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            var sabers = RetrieveCustomSabers();
            if (sabers.Count == 0)
            {
                Logger.log.Info("No custom sabers found.");
                return;
            }

            _currentSaberPath = PlayerPrefs.GetString("lastSaber", null);
            if (_currentSaberPath == null || !sabers.Contains(_currentSaberPath))
            {
                _currentSaberPath = sabers[0];
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
        
        bool FirstFetch = true;
        public void OnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex > 0)
            {
                if (FirstFetch)
                {
                    //Logger.log.Info("Launching coroutine to grab original sabers!");
                    //SharedCoroutineStarter.instance.StartCoroutine(PreloadDefaultSabers());
                    //Logger.log.Info("Launched!");
                }
            }

            if (scene.name == "GameCore")
            {
                LoadNewSaber(_currentSaberPath);
                SaberScript.LoadAssets();
            }

            if (scene.name == "MenuCore")
            {
                if (_currentSaber != null)
                {
                    _currentSaber.Unload(true);
                }
                CustomSaberUI.OnLoad();
            }
        }
        
        private IEnumerator PreloadDefaultSabers()
        {
            FirstFetch = false;

            Logger.log.Info("Preloading default sabers!");
            var harmony = HarmonyInstance.Create("CustomSaberHarmonyInstance");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.log.Info("Loading GameCore scene");
            SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);
            Logger.log.Info("Loaded!");

            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Count() > 1);
            Logger.log.Info("Got sabers!");

            foreach (Saber s in Resources.FindObjectsOfTypeAll<Saber>())
            {
                Logger.log.Info($"Saber: {s.name}, GameObj: {s.gameObject.name}, {s.ToString()}");
                if (s.name == "LeftSaber") LeftSaber = Saber.Instantiate(s);
                else if (s.name == "RightSaber") RightSaber = Saber.Instantiate(s);
            }
            Logger.log.Info("Finished! Got default sabers! Setting active state");
            if (LeftSaber) { UnityEngine.Object.DontDestroyOnLoad(LeftSaber.gameObject); LeftSaber.gameObject.SetActive(false); LeftSaber.name = "___OriginalSaberPreviewB"; }
            if (RightSaber) { UnityEngine.Object.DontDestroyOnLoad(RightSaber.gameObject); RightSaber.gameObject.SetActive(false); RightSaber.name = "___OriginalSaberPreviewA"; }

            Logger.log.Info("Unloading GameCore");
            SceneManager.UnloadSceneAsync("GameCore");
            Logger.log.Info("Unloading harmony patches");
            harmony.UnpatchAll("CustomSaberHarmonyInstance");
        }

        public static List<string> RetrieveCustomSabers()
        {
            _saberPaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomSabers/"),
                "*.saber", SearchOption.AllDirectories).ToList());
            Logger.log.Info("Found " + _saberPaths.Count + " sabers");
            _saberPaths.Insert(0, "DefaultSabers");
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

            if (path != "DefaultSabers")
            {
                _currentSaberPath = path;

                _currentSaber = AssetBundle.LoadFromFile(_currentSaberPath);
                Logger.log.Info(_currentSaber.GetAllAssetNames()[0]);
                if (_currentSaber == null)
                {
                    Logger.log.Warn("Something went wrong while getting the asset bundle");
                }
                else
                {
                    Logger.log.Info("Successfully obtained the asset bundle!");
                    SaberScript.CustomSaber = _currentSaber;
                }
            }

            PlayerPrefs.SetString("lastSaber", _currentSaberPath);
            Logger.log.Info($"Loaded saber {path}");
        }

        public void OnFixedUpdate()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger prepared");
        }

        /// <param name="pluginNameOrId">The name or id defined in the manifest.json</param>
        public string GetPluginVersion(string pluginNameOrId)
        {
            foreach (PluginLoader.PluginInfo p in PluginManager.AllPlugins)
                if (p.Metadata.Id == pluginNameOrId || p.Metadata.Name == pluginNameOrId)
                    return p.Metadata.Version.ToString();

            return "Plugin Not Found";
        }
    }
}
