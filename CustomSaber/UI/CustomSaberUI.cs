using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomSaber
{
    class CustomSaberUI : MonoBehaviour
    {
        static RectTransform _rightPos;
        static VRUIViewController _rightScreen;
        internal static CustomSaberUI Instance;

        public static List<Sprite> Icons = new List<Sprite>();

        private MainMenuViewController _mainMenuViewController;
        private MainFlowCoordinator _menuMasterViewController;

        private static CustomSaberMasterViewController _sabersMasterViewController;

        private Button _buttonInstance;
        private Button _cogWheelButtonInstance;
        private Button _backButtonInstance;

        private Button _upArrowBtn;
        private Button _downArrowBtn;
        private RectTransform _mainMenuRectTransform;
        
        public static void OnLoad()
        {

            if (Instance != null)
            {
                Instance.Awake();
                return;
            }
            new GameObject("modmenu").AddComponent<CustomSaberUI>();
        }

        void Awake()
        {

            Instance = this;

            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                Icons.Add(sprite);
            }

            try
            {
                var allButtons = Resources.FindObjectsOfTypeAll<Button>();

                _buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == "QuitButton");
                _cogWheelButtonInstance = allButtons.FirstOrDefault(x => x.name == "SettingsButton");
                _downArrowBtn = allButtons.First(x => x.name == "PageDownButton");
                _upArrowBtn = allButtons.First(x => x.name == "PageUpButton");
                _backButtonInstance = allButtons.First(x => x.name == "BackArrowButton");
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _menuMasterViewController = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                _mainMenuRectTransform = (RectTransform)_buttonInstance.transform.parent;

                AddCustomSaberButton();
            }
            catch (Exception)
            {
            }
        }

        private void AddCustomSaberButton()
        {
            _rightScreen = ReflectionUtil.GetPrivateField<VRUIViewController>(_mainMenuViewController, "_releaseInfoViewController");
            _rightPos = _rightScreen.rectTransform;
            var customSaberButton = CreateButton(_mainMenuRectTransform, new Vector3(0, 10, 0));

            if (customSaberButton == null)
            {
                return;
            }

            SetButtonText(ref customSaberButton, "Saber Menu");
            customSaberButton.onClick.AddListener(delegate
            {

                if (_sabersMasterViewController == null)
                {
                    _sabersMasterViewController = CreateViewController<CustomSaberMasterViewController>();
                }

                _rightScreen.PresentModalViewController(_sabersMasterViewController, null);
            });
        }

        public T CreateViewController<T>() where T : VRUIViewController
        {
            var vc = new GameObject("CustomViewController").AddComponent<T>();

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);

            return vc;
        }

        public Button CreateButton(RectTransform parent, Vector3 localOffset = new Vector3())
        {
            try
            {
                if (_cogWheelButtonInstance == null)
                {
                    return null;
                }

                // Create temporary button to return as new button
                var tmp = Instantiate(_cogWheelButtonInstance, parent, false);
                tmp.transform.localPosition = tmp.transform.localPosition + localOffset;
                DestroyImmediate(tmp.GetComponent<SignalOnUIButtonClick>());
                tmp.onClick = new Button.ButtonClickedEvent();

                return tmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Button CreateButton(RectTransform parent, string templateButtonName)
        {
            try
            {
                var templateButton = Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == templateButtonName);
                if (templateButton == null)
                {
                    return null;
                }

                // Create temporary button to return as new button
                var tmp = Instantiate(templateButton, parent, false);
                DestroyImmediate(tmp.GetComponent<SignalOnUIButtonClick>());
                tmp.onClick = new Button.ButtonClickedEvent();

                return tmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Button CreateDownButton(RectTransform parent)
        {
            if (_downArrowBtn == null)
            {
                return null;
            }

            var tmp = Instantiate(_downArrowBtn, parent, false);
            DestroyImmediate(tmp.GetComponent<SignalOnUIButtonClick>());
            tmp.onClick = new Button.ButtonClickedEvent();

            return tmp;
        }

        public Button CreateUpButton(RectTransform parent)
        {
            if (_upArrowBtn == null)
            {
                return null;
            }

            var tmp = Instantiate(_upArrowBtn, parent, false);
            DestroyImmediate(tmp.GetComponent<SignalOnUIButtonClick>());
            tmp.onClick = new Button.ButtonClickedEvent();
            return tmp;
        }

        // ReSharper disable once InconsistentNaming
        public TextMeshProUGUI CreateTMPText(RectTransform parent, string text, Vector2 position)
        {
            var textMesh = new GameObject().AddComponent<TextMeshProUGUI>();
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;
            textMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            textMesh.rectTransform.anchorMax = new Vector2(1f, 1f);
            textMesh.rectTransform.sizeDelta = new Vector2(60f, 10f);
            textMesh.rectTransform.anchoredPosition = position;

            return textMesh;
        }

        public void SetButtonText(ref Button button, string text)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>() != null)
            {

                button.GetComponentInChildren<TextMeshProUGUI>().text = text;
            }

        }

        public void SetButtonIcon(ref Button button, Sprite icon)
        {
            if (button.GetComponentsInChildren<UnityEngine.UI.Image>().Count() > 1)
            {

                button.GetComponentsInChildren<UnityEngine.UI.Image>()[1].sprite = icon;
            }

        }

        public void SetButtonBackground(ref Button button, Sprite background)
        {
            if (button.GetComponentsInChildren<Image>().Any())
            {

                button.GetComponentsInChildren<UnityEngine.UI.Image>()[0].sprite = background;
            }

        }
        public Button CreateBackButton(RectTransform parent)
        {
            if (_upArrowBtn == null)
            {
                return null;
            }

            var tmp = Instantiate(_backButtonInstance, parent, false);
            DestroyImmediate(tmp.GetComponent<SignalOnUIButtonClick>());
            tmp.onClick = new Button.ButtonClickedEvent();
            return tmp;
        }

    }
}