using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputController
{
    void Init();
    IDirectionalInput GetDirectionalInput();
    IButtonInput GetFireInput();
}

public interface IButtonInput
{
    event  Defines.ButtonInputDelegate OnButtonInput;
}
