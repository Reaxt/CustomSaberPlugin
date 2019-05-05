using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CustomSaber.Harmony_Patches
{
    /*
    [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded",
    new Type[] { typeof(Scene), typeof(LoadSceneMode) })]
    class SceneLoadedHooks
    {

        public static bool Prefix(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameCore")
            {
                Logger.log.Notice($"Blocked scene {scene.name} from firing sceneLoaded event!");
                return false;
            }
            return true;
        }

    }
    */

   // [HarmonyPatch(typeof(SceneManager), "Internal_ActiveSceneChanged",
   //new Type[] { typeof(Scene), typeof(Scene) })]
   // class ActiveSceneChangedHooks
   // {
   //     public static bool Prefix(Scene previousActiveScene, Scene newActiveScene)
   //     {
   //         if (newActiveScene.name == "GameCore")
   //         {
   //             Logger.log.Notice($"Blocked scene {newActiveScene.name} from firing activeSceneChanged event!");
   //             return false;
   //         }
   //         return true;
   //     }

    // }
}
