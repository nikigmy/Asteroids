using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public abstract class FSMDescriptor
    {
        public const int cUndefinedState = -1;
        public const int cUndefinedAction = -1;

        private static readonly Dictionary<Type, FSMDescriptor> mInstances = new Dictionary<Type, FSMDescriptor>();

        private readonly Dictionary<int, string> mStates = new Dictionary<int, string>();
        private readonly List<TransitionInfo> mTransitions = new List<TransitionInfo>();

        /// <summary>
        /// Contains the data that defines a transition
        /// </summary>
        public class TransitionInfo
        {
            public int mActionID = cUndefinedAction;
            public int mFromState = cUndefinedState;
            public int mToState = cUndefinedState;
            
            /// <param name="actionID">The ID of the action</param>
            /// <param name="fromStateID">The ID of the 'from' state</param>
            /// <param name="toStateID">The ID of the 'to' state</param>
            public TransitionInfo(int actionID, int fromStateID, int toStateID)
            {
                mActionID = actionID;
                Debug.Assert(mActionID != cUndefinedAction,
                    string.Format("Undefined action ID for action {0}", mActionID));

                mFromState = fromStateID;
                mToState = toStateID;
            }
        }

        #region Public

        /// <summary>
        /// Gets a singleton of the specified descriptor
        /// </summary>
        /// <typeparam name="T">Type of descriptor</typeparam>
        /// <returns>The reference to the singleton</returns>
        public static T GetInstance<T>() where T : FSMDescriptor, new()
        {
            var type = typeof(T);
            if (mInstances.ContainsKey(type) == false) mInstances.Add(type, new T());
            return (T) mInstances[type];
        }

        /// <summary>
        /// Adds a new transition to the FSM description
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="fromState">The 'from' state</param>
        /// <param name="toState">The 'to' state</param>
        /// <typeparam name="ActionType">Action enum</typeparam>
        /// <typeparam name="StateType">State enum</typeparam>
        public void AddTransitionInfo<ActionType, StateType>(ActionType action, StateType fromState,
            StateType toState) where StateType : struct, IConvertible
            where ActionType : struct, IConvertible
        {
            var fromStateID = Convert.ToInt32(fromState);
            AddState(fromStateID, fromState.ToString());

            var toStateID = Convert.ToInt32(toState);
            AddState(toStateID, toState.ToString());

            AddTransitionInfo(Convert.ToInt32(action), fromStateID, toStateID);
        }

        /// <summary>
        /// Gets the number of states;
        /// </summary>
        /// <returns>The number of states</returns>
        public int GetStatesCount()
        {
            return mStates.Count;
        }
        
        /// <summary>
        /// Gets a reference to the transition
        /// </summary>
        /// <param name="actionID">The ID of the action</param>
        /// <param name="fromStateID">The ID of the 'from' state</param>
        /// <returns>The reference to the transition if existing, null otherwise</returns>
        public TransitionInfo GetTransition(int actionID, int fromStateID)
        {
            Debug.Assert(mStates.ContainsKey(fromStateID),
                string.Format("The FSM description doesn't contain a state '{0}'", fromStateID));

            return mTransitions.Find(item => item.mActionID == actionID && item.mFromState == fromStateID);
        }
        
        /// <summary>
        /// Gets whether the state exists or not
        /// </summary>
        /// <param name="stateID">The ID of the state</param>
        /// <returns>Whether the state exists or not</returns>
        public bool HasState(int stateID)
        {
            return mStates.ContainsKey(stateID);
        }

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        /// <param name="stateID">The ID of the state</param>
        /// <returns>The name of the state</returns>
        public string GetStateName(int stateID)
        {
            return mStates.GetValueOrDefault(stateID, string.Empty);
        }

        #endregion

        #region Private

        /// <summary>
        /// Adds a new state to the FSM description
        /// </summary>
        /// <param name="stateID">The ID of the state</param>
        /// <param name="stateName">The name of the state</param>
        private void AddState(int stateID, string stateName)
        {
            if (mStates.ContainsKey(stateID) == false) mStates.Add(stateID, stateName);
        }

        /// <summary>
        /// Adds a new transition to the FSM description
        /// </summary>
        /// <param name="actionID">The ID of the action</param>
        /// <param name="fromStateID">The ID of the state where to apply the action</param>
        /// <param name="toStateID">The ID of the destination state</param>
        private void AddTransitionInfo(int actionID, int fromStateID, int toStateID)
        {
            Debug.Assert(mStates.ContainsKey(fromStateID),
                string.Format("The FSM description doesn't contain a state with id {0}", fromStateID));
            Debug.Assert(mStates.ContainsKey(toStateID),
                string.Format("The FSM description doesn't contain a state with id {0}", toStateID));

            var transition = GetTransition(actionID, fromStateID);
            Debug.Assert(transition == null,
                string.Format("The FSM already contains a transition from state {0} with action {1}", fromStateID,
                    actionID));

            if (transition == null) mTransitions.Add(new TransitionInfo(actionID, fromStateID, toStateID));
        }

        #endregion
    }
}