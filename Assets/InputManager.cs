using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour
{
    [Serializable]
    public class ControlSchemeInputPair
    {
        public Defines.ControlScheme ControlScheme;
        public GameObject InputControllerPrefab;
    }

    private IInputController inputController;
    [SerializeField] private ControlSchemeInputPair[] inputControllerMap;
    private Defines.ControlScheme controlScheme;
    public void Init(Defines.ControlScheme controlScheme)
    {
        this.controlScheme = controlScheme;
        
        for (int i = 0; i < inputControllerMap.Length; i++)
        {
            if (inputControllerMap[i].ControlScheme == this.controlScheme)
            {
                var obj = Instantiate(inputControllerMap[i].InputControllerPrefab, transform);
                inputController = obj.GetComponent<IInputController>();
                if (inputController != null)
                {
                    inputController.Init();
                }
                else
                {
                    Debug.LogError("Invalid prefab for control scheme: " + controlScheme);
                }

                return;
            }
        }
        
        if (inputController == null)
        {
            Debug.LogError("Input controller not found for control scheme: " + controlScheme);
        }
    }

    public IInputController GetInputController()
    {
        return inputController;
    }
}
