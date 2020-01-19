using System;

namespace Iterfaces
{
    /// <summary>
    /// Interface for button input
    /// </summary>
    public interface IButtonInput
    {
        event EventHandler<EventArgs> OnButtonInput;
    }
}