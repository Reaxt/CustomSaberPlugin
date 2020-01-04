using IPA;
using IPA.Loader;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
namespace CustomSaber
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string PluginName => "Custom Sabers";
        public static string PluginVersion { get; private set; } = "0"; // Default. Actual version is retrieved from the manifest

        private static List<string> _saberPaths;
        public static string _currentSaberName;
        public static ColorManager colorManager;
        public static Saber LeftSaber;
        public static Saber RightSaber;

        private bool _init;
        public bool FirstFetch = true;

        public void Init(IPALogger logger, PluginLoader.PluginMetadata metadata)
        {
            if (logger != null)
            {
                Logger.logger = logger;
                Logger.Log("Logger prepared");
            }

            if (metadata != null)
            {
                Plugin.PluginVersion = metadata.Version.ToString();
            }
        }

        public void OnApplicationStart()
        {
            if (_init)
            {
                return;
            }
            _init = true;

            Logger.Log($"Custom Sabers v{Plugin.PluginVersion} has started", Logger.LogLevel.Notice);

            SaberLoader.LoadSabers();

            if (SaberLoader.AllSabers.Count == 0)
            {
                Logger.Log("No custom sabers found.");
                return;
            }

            _currentSaberName = PlayerPrefs.GetString("lastSaber", null);
            if (_currentSaberName == null || SaberLoader.FindSaberByName(_currentSaberName) == -1)
            {
                _currentSaberName = SaberLoader.AllSabers[0].Name;
            }
            CustomSaberUI.OnLoad();
        }

        public void OnApplicationQuit() { }

        public void OnActiveSceneChanged(Scene from, Scene to)
        {
            //if (scene.buildIndex > 0)
            //{
            //    if (FirstFetch)
            //    {
            //        //Logger.Log("Launching coroutine to grab original sabers!", LogLevel.Debug);
            //        //SharedCoroutineStarter.instance.StartCoroutine(PreloadDefaultSabers());
            //        //Logger.Log("Launched!", LogLevel.Debug);
            //    }
            //}

            if (to.name == "GameCore")
            {
                colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().LastOrDefault();
                LoadNewSaber(_currentSaberName);
                SaberScript.LoadAssets();
            }
        }

        private IEnumerator PreloadDefaultSabers()
        {
            FirstFetch = false;

            //Logger.Log("Preloading default sabers!", LogLevel.Debug);
            //HarmonyInstance harmony = HarmonyInstance.Create("CustomSaberHarmonyInstance");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.Log("Loading GameCore scene");
            SceneManager.LoadSceneAsync("GameCore", LoadSceneMode.Additive);
            Logger.Log("Loaded!");

            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Count() > 1);
            Logger.Log("Got sabers!");

            foreach (var s in Resources.FindObjectsOfTypeAll<Saber>())
            {
                Logger.Log($"Saber: {s.name}, GameObj: {s.gameObject.name}, {s.ToString()}");
                if (s.name == "LeftSaber")
                {
                    LeftSaber = Saber.Instantiate(s);
                }
                else if (s.name == "RightSaber")
                {
                    RightSaber = Saber.Instantiate(s);
                }
            }
            Logger.Log("Finished! Got default sabers! Setting active state");

            if (LeftSaber)
            {
                Object.DontDestroyOnLoad(LeftSaber.gameObject);
                LeftSaber.gameObject.SetActive(false);
                LeftSaber.name = "___OriginalSaberPreviewB";
            }

            if (RightSaber)
            {
                Object.DontDestroyOnLoad(RightSaber.gameObject);
                RightSaber.gameObject.SetActive(false);
                RightSaber.name = "___OriginalSaberPreviewA";
            }

            Logger.Log("Unloading GameCore");
            SceneManager.UnloadSceneAsync("GameCore");

            //Logger.Log("Unloading harmony patches", LogLevel.Debug);
            //harmony.UnpatchAll("CustomSaberHarmonyInstance");
        }

        public static List<string> RetrieveCustomSabers()
        {
            if (!Directory.Exists(Path.Combine(Application.dataPath, "../CustomSabers/")))
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "../CustomSabers/"));

            _saberPaths = (Directory.GetFiles(Path.Combine(Application.dataPath, "../CustomSabers/"), "*.saber", SearchOption.AllDirectories).ToList());
            Logger.Log($"Found {_saberPaths.Count} sabers");

            _saberPaths.Insert(0, "DefaultSabers");
            return _saberPaths;
        }

        public void OnUpdate() { }

        public static void LoadNewSaber(string name)
        {
            if (name != "DefaultSabers")
            {
                SaberScript.CustomSaber = SaberLoader.GetSaberAssetBundle(name);
            }

            PlayerPrefs.SetString("lastSaber", name);
            Logger.Log($"Loaded saber {name}");
        }

        public void OnFixedUpdate() { }
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }
        public void OnSceneUnloaded(Scene scene) { }
    }
}
