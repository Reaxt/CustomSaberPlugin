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
        private int selected;
        public GameObject _saberPreview;
        private GameObject PreviewSaber;
        private GameObject _previewParent;
        public GameObject _saberPreviewA;
        public GameObject _saberPreviewB;
        public GameObject _saberPreviewAParent;
        public GameObject _saberPreviewBParent;
        private Mesh _BladeA;
        private Mesh _GlowingA;
        private Mesh _NormalA;
        private Mesh _BladeB;
        private Mesh _GlowingB;
        private Mesh _NormalB;


        private MenuShockwave menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();

        public Button _pageUpButton;
        public Button _pageDownButton;
        public Button _backButton;
        public TextMeshProUGUI _versionNumber;

        private List<SaberSelection> _saberSelections = new List<SaberSelection>();
        private List<CustomSaber> _sabers = new List<CustomSaber>();

        public TableView _sabersTableView;
        LevelListTableCell _songListTableCellInstance;

        private bool PreviewStatus;
        private bool menuShockwaveOriginalState;

        public Action backButtonPressed;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            try
            {
                LoadSabers(firstActivation);
                menuShockwaveOriginalState = menuShockwave.enabled;
                menuShockwave.enabled = false;

                if (firstActivation)
                {
                    for (int i=0; i<_sabers.Count; i++)
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
                            DestroyPreview();
                            UnLoadSabers();
                            menuShockwave.enabled = menuShockwaveOriginalState;
                        });
                    }
                }
                //else
                //{
                //    _sabersTableView.ReloadData();
                //}
                
                _sabersTableView.SelectRow(selected);
                _sabersTableView.ScrollToRow(selected, true);

                PreviewCurrent();
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

        private void PreviewCurrent()
        {
            if (selected != 0)
            {
                GeneratePreview(selected);
            }
            else
            {
                GeneratePreviewOriginal();
            }
        }

        public void RefreshScreen()
        {
            _sabersTableView.ReloadData();
        }

        // ReSharper disable once InconsistentNaming
        public void LoadSabers(bool FirstRun)
        {
            Console.WriteLine("Loading sabers!");
            if (FirstRun)
            {
                foreach (string sab in Plugin.RetrieveCustomSabers())
                {
                    CustomSaber tempsab = new CustomSaber();
                    if (sab == "DefaultSabers")
                    {
                        tempsab.Name = "Default Sabers";
                        tempsab.Author = "Beat Games";
                        tempsab.Path = "DefaultSabers";
                        tempsab.AssetBundle = null;
                        tempsab.GameObject = null;
                    }
                    else
                    {
                        try
                        {
                            AssetBundle tempbundle = AssetBundle.LoadFromFile(sab);
                            GameObject sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                            SaberDescriptor tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                            if (tempdesciptor == null)
                            {
                                tempsab.Name = sab.Split('/').Last().Split('.').First();
                                tempsab.Author = "THIS SHOULD NEVER HAPPEN";
                                tempsab.Path = sab;
                                tempsab.AssetBundle = null;
                                tempsab.GameObject = null;
                            }
                            else
                            {
                                tempsab.Name = tempdesciptor.SaberName;
                                tempsab.Author = tempdesciptor.AuthorName;
                                tempsab.Path = sab;
                                tempsab.AssetBundle = tempbundle;
                                tempsab.GameObject = sabroot;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            tempsab.Name = "This saber is broken, delete it.";
                            tempsab.Author = sab.Split('/').Last();//.Split('.').First();
                            tempsab.Path = sab;
                            tempsab.AssetBundle = null;
                            tempsab.GameObject = null;
                        }
                    }
                    _sabers.Add(tempsab);
                }
            }
            else
            {
                foreach (CustomSaber tempsab in _sabers)
                {
                    if (tempsab.Path != "DefaultSabers")
                    {
                        var tempbundle = AssetBundle.LoadFromFile(tempsab.Path);
                        GameObject sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                        SaberDescriptor tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                        if (tempdesciptor == null)
                        {
                            tempsab.AssetBundle = null;
                            tempsab.GameObject = null;
                        }
                        else
                        {
                            tempsab.AssetBundle = tempbundle;
                            tempsab.GameObject = sabroot;
                        }
                    }
                }
            }
            Console.WriteLine("Added all sabers");
        }

        public void UnLoadSabers()
        {
            Console.WriteLine("Unloading sabers!");
            foreach (CustomSaber saber in _sabers)
            {
                if(saber.Path != "DefaultSabers") { 
                    saber.AssetBundle.Unload(true);
                    saber.AssetBundle = null;
                    saber.GameObject = null;
                }
            }
        }

        private void _sabersTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Plugin._currentSaberPath = _sabers[row].Path;
            selected = row;
            if (row == 0)
            {
                GeneratePreviewOriginal();
            }
            else
            {
                GeneratePreview(row);
            }
        }

        public void DestroyPreview()
        {
            DestroyOriginalPreview();
            if (_saberPreview)
                Destroy(_saberPreview);
            PreviewSaber = null;
            if(_previewParent)
                Destroy(_previewParent);
        }

        public void GeneratePreview(int SaberIndex)
        {
            Plugin._currentSaberPath = _sabers[SaberIndex].Path;
            selected = SaberIndex;
            Console.WriteLine($"Selected saber {_sabers[SaberIndex].Name} created by {_sabers[SaberIndex].Author}");

            if (PreviewStatus)
            {
                return;
            }
            PreviewStatus = true;
            DestroyPreview();

            if (_sabers[SaberIndex] != null)
            {
                try
                {
                    PreviewSaber = _sabers[SaberIndex].GameObject;

                    _previewParent = new GameObject();
                    _previewParent.transform.Translate(2.2f, 1.1f, 0.6f);
                    _previewParent.transform.Rotate(0, -30, 0);
                    if (PreviewSaber)
                    {
                        _saberPreview = Instantiate(PreviewSaber, _previewParent.transform);
                        _saberPreview.transform.Find("LeftSaber").transform.localPosition = new Vector3(0, 0, 0);
                        _saberPreview.transform.Find("RightSaber").transform.localPosition = new Vector3(0, 0, 0);
                        _saberPreview.transform.Find("RightSaber").transform.Translate(0, 0.5f, 0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("Failed to load preview. " + _sabers[SaberIndex].Name);
            }
            PreviewStatus = false;
        }

        public void DestroyOriginalPreview()
        {
            if (_saberPreviewAParent)
                _saberPreviewAParent.SetActive(false);
            if (_saberPreviewBParent)
                _saberPreviewBParent.SetActive(false);
        }

        public void GeneratePreviewOriginal()
        {
            if (Plugin.LeftSaber == null || Plugin.RightSaber == null)
                return;
            PreviewStatus = true;
            DestroyPreview();
            try
            {
                if (_saberPreviewAParent)
                {
                    _saberPreviewAParent.SetActive(true);
                    if (_saberPreviewA)
                        _saberPreviewA.SetActive(true);

                    foreach (Transform t in _saberPreviewA.transform)
                    {
                        var filter = t.GetComponentInChildren<MeshFilter>();
                        foreach (Transform t2 in t)
                        {
                            filter = t2.GetComponentInChildren<MeshFilter>();
                            if (filter.name == "Blade")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _BladeA;
                            if (filter.name == "Normal")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _NormalA;
                            if (filter.name == "Glowing")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _GlowingA;
                        }
                    }
                }
                else
                {
                    _saberPreviewAParent = new GameObject("Saber Prewview A Parent");
                    if (_saberPreviewAParent) DontDestroyOnLoad(_saberPreviewAParent.gameObject);
                    _saberPreviewA = Instantiate(Plugin.LeftSaber.gameObject, _saberPreviewAParent.transform);
                    Destroy(_saberPreviewA.GetComponent<VRController>());
                    _saberPreviewA.SetActive(true);

                    foreach (Transform t in _saberPreviewA.transform)
                    {
                        var filter = t.GetComponentInChildren<MeshFilter>();
                        foreach (Transform t2 in t)
                        {
                            filter = t2.GetComponentInChildren<MeshFilter>();
                            if (filter.name == "Blade")
                                _BladeA = filter.mesh;
                            if (filter.name == "Normal")
                                _NormalA = filter.mesh;
                            if (filter.name == "Glowing")
                                _GlowingA = filter.mesh;
                        }
                    }

                    _saberPreviewA.transform.localEulerAngles = new Vector3(0, 0, 0);
                    _saberPreviewA.transform.localPosition = new Vector3(0, 0, 0);
                    _saberPreviewAParent.transform.position = new Vector3(2.2f, 1.1f, 0.6f);
                    _saberPreviewAParent.transform.eulerAngles = new Vector3(0, -30, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try {
                if (_saberPreviewBParent)
                {
                    _saberPreviewBParent.SetActive(true);
                    if (_saberPreviewB)
                        _saberPreviewB.SetActive(true);
                    foreach (Transform t in _saberPreviewB.transform)
                    {
                        var filter = t.GetComponentInChildren<MeshFilter>();
                        foreach (Transform t2 in t)
                        {
                            filter = t2.GetComponentInChildren<MeshFilter>();
                            if (filter.name == "Blade")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _BladeB;
                            if (filter.name == "Normal")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _NormalB;
                            if (filter.name == "Glowing")
                                if (filter.sharedMesh == null)
                                    filter.sharedMesh = _GlowingB;
                        }
                    }
                }
                else
                {
                    _saberPreviewBParent = new GameObject("Saber Prewview B Parent");
                    if (_saberPreviewBParent) DontDestroyOnLoad(_saberPreviewBParent.gameObject);
                    _saberPreviewB = Instantiate(Plugin.RightSaber.gameObject, _saberPreviewBParent.transform);
                    Destroy(_saberPreviewB.GetComponent<VRController>());
                    _saberPreviewB.SetActive(true);

                    foreach (Transform t in _saberPreviewB.transform)
                    {
                        var filter = t.GetComponentInChildren<MeshFilter>();
                        foreach (Transform t2 in t)
                        {
                            filter = t2.GetComponentInChildren<MeshFilter>();
                            if (filter.name == "Blade")
                                _BladeB = filter.mesh;
                            if (filter.name == "Normal")
                                _NormalB = filter.mesh;
                            if (filter.name == "Glowing")
                                _GlowingB = filter.mesh;
                        }
                    }

                    _saberPreviewB.transform.localEulerAngles = new Vector3(0, 0, 0);
                    _saberPreviewB.transform.localPosition = new Vector3(0, 0, 0);
                    _saberPreviewBParent.transform.position = new Vector3(2.2f, 1.6f, 0.6f);
                    _saberPreviewBParent.transform.eulerAngles = new Vector3(0, -30, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            PreviewStatus = false;
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
