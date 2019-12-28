using UnityEngine;

// Class has to be in this namespace due to compatibility
namespace CustomSaber
{
    [RequireComponent(typeof(EventManager))]
    public class EventFilterBehaviour : MonoBehaviour
    {
        private EventManager eventManager;
        protected EventManager EventManager
        {
            get
            {
                if (eventManager == null)
                {
                    eventManager = GetComponent<EventManager>();
                }

                return eventManager;
            }
        }
    }
}
