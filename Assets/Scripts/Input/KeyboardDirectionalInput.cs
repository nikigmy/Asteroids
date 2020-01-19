using System;
using Iterfaces;
using UnityEngine;
using Utils;
using DirectionalArgs = Utils.ValueArgs<UnityEngine.Vector2>;

namespace Input
{
    /// <summary>
    /// Handles keyboard directional input
    /// </summary>
    public class KeyboardDirectionalInput : MonoBehaviour, IDirectionalInput
    {
        public event EventHandler<DirectionalArgs> OnDirectinalInput;

        private void Update()
        {
            var horizontal = UnityEngine.Input.GetAxis("Horizontal");
            var vertical = UnityEngine.Input.GetAxis("Vertical");
            
            var result = new Vector2(horizontal, vertical);
            if (result != Vector2.zero)
            {
                this.Raise(OnDirectinalInput, new DirectionalArgs(result));
            }
        }
    }
}