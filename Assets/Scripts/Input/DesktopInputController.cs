using Iterfaces;
using UnityEngine;

namespace Input
{
    /// <summary>
    /// Handles desktop input
    /// </summary>
    [RequireComponent(typeof(KeyboardDirectionalInput), typeof(KeyboardButtonInput))]
    public class DesktopInputController : MonoBehaviour, IInputController
    {
        private void Awake()
        {
            mDirectionalInput = GetComponent<KeyboardDirectionalInput>();
            mFireInput = GetComponent<KeyboardButtonInput>();
        }

        public void Init()
        {
            mFireInput.Init(KeyCode.Space);
        }

        public IDirectionalInput GetDirectionalInput()
        {
            return mDirectionalInput;
        }

        public IButtonInput GetFireInput()
        {
            return mFireInput;
        }
        
        private KeyboardDirectionalInput mDirectionalInput;
        
        private KeyboardButtonInput mFireInput;
    }
}