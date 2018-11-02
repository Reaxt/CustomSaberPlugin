using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomSaber;
using UnityEngine.Events;

public class EventTester : MonoBehaviour {

    List<EventManager> managers;
    List<ComboReachedEvent> comboNbEvents;
    List<EveryNthComboFilter> comboNthEvents;

    public int combo = 0;

	// Use this for initialization
	void Start () {
        managers = new List<EventManager>(FindObjectsOfType<EventManager>());
        comboNbEvents = new List<ComboReachedEvent>(FindObjectsOfType<ComboReachedEvent>());
        comboNthEvents = new List<EveryNthComboFilter>(FindObjectsOfType<EveryNthComboFilter>());
    }

    public void OnSlice()
    {
        foreach(EventManager manager in managers)
        {
            manager.OnSlice.Invoke();
        }
    }

    public void ComboBreak()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnComboBreak.Invoke();
        }
    }

    public void MultiplierUp()
    {
        foreach (EventManager manager in managers)
        {
            manager.MultiplierUp.Invoke();
        }
    }

    public void StartColliding()
    {
        foreach (EventManager manager in managers)
        {
            manager.SaberStartColliding.Invoke();
        }
    }

    public void StopColliding()
    {
        foreach (EventManager manager in managers)
        {
            manager.SaberStopColliding.Invoke();
        }
    }

    public void LevelStart()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnLevelStart.Invoke();
        }
    }

    public void LevelFail()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnLevelFail.Invoke();
        }
    }

    public void LevelEnded()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnLevelEnded.Invoke();
        }
    }

    public void BlueLightOn()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnBlueLightOn.Invoke();
        }
    }

    public void RedLightOn()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnRedLightOn.Invoke();
        }
    }

    public void TestCombo()
    {
        foreach (EventManager manager in managers)
        {
            manager.OnComboChanged.Invoke(combo);
        }

        foreach (ComboReachedEvent ev in comboNbEvents)
        {
            if (ev.ComboTarget == combo)
                ev.NthComboReached.Invoke();
        }

        foreach (EveryNthComboFilter ev in comboNthEvents)
        {
            if (ev.ComboStep == combo)
                ev.NthComboReached.Invoke();
        }
    }

    public void SetCombo(string input)
    {
        combo = int.Parse(input);
    }
}
