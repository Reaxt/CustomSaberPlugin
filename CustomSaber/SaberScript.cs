using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using Object = UnityEngine.Object;

namespace CustomSaber
{
    class SaberScript : MonoBehaviour
    {
        private enum AudioEvent
        {
            Play,
            Stop
        };

        public static AssetBundle CustomSaber;

        public static SaberScript Instance;

        private GameObject _leftTop;
        private GameObject _rightTop;

        private Vector3 _leftTopLocation;
        private Vector3 _rightTopLocation;

        private EventManager _leftEventManager;
        private EventManager _rightEventManager;

        private GameObject _leftSaber;
        private GameObject _rightSaber;
        private GameObject _saberRoot;

        private int LastNoteId;

        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private ScoreController _scoreController;
        private ObstacleSaberSparkleEffectManager _saberCollisionManager;
        private GameEnergyCounter _gameEnergyCounter;
        private BeatmapObjectCallbackController _beatmapCallback;
        private GamePauseManager _gamePauseManager;
        private PlayerHeadAndObstacleInteraction _playerHeadAndObstacleInteraction;
        internal static GameObject leftBackup;
        internal static GameObject rightBackup;
        private bool _playerHeadWasInObstacle;

        public static void LoadAssets()
        {
            if (CustomSaber == null)
            {
                Console.WriteLine("SABER ASSET BUNDLE DOESNT EXIST");
            }
            if (Instance != null)
            {
                Destroy(Instance._leftSaber);
                Destroy(Instance._rightSaber);
                Destroy(Instance._saberRoot);
                Destroy(Instance.gameObject);
            }

            var loader = new GameObject("Saber Loader");
            Instance = loader.AddComponent<SaberScript>();

        }

        public void Restart()
        {
            CancelInvoke("_Restart");
            Invoke("_Restart", 0.5f);
        }

        private void _Restart()
        {
            OnDestroy();
            AddEvents();
        }

        private void SceneManagerOnSceneLoaded(Scene newScene, LoadSceneMode mode)
        {
            Restart();
        }

        private void Start()
        {
            LastNoteId = -1;
            Restart();
        }

        BS_Utils.Gameplay.LevelData GetGameSceneSetup()
        {

            return BS_Utils.Plugin.LevelData;
        }

