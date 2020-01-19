using Iterfaces;
using UnityEngine;

namespace Input
{
    /// <summary>
    /// Handles desktop input
    /// </summary>
    public class DesktopInputController : MonoBehaviour, IInputController
    {
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
        
        [SerializeField] private KeyboardDirectionalInput directionalInput;
        [SerializeField] private KeyboardButtonInput fireInput;
    }
}