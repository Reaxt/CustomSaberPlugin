using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;
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

            _defaultImage = LoadSpriteFromResources("CustomSaber.Resources.fa-magic.png");
            _defaultImage.texture.wrapMode = TextureWrapMode.Clamp;
            _defaultImageError = LoadSpriteFromResources("CustomSaber.Resources.fa-magic-error.png");
            _defaultImageError.texture.wrapMode = TextureWrapMode.Clamp;
            var defaultSabersImageFolder = "CustomSaber.Resources.DefaultSabers";
            var defaultSabersImagePaths = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(file => file.StartsWith(defaultSabersImageFolder)).ToList();

            var r = new System.Random();
            _defaultSabersImage = LoadSpriteFromResources(defaultSabersImagePaths.ElementAt(r.Next(defaultSabersImagePaths.Count)));
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
                            var tempDescriptor = sabroot.GetComponent<SaberDescriptor>();
                            Logger.Log($"Loading {tempDescriptor?.SaberName}");
                            if (tempDescriptor == null)
                            {
                                Logger.Log($"SaberDescriptor not found for {sab}", Logger.LogLevel.Warning);
                                tempsab.Name = sab.Split('/').Last().Split('.').First();
                                tempsab.Author = "THIS SHOULD NEVER HAPPEN";
                                tempsab.CoverImage = _defaultImageError;
                                tempsab.Path = sab;
                                tempsab.AssetBundle = null;
                                tempsab.GameObject = null;
                            }
                            else
                            {
                                tempsab.Name = tempDescriptor.SaberName;
                                tempsab.Author = tempDescriptor.AuthorName;
                                if (tempDescriptor.CoverImage)
                                {
                                    tempDescriptor.CoverImage.texture.wrapMode = TextureWrapMode.Clamp;
                                }

                                tempsab.CoverImage = (tempDescriptor.CoverImage) ? tempDescriptor.CoverImage : _defaultImage;
                                tempsab.Path = sab;
                                tempsab.AssetBundle = tempbundle;
                                tempsab.GameObject = sabroot;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Saber {sab} failed to load.");
                            Logger.Log($"{ex.Message}\n{ex.StackTrace}" , Logger.LogLevel.Warning);
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
            Logger.Log($"Added {AllSabers.Count - 1} sabers");
        }

        public static void UnLoadSabers()
        {
            Logger.Log("Unloading sabers!");
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
            return (index >= 0) ? index : -1;
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

        // Image helpers

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(file))
                    return Tex2D;
            }
            return null;
        }

        public static Texture2D LoadTextureFromFile(string FilePath)
        {
            if (File.Exists(FilePath))
                return LoadTextureRaw(File.ReadAllBytes(FilePath));

            return null;
        }

        public static Texture2D LoadTextureFromResources(string resourcePath)
        {
            return LoadTextureRaw(GetResource(Assembly.GetCallingAssembly(), resourcePath));
        }

        public static Sprite LoadSpriteRaw(byte[] image, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (SpriteTexture)
                return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
            return null;
        }

        public static Sprite LoadSpriteFromFile(string FilePath, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureFromFile(FilePath), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromResources(string resourcePath, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteRaw(GetResource(Assembly.GetCallingAssembly(), resourcePath), PixelsPerUnit);
        }

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            System.IO.Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
