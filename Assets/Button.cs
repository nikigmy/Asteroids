using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : UnityEngine.UI.Button, IButtonInput
{
    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnButtonInput?.Invoke();
    }
    
    public event Defines.ButtonInputDelegate OnButtonInput;
}
