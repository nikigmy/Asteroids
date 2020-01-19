using System;
using DirectionalArgs = Utils.ValueArgs<UnityEngine.Vector2>;

namespace Iterfaces
{
    /// <summary>
    /// Interface for directional input
    /// </summary>
    public interface IDirectionalInput
    {
        event EventHandler<DirectionalArgs> OnDirectinalInput;
    }
}