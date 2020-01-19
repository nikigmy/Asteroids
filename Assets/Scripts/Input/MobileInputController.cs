using Iterfaces;
using UI;
using UnityEngine;

namespace Input
{
    /// <summary>
    /// Handles mobile input
    /// </summary>
    public class MobileInputController : MonoBehaviour, IInputController
    {
        public void Init()
        {
            Debug.Assert(mJoypad != null);
            Debug.Assert(mButton != null);
        }

        public IDirectionalInput GetDirectionalInput()
        {
            return mJoypad;
        }

        public IButtonInput GetFireInput()
        {
            return mButton;
        }
        
        [SerializeField] private Button mButton;
        [SerializeField] private Joypad mJoypad;
    }
}