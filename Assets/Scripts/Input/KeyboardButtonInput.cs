using System;
using Iterfaces;
using UnityEngine;
using Utils;

namespace Input
{
    /// <summary>
    /// Handles keyboard button input
    /// </summary>
    public class KeyboardButtonInput : MonoBehaviour, IButtonInput
    {
        public event EventHandler<EventArgs> OnButtonInput;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(mKey)) this.Raise(OnButtonInput, new EventArgs());
        }

        /// <summary>
        /// Initialise the input
        /// </summary>
        /// <param name="key">Keycode of the button</param>
        public void Init(KeyCode key)
        {
            this.mKey = key;
        }
        private KeyCode mKey;
    }
}