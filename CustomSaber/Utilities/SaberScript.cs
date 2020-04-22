using BS_Utils.Gameplay;
using CustomSaber.Data;
using CustomSaber.Settings;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        // EventManagers
        private EventManager leftEventManager;
        private EventManager rightEventManager;

        // Controllers
        private BeatmapObjectManager beatmapObjectManager;
        private ScoreController scoreController;
        private ObstacleSaberSparkleEffectManager saberCollisionManager;
        private GameEnergyCounter gameEnergyCounter;
        private BeatmapObjectCallbackController beatmapCallback;
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

            if (sabers && Configuration.CustomEventsEnabled)
            {
                AddEvents();
            }
        }

        private void Start()
        {
            lastNoteId = -1;
            Restart();
        }

        private void AddEvents()
        {
            leftEventManager = leftSaber?.GetComponent<EventManager>();
            if (!leftEventManager)
            {
                leftEventManager = leftSaber.AddComponent<EventManager>();
            }

            rightEventManager = rightSaber?.GetComponent<EventManager>();
            if (!rightEventManager)
            {
                rightEventManager = rightSaber.AddComponent<EventManager>();
            }

            if (leftEventManager?.OnLevelStart == null
                || rightEventManager?.OnLevelStart == null)
            {
                return;
            }

            leftEventManager.OnLevelStart.Invoke();
            rightEventManager.OnLevelStart.Invoke();

            try
            {
                beatmapObjectManager = Resources.FindObjectsOfTypeAll<BeatmapObjectManager>().FirstOrDefault();
                if (beatmapObjectManager)
                {
                    beatmapObjectManager.noteWasCutEvent += SliceCallBack;
                    beatmapObjectManager.noteWasMissedEvent += NoteMissCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectManager)}'.");
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
                }

                gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                if (gameEnergyCounter)
                {
                    gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(GameEnergyCounter)}'.");
                }

                beatmapCallback = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
                if (beatmapCallback)
                {
                    beatmapCallback.beatmapEventDidTriggerEvent += LightEventCallBack;
                }
                else
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectCallbackController)}'.");
                }

                playerHeadAndObstacleInteraction = Resources.FindObjectsOfTypeAll<PlayerHeadAndObstacleInteraction>().FirstOrDefault();
                if (!playerHeadAndObstacleInteraction)
                {
                    Logger.log.Warn($"Failed to locate a suitable '{nameof(PlayerHeadAndObstacleInteraction)}'.");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                throw;
            }

            try
            {
                float LastTime = 0.0f;
                LevelData levelData = BS_Utils.Plugin.LevelData;
                BeatmapData beatmapData = levelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;

                IEnumerable<BeatmapLineData> beatmapLinesData = beatmapData.beatmapLinesData;
                foreach (BeatmapLineData beatMapLineData in beatmapLinesData)
                {
                    IList<BeatmapObjectData> beatmapObjectsData = beatMapLineData.beatmapObjectsData;
                    for (int i = beatmapObjectsData.Count - 1; i >= 0; i--)
                    {
                        BeatmapObjectData beatmapObjectData = beatmapObjectsData[i];
                        if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note
                            && ((NoteData)beatmapObjectData).noteType != NoteType.Bomb)
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
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                throw;
            }
        }

        private void OnDestroy()
        {
            if (beatmapObjectManager)
            {
                beatmapObjectManager.noteWasCutEvent -= SliceCallBack;
                beatmapObjectManager.noteWasMissedEvent -= NoteMissCallBack;
            }

            if (scoreController)
            {
                scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
                scoreController.comboDidChangeEvent -= ComboChangeEvent;
            }

            if (saberCollisionManager)
            {
                saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;
            }

            if (gameEnergyCounter)
            {
                gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;
            }

            if (beatmapCallback)
            {
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

            ResetVanillaTrails();

            CustomSaberData customSaber = (Configuration.RandomSabersEnabled) ? SaberAssetLoader.GetRandomSaber() : SaberAssetLoader.CustomSabers[SaberAssetLoader.SelectedSaber];
            if (customSaber != null)
            {
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
        }

        private IEnumerator WaitForSabers(GameObject saberRoot)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            if (Configuration.TrailType == TrailType.None)
            {
                HideVanillaTrails();
            }

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();
            foreach (Saber defaultSaber in defaultSabers)
            {
                Logger.log.Debug($"Hiding default '{defaultSaber.saberType}'");
                IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    meshFilter.gameObject.SetActive(!saberRoot);

                    MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                    filter?.gameObject.SetActive(!saberRoot);
                }

                Logger.log.Debug($"Attaching custom saber to '{defaultSaber.saberType}'");
                GameObject saber = GetCustomSaberByType(defaultSaber.saberType);
                if (saber)
                {
                    saber.transform.parent = defaultSaber.transform;
                    saber.transform.position = defaultSaber.transform.position;
                    saber.transform.rotation = defaultSaber.transform.rotation;

                    if (Configuration.TrailType == TrailType.Custom)
                    {
                        IEnumerable<CustomTrail> customTrails = saber.GetComponents<CustomTrail>();
                        foreach (CustomTrail trail in customTrails)
                        {
                            trail.Init(defaultSaber, colorManager);
                        }
                    }

                    ApplyColorsToSaber(saber, colorManager.ColorForSaberType(defaultSaber.saberType));
                }
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

                        if (renderMaterial.HasProperty("_Color"))
                        {
                            if (renderMaterial.HasProperty("_CustomColors"))
                            {
                                if (renderMaterial.GetFloat("_CustomColors") > 0)
                                    renderMaterial.SetColor("_Color", color);
                            }
                            else if (renderMaterial.HasProperty("_Glow") && renderMaterial.GetFloat("_Glow") > 0
                                || renderMaterial.HasProperty("_Bloom") && renderMaterial.GetFloat("_Bloom") > 0)
                            {
                                renderMaterial.SetColor("_Color", color);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator WaitToCheckDefault()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());


            if (Configuration.TrailType == TrailType.None)
            {
                HideVanillaTrails();
            }

            bool hideOneSaber = false;
            SaberType hiddenSaberType = SaberType.SaberA;
            if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.characteristicNameLocalizationKey.Contains("ONE_SABER"))
            {
                hideOneSaber = true;
                hiddenSaberType = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.leftHanded ? SaberType.SaberB : SaberType.SaberA;
            }

            Logger.log.Debug("Default Sabers. Not Replacing");
            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();
            foreach (Saber defaultSaber in defaultSabers)
            {
                bool activeState = !hideOneSaber ? true : defaultSaber.saberType != hiddenSaberType;
                defaultSaber.gameObject.SetActive(activeState);

                if (defaultSaber.saberType == hiddenSaberType)
                {
                    IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                    foreach (MeshFilter meshFilter in meshFilters)
                    {
                        meshFilter.gameObject.SetActive(!sabers);

                        MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                        filter?.gameObject.SetActive(!sabers);
                    }
                }
            }
        }

        private void Update()
        {
            if (playerHeadAndObstacleInteraction != null
                && playerHeadAndObstacleInteraction.intersectingObstacles.Count > 0)
            {
                if (!playerHeadWasInObstacle)
                {
                    leftEventManager?.OnComboBreak?.Invoke();
                    rightEventManager?.OnComboBreak?.Invoke();
                }

                playerHeadWasInObstacle = !playerHeadWasInObstacle;
            }
        }

        private void HideVanillaTrails() => SetVanillaTrailVisibility(0f);
        private void ResetVanillaTrails() => SetVanillaTrailVisibility(1.007f);
        private void SetVanillaTrailVisibility(float trailWidth)
        {
            IEnumerable<XWeaponTrail> trails = Resources.FindObjectsOfTypeAll<XWeaponTrail>();
            foreach (XWeaponTrail trail in trails)
            {
                ReflectionUtil.SetField(trail, "_trailWidth", trailWidth);
            }
        }

        private GameObject GetCustomSaberByType(SaberType saberType)
        {
            GameObject saber = null;
            if (saberType == SaberType.SaberA)
            {
                saber = leftSaber;
            }
            else if (saberType == SaberType.SaberB)
            {
                saber = rightSaber;
            }

            return saber;
        }

        private EventManager GetEventManagerByType(SaberType saberType)
        {
            EventManager eventManager = null;
            if (saberType == SaberType.SaberA)
            {
                eventManager = leftEventManager;
            }
            else if (saberType == SaberType.SaberB)
            {
                eventManager = rightEventManager;
            }

            return eventManager;
        }

        #region Events

        private void SliceCallBack(INoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK)
            {
                leftEventManager?.OnComboBreak?.Invoke();
                rightEventManager?.OnComboBreak?.Invoke();
            }
            else
            {
                EventManager eventManager = GetEventManagerByType(noteCutInfo.saberType);
                eventManager?.OnSlice?.Invoke();
            }

            if (noteController.noteData.id == lastNoteId)
            {
                leftEventManager?.OnLevelEnded?.Invoke();
                rightEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void NoteMissCallBack(INoteController noteController)
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

        private void SaberStartCollide(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStartColliding?.Invoke();
        }

        private void SaberEndCollide(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStopColliding?.Invoke();
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
