using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class FSM<T> where T : FSMDescriptor, new()
    {
        private const int cInvalidStateIndex = int.MaxValue;

        private readonly Dictionary<int, StateBinding> mStateBindings = new Dictionary<int, StateBinding>();

        /// <summary>
        /// Stores references to initialise and update delegates for a state
        /// </summary>
        private class StateBinding
        {
            public Action mExitStateFunc;
            public Action mInitStateFunc;
            public int mStateID = FSMDescriptor.cUndefinedState;

            /// <param name="stateID">The ID of the state</param>
            /// <param name="initStateFunc">The initialise delegate of the state</param>
            /// <param name="exitStateFunc">The exit delegate of the state</param>
            public StateBinding(int stateID, Action initStateFunc, Action exitStateFunc)
            {
                mStateID = stateID;
                mInitStateFunc = initStateFunc;
                mExitStateFunc = exitStateFunc;
            }
        }

        #region Public

        public FSM()
        {
            mFsmDescriptor = FSMDescriptor.GetInstance<T>();
        }

        /// <summary>
        /// Adds a new state to the FSM
        /// </summary>
        /// <param name="state">State to add</param>
        /// <param name="initStateFunc">The initialise delegate of the state</param>
        /// <param name="exitStateFunc">The exit delegate of the state</param>
        /// <typeparam name="StateType"></typeparam>
        public void AddStateBinding<StateType>(StateType state, Action initStateFunc,
            Action exitStateFunc) where StateType : struct, IConvertible
        {
            AddStateBinding(Convert.ToInt32(state), initStateFunc, exitStateFunc);
        }

       /// <summary>
       /// Asks to the FSM to execute an action
       /// </summary>
       /// <param name="action">Action to execute</param>
       /// <typeparam name="ActionType">Action enum</typeparam>
        public void ExecuteAction<ActionType>(ActionType action) where ActionType : struct, IConvertible
        {
            ExecuteAction(Convert.ToInt32(action), action.ToString());
        }

        /// <summary>
        /// Starts the FSM
        /// </summary>
        /// <param name="state">The state to start in</param>
        /// <typeparam name="StateType">State enum</typeparam>
        public void Start<StateType>(StateType state) where StateType : struct, IConvertible
        {
            Start(Convert.ToInt32(state));
        }

        /// <summary>
        /// Starts the FSM
        /// </summary>
        /// <param name="startingStateID">The state to start in</param>
        public void Start(int startingStateID = 0)
        {
            Debug.Assert(mFsmDescriptor.GetStatesCount() == mStateBindings.Count,
                "The FSM is not completely initialised, some bindings are missing");
            Debug.Assert(HasStarted() == false,
                "The FSM has been already initialised, you cannnot call the Start method more than once!");
            Debug.Assert(startingStateID != FSMDescriptor.cUndefinedState, "Undefined starting state for this FSM");

            ChangeState(startingStateID);
        }

        /// <summary>
        /// Stops the FSM
        /// </summary>
        public void Stop()
        {
            mCurrentState = null;
        }
        
        /// <summary>
        /// Gets whether the FSM was started
        /// </summary>
        /// <returns>Whether the FSM was started</returns>
        public bool HasStarted()
        {
            return mCurrentState != null;
        }

        /// <summary>
        /// Gets the currents state of the FSM
        /// </summary>
        /// <returns>The ID of the current state</returns>
        public int GetCurrentState()
        {
            return HasStarted() ? mCurrentState.mStateID : FSMDescriptor.cUndefinedState;
        }

        /// <summary>
        /// Gets the name of the current state of the FSM
        /// </summary>
        /// <returns>Name of the current state</returns>
        public string GetCurrentStateName()
        {
            return mFsmDescriptor.GetStateName(GetCurrentState());
        }

        /// <summary>
        /// Checks whether the FSM is in the given state
        /// </summary>
        /// <param name="state">State to check</param>
        /// <typeparam name="StateType">State enum</typeparam>
        /// <returns>Whether the FSM is in the given state</returns>
        public bool IsInState<StateType>(StateType state) where StateType : struct, IConvertible
        {
            return IsInState(Convert.ToInt32(state));
        }

        /// <summary>
        /// Checks whether the FSMs previous state was the target state
        /// </summary>
        /// <param name="state">State to check</param>
        /// <typeparam name="StateType">State enum</typeparam>
        /// <returns>Whether the FSMs previous state was the target state</returns>
        public bool WasInState<StateType>(StateType state) where StateType : struct, IConvertible
        {
            return WasInState(Convert.ToInt32(state));
        }

        #endregion
        
        #region Protected

        /// <summary>
        /// Transitions to the next state
        /// </summary>
        /// <param name="nextStateID">Next state id</param>
        private void ChangeState(int nextStateID)
        {
            var nextState = mStateBindings.TryGetValue(nextStateID);
            Debug.Assert(nextState != null, string.Format("Cannot find the desired state with id {0}", nextStateID));

            if (mCurrentState != null)
            {
                if (mCurrentState.mExitStateFunc != null) mCurrentState.mExitStateFunc();

                mPreviousStateID = mCurrentState.mStateID;
            }

            mCurrentState = nextState;
            if (mCurrentState.mInitStateFunc != null) mCurrentState.mInitStateFunc();
        }

        #endregion

        #region Private
        
        /// <summary>
        /// Adds a new state to the FSM
        /// </summary>
        /// <param name="stateID">The ID of the state</param>
        /// <param name="initStateFunc">The initialise delegate of the state</param>
        /// <param name="exitStateFunc">The exit delegate of the state</param>
        private void AddStateBinding(int stateID, Action initStateFunc,
            Action exitStateFunc)
        {
            if (mStateBindings.ContainsKey(stateID) == false)
                mStateBindings.Add(stateID, new StateBinding(stateID, initStateFunc, exitStateFunc));
        }

        /// <summary>
        /// Asks to the FSM to execute an action
        /// </summary>
        /// <param name="actionID">The ID of the action to execute</param>
        /// <param name="actionName">Action enum</param>
        private void ExecuteAction(int actionID, string actionName)
        {
            Debug.Assert(HasStarted(), "The FSM is not initialised");

            if (HasStarted())
            {
                var transition = mFsmDescriptor.GetTransition(actionID, mCurrentState.mStateID);
                Debug.Assert(transition != null,
                    string.Format("There are no transitions from state {0} for the action '{1}'",
                        mFsmDescriptor.GetStateName(mCurrentState.mStateID), actionName));

                if (transition != null) ChangeState(transition.mToState);
            }
        }

        /// <summary>
        /// Checks whether the FSM is in the given state
        /// </summary>
        /// <param name="stateID">The ID of the state</param>
        /// <returns>Whether the FSM is in the given state</returns>
        private bool IsInState(int stateID)
        {
            return mCurrentState != null && mCurrentState.mStateID == stateID;
        }
        
        /// <summary>
        /// Checks whether the FSMs previous state was the target state
        /// </summary>
        /// <param name="stateID">State to check</param>
        /// <returns>Whether the FSMs previous state was the target state</returns>
        private bool WasInState(int stateID)
        {
            return mPreviousStateID == stateID;
        }

        #endregion

        private StateBinding mCurrentState;

        private FSMDescriptor mFsmDescriptor;
        
        private int mPreviousStateID = cInvalidStateIndex;
    }
}