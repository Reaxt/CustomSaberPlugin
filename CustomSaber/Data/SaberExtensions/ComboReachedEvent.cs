using UnityEngine.Events;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    [UnityEngine.AddComponentMenu("Custom Sabers/Combo Reached Event")]
    public class ComboReachedEvent : EventFilterBehaviour
    {
        public int ComboTarget = 50;
        public UnityEvent NthComboReached;

#if PLUGIN
        private void OnEnable() => EventManager.OnComboChanged.AddListener(OnComboReached);
        private void OnDisable() => EventManager.OnComboChanged.RemoveListener(OnComboReached);

        private void OnComboReached(int combo)
        {
            if (combo == ComboTarget)
            {
                NthComboReached.Invoke();
            }
        }
#endif
    }
}
