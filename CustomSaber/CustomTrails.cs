using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Xft;

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
        public Color TrailColor = new Color(1.0f,1.0f,1.0f,1.0f);
        public Color MultiplierColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public int Length = 20;

        private CustomWeaponTrail trail;
        private ColorManager oldColorManager;
        private XWeaponTrailRenderer oldTrailRendererPrefab;
        private Saber saber;

        public void Init(Saber parentSaber)
        {
            Logger.log.Info("Replacing Trail");

            saber = parentSaber;

            if (gameObject.name != "LeftSaber" && gameObject.name != "RightSaber")
            {
                Logger.log.Warn("Parent not LeftSaber or RightSaber");
                Destroy(this);
            }

            if (saber == null)
            {
                Logger.log.Warn("Saber not found");
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
                    Logger.log.Info(ReflectionUtil.GetPrivateField<Color>(oldtrail, "_multiplierSaberColor").ToString());
                    //ReflectionUtil.SetPrivateField(oldtrail, "_multiplierSaberColor", new Color(0f, 0f, 0f, 0f));
                    oldColorManager = ReflectionUtil.GetPrivateField<ColorManager>(oldtrail, "_colorManager");
                    oldTrailRendererPrefab = ReflectionUtil.GetPrivateField<XWeaponTrailRenderer>(oldtrail, "_trailRendererPrefab");

                    //  oldtrail.Start();
                    //    oldtrail.gameObject.SetActive(false);
                }
                catch (Exception ex)
                {
                    Logger.log.Critical($"CustomTrail.Init() threw an exception: {ex.Message}\n{ex.StackTrace}");
                    throw;
                }

                trail = gameObject.AddComponent<CustomWeaponTrail>();
                trail.init(oldTrailRendererPrefab, oldColorManager, PointStart, PointEnd, TrailMaterial, TrailColor, Length, MultiplierColor, colorType);
            }
            else
            {
                Logger.log.Info("Trail not found");
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
