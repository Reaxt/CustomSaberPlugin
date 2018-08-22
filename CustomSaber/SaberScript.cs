using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
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

        private ScoreController _scoreController;
        private ObstacleSaberSparkleEffectManager _saberCollisionManager;
        private GameEnergyCounter _gameEnergyCounter;
        private BeatmapObjectCallbackController _beatmapCallback;
        private GamePauseManager _gamePauseManager;

        public static void LoadAssets()
        {
            if (CustomSaber == null)
            {
                Console.WriteLine("SABER ASSET BUNDLE DOESNT EXIST");
            }
            else
            {
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
        }

        private void Start()
        {
            _leftEventManager = _leftSaber.GetComponent<EventManager>();
            if (_leftEventManager == null)
                _leftEventManager = _leftSaber.AddComponent<EventManager>();

            _rightEventManager = _rightSaber.GetComponent<EventManager>();
            if (_rightEventManager == null)
                _rightEventManager = _rightSaber.AddComponent<EventManager>();
            
            _leftEventManager.OnLevelStart.Invoke();
            _rightEventManager.OnLevelStart.Invoke();
            try
            {
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                if(_scoreController == null)
                {
                    Console.WriteLine("SCORE CONTROLLER NULL");
                }
                _saberCollisionManager =
                    Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                if(_saberCollisionManager == null)
                {
                    Console.WriteLine("COLLISION MANAGER NULL");
                }
                _gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                if(_gameEnergyCounter == null)
                {
                    Console.WriteLine("energery counter null");

                }
                _beatmapCallback = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
                if(_beatmapCallback == null)
                {
                    Console.WriteLine("BEATMAP CALLBACK NULL");
                }

                _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
                if (_gamePauseManager == null)
                {
                    Console.WriteLine("GamePauseManager Null");
                }

                _scoreController.noteWasCutEvent += SliceCallBack;
                _scoreController.noteWasMissedEvent += NoteMissCallBack;
                _scoreController.multiplierDidChangeEvent += MultiplierCallBack;
                _scoreController.comboDidChangeEvent += ComboChangeEvent;

                _saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
                _saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;

                _gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;

                _beatmapCallback.beatmapEventDidTriggerEvent += LightEventCallBack;
                ReflectionUtil.SetPrivateField(_gamePauseManager, "_gameDidResumeSignal", (Action)OnPauseMenuClosed); //For some reason _gameDidResumeSignal isn't public.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                throw;
            }

        }

        void Awake()
        {
            _leftTopLocation = Vector3.zero;
            _rightTopLocation = Vector3.zero;

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
            Console.WriteLine("Saber list get: " + sabers.Length);
            foreach (var saber in sabers)
            {
                Console.WriteLine(saber.saberType + " " + saber.transform.GetChild(0) + saber.transform.GetChild(1) + saber.transform.GetChild(2) + saber.transform.GetChild(3));
                var handle = saber.transform.Find("Handle");
                var blade = saber.transform.Find("Blade");
                var top = saber.transform.Find("Top");
                Console.WriteLine("Saber Transform Found");

                blade.GetComponent<MeshFilter>().sharedMesh = null;
                blade.transform.localRotation = Quaternion.identity;
                blade.transform.localScale = new Vector3(1, 1, 1);
                blade.transform.localPosition = new Vector3(0, -0.01f, 0);
                handle.gameObject.SetActive(false);

                if (saber.saberType == Saber.SaberType.SaberB)
                {
                    if (saberRoot == null) { }
                    else
                        _rightSaber.transform.parent = blade.transform;
                    _rightSaber.transform.position = saber.transform.position;
                    _rightSaber.transform.rotation = saber.transform.rotation;
                    _rightTop = top.gameObject;
                }
                else if (saber.saberType == Saber.SaberType.SaberA)
                {
                    if (saberRoot == null) { }
                    else
                        _leftSaber.transform.parent = blade.transform;
                    _leftSaber.transform.position = saber.transform.position;
                    _leftSaber.transform.rotation = saber.transform.rotation;
                    _leftTop = top.gameObject;
                }
            }
        }

        private void OnPauseMenuClosed()
        {
            StartCoroutine(WaitForSabers(_saberRoot));
        }

        private void Update()
        {
            /*Vector3 LeftVelocity = LeftTop.transform.position - LeftTopLocation;
            Vector3 RightVelocity = RightTop.transform.position - RightTopLocation;

            Shader.SetGlobalColor("LeftSaberVelocity", new Color(LeftVelocity.x, LeftVelocity.y, LeftVelocity.z, 0.0f));
            Shader.SetGlobalColor("RightSaberVelocity", new Color(RightVelocity.x, RightVelocity.y, RightVelocity.z, 0.0f));

            LeftTopLocation = LeftTop.transform.position;
            RightTopLocation = RightTop.transform.position;*/
        }

        private void SliceCallBack(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
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
        }

        private void NoteMissCallBack(NoteData noteData, int multiplier)
        {
            
            if (noteData.noteType != NoteType.Bomb)
            {
                _leftEventManager.OnComboBreak.Invoke();
                _rightEventManager.OnComboBreak.Invoke();
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
            if ((int) songEvent.type < 5)
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