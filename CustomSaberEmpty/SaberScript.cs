using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;

namespace CustomSaber
{
    class SaberScript : MonoBehaviour
    {
        private enum AudioEvent
        {
            Play,
            Stop
        };

        public static AssetBundle CustomSaber;

        public static SaberScript Instance;

        public static void LoadAssets()
        {
        }

        private void Start()
        {
        }

        void Awake()
        {
        }

        private void Update()
        {
        }


        private void SliceCallBack()
        {
        }

        private void NoteMissCallBack()
        {
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
        }

        private void SaberStartCollide()
        {
        }

        private void SaberEndCollide()
        {
        }

        private void FailLevelCallBack()
        {
        }

        private void LightEventCallBack()
        {
        }

        private void ComboChangeEvent(int combo)
        {
        }
    }
}