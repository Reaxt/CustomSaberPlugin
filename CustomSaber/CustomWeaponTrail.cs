using System;
using UnityEngine;
using Xft;


namespace CustomSaber
{
    class CustomWeaponTrail : XWeaponTrail
    {
        public ColorType _saberType;
        public ColorManager _colorManager;
        public Color _multiplierSaberColor;
        public Color _customColor;
        public Material _customMaterial;

        protected override Color color
        {
            get
            {
                if (_saberType.Equals(ColorType.LeftSaber) && _colorManager!=null)
                {
                    return _colorManager.ColorForSaberType(Saber.SaberType.SaberA) * _multiplierSaberColor;
                }
                else if (_saberType.Equals(ColorType.RightSaber) && _colorManager != null)
                {
                    return _colorManager.ColorForSaberType(Saber.SaberType.SaberB) * _multiplierSaberColor;
                }
                else
                {
                    return _customColor * _multiplierSaberColor;
                }
            }
        }

        public void init(XWeaponTrailRenderer TrailRendererPrefab, ColorManager colorManager, Transform PointStart, Transform PointEnd,Material TrailMaterial, Color TrailColor, int Length, Color multiplierSaberColor, ColorType colorType)
        {
            _pointStart = PointStart;
            _pointEnd = PointEnd;
            _maxFrame = Length;
            _colorManager = colorManager;
            _trailRendererPrefab = TrailRendererPrefab;
            _multiplierSaberColor = multiplierSaberColor;
            _customColor = TrailColor;
            _customMaterial = TrailMaterial;
            _saberType = colorType;
        }

        public override void Start()
        {
            base.Start();

            ReflectionUtil.GetPrivateField<MeshRenderer>(_trailRenderer, "_meshRenderer").material = _customMaterial;
        }

        public void SetColor(Color newColor)
        {
            _customColor = newColor;
        }

        public void SetMaterial(Material newMaterial)
        {
            _customMaterial = newMaterial;
            ReflectionUtil.GetPrivateField<MeshRenderer>(_trailRenderer, "_meshRenderer").material = _customMaterial;
        }
    }
}
