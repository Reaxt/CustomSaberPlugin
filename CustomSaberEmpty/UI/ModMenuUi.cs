using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomSaber
{
    class ModMenuUi : MonoBehaviour
    {
        static RectTransform _rightPos;
        static VRUIViewController _rightScreen;
        internal static ModMenuUi Instance;

        public static List<Sprite> Icons = new List<Sprite>();

        private MainMenuViewController _mainMenuViewController;
        private MainFlowCoordinator _menuMasterViewController;
        // Custom view controller
        private static ModMenuMasterViewController _modMenuController;

        private Button _buttonInstance;
        private Button _cogWheelButtonInstance;
        private Button _backButtonInstance;

        private Button _upArrowBtn;
        private Button _downArrowBtn;
        private RectTransform _mainMenuRectTransform;
        void Update()
        {
            //DEBUG LINE<
   

        }
        public static void OnLoad()
        {

            if (ModMenuUi.Instance != null)
            {
                ModMenuUi.Instance.Awake();
                return;
            }
            if (GameObject.FindObjectOfType<ModMenuUi>() != null)
            {
                return;
            }
            new GameObject("modmenu").AddComponent<ModMenuUi>();
        }

        void Awake()
        {

            Instance = this;
            //DontDestroyOnLoad(gameObject);

            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                Icons.Add(sprite);
            }

            try
            {
                // Get necessary button instances and main menu VC
                var allButtons = Resources.FindObjectsOfTypeAll<Button>();

                _buttonInstance = Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == "QuitButton");
                _cogWheelButtonInstance = allButtons.FirstOrDefault(x => x.name == "SettingsButton");
                _downArrowBtn = allButtons.First(x => x.name == "PageDownButton");
                _upArrowBtn = allButtons.First(x => x.name == "PageUpButton");
                _backButtonInstance = allButtons.First(x => x.name == "BackArrowButton");
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _menuMasterViewController = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                _mainMenuRectTransform = (RectTransform)_buttonInstance.transform.parent;

                AddModMenuButton();
            }
            catch (Exception ex)
            {
            }
        }

        private void AddModMenuButton()
        {
            try
            {
                _rightScreen =
                    ReflectionUtil.GetPrivateField<VRUIViewController>(_mainMenuViewController,
                        "_releaseInfoViewController");
                _rightPos = _rightScreen.gameObject.transform as RectTransform;
                var modMenuButton = CreateButton(_rightPos);
                SetButtonText(ref modMenuButton, "Saber Menu");
                SetButtonIcon(ref modMenuButton, Icons.First(x => x.name == "SingleSaberIcon"));

                if (modMenuButton == null)
                {
                    return;
                }

                // Change button text and add listener
                modMenuButton.onClick.AddListener(delegate
                {
                    try
                    {
                        if (_modMenuController == null)
                        {
                            _modMenuController = CreateViewController<ModMenuMasterViewController>();
                        }
                        try
                        {

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            throw;
                        }
                        _rightScreen.PresentModalViewController(_modMenuController, null);
                        //DELETE POSSIBLE DIFFICULTY TEXT
                        var modlisttemp = GameObject.FindObjectOfType<ModsListViewController>();
                        if (modlisttemp != null)
                        {
                            var textmeshs = modlisttemp.gameObject.GetComponentsInChildren<TextMeshProUGUI>();


                            foreach (TextMeshProUGUI textmesh in textmeshs)
                            {
                                if (textmesh.rectTransform.parent.name == "DifficultyTableCell(Clone)")
                                {
                                    DestroyImmediate(textmesh.rectTransform.parent.gameObject);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                });
            }

            catch (Exception ex)
            {
            }
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

        public Button CreateButton(RectTransform parent)
        {
            try
            {
                if (_cogWheelButtonInstance == null)
                {
                    return null;
                }

                // Create temporary button to return as new button
                var tmp = Instantiate(_cogWheelButtonInstance, parent, false);
                DestroyImmediate(tmp.GetComponent<GameEventOnUIButtonClick>());
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
                DestroyImmediate(tmp.GetComponent<GameEventOnUIButtonClick>());
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
            DestroyImmediate(tmp.GetComponent<GameEventOnUIButtonClick>());
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
            DestroyImmediate(tmp.GetComponent<GameEventOnUIButtonClick>());
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
            DestroyImmediate(tmp.GetComponent<GameEventOnUIButtonClick>());
            tmp.onClick = new Button.ButtonClickedEvent();
            return tmp;
        }

    }
}