using CustomUI.BeatSaber;
using CustomUI.Utilities;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomSaber
{
    class SaberListViewController : VRUIViewController, TableView.IDataSource
    {
        private Button _toggleButton;
        private int selected;
        public GameObject previewobj;
        private GameObject previewparent;
        public AssetBundle preview;

        public Button _pageUpButton;
        public Button _pageDownButton;
        public Button _backButton;
        public TextMeshProUGUI _versionNumber;

        private List<SaberSelection> _saberSelections = new List<SaberSelection>();
        private List<CustomSaber> _sabers = new List<CustomSaber>();

        public TableView _sabersTableView;
        LevelListTableCell _songListTableCellInstance;

        public Action backButtonPressed;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                if (firstActivation)
                {
                    LoadSabers();
                    
                    for(int i=0; i<_sabers.Count; i++)
                        if (_sabers[i].Path == Plugin._currentSaberPath)
                            selected = i;

                    _songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                    RectTransform container = new GameObject("SabersListContainer", typeof(RectTransform)).transform as RectTransform;
                    container.SetParent(rectTransform, false);
                    container.sizeDelta = new Vector2(60f, 0f);

                    _sabersTableView = new GameObject("SabersListTableView").AddComponent<TableView>();
                    _sabersTableView.gameObject.AddComponent<RectMask2D>();
                    _sabersTableView.transform.SetParent(container, false);

                    (_sabersTableView.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
                    (_sabersTableView.transform as RectTransform).anchorMax = new Vector2(1f, 1f);
                    (_sabersTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 60f);
                    (_sabersTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, 0f);

                    _sabersTableView.SetPrivateField("_preallocatedCells", new TableView.CellsGroup[0]);
                    _sabersTableView.SetPrivateField("_isInitialized", false);
                    _sabersTableView.dataSource = this;

                    _sabersTableView.didSelectRowEvent += _sabersTableView_DidSelectRowEvent;

                    _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), container, false);
                    (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 30f);//-14
                    _pageUpButton.interactable = true;
                    _pageUpButton.onClick.AddListener(delegate ()
                    {
                        _sabersTableView.PageScrollUp();
                    });

                    _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), container, false);
                    (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -30f);//8
                    _pageDownButton.interactable = true;
                    _pageDownButton.onClick.AddListener(delegate ()
                    {
                        _sabersTableView.PageScrollDown();
                    });


                    _versionNumber = Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(x => (x.name == "Text")), rectTransform, false);

                    (_versionNumber.transform as RectTransform).anchoredPosition = new Vector2(-10f, 10f);
                    (_versionNumber.transform as RectTransform).anchorMax = new Vector2(1f, 0f);
                    (_versionNumber.transform as RectTransform).anchorMin = new Vector2(1f, 0f);

                    string versionNumber = (IllusionInjector.PluginManager.Plugins.Where(x => x.Name == "Saber Mod").First()).Version;
                    _versionNumber.text = "v" + versionNumber;
                    _versionNumber.fontSize = 5;
                    _versionNumber.color = Color.white;

                    if (_backButton == null)
                    {
                        _backButton = BeatSaberUI.CreateBackButton(rectTransform as RectTransform);
                        _backButton.onClick.AddListener(delegate ()
                        {
                            if (backButtonPressed != null) backButtonPressed();
                        });
                    }
                }
                //else
                //{
                //    _sabersTableView.ReloadData();
                //}
                
                _sabersTableView.SelectRow(selected);
                _sabersTableView.ScrollToRow(selected, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("[CustomSaber] EXCEPTION IN DidActivate: " + e);
            }
        }

        protected override void DidDeactivate(DeactivationType type)
        {
            base.DidDeactivate(type);
        }

        public void RefreshScreen()
        {
            _sabersTableView.ReloadData();
        }

        // ReSharper disable once InconsistentNaming
        public void LoadSabers()
        {
            Console.WriteLine("Loading sabers!");
            foreach (string sab in Plugin.RetrieveCustomSabers())
            {
                CustomSaber tempsab = new CustomSaber();
                if (sab == "DefaultSabers")
                {
                    tempsab.Name = "Default Sabers";
                    tempsab.Author = "Beat Saber";
                    tempsab.Path = "DefaultSabers";
                }
                else
                {
                    var tempbundle = AssetBundle.LoadFromFile(sab);
                    GameObject sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                    SaberDescriptor tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                    if (tempdesciptor == null)
                    {
                        tempsab.Name = sab.Split('/').Last().Split('.').First();
                        tempsab.Author = "THIS SHOULD NEVER HAPPEN";
                        tempsab.Path = sab;
                    }
                    else
                    {
                        tempsab.Name = tempdesciptor.SaberName;
                        tempsab.Author = tempdesciptor.AuthorName;
                        tempsab.Path = sab;
                    }
                    tempbundle.Unload(true);
                }

                _sabers.Add(tempsab);
            }
            Console.WriteLine("Added all sabers");

        }

        private void _sabersTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Plugin._currentSaberPath = _sabers[row].Path;
            selected = row;
            Console.WriteLine($"Selected saber {_sabers[row].Name} created by {_sabers[row].Author}");

            //if (preview != null)
            //{
            //    Destroy(previewobj);
            //    preview.Unload(true);
            //    preview = null;
            //}
            //preview = AssetBundle.LoadFromFile(_sabers[row].Path);
            //GameObject previewsabers = preview.LoadAsset<GameObject>("_customsaber");
            //previewobj = Instantiate(previewsabers, previewparent.transform);
            //previewobj.transform.Find("RightSaber").transform.Translate(0, 0.5f, 0);

            //try
            //{
            //    if (_parentViewController.customSaberDetailsViewController == null)
            //    {
            //        GameObject _songDetailGameObject = Instantiate(Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First(), rectTransform, false).gameObject;
            //        Destroy(_songDetailGameObject.GetComponent<StandardLevelDetailViewController>());
            //        _parentViewController.customSaberDetailsViewController = _songDetailGameObject.AddComponent<CustomSaberDetailViewController>();

            //        StartCoroutine(_parentViewController.PushViewControllerCoroutine(_parentViewController.customSaberDetailsViewController, () =>
            //        {
            //            _parentViewController.ModDetailsPushed = true;
            //            SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
            //        }, true));


            //    }
            //    else
            //    {
            //        if (_parentViewController.ModDetailsPushed)
            //        {
            //            SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
            //        }
            //        else
            //        {
            //            SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
            //            StartCoroutine(_parentViewController.PushViewControllerCoroutine(_parentViewController.customSaberDetailsViewController, () => { _parentViewController.ModDetailsPushed = true; }, true));


            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }

        private void SetSaberDetailsData(object saberDetails, int selectedMod)
        {
            //try
            //{
            //    saberDetails.UpdateContent(_sabers[selectedMod]);

            //    if (_toggleButton == null)
            //    {
            //        _toggleButton = saberDetails.GetComponentInChildren<Button>();
            //    }

            //    //TODO: IMPLEMENT STUFFS
            //    _toggleButton.gameObject.SetActive(true);
            //    if (Plugin._currentSaberPath == _sabers[selected].Path)
            //    {
            //        _toggleButton.SetButtonText("Selected");
            //        _toggleButton.interactable = false;
            //    }
            //    else
            //    {
            //        _toggleButton.SetButtonText("Select");
            //        _toggleButton.interactable = true;
            //    }


            //    DestroyImmediate(_toggleButton.GetComponent<SignalOnUIButtonClick>());
            //    _toggleButton.onClick = new Button.ButtonClickedEvent();
            //    _toggleButton.onClick.AddListener(delegate
            //    {
            //        Plugin._currentSaberPath = _sabers[selected].Path;
            //        _toggleButton.SetButtonText("Selected");
            //        _toggleButton.interactable = false;
            //    });
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"Died in SetSaberDetailData {e.ToString()}");
            //}
        }

        public float RowHeight()
        {
            return 10f;
        }

        public int NumberOfRows()
        {
            return _sabers.Count;
        }

        public TableCell CellForRow(int row)
        {
            var tableCell = Instantiate(_songListTableCellInstance);

            var saber = _sabers.ElementAtOrDefault(row);
            tableCell.songName = saber?.Name;
            tableCell.author = saber?.Author;
            tableCell.coverImage = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);

            return tableCell;
        }
    }
}
