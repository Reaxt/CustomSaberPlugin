using UnityEngine.Events;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    [UnityEngine.AddComponentMenu("Custom Sabers/Every Nth Combo")]
    public class EveryNthComboFilter : EventFilterBehaviour
    {
        public int ComboStep = 50;
        public UnityEvent NthComboReached;

#if PLUGIN
        private void OnEnable() => EventManager.OnComboChanged.AddListener(OnComboStep);
        private void OnDisable() => EventManager.OnComboChanged.RemoveListener(OnComboStep);

        private void OnComboStep(int combo)
        {
            if (combo % ComboStep == 0 && combo != 0)
            {
                NthComboReached.Invoke();
            }
        }
#endif
    }
}
