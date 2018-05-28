using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;

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
        private SongController _songController;

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

            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            _saberCollisionManager =
                Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
            _gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
            _songController = Resources.FindObjectsOfTypeAll<SongController>().FirstOrDefault();


            _scoreController.noteWasCutEvent += SliceCallBack;
            _scoreController.noteWasMissedEvent += NoteMissCallBack;
            _scoreController.multiplierDidChangeEvent += MultiplierCallBack;
            _scoreController.comboDidChangeEvent += ComboChangeEvent;

            _saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
            _saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;

            _gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;

            _songController.songEvent += LightEventCallBack;
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

            var sabers = Resources.FindObjectsOfTypeAll<Saber>();
            foreach (var saber in sabers)
            {
                var handle = saber.transform.Find("Handle");
                var blade = saber.transform.Find("Blade");
                var top = saber.transform.Find("Top");

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
                    _rightSaber.transform.position = blade.transform.position;
                    _rightTop = top.gameObject;
                }
                else if (saber.saberType == Saber.SaberType.SaberA)
                {
                    if (saberRoot == null) { }
                    else
                        _leftSaber.transform.parent = blade.transform;
                    _leftSaber.transform.position = blade.transform.position;

                    _leftTop = top.gameObject;
                }
            }
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
            if (noteData.noteType != NoteData.NoteType.Bomb)
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

        private void LightEventCallBack(SongEventData songEvent)
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