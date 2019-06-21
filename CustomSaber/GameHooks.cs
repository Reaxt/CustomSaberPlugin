using System;
using Harmony;
using LogLevel = IPA.Logging.Logger.Level;
using UnityEngine.SceneManagement;

namespace CustomSaber.Harmony_Patches
{
    //[HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded",
    //new Type[] { typeof(Scene), typeof(LoadSceneMode) })]
    //class SceneLoadedHooks
    //{
    //    public static bool Prefix(Scene scene, LoadSceneMode mode)
    //    {
    //        if (scene.name == "GameCore")
    //        {
    //            Logger.Log($"Blocked scene {scene.name} from firing sceneLoaded event!", LogLevel.Notice);
    //            return false;
    //        }
    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(SceneManager), "Internal_ActiveSceneChanged",
    //new Type[] { typeof(Scene), typeof(Scene) })]
    //class ActiveSceneChangedHooks
    //{
    //    public static bool Prefix(Scene previousActiveScene, Scene newActiveScene)
    //    {
    //        if (newActiveScene.name == "GameCore")
    //        {
    //            Logger.Log($"Blocked scene {newActiveScene.name} from firing activeSceneChanged event!", LogLevel.Notice);
    //            return false;
    //        }
    //        return true;
    //    }
    //}
}
