namespace Iterfaces
{
    /// <summary>
    /// Base input controller
    /// </summary>
    public interface IInputController
    {
        /// <summary>
        /// Initialise the controller
        /// </summary>
        void Init();
        
        /// <summary>
        /// Gets the directional input
        /// </summary>
        /// <returns>Directional input of the controller</returns>
        IDirectionalInput GetDirectionalInput();
        
        
        /// <summary>
        /// Gets the fire input
        /// </summary>
        /// <returns>Fire input of the controller</returns>
        IButtonInput GetFireInput();
    }
}