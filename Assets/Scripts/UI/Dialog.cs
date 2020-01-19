using System;
using UnityEngine;
using Utils;

namespace UI
{
    public abstract class Dialog : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnDialogClosed;
        public abstract void Open();

        public virtual void Close()
        {
            gameObject.SetActive(false);
            this.Raise(OnDialogClosed, EventArgs.Empty);
        }
    }
}