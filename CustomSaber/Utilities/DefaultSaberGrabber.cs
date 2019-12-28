using CustomSaber.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomSaber.Utilities
{
    /// <summary>
    /// Grabbing the default sabers, so we can display them in their full glory in the settings (Doesn't work. Game hangs on WaitUntil())
    /// Also fails in regular GameCore without forcing scene change... Something about null, but I gave up
    /// </summary>
    public class DefaultSaberGrabber : MonoBehaviour
    {
        public static bool isCompleted { get; private set; } = false;

        private void Awake()
        {
            if (!isCompleted)
            {
                StartCoroutine(PreloadDefaultSabers());
            }
        }

        private IEnumerator PreloadDefaultSabers()
        {
            bool isSceneLoaded = false;
            string sceneName = "GameCore";

            try
            {
                Saber defaultLeftSaber = null;
                Saber defaultRightSaber = null;

                Logger.log.Debug($"Loading {sceneName} scene");
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                isSceneLoaded = true;
                Logger.log.Debug("Loaded!");

                yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Count() > 1);

                Logger.log.Debug("Got sabers!");

                IEnumerable<Saber> sabers = Resources.FindObjectsOfTypeAll<Saber>();
                foreach (Saber saber in sabers)
                {
                    Logger.log.Debug($"Saber: {saber.name}, GameObj: {saber.gameObject.name}, {saber.ToString()}");
                    if (saber.name == "LeftSaber")
                    {
                        defaultLeftSaber = Instantiate(saber);
                    }
                    else if (saber.name == "RightSaber")
                    {
                        defaultRightSaber = Instantiate(saber);
                    }
                }

                Logger.log.Debug("Finished! Got default sabers! Setting active state");

                if (defaultLeftSaber)
                {
                    Logger.log.Debug("Found default left saber");
                    defaultLeftSaber.gameObject.SetActive(false);
                    //defaultLeftSaber.name = "___OriginalSaberPreviewB";
                    //DontDestroyOnLoad(defaultLeftSaber.gameObject);
                }

                if (defaultRightSaber)
                {
                    Logger.log.Debug("Found default right saber");
                    defaultRightSaber.gameObject.SetActive(false);
                    //defaultRightSaber.name = "___OriginalSaberPreviewA";
                    //DontDestroyOnLoad(defaultRightSaber.gameObject);
                }

                if (defaultLeftSaber && defaultRightSaber)
                {
                    // Add them as the first Object in the list, replacing the empty version.
                    CustomSaberData defaultSabers = new CustomSaberData(defaultLeftSaber, defaultRightSaber);
                    SaberAssetLoader.CustomSabers[0] = defaultSabers;
                    isCompleted = true;
                }
            }
            finally
            {
                if (isSceneLoaded)
                {
                    Logger.log.Debug($"Unloading {sceneName}");
                    SceneManager.UnloadSceneAsync(sceneName);
                }
            }
        }
    }
}