        private void AddEvents()
        {
            if (_leftSaber)
            {
                _leftEventManager = _leftSaber.GetComponent<EventManager>();
                if (_leftEventManager == null)
                    _leftEventManager = _leftSaber.AddComponent<EventManager>();
            }
            if (_rightSaber)
            {
                _rightEventManager = _rightSaber.GetComponent<EventManager>();
                if (_rightEventManager == null)
                    _rightEventManager = _rightSaber.AddComponent<EventManager>();
            }

            if (_leftEventManager)
                _leftEventManager.OnLevelStart.Invoke();
            else
                return;
            if (_rightEventManager)
                _rightEventManager.OnLevelStart.Invoke();
            else
                return;
            try
            {
                _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
                if (_beatmapObjectSpawnController == null)
                {
                    Console.WriteLine("SPAWN CONTROLLER NULL");
                    //_beatmapObjectSpawnController = _saberRoot.AddComponent<BeatmapObjectSpawnController>();
                }
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                if (_scoreController == null)
                {
                    Console.WriteLine("SCORE CONTROLLER NULL");
                    //_scoreController = _saberRoot.AddComponent<ScoreController>();
                }
                _saberCollisionManager =
                    Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                if (_saberCollisionManager == null)
                {
                    Console.WriteLine("COLLISION MANAGER NULL");
                    //_saberCollisionManager = _saberRoot.AddComponent<ObstacleSaberSparkleEffectManager>();
                }
                _gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                if (_gameEnergyCounter == null)
                {
                    Console.WriteLine("energery counter null");
                    //_gameEnergyCounter = _saberRoot.AddComponent<GameEnergyCounter>();
                }
                _beatmapCallback = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
                if (_beatmapCallback == null)
                {
                    Console.WriteLine("BEATMAP CALLBACK NULL");
                    //_beatmapCallback = _saberRoot.AddComponent<BeatmapObjectCallbackController>();
                }

                _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
                if (_gamePauseManager == null)
                {
                    Console.WriteLine("GamePauseManager Null");
                    //_gamePauseManager = _saberRoot.AddComponent<GamePauseManager>();
                }

                _playerHeadAndObstacleInteraction = Resources.FindObjectsOfTypeAll<PlayerHeadAndObstacleInteraction>().FirstOrDefault();
                if (_playerHeadAndObstacleInteraction == null)
                {
                    Console.WriteLine("PlayerHeadAndObstacleInteraction Null");
                    //_playerHeadAndObstacleInteraction = _saberRoot.AddComponent<PlayerHeadAndObstacleInteraction>();
                }

                _beatmapObjectSpawnController.noteWasCutEvent += SliceCallBack;
                _beatmapObjectSpawnController.noteWasMissedEvent += NoteMissCallBack;
                _scoreController.multiplierDidChangeEvent += MultiplierCallBack;
                _scoreController.comboDidChangeEvent += ComboChangeEvent;

                _saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
                _saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;

                _gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;

                _beatmapCallback.beatmapEventDidTriggerEvent += LightEventCallBack;
                //  ReflectionUtil.SetPrivateField(_gamePauseManager, "_gameDidResumeSignal", (Action)OnPauseMenuClosed); //For some reason _gameDidResumeSignal isn't public.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                throw;
            }

            try
            {
                BS_Utils.Gameplay.LevelData mgs = GetGameSceneSetup();
                BeatmapData beatmapData = mgs.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;

                BeatmapLineData[] beatmapLinesData = beatmapData.beatmapLinesData;
                float LastTime = 0.0f;

                for (int i = 0; i < beatmapLinesData.Length; i++)
                {
                    BeatmapObjectData[] beatmapObjectsData = beatmapLinesData[i].beatmapObjectsData;
                    for (int j = beatmapObjectsData.Length - 1; j > 0; j--)
                    {
                        if (beatmapObjectsData[j].beatmapObjectType == BeatmapObjectType.Note)
                        {
                            if (((NoteData)beatmapObjectsData[j]).noteType != NoteType.Bomb)
                            {
                                if (beatmapObjectsData[j].time > LastTime)
                                {
                                    LastNoteId = beatmapObjectsData[j].id;
                                    LastTime = beatmapObjectsData[j].time;
                                }
                                break;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                throw;
            }

        }

        private void OnDestroy()
        {
            if (_scoreController == null) return;
            _beatmapObjectSpawnController.noteWasCutEvent -= SliceCallBack;
            _beatmapObjectSpawnController.noteWasMissedEvent -= NoteMissCallBack;
            _scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
            _scoreController.comboDidChangeEvent -= ComboChangeEvent;

            _saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
            _saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;

            _gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;
            _beatmapCallback.beatmapEventDidTriggerEvent -= LightEventCallBack;
        }

        void Awake()
        {
            _leftTopLocation = Vector3.zero;
            _rightTopLocation = Vector3.zero;
            _saberRoot = null;
            //      Console.WriteLine(Plugin._currentSaberPath);
            if (Plugin._currentSaberPath == "DefaultSabers")
            {
                StartCoroutine(WaitToCheckDefault());
                return;
            }
            Console.WriteLine("Replacing sabers");
            if (CustomSaber == null)
            {
                Console.WriteLine("Assets Bundle is null");
                return;
            }

            GameObject saberRoot = CustomSaber.LoadAsset<GameObject>("_customsaber");

            if (saberRoot != null)
            {
                Console.WriteLine(saberRoot.GetComponent<SaberDescriptor>().SaberName);
                _saberRoot = Instantiate(saberRoot);
                _rightSaber = _saberRoot.transform.Find("RightSaber").gameObject;
                _leftSaber = _saberRoot.transform.Find("LeftSaber").gameObject;
            }
            StartCoroutine(WaitForSabers(saberRoot));
        }

        private IEnumerator WaitForSabers(GameObject saberRoot)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
            var sabers = Resources.FindObjectsOfTypeAll<Saber>();
            Saber.SaberType[] typeForHands = new Saber.SaberType[] { Saber.SaberType.SaberB, Saber.SaberType.SaberA };
            var playerDataModel = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().FirstOrDefault();
            Console.WriteLine("DataModel");
            if (playerDataModel && playerDataModel.currentLocalPlayer.playerSpecificSettings.swapColors) typeForHands = typeForHands.Reverse().ToArray();

            foreach (var saber in sabers)
            {
                //Disappear default saber
                foreach (MeshFilter t in saber.transform.GetComponentsInChildren<MeshFilter>())
                {
                    t.gameObject.SetActive(saberRoot == null);
                    var filter = t.GetComponentInChildren<MeshFilter>();
                    if (filter) filter.gameObject.SetActive(saberRoot == null);//.sharedMesh = null;
                }
                Console.WriteLine("Replacing " + saber.saberType);
                CustomTrail[] trails;
                if (saber.saberType == typeForHands[0])
                {
                    if (saberRoot == null) { }
                    else
                        _rightSaber.transform.parent = saber.transform;
                    _rightSaber.transform.position = saber.transform.position;
                    _rightSaber.transform.rotation = saber.transform.rotation;

                    //    Console.WriteLine("PreTrail");
                    trails = _rightSaber.GetComponents<CustomTrail>();
                    foreach (CustomTrail trail in trails)
                    {
                        trail.Init(saber);
                    }
                }
                else if (saber.saberType == typeForHands[1])
                {
                    if (saberRoot == null) { }
                    else
                        _leftSaber.transform.parent = saber.transform;
                    _leftSaber.transform.position = saber.transform.position;
                    _leftSaber.transform.rotation = saber.transform.rotation;

                    //    Console.WriteLine("PreTrail");
                    trails = _leftSaber.GetComponents<CustomTrail>();
                    foreach (CustomTrail trail in trails)
                    {
                        trail.Init(saber);
                    }
                }






            }
        }

        private IEnumerator WaitToCheckDefault()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
            var sabers = Resources.FindObjectsOfTypeAll<Saber>();
            Console.WriteLine("Default Sabers. Not Replacing");
            foreach (var saber in sabers)
            {
                saber.gameObject.SetActive(true);
                foreach (MeshFilter t in saber.transform.GetComponentsInChildren<MeshFilter>())
                {
                    t.gameObject.SetActive(_saberRoot == null);
                    var filter = t.GetComponentInChildren<MeshFilter>();
                    if (filter) filter.gameObject.SetActive(_saberRoot == null);//.sharedMesh = null;
                }
            }

            SaberWeaponTrail[] trails = Resources.FindObjectsOfTypeAll<SaberWeaponTrail>().ToArray();
            for (int i = 0; i < trails.Length; i++)
            {
                ReflectionUtil.SetPrivateField(trails[i], "_multiplierSaberColor", new Color(1f, 1f, 1f, 0.251f));
            }

        }

        private void OnPauseMenuClosed()
        {
            StartCoroutine(WaitForSabers(_saberRoot));
        }

        private void Update()
        {
            if (_playerHeadAndObstacleInteraction != null)
            {
                if (_playerHeadAndObstacleInteraction.intersectingObstacles.Count > 0)
                {
                    if (!_playerHeadWasInObstacle)
                    {
                        if (_leftEventManager != null && _rightEventManager != null)
                        {
                            _leftEventManager.OnComboBreak.Invoke();
                            _rightEventManager.OnComboBreak.Invoke();
                        }
                        _playerHeadWasInObstacle = true;
                    }
                    else
                    {
                        _playerHeadWasInObstacle = false;
                    }
                }
            }

            /*Vector3 LeftVelocity = LeftTop.transform.position - LeftTopLocation;
            Vector3 RightVelocity = RightTop.transform.position - RightTopLocation;

            Shader.SetGlobalColor("LeftSaberVelocity", new Color(LeftVelocity.x, LeftVelocity.y, LeftVelocity.z, 0.0f));
            Shader.SetGlobalColor("RightSaberVelocity", new Color(RightVelocity.x, RightVelocity.y, RightVelocity.z, 0.0f));

            LeftTopLocation = LeftTop.transform.position;
            RightTopLocation = RightTop.transform.position;*/
        }

        private void SliceCallBack(BeatmapObjectSpawnController beatmapObjectSpawnController, NoteController noteController, NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK)
            {
                _leftEventManager.OnComboBreak.Invoke();
                _rightEventManager.OnComboBreak.Invoke();
            }
            else
            {
                if (noteCutInfo.saberType == Saber.SaberType.SaberA)
                {
                    _leftEventManager.OnSlice.Invoke();
                }
                else if (noteCutInfo.saberType == Saber.SaberType.SaberB)
                {
                    _rightEventManager.OnSlice.Invoke();
                }
            }

            if (noteController.noteData.id == LastNoteId)
            {
                _leftEventManager.OnLevelEnded.Invoke();
                _rightEventManager.OnLevelEnded.Invoke();

            }
        }

        private void NoteMissCallBack(BeatmapObjectSpawnController beatmapObjectSpawnController, NoteController noteController)
        {

            if (noteController.noteData.noteType != NoteType.Bomb)
            {
                _leftEventManager.OnComboBreak.Invoke();
                _rightEventManager.OnComboBreak.Invoke();
            }

            if (noteController.noteData.id == LastNoteId)
            {
                _leftEventManager.OnLevelEnded.Invoke();
                _rightEventManager.OnLevelEnded.Invoke();
            }
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                _leftEventManager.MultiplierUp.Invoke();
                _rightEventManager.MultiplierUp.Invoke();
            }
        }

