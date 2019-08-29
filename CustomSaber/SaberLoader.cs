using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomSaber
{
    class SaberLoader
    {
        internal static List<CustomSaber> AllSabers = new List<CustomSaber>();
        internal static List<AssetBundle> AssetBundles = new List<AssetBundle>();

        static Sprite _defaultImage;
        static Sprite _defaultImageError;
        static Sprite _defaultSabersImage;

        private static bool _firstRun = true;

        public static void LoadSabers()
        {
            Logger.Log("Loading sabers!");

            _defaultImage = UIUtilities.LoadSpriteFromResources("CustomSaber.Resources.fa-magic.png");
            _defaultImage.texture.wrapMode = TextureWrapMode.Clamp;
            _defaultImageError = UIUtilities.LoadSpriteFromResources("CustomSaber.Resources.fa-magic-error.png");
            _defaultImageError.texture.wrapMode = TextureWrapMode.Clamp;
            var defaultSabersImageFolder = "CustomSaber.Resources.DefaultSabers";
            var defaultSabersImagePaths = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(file => file.StartsWith(defaultSabersImageFolder)).ToList();
            Logger.Log(defaultSabersImagePaths.Count.ToString());
            foreach (var s in defaultSabersImagePaths)
            {
                Logger.Log(s);
            }

            var r = new System.Random();
            _defaultSabersImage = UIUtilities.LoadSpriteFromResources(defaultSabersImagePaths.ElementAt(r.Next(defaultSabersImagePaths.Count)));
            _defaultSabersImage.texture.wrapMode = TextureWrapMode.Clamp;

            if (_firstRun)
            {
                _firstRun = false;
                foreach (var sab in Plugin.RetrieveCustomSabers())
                {
                    var tempsab = new CustomSaber();
                    if (sab == "DefaultSabers")
                    {
                        AssetBundles.Add(null);
                        tempsab.Name = "Default Sabers";
                        tempsab.Author = "Beat Games";
                        tempsab.CoverImage = _defaultSabersImage;
                        tempsab.Path = "DefaultSabers";
                        tempsab.AssetBundle = null;
                        tempsab.GameObject = null;
                    }
                    else
                    {
                        try
                        {
                            var tempbundle = AssetBundle.LoadFromFile(sab);
                            AssetBundles.Add(tempbundle);
                            var sabroot = tempbundle.LoadAsset<GameObject>("_CustomSaber");
                            Logger.Log(sabroot.name);
                            var tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
                            if (tempdesciptor == null)
                            {
                                tempsab.Name = sab.Split('/').Last().Split('.').First();
                                tempsab.Author = "THIS SHOULD NEVER HAPPEN";
                                tempsab.CoverImage = _defaultImageError;
                                tempsab.Path = sab;
                                tempsab.AssetBundle = null;
                                tempsab.GameObject = null;
                            }
                            else
                            {
                                tempsab.Name = tempdesciptor.SaberName;
                                tempsab.Author = tempdesciptor.AuthorName;
                                if (tempdesciptor.CoverImage)
                                {
                                    tempdesciptor.CoverImage.texture.wrapMode = TextureWrapMode.Clamp;
                                }

                                tempsab.CoverImage = (tempdesciptor.CoverImage) ? tempdesciptor.CoverImage : _defaultImage;
                                tempsab.Path = sab;
                                tempsab.AssetBundle = tempbundle;
                                tempsab.GameObject = sabroot;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex, LogLevel.Warning);
                            tempsab.Name = "This saber is broken, delete it.";
                            tempsab.Author = sab.Split('/').Last();//.Split('.').First();
                            tempsab.CoverImage = _defaultImageError;
                            tempsab.Path = sab;
                            tempsab.AssetBundle = null;
                            tempsab.GameObject = null;
                        }
                    }
                    AllSabers.Add(tempsab);
                }
            }
            else
            {
                foreach (var tempsab in AllSabers)
                {
                    if (tempsab.Path != "DefaultSabers")
                    {
                        var tempbundle = AssetBundle.LoadFromFile(tempsab.Path);
                        var sabroot = tempbundle.LoadAsset<GameObject>("_customsaber");
                        var tempdesciptor = sabroot.GetComponent<SaberDescriptor>();
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
            Logger.Log("Added all sabers", LogLevel.Debug);
        }

        public static void UnLoadSabers()
        {
            Logger.Log("Unloading sabers!", LogLevel.Debug);
            foreach (var saber in SaberLoader.AllSabers)
            {
                if (saber.Path != "DefaultSabers")
                {
                    saber.AssetBundle.Unload(true);
                    saber.AssetBundle = null;
                    saber.GameObject = null;
                }
            }
        }

        public static int FindSaberByName(string name)
        {
            var index = AllSabers.FindIndex(saber => saber.Name == name);
            return (index >= 0) ? index : 0;
        }

        public static AssetBundle GetSaberAssetBundle(string name)
        {
            var index = FindSaberByName(name);
            if (index < 0 || index > AssetBundles.Count)
            {
                return null;
            }

            return AssetBundles[index];
        }
    }
}
