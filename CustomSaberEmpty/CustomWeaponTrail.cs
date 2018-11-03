using System;
using UnityEngine;


namespace CustomSaber
{
    class CustomWeaponTrail
    {
        public ColorType _saberType;
        public Color _multiplierSaberColor;
        public Color _customColor;
        public Material _customMaterial;

        public void init()
        {
        }

        public void Start()
        {
        }

        public void SetColor(Color newColor)
        {
            _customColor = newColor;
        }

        public void SetMaterial(Material newMaterial)
        {
            _customMaterial = newMaterial;
        }
    }
}
