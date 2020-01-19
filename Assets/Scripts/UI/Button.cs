using System;
using Iterfaces;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    public class Button : UnityEngine.UI.Button, IButtonInput
    {
        private bool pressed;

        public event EventHandler<EventArgs> OnButtonInput;

        protected override void OnDisable()
        {
            base.OnDisable();
            pressed = false;
        }

        private void Update()
        {
            if (pressed) this.Raise(OnButtonInput, EventArgs.Empty);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            base.OnPointerDown(eventData);
            this.Raise(OnButtonInput, EventArgs.Empty);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            base.OnPointerUp(eventData);
        }
    }
}