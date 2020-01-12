using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDirectionalInput : MonoBehaviour, IDirectionalInput
{
    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var result = new Vector2(horizontal, vertical);
        if (result != Vector2.zero)
        {
            OnDirectinalInput?.Invoke(result);
        }
    }


    public event Defines.DirectionalInputDelegate OnDirectinalInput;
}
