﻿using System;
using UnityEngine;
using UnityEngine.Events;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    [AddComponentMenu("Custom Sabers/Event Manager")]
    public class EventManager : MonoBehaviour
    {
        public UnityEvent OnSlice;
        public UnityEvent OnComboBreak;
        public UnityEvent MultiplierUp;
        public UnityEvent SaberStartColliding;
        public UnityEvent SaberStopColliding;
        public UnityEvent OnLevelStart;
        public UnityEvent OnLevelFail;
        public UnityEvent OnLevelEnded;
        public UnityEvent OnBlueLightOn;
        public UnityEvent OnRedLightOn;

        [HideInInspector]
        public ComboChangedEvent OnComboChanged = new ComboChangedEvent();
        [HideInInspector]
        public AccuracyChangedEvent OnAccuracyChanged = new AccuracyChangedEvent();

        [Serializable]
        public class ComboChangedEvent : UnityEvent<int> { }

        [Serializable]
        public class AccuracyChangedEvent : UnityEvent<float> { }
    }
}
