using Iterfaces;
using UnityEngine;
using Utils;

namespace Managers
{
    /// <summary>
    /// Handles input
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        /// <summary>
        /// Initialise the manager
        /// </summary>
        /// <param name="controlScheme">Control scheme to use</param>
        public void Init(Declarations.ControlScheme controlScheme)
        {
            for (var i = 0; i < mInputControllerMap.Length; i++)
                if (mInputControllerMap[i].ControlScheme == controlScheme)
                {
                    mInputControllerObj = Instantiate(mInputControllerMap[i].InputControllerPrefab, transform);
                    mInputController = mInputControllerObj.GetComponent<IInputController>();
                    if (mInputController != null)
                        mInputController.Init();
                    else
                        Debug.LogError("Invalid prefab for control scheme: " + controlScheme);

                    return;
                }

            if (mInputController == null) Debug.LogError("Input controller not found for control scheme: " + controlScheme);
        }

        /// <summary>
        /// Get the current input controller
        /// </summary>
        /// <returns>Current input controller</returns>
        public IInputController GetInputController()
        {
            return mInputController;
        }

        /// <summary>
        /// Enable input visuals
        /// </summary>
        public void EnableVisuals()
        {
            mInputControllerObj.SetActive(true);
        }
        
        /// <summary>
        /// Disable input visuals
        /// </summary>
        public void DisableVisuals()
        {
            mInputControllerObj.SetActive(false);
        }
        
        private IInputController mInputController;
        private GameObject mInputControllerObj;

        [SerializeField] private Declarations.ControlSchemeInputPair[] mInputControllerMap;
    }
}