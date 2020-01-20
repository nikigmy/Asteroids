using System;
using Iterfaces;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    /// <summary>
    /// Extends the standard unity button with additional event
    /// </summary>
    public class Button : UnityEngine.UI.Button, IButtonInput
    {
        public event EventHandler<EventArgs> OnButtonInput;

        protected override void OnDisable()
        {
            base.OnDisable();
            mPressed = false;
        }

        private void Update()
        {
            if (mPressed) this.Raise(OnButtonInput, EventArgs.Empty);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            mPressed = true;
            base.OnPointerDown(eventData);
            this.Raise(OnButtonInput, EventArgs.Empty);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            mPressed = false;
            base.OnPointerUp(eventData);
        }

        private bool mPressed;
    }
}