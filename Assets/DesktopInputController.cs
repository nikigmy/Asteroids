using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInputController : MonoBehaviour ,IInputController
{
    [SerializeField] private KeyboardDirectionalInput directionalInput;
    [SerializeField] private KeyboardButtonInput fireInput;

    public void Init()
    {
        fireInput.Init(KeyCode.Space);
        
        Debug.Assert(directionalInput != null);
        Debug.Assert(fireInput != null);
    }
    
    public IDirectionalInput GetDirectionalInput()
    {
        return directionalInput;
    }

    public IButtonInput GetFireInput()
    {
        return fireInput;
    }
}
