using CustomSaber.Utilities;
using System.IO;
using UnityEngine;

namespace CustomSaber.Data
{
    public class CustomSaberData
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public SaberDescriptor SaberDescriptor { get; }
        public GameObject Sabers { get; }

        public CustomSaberData(string fileName)
        {
            FileName = fileName;

            if (fileName != "DefaultSabers")
            {
                try
                {
                    AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.PluginAssetPath, fileName));
                    Sabers = AssetBundle.LoadAsset<GameObject>("_CustomSaber");
                    SaberDescriptor = Sabers.GetComponent<SaberDescriptor>();

                    if (SaberDescriptor.CoverImage == null)
                    {
                        SaberDescriptor.CoverImage = Utils.GetDefaultCoverImage();
                    }
                }
                catch
                {
                    Logger.log.Warn($"Something went wrong getting the AssetBundle for '{fileName}'!");

                    SaberDescriptor = new SaberDescriptor
                    {
                        SaberName = "Invalid Saber (Delete it)",
                        AuthorName = FileName,
                        CoverImage = Utils.GetErrorCoverImage()
                    };

                    FileName = "DefaultSabers";
                }
            }
            else
            {
                SaberDescriptor = new SaberDescriptor
                {
                    SaberName = "Default",
                    AuthorName = "Beat Saber",
                    CoverImage = Utils.GetRandomCoverImage(),
                };
            }
        }

        public CustomSaberData(Saber leftSaber, Saber rightSaber)
        {
            FileName = "DefaultSabers";

            SaberDescriptor = new SaberDescriptor
            {
                SaberName = "Default",
                AuthorName = "Beat Games",
                CoverImage = Utils.GetRandomCoverImage(),
            };

            GameObject saberParent = new GameObject();
            if (saberParent)
            {
                leftSaber.transform.SetParent(saberParent.transform);
                rightSaber.transform.SetParent(saberParent.transform);
            }

            Sabers = saberParent;
        }
    }
}
