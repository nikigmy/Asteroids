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
        private void Awake()
        {
            Debug.Assert(joypad != null);
            Debug.Assert(fireButton != null);
        }

        public void Init()
        {
            
        }

        public IDirectionalInput GetDirectionalInput()
        {
            return joypad;
        }

        public IButtonInput GetFireInput()
        {
            return fireButton;
        }
        
        [SerializeField] private Button fireButton;
        [SerializeField] private Joypad joypad;
    }
}