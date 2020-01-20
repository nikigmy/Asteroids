using System;
using UnityEngine;
using Utils;

namespace UI
{
    /// <summary>
    /// Base behaviour of dialogs
    /// </summary>
    public abstract class Dialog : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnDialogClosed;
        
        /// <summary>
        /// Open the dialog
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Close the dialog
        /// </summary>
        public virtual void Close()
        {
            gameObject.SetActive(false);
            this.Raise(OnDialogClosed, EventArgs.Empty);
        }
    }
}