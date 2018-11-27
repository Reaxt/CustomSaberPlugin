using System;
using System.Collections.Generic;
using System.Linq;
using CustomSaber.UI;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace CustomSaber
{
    class CustomSaberListViewController : VRUIViewController, TableView.IDataSource
    {
        private new CustomSaberMasterViewController _parentViewController;

        private CustomSaberUI _customSaberUI;

        private Button _pageUpButton;
        private Button _pageDownButton;
        private Button _toggleButton;
        private int selected;
        public GameObject previewobj;
        private GameObject previewparent;
        private List<SaberSelection> _saberSelections = new List<SaberSelection>();
        private List<CustomSaber> _sabers = new List<CustomSaber>();
        public AssetBundle preview;
        private TableView _sabersTableView;
        LevelListTableCell _songListTableCellInstance;
        
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            previewparent = Instantiate(new GameObject("preview parent"));
            previewparent.transform.position = new Vector3(2.5f, 1, 0.3f);
            previewparent.transform.Rotate(new Vector3(0, -30, 0));
            _customSaberUI = CustomSaberUI.Instance;
            _parentViewController = GetComponentInParent<CustomSaberMasterViewController>();
            try
            {
                if (_pageDownButton == null)
                {
                    try
                    {
                        _pageDownButton = _customSaberUI.CreateButton(rectTransform, "PageDownButton");
                        ((RectTransform)_pageDownButton.transform).anchorMin = new Vector2(0.5f, 0f);
                        ((RectTransform)_pageDownButton.transform).anchorMax = new Vector2(0.5f, 0f);
                        ((RectTransform)_pageDownButton.transform).anchoredPosition = new Vector2(0f, 10f);
                        _pageDownButton.interactable = true;
                        _pageDownButton.onClick.AddListener(delegate ()
                        {
                            _sabersTableView.PageScrollDown();
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }
                if (_pageUpButton == null)
                {
                    try
                    {
                        _pageUpButton = _customSaberUI.CreateButton(rectTransform, "PageUpButton");
                        ((RectTransform)_pageUpButton.transform).anchorMin = new Vector2(0.5f, 1f);
                        ((RectTransform)_pageUpButton.transform).anchorMax = new Vector2(0.5f, 1f);
                        ((RectTransform)_pageUpButton.transform).anchoredPosition = new Vector2(0f, -14f);
                        _pageUpButton.interactable = true;
                        _pageUpButton.onClick.AddListener(delegate ()
                        {
                            _sabersTableView.PageScrollUp();
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }


                if (_sabersTableView == null)
                {
                    _sabersTableView = new GameObject().AddComponent<TableView>();

                    _sabersTableView.transform.SetParent(rectTransform, false);

                    var viewportMask = Instantiate(Resources.FindObjectsOfTypeAll<Mask>().First(), _sabersTableView.transform, false);
                    viewportMask.transform.DetachChildren();
                    _sabersTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Content").transform.SetParent(viewportMask.rectTransform, false);

                    ((RectTransform)_sabersTableView.transform).anchorMin = new Vector2(0f, 0.5f);
                    ((RectTransform)_sabersTableView.transform).anchorMax = new Vector2(1f, 0.5f);
                    ((RectTransform)_sabersTableView.transform).sizeDelta = new Vector2(0f, 60f);
                    ((RectTransform)_sabersTableView.transform).anchoredPosition = new Vector2(0f, 0f);

                    _sabersTableView.didSelectRowEvent += _sabersTableView_DidSelectRowEvent;

                    ReflectionUtil.SetPrivateField(_sabersTableView, "_pageUpButton", _pageUpButton);
                    ReflectionUtil.SetPrivateField(_sabersTableView, "_pageDownButton", _pageDownButton);

                    _sabersTableView.dataSource = this;

                    _sabersTableView.ScrollToRow(0, false);
                }
                else
                {
                    _sabersTableView.ReloadData();
                    _sabersTableView.ScrollToRow(0, false);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            /*_songListTableCellInstance =*/ Resources.FindObjectsOfTypeAll<LevelListTableCell>().ToList().ForEach(l => Console.WriteLine($"Cell: {l.name}"));//.First(x => (x.name == "StandardLevelListTableCell"));

            LoadSabers();

            base.DidActivate(true, ActivationType.AddedToHierarchy);
        }

        // ReSharper disable once InconsistentNaming
        public void LoadSabers()
        {
            foreach (string sab in Plugin.RetrieveCustomSabers())
            {
                var tempbundle = AssetBundle.LoadFromFile(sab);
                GameObject sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                SaberDescriptor tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                CustomSaber tempsab = new CustomSaber();
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

                _sabers.Add(tempsab);
                tempbundle.Unload(true);
            }
            Console.WriteLine("Added all sabers");
            
        }

        private void _sabersTableView_DidSelectRowEvent(TableView sender, int row)
        {
            Console.WriteLine(row);
            if (preview != null)
            {
                Destroy(previewobj);
                preview.Unload(true);
                preview = null;
            }
            preview = AssetBundle.LoadFromFile(_sabers[row].Path);
            GameObject previewsabers = preview.LoadAsset<GameObject>("_customsaber");
            previewobj = Instantiate(previewsabers, previewparent.transform);
            previewobj.transform.Find("RightSaber").transform.Translate(0, 0.5f, 0);

            selected = row;
            try
            {
                if (_parentViewController.customSaberDetailsViewController == null)
                {
                    GameObject _songDetailGameObject = Instantiate(Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First(), rectTransform, false).gameObject;
                    Destroy(_songDetailGameObject.GetComponent<StandardLevelDetailViewController>());
                    _parentViewController.customSaberDetailsViewController = _songDetailGameObject.AddComponent<CustomSaberDetailViewController>();

                    //_parentViewController.PushViewController(_parentViewController.customSaberDetailsViewController, false);
                    
                    _parentViewController.ModDetailsPushed = true;
                    SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
                }
                else
                {
                    if (_parentViewController.ModDetailsPushed)
                    {
                        SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
                    }
                    else
                    {
                        SetSaberDetailsData(_parentViewController.customSaberDetailsViewController, row);
                        //_parentViewController.PushViewController(_parentViewController.customSaberDetailsViewController, false);

                        _parentViewController.ModDetailsPushed = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetSaberDetailsData(CustomSaberDetailViewController saberDetails, int selectedMod)
        {
            saberDetails.UpdateContent(_sabers[selectedMod]);

            if (_toggleButton == null)
            {
                _toggleButton = saberDetails.GetComponentInChildren<Button>();
            }
            
            //TODO: IMPLEMENT STUFFS
            _toggleButton.gameObject.SetActive(true);
            if (Plugin._currentSaberPath == _sabers[selected].Path)
            {
                _customSaberUI.SetButtonText(ref _toggleButton, "Selected");
                _toggleButton.interactable = false;
            }
            else
            {
                _customSaberUI.SetButtonText(ref _toggleButton, "Select");
                _toggleButton.interactable = true;
            }
            

            DestroyImmediate(_toggleButton.GetComponent<SignalOnUIButtonClick>());
            _toggleButton.onClick = new Button.ButtonClickedEvent();
            _toggleButton.onClick.AddListener(delegate
            {
                Plugin._currentSaberPath = _sabers[selected].Path;
                _customSaberUI.SetButtonText(ref _toggleButton, "Selected");
                _toggleButton.interactable = false;
            });
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

