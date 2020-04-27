using IPA.Utilities;
using UnityEngine;
using Xft;

namespace CustomSaber.Utilities
{
    public enum TrailType
    {
        Custom,
        Vanilla,
        None
    }

    internal class CustomWeaponTrail : XWeaponTrail
    {
        public ColorType _saberType;
        public ColorManager _colorManager;
        public Color _multiplierSaberColor;
        public Color _customColor;
        public Material _customMaterial;

        public override Color color
        {
            get
            {
                Color tempColor = _customColor * _multiplierSaberColor;
                if (_colorManager)
                {
                    if (_saberType.Equals(ColorType.LeftSaber))
                    {
                        tempColor = _colorManager.ColorForSaberType(SaberType.SaberA) * _multiplierSaberColor;
                    }
                    else if (_saberType.Equals(ColorType.RightSaber))
                    {
                        tempColor = _colorManager.ColorForSaberType(SaberType.SaberB) * _multiplierSaberColor;
                    }
                }

                return tempColor;
            }
        }

        public void Init(XWeaponTrailRenderer TrailRendererPrefab, ColorManager colorManager, Transform PointStart, Transform PointEnd, Material TrailMaterial, Color TrailColor, int Length, Color multiplierSaberColor, ColorType colorType)
        {
            _colorManager = colorManager;
            _multiplierSaberColor = multiplierSaberColor;
            _customColor = TrailColor;
            _customMaterial = TrailMaterial;
            _saberType = colorType;

            _pointStart = PointStart;
            _pointEnd = PointEnd;
            _maxFrame = Length;
            _trailRendererPrefab = TrailRendererPrefab;
        }

        public override void Start()
        {
            base.Start();
            ReflectionUtil.GetField<MeshRenderer, XWeaponTrailRenderer>(_trailRenderer, "_meshRenderer").material = _customMaterial;
            if (Settings.Configuration.DisableWhitestep) ReflectionUtil.SetField<XWeaponTrail, int>(this, "_whiteSteps", 0);
        }

        public void SetColor(Color newColor)
        {
            _customColor = newColor;
        }

        public void SetMaterial(Material newMaterial)
        {
            _customMaterial = newMaterial;
            ReflectionUtil.GetField<MeshRenderer, XWeaponTrailRenderer>(_trailRenderer, "_meshRenderer").material = _customMaterial;
        }
    }
}
