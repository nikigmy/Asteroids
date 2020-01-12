using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputController : MonoBehaviour, IInputController
{
    [SerializeField] private Joypad joypad;
    [SerializeField] private Button button;

    public void Init()
    {
        Debug.Assert(joypad != null);
        Debug.Assert(button != null);
    }

    public IDirectionalInput GetDirectionalInput()
    {
        return joypad;
    }

    public IButtonInput GetFireInput()
    {
        return button;
    }
}
