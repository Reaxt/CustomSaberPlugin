using BS_Utils.Gameplay;
using CustomSaber.Data;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace CustomSaber.Utilities
{
    internal class SaberScript : MonoBehaviour
    {
        // CustomSabers
        private GameObject sabers;
        private GameObject leftSaber;
        private GameObject rightSaber;

        private int lastNoteId;
        private bool playerHeadWasInObstacle;
        private ColorManager colorManager;

        // Events
        private EventManager leftEventManager;
        private EventManager rightEventManager;

        // Controllers
        private BeatmapObjectSpawnController beatmapObjectSpawnController;
        private ScoreController scoreController;
        private ObstacleSaberSparkleEffectManager saberCollisionManager;
        private GameEnergyCounter gameEnergyCounter;
        private BeatmapObjectCallbackController beatmapCallback;
        private PauseController gamePauseManager;
        private PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction;

        public static SaberScript instance;

        /// <summary>
        /// Load the Saber swapper script
        /// </summary>
        public static void Load()
        {
            if (instance != null)
            {
                Destroy(instance.leftSaber);
                Destroy(instance.rightSaber);
                Destroy(instance.sabers);
                Destroy(instance.gameObject);
            }

            GameObject loader = new GameObject("Saber Loader");
            instance = loader.AddComponent<SaberScript>();
        }

        public void Restart()
        {
            CancelInvoke("_Restart");
            Invoke("_Restart", 0.5f);
        }

        private void _Restart()
        {
            OnDestroy();

            if (sabers)
            {
                AddEvents();
            }
        }

        private void SceneManagerOnSceneLoaded(Scene newScene, LoadSceneMode mode) => Restart();

        private void Start()
        {
            lastNoteId = -1;

            Restart();
        }

        private LevelData GetGameSceneSetup() => BS_Utils.Plugin.LevelData;

        private void AddEvents()
        {
            leftEventManager = leftSaber?.GetComponent<EventManager>();
            if (!leftEventManager)
            {
                leftEventManager = leftSaber.AddComponent<EventManager>();
            }

            if (leftEventManager?.OnLevelStart != null)
            {
                leftEventManager.OnLevelStart.Invoke();
            }
            else
            {
                return;
            }

            rightEventManager = rightSaber?.GetComponent<EventManager>();
            if (!rightEventManager)
            {
                rightEventManager = rightSaber.AddComponent<EventManager>();
            }

            if (rightEventManager?.OnLevelStart != null)
            {
                rightEventManager.OnLevelStart.Invoke();
            }
            else
            {
                return;
            }

            try
            {
                beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
                if (beatmapObjectSpawnController)
                {
                    beatmapObjectSpawnController.noteWasCutEvent += SliceCallBack;
                    beatmapObjectSpawnController.noteWasMissedEvent += NoteMissCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectSpawnController)}'.");
                    //beatmapObjectSpawnController = sabers.AddComponent<BeatmapObjectSpawnController>();
                }

                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                if (scoreController)
                {
                    scoreController.multiplierDidChangeEvent += MultiplierCallBack;
                    scoreController.comboDidChangeEvent += ComboChangeEvent;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(ScoreController)}'.");
                    //scoreController = sabers.AddComponent<ScoreController>();
                }

                saberCollisionManager = Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                if (saberCollisionManager)
                {
                    saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
                    saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(ObstacleSaberSparkleEffectManager)}'.");
                    //saberCollisionManager = sabers.AddComponent<ObstacleSaberSparkleEffectManager>();
                }

                gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                if (gameEnergyCounter)
                {
                    gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(GameEnergyCounter)}'.");
                    //gameEnergyCounter = sabers.AddComponent<GameEnergyCounter>();
                }

                beatmapCallback = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
                if (beatmapCallback)
                {
                    beatmapCallback.beatmapEventDidTriggerEvent += LightEventCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectCallbackController)}'.");
                    //beatmapCallback = sabers.AddComponent<BeatmapObjectCallbackController>();
                }

                gamePauseManager = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
                if (!gamePauseManager)
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(PauseController)}'.");
                    //gamePauseManager = sabers.AddComponent<PauseController>();
                }

                playerHeadAndObstacleInteraction = Resources.FindObjectsOfTypeAll<PlayerHeadAndObstacleInteraction>().FirstOrDefault();
                if (!playerHeadAndObstacleInteraction)
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(PlayerHeadAndObstacleInteraction)}'.");
                    //playerHeadAndObstacleInteraction = sabers.AddComponent<PlayerHeadAndObstacleInteraction>();
                }

                //ReflectionUtil.SetPrivateField(_gamePauseManager, "_gameDidResumeSignal", (Action)OnPauseMenuClosed); //For some reason _gameDidResumeSignal isn't public.
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                throw;
            }

            try
            {
                float LastTime = 0.0f;
                LevelData levelData = GetGameSceneSetup();
                BeatmapData beatmapData = levelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;

                IEnumerable<BeatmapLineData> beatmapLinesData = beatmapData.beatmapLinesData;
                foreach (BeatmapLineData beatMapLineData in beatmapLinesData)
                {
                    IList<BeatmapObjectData> beatmapObjectsData = beatMapLineData.beatmapObjectsData;
                    for (int i = beatmapObjectsData.Count - 1; i >= 0; i--)
                    {
                        BeatmapObjectData beatmapObjectData = beatmapObjectsData[i];
                        if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note)
                        {
                            if (((NoteData)beatmapObjectData).noteType != NoteType.Bomb)
                            {
                                if (beatmapObjectData.time > LastTime)
                                {
                                    lastNoteId = beatmapObjectData.id;
                                    LastTime = beatmapObjectData.time;
                                }

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                throw;
            }
        }

        private void OnDestroy()
        {
            if (scoreController)
            {
                beatmapObjectSpawnController.noteWasCutEvent -= SliceCallBack;
                beatmapObjectSpawnController.noteWasMissedEvent -= NoteMissCallBack;

                scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
                scoreController.comboDidChangeEvent -= ComboChangeEvent;

                saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;

                gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;

                beatmapCallback.beatmapEventDidTriggerEvent -= LightEventCallBack;
            }
        }

        private void Awake()
        {
            if (sabers)
            {
                Destroy(sabers);
                sabers = null;
            }

            colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().LastOrDefault();

            //Reset Trails
            IEnumerable<SaberWeaponTrail> trails = Resources.FindObjectsOfTypeAll<SaberWeaponTrail>().ToArray();
            foreach (SaberWeaponTrail trail in trails)
            {
                ReflectionUtil.SetPrivateField(trail, "_multiplierSaberColor", new Color(1f, 1f, 1f, 0.251f));
                ReflectionUtil.SetPrivateField(trail as XWeaponTrail, "_whiteSteps", 4);
            }

            CustomSaberData customSaber = SaberAssetLoader.CustomSabers[SaberAssetLoader.SelectedSaber];
            if (customSaber.FileName == "DefaultSabers")
            {
                StartCoroutine(WaitToCheckDefault());
            }
            else
            {
                Logger.log.Debug("Replacing sabers");

                if (customSaber.Sabers)
                {
                    sabers = Instantiate(customSaber.Sabers);
                    rightSaber = sabers?.transform.Find("RightSaber").gameObject;
                    leftSaber = sabers?.transform.Find("LeftSaber").gameObject;
                }

                StartCoroutine(WaitForSabers(customSaber.Sabers));
            }
        }

        private IEnumerator WaitForSabers(GameObject saberRoot)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            PlayerDataModelSO playerDataModel = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().FirstOrDefault();

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();
            foreach (Saber defaultSaber in defaultSabers)
            {
                Logger.log.Debug($"Hiding default '{defaultSaber.saberType}'");
                IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    meshFilter.gameObject.SetActive(!saberRoot);

                    MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                    if (filter)
                    {
                        filter.gameObject.SetActive(!saberRoot);
                    }
                }

                Logger.log.Debug($"Attaching custom saber to '{defaultSaber.saberType}'");
                if (defaultSaber.saberType == Saber.SaberType.SaberA)
                {
                    PrepareSaber(saberRoot, defaultSaber, leftSaber);
                    ApplyColorsToSaber(leftSaber, colorManager.ColorForSaberType(defaultSaber.saberType));
                }
                else if (defaultSaber.saberType == Saber.SaberType.SaberB)
                {
                    PrepareSaber(saberRoot, defaultSaber, rightSaber);
                    ApplyColorsToSaber(rightSaber, colorManager.ColorForSaberType(defaultSaber.saberType));
                }
            }
        }

        private void PrepareSaber(GameObject saberRoot, Saber saber, GameObject customSaber)
        {
            if (saberRoot)
            {
                customSaber.transform.parent = saber.transform;
            }

            customSaber.transform.position = saber.transform.position;
            customSaber.transform.rotation = saber.transform.rotation;

            IEnumerable<CustomTrail> trails = customSaber.GetComponents<CustomTrail>();
            foreach (CustomTrail trail in trails)
            {
                trail.Init(saber, colorManager);
            }
        }

        public void ApplyColorsToSaber(GameObject saber, Color color)
        {
            //Logger.log.Debug($"Applying Color: {color} to saber: {saber.name}");
            IEnumerable<Renderer> renderers = saber.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    foreach (Material renderMaterial in renderer.sharedMaterials)
                    {
                        if (renderMaterial == null)
                        {
                            continue;
                        }

                        if (renderMaterial.HasProperty("_Glow") && renderMaterial.GetFloat("_Glow") > 0
                            || renderMaterial.HasProperty("_Bloom") && renderMaterial.GetFloat("_Bloom") > 0)
                        {
                            renderMaterial.SetColor("_Color", color);
                        }
                    }
                }
            }
        }

        private IEnumerator WaitToCheckDefault()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            bool hideOneSaber = false;
            Saber.SaberType hiddenSaberType = Saber.SaberType.SaberA;
            if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.characteristicNameLocalizationKey.Contains("ONE_SABER"))
            {
                hideOneSaber = true;
                hiddenSaberType = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.leftHanded ? Saber.SaberType.SaberB : Saber.SaberType.SaberA;
            }

            Logger.log.Debug("Default Sabers. Not Replacing");
            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();
            foreach (Saber defaultSaber in defaultSabers)
            {
                if (!hideOneSaber)
                {
                    defaultSaber.gameObject.SetActive(true);
                }
                else
                {
                    defaultSaber.gameObject.SetActive(defaultSaber.saberType != hiddenSaberType);
                }

                if (defaultSaber.saberType == hiddenSaberType)
                {
                    IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                    foreach (MeshFilter meshFilter in meshFilters)
                    {
                        meshFilter.gameObject.SetActive(!sabers);

                        MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                        if (filter)
                        {
                            filter.gameObject.SetActive(!sabers);
                        }
                    }
                }

            }
        }

        private void OnPauseMenuClosed() => StartCoroutine(WaitForSabers(sabers));

        private void Update()
        {
            if (playerHeadAndObstacleInteraction != null)
            {
                if (playerHeadAndObstacleInteraction.intersectingObstacles.Count > 0)
                {
                    if (!playerHeadWasInObstacle)
                    {
                        if (leftEventManager != null && rightEventManager != null)
                        {
                            leftEventManager.OnComboBreak?.Invoke();
                            rightEventManager.OnComboBreak?.Invoke();
                        }

                        playerHeadWasInObstacle = true;
                    }
                    else
                    {
                        playerHeadWasInObstacle = false;
                    }
                }
            }
        }

        #region Events

        private void SliceCallBack(BeatmapObjectSpawnController beatmapObjectSpawnController, INoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK)
            {
                leftEventManager?.OnComboBreak?.Invoke();
                rightEventManager?.OnComboBreak?.Invoke();
            }
            else
            {
                if (noteCutInfo.saberType == Saber.SaberType.SaberA)
                {
                    leftEventManager?.OnSlice?.Invoke();
                }
                else if (noteCutInfo.saberType == Saber.SaberType.SaberB)
                {
                    rightEventManager?.OnSlice?.Invoke();
                }
            }

            if (noteController.noteData.id == lastNoteId)
            {
                leftEventManager?.OnLevelEnded?.Invoke();
                rightEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void NoteMissCallBack(BeatmapObjectSpawnController beatmapObjectSpawnController, INoteController noteController)
        {
            if (noteController.noteData.noteType != NoteType.Bomb)
            {
                leftEventManager?.OnComboBreak?.Invoke();
                rightEventManager?.OnComboBreak?.Invoke();
            }

            if (noteController.noteData.id == lastNoteId)
            {
                leftEventManager?.OnLevelEnded?.Invoke();
                rightEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                leftEventManager?.MultiplierUp?.Invoke();
                rightEventManager?.MultiplierUp?.Invoke();
            }
        }

        private void SaberStartCollide(Saber.SaberType saberType)
        {
            if (saberType == Saber.SaberType.SaberA)
            {
                leftEventManager?.SaberStartColliding?.Invoke();
            }
            else if (saberType == Saber.SaberType.SaberB)
            {
                rightEventManager?.SaberStartColliding?.Invoke();
            }
        }

        private void SaberEndCollide(Saber.SaberType saberType)
        {
            if (saberType == Saber.SaberType.SaberA)
            {
                leftEventManager?.SaberStopColliding?.Invoke();
            }
            else if (saberType == Saber.SaberType.SaberB)
            {
                rightEventManager?.SaberStopColliding?.Invoke();
            }
        }

        private void FailLevelCallBack()
        {
            leftEventManager?.OnLevelFail?.Invoke();
            rightEventManager?.OnLevelFail?.Invoke();
        }

        private void LightEventCallBack(BeatmapEventData songEvent)
        {
            if ((int)songEvent.type < 5)
            {
                if (songEvent.value > 0 && songEvent.value < 4)
                {
                    leftEventManager?.OnBlueLightOn?.Invoke();
                    rightEventManager?.OnBlueLightOn?.Invoke();
                }

                if (songEvent.value > 4 && songEvent.value < 8)
                {
                    leftEventManager?.OnRedLightOn?.Invoke();
                    rightEventManager?.OnRedLightOn?.Invoke();
                }
            }
        }

        private void ComboChangeEvent(int combo)
        {
            leftEventManager?.OnComboChanged?.Invoke(combo);
            rightEventManager?.OnComboChanged?.Invoke(combo);
        }

        #endregion
    }
}
