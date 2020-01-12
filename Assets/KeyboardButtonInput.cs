using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardButtonInput : MonoBehaviour, IButtonInput
{
    private KeyCode key;
    public void Init(KeyCode key)
    {
        this.key = key;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            OnButtonInput?.Invoke();
        }
    }

    public event Defines.ButtonInputDelegate OnButtonInput;
}
