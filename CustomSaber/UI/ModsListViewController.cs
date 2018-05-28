using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using IllusionInjector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomSaber
{
    class ModsListViewController : VRUIViewController, TableView.IDataSource
    {
        private new ModMenuMasterViewController _parentViewController;

        private ModMenuUi _modMenuUi;

        private Button _pageUpButton;
        private Button _pageDownButton;
        private Button _toggleButton;
        private int selected;
        public GameObject previewobj;
        private GameObject previewparent;
        private UnityEngine.UI.Toggle _toggleSwitch;
        private List<ModSelection> _modSelections = new List<ModSelection>();
        private TextMeshProUGUI _compatibilitytext;
        private List<CustSaber> _mods = new List<CustSaber>();
        public AssetBundle preview;
        private TableView _modsTableView;
        SongListTableCell _songListTableCellInstance;

        protected override void DidActivate()
        {
            previewparent = Instantiate(new GameObject("preview parent"));
            previewparent.transform.position = new Vector3(2.5f, 1, 0.3f);
            previewparent.transform.Rotate(new Vector3(0, -30, 0));
            _modMenuUi = FindObjectOfType<ModMenuUi>();
            _parentViewController = transform.parent.GetComponent<ModMenuMasterViewController>();

            try
            {
                if (_pageDownButton == null)
                {
                    try
                    {
                        _pageDownButton = _modMenuUi.CreateButton(rectTransform, "PageDownButton");

                        ((RectTransform)_pageDownButton.transform).anchorMin = new Vector2(0.5f, 0f);
                        ((RectTransform)_pageDownButton.transform).anchorMax = new Vector2(0.5f, 0f);
                        ((RectTransform)_pageDownButton.transform).anchoredPosition = new Vector2(0f, 10f);
                        _pageDownButton.interactable = true;
                        _pageDownButton.onClick.AddListener(delegate ()
                        {
                            _modsTableView.PageScrollDown();
                        });
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                if (_pageUpButton == null)
                {
                    try
                    {
                        _pageUpButton = _modMenuUi.CreateButton(rectTransform, "PageUpButton");
                        ((RectTransform)_pageUpButton.transform).anchorMin = new Vector2(0.5f, 1f);
                        ((RectTransform)_pageUpButton.transform).anchorMax = new Vector2(0.5f, 1f);
                        ((RectTransform)_pageUpButton.transform).anchoredPosition = new Vector2(0f, -14f);
                        _pageUpButton.interactable = true;
                        _pageUpButton.onClick.AddListener(delegate ()
                        {
                            _modsTableView.PageScrollUp();
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            _songListTableCellInstance = Resources.FindObjectsOfTypeAll<SongListTableCell>().First(x => (x.name == "SongListTableCell"));

            LoadMods();

            base.DidActivate();
        }

        private void LoadMods()
        {
            try
            {
                _mods = GetModsFromIPA();
                if (_mods.Count == 0)
                {
                }
                foreach (var mod in _mods)
                {

                }

                RefreshScreen();

            }
            catch (Exception ex)
            {
            }
        }

        // ReSharper disable once InconsistentNaming
        public List<CustSaber> GetModsFromIPA()
        {

            List<CustSaber> modsList = new List<CustSaber>();
            foreach (string sab in Plugin.RetrieveCustomSabers())
            {
                var tempbundle = AssetBundle.LoadFromFile(sab);
                GameObject sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                SaberDescriptor tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                CustSaber tempsab = new CustSaber();
                if (tempdesciptor == null)
                {
                    tempsab.Name = sab.Split('/').Last().Split('.').First();
                    tempsab.Author = "THIS SHOULD NEVER HAPPEN";
                    tempsab.Path = sab; 
                } else
                {
                    tempsab.Name = tempdesciptor.SaberName;
                    tempsab.Author = tempdesciptor.AuthorName;
                    tempsab.Path = sab;
                }

                modsList.Add(tempsab);
                tempbundle.Unload(true);
            }
            Console.WriteLine("Added all sabers");
            return modsList.ToList();
            
        }

        public void RefreshScreen()
        {
            if (_modsTableView == null)
            {
                _modsTableView = new GameObject().AddComponent<TableView>();

                _modsTableView.transform.SetParent(rectTransform, false);

                _modsTableView.dataSource = this;

                try
                {
                    var viewportMask = Instantiate(Resources.FindObjectsOfTypeAll<UnityEngine.UI.Mask>().First(), _modsTableView.transform, false);

                    _modsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Content").transform.SetParent(viewportMask.rectTransform, false);
                }
                catch
                {
                }

                ((RectTransform)_modsTableView.transform).anchorMin = new Vector2(0f, 0.5f);
                ((RectTransform)_modsTableView.transform).anchorMax = new Vector2(1f, 0.5f);
                ((RectTransform)_modsTableView.transform).sizeDelta = new Vector2(0f, 60f);
                ((RectTransform)_modsTableView.transform).anchoredPosition = new Vector2(0f, 0f);

                _modsTableView.DidSelectRowEvent += _modsTableView_DidSelectRowEvent;

                ReflectionUtil.SetPrivateField(_modsTableView, "_pageUpButton", _pageUpButton);
                ReflectionUtil.SetPrivateField(_modsTableView, "_pageDownButton", _pageDownButton);

                _modsTableView.ScrollToRow(0, false);
            }
            else
            {
                _modsTableView.ReloadData();
                _modsTableView.ScrollToRow(0, false);
            }
        }

        private void _modsTableView_DidSelectRowEvent(TableView sender, int row)
        {
            if (preview != null)
            {
                Destroy(previewobj);
                preview.Unload(true);
                preview = null;
                Console.WriteLine("aa");
            }
            Console.WriteLine("h");
            preview = AssetBundle.LoadFromFile(_mods[row].Path);
            GameObject previewsabers = preview.LoadAsset<GameObject>("_customsaber");
            previewobj = Instantiate(previewsabers, previewparent.transform);
            previewobj.transform.Find("RightSaber").transform.Translate(0, 0.5f, 0);

            selected = row;
            try
            {
                if (_parentViewController.ModDetailsViewController == null)
                {
                    _parentViewController.ModDetailsViewController = Instantiate(Resources.FindObjectsOfTypeAll<SongDetailViewController>().First(), rectTransform, false);

                    SetModDetailsData(_parentViewController.ModDetailsViewController, row);

                    _parentViewController.PushViewController(_parentViewController.ModDetailsViewController, false);

                    _parentViewController.ModDetailsPushed = true;
                }
                else
                {
                    if (_parentViewController.ModDetailsPushed)
                    {
                        SetModDetailsData(_parentViewController.ModDetailsViewController, row);
                        _parentViewController.PushViewController(_parentViewController.ModDetailsViewController, false);
                    }
                    else
                    {
                        SetModDetailsData(_parentViewController.ModDetailsViewController, row);
                        _parentViewController.PushViewController(_parentViewController.ModDetailsViewController, false);

                        _parentViewController.ModDetailsPushed = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetModDetailsData(SongDetailViewController modDetails, int selectedMod)
        {
            modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "SongNameText").text = _mods[selectedMod].Name;
            modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "DurationText").text = "Author";
            modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "DurationValueText").text = _mods[selectedMod].Author;
            try
            {
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "BPMText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "BPMValueText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "BPMValueText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "NotesCountText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "NotesCountValueText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "DurationText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "DurationValueText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "ObstaclesCountText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "ObstaclesCountValueText").gameObject);

                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "Title").gameObject);

                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "HighScoreText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "HighScoreValueText").gameObject);

                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "MaxComboText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "MaxComboValueText").gameObject);

                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "MaxRankText").gameObject);
                Destroy(modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "MaxRankValueText").gameObject);

                Destroy(modDetails.GetComponentsInChildren<RectTransform>().First(x => x.name == "YourStats").gameObject);

            }
            catch (Exception e)
            {
            }
            if (_toggleButton == null)
            {
                _toggleButton = modDetails.GetComponentInChildren<Button>();
            }
            if (_compatibilitytext == null)
            {
               // var temp = modDetails.GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name == "DurationText");

                //_compatibilitytext = _modMenuUi.CreateTMPText(temp.rectTransform, "a", new Vector2(0.7f, 0.5f));
                // ive tried a lot of stuff im sorry for this horrible solution
                // it's ok ^ - Kaori
                //_compatibilitytext.text = "                   This plugin doesnt work with BSMODUI";
            }
            if (true)
            {
                //Utils.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                //TODO: IMPLEMENT STUFFS
                _toggleButton.gameObject.SetActive(true);
                _modMenuUi.SetButtonText(ref _toggleButton, "Select");
                //_compatibilitytext.gameObject.SetActive(false);
                
                DestroyImmediate(_toggleButton.GetComponent<GameEventOnUIButtonClick>());
                _toggleButton.onClick = new Button.ButtonClickedEvent();
                _toggleButton.onClick.AddListener(delegate {
                    Plugin._currentSaberPath =_mods[selected].Path;
                });
            }
            else
            {
                _toggleButton.gameObject.SetActive(false);
                _compatibilitytext.gameObject.SetActive(true);
            }
        }

        public float RowHeight()
        {
            return 10f;
        }

        public int NumberOfRows()
        {
            return _mods.Count;
        }

        public TableCell CellForRow(int row)
        {
            var tableCell = Instantiate(_songListTableCellInstance);

            var mod = _mods.ElementAtOrDefault(row);
            tableCell.songName = mod?.Name;
            tableCell.author = mod?.Author;
            tableCell.coverImage = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);
            
            return tableCell;
        }


    }
}
