using UnityEngine.Events;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    [UnityEngine.AddComponentMenu("Custom Sabers/Accuracy Reached Event")]
    public class AccuracyReachedEvent : EventFilterBehaviour
    {
        [UnityEngine.Tooltip("Event will be triggered when accuracy crosses this value, expressed as a value between 0 and 1")]
        [UnityEngine.RangeAttribute(0f, 1f)]
        public float Target = 1.0f;
        public UnityEvent OnAccuracyReachTarget;
        public UnityEvent OnAccuracyHigherThanTarget;
        public UnityEvent OnAccuracyLowerThanTarget;

#if PLUGIN
        private void OnEnable()
        {
            EventManager.OnAccuracyChanged.AddListener(OnAccuracyReached);
            prevAccuracy = 1.0f;
        }

        private void OnDisable() => EventManager.OnAccuracyChanged.RemoveListener(OnAccuracyReached);

        private float prevAccuracy;

        private void OnAccuracyReached(float accuracy)
        {
            if (prevAccuracy > Target && accuracy < Target || prevAccuracy < Target && accuracy > Target)
            {
                OnAccuracyReachTarget.Invoke();
            }
            if (prevAccuracy < Target && accuracy > Target)
            {
                OnAccuracyHigherThanTarget.Invoke();
            }
            if (prevAccuracy > Target && accuracy < Target)
            {
                OnAccuracyLowerThanTarget.Invoke();
            }

            prevAccuracy = accuracy;
        }
#endif
    }
}
