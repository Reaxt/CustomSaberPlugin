using System;
using System.Linq;
using LogLevel = IPA.Logging.Logger.Level;
using UnityEngine;

namespace CustomSaber
{
    public enum ColorType
    {
        LeftSaber,
        RightSaber,
        CustomColor
    }

    public class CustomTrail : MonoBehaviour
    {
        public Transform PointStart;
        public Transform PointEnd;
        public Material TrailMaterial;
        public ColorType colorType = ColorType.CustomColor;
        public Color TrailColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color MultiplierColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public int Length = 20;

        private CustomWeaponTrail trail;
        private ColorManager oldColorManager;
        private XWeaponTrailRenderer oldTrailRendererPrefab;
        private Saber saber;

        public void Init(Saber parentSaber)
        {
            Logger.Log("Replacing Trail", LogLevel.Debug);

            saber = parentSaber;

            if (gameObject.name != "LeftSaber" && gameObject.name != "RightSaber")
            {
                Logger.Log("Parent not LeftSaber or RightSaber", LogLevel.Warning);
                Destroy(this);
            }

            if (saber == null)
            {
                Logger.Log("Saber not found", LogLevel.Warning);
                Destroy(this);
            }

            SaberWeaponTrail[] trails = Resources.FindObjectsOfTypeAll<SaberWeaponTrail>().ToArray();
            for (int i = 0; i < trails.Length; i++)
            {
                SaberWeaponTrail trail = trails[i];
                ReflectionUtil.SetPrivateField(trail, "_multiplierSaberColor", new Color(0f, 0f, 0f, 0f));
            }

            SaberWeaponTrail oldtrail = Resources.FindObjectsOfTypeAll<GameCoreInstaller>()
                .FirstOrDefault()?.GetPrivateField<BasicSaberModelController>("_basicSaberModelControllerPrefab")
                ?.GetPrivateField<SaberWeaponTrail>("_saberWeaponTrail");

            if (oldtrail != null)
            {
                try
                {
                    Logger.Log(ReflectionUtil.GetPrivateField<Color>(oldtrail, "_multiplierSaberColor").ToString(), LogLevel.Debug);
                    //ReflectionUtil.SetPrivateField(oldtrail, "_multiplierSaberColor", new Color(0f, 0f, 0f, 0f));
                    oldColorManager = ReflectionUtil.GetPrivateField<ColorManager>(oldtrail, "_colorManager");
                    oldTrailRendererPrefab = ReflectionUtil.GetPrivateField<XWeaponTrailRenderer>(oldtrail, "_trailRendererPrefab");

                    //oldtrail.Start();
                    //oldtrail.gameObject.SetActive(false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, LogLevel.Critical);
                    throw;
                }

                trail = gameObject.AddComponent<CustomWeaponTrail>();
                trail.init(oldTrailRendererPrefab, oldColorManager, PointStart, PointEnd, TrailMaterial, TrailColor, Length, MultiplierColor, colorType);
            }
            else
            {
                Logger.Log("Trail not found", LogLevel.Debug);
                Destroy(this);
            }
        }

        public void EnableTrail(bool enable)
        {
            trail.enabled = enable;
        }

        public void SetMaterial(Material newMat)
        {
            TrailMaterial = newMat;
            trail.SetMaterial(newMat);
        }

        public void SetColor(Color newColor)
        {
            TrailColor = newColor;
            trail.SetColor(newColor);
        }
    }
}