        private void SaberStartCollide(Saber.SaberType saber)
        {
            if (saber == Saber.SaberType.SaberA)
            {
                _leftEventManager.SaberStartColliding.Invoke();
            }
            else if (saber == Saber.SaberType.SaberB)
            {
                _rightEventManager.SaberStartColliding.Invoke();
            }
        }

        private void SaberEndCollide(Saber.SaberType saber)
        {
            if (saber == Saber.SaberType.SaberA)
            {
                _leftEventManager.SaberStopColliding.Invoke();
            }
            else if (saber == Saber.SaberType.SaberB)
            {
                _rightEventManager.SaberStopColliding.Invoke();
            }
        }

        private void FailLevelCallBack()
        {
            _leftEventManager.OnLevelFail.Invoke();
            _rightEventManager.OnLevelFail.Invoke();
        }

        private void LightEventCallBack(BeatmapEventData songEvent)
        {
            if ((int)songEvent.type < 5)
            {
                if (songEvent.value > 0 && songEvent.value < 4)
                {
                    _leftEventManager.OnBlueLightOn.Invoke();
                    _rightEventManager.OnBlueLightOn.Invoke();
                }

                if (songEvent.value > 4 && songEvent.value < 8)
                {
                    _leftEventManager.OnRedLightOn.Invoke();
                    _rightEventManager.OnRedLightOn.Invoke();
                }
            }
        }

        private void ComboChangeEvent(int combo)
        {
            _leftEventManager.OnComboChanged.Invoke(combo);
            _rightEventManager.OnComboChanged.Invoke(combo);
        }
    }
}