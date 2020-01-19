using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;
using EffectArgs = Utils.ValueArgs<Utils.Declarations.EffectType[]>;

namespace Game
{
    /// <summary>
    /// Holder for effects
    /// </summary>
    public class EffectHandler : MonoBehaviour
    {
        public event EventHandler<EffectArgs> OnEffectsUpdated;

        private void Awake()
        {
            mCurrentEffects = new List<(Declarations.EffectType type, float expirationTime)>();
        }

        private void Update()
        {
            ClearExpiredEffects();
        }
        
        /// <summary>
        /// Add an effect
        /// </summary>
        /// <param name="effect">The effect to add</param>
        public void AddEffect(Declarations.EffectDuration effect)
        {
            mCurrentEffects.Add((effect.effectType, Time.time + effect.duration));

            this.Raise(OnEffectsUpdated, new EffectArgs(GetCurrentEffects()));
        }
            
        /// <summary>
        /// Adds an effect
        /// </summary>
        /// <param name="effectType">Effect type</param>
        /// <param name="duration">Effect duration</param>
        public void AddEffect(Declarations.EffectType effectType, float duration)
        {
            mCurrentEffects.Add((effectType, Time.time + duration));

            this.Raise(OnEffectsUpdated, new EffectArgs(GetCurrentEffects()));
        }

        /// <summary>
        /// Retrieves current effects of the handler
        /// </summary>
        /// <returns>Current effects</returns>
        public Declarations.EffectType[] GetCurrentEffects()
        {
            var result = new Declarations.EffectType[mCurrentEffects.Count];
            
            for (int i = 0; i < mCurrentEffects.Count; i++)
            {
                result[i] = mCurrentEffects[i].type;
            }

            return result;
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
            bool removed = false;
            for (var i = 0; i < mCurrentEffects.Count; i++)
            {
                if (mCurrentEffects[i].expirationTime < Time.time)
                {
                    mCurrentEffects.RemoveAt(i);
                    i--;
                    removed = true;
                }
            }

            if (removed)
            {
                this.Raise(OnEffectsUpdated, new EffectArgs(GetCurrentEffects()));
            }
        }
        
        private List<(Declarations.EffectType type, float expirationTime)> mCurrentEffects;
    }
}