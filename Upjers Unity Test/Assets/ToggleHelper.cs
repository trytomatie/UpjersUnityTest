using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleHelper : MonoBehaviour
{
    public UnityEvent onToggleIsOn;
    private Toggle _toggle;

    void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(CheckIfToggleIsOn);
    }

    private void CheckIfToggleIsOn(bool value)
    {
        if (value)
        {
            onToggleIsOn?.Invoke();
        }
    }
}
