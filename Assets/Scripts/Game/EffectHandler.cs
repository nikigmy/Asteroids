using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using EffectArgs = Utils.ValueArgs<Utils.Declarations.EffectType>;

namespace Game
{
    /// <summary>
    /// Holder for effects
    /// </summary>
    public class EffectHandler : MonoBehaviour
    {
        public event EventHandler<EffectArgs> OnEffectAdded;
        public event EventHandler<EffectArgs> OnEffectExpired;

        private void Awake()
        {
            mCurrentEffects = new Dictionary<Declarations.EffectType, float>();
        }

        private void Update()
        {
            ClearExpiredEffects();
        }

        public bool IsEffectActive(Declarations.EffectType effect)
        {
            return mCurrentEffects.ContainsKey(effect);
        }
        
        /// <summary>
        /// Add an effect
        /// </summary>
        /// <param name="effect">The effect to add</param>
        public void AddEffect(Declarations.EffectDuration effect)
        {
            AddEffect(effect.effectType, effect.duration);
        }
            
        /// <summary>
        /// Adds an effect
        /// </summary>
        /// <param name="effectType">Effect type</param>
        /// <param name="duration">Effect duration</param>
        public void AddEffect(Declarations.EffectType effectType, float duration)
        {
            var expirationTime = Time.time + duration;
            if (mCurrentEffects.ContainsKey(effectType))
            {
                var currentEffectExpiration = mCurrentEffects[effectType];
                if (currentEffectExpiration < expirationTime)
                {
                    mCurrentEffects[effectType] = expirationTime;
                }
            }
            else
            {
                mCurrentEffects.Add(effectType, expirationTime);
            }

            this.Raise(OnEffectAdded, new EffectArgs(effectType));
        }

        /// <summary>
        /// Retrieves current effects of the handler
        /// </summary>
        /// <returns>Current effects</returns>
        public Declarations.EffectType[] GetCurrentEffects()
        {
            return mCurrentEffects.Keys.ToArray();
        }

        /// <summary>
        /// Clears all active effects
        /// </summary>
        public void ClearEffects()
        {
            mCurrentEffects.Clear();
        }
        
        private void ClearExpiredEffects()
        {
            List<Declarations.EffectType> effectsToRemove = new List<Declarations.EffectType>();
            foreach (var effect in mCurrentEffects)
            {
                if (effect.Value < Time.time)
                {
                    effectsToRemove.Add(effect.Key);
                }
            }

            for (int i = 0; i < effectsToRemove.Count; i++)
            {
                mCurrentEffects.Remove(effectsToRemove[i]);
                this.Raise(OnEffectExpired, new EffectArgs(effectsToRemove[i]));
            }
        }
        
        private Dictionary<Declarations.EffectType, float> mCurrentEffects;
    }
}