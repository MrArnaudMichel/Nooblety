using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Stats
{
    [Serializable]
    public class RuntimeStatusEffects
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        private Traits m_Traits;
        private Dictionary<int, RuntimeStatusEffectList> m_Active;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int Count => this.m_Active.Count;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<IdString> EventChange;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        internal RuntimeStatusEffects(Traits traits)
        {
            this.m_Traits = traits;
            this.m_Active = new Dictionary<int, RuntimeStatusEffectList>();
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void Update()
        {
            if (this.m_Active == null) return;
            foreach (KeyValuePair<int, RuntimeStatusEffectList> entry in this.m_Active)
            {
                entry.Value?.Update();
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void ExecuteEventChange(IdString statusEffectID)
        {
            this.EventChange?.Invoke(statusEffectID);
        }
        
        // SETTERS: -------------------------------------------------------------------------------

        /// <summary>
        /// Adds a new Status Effect onto the Traits target. If the max stack is full, it removes
        /// the oldest Status Effect and creates a new one.
        /// </summary>
        /// <param name="statusEffect"></param>
        /// <param name="timeElapsed"></param>
        public void Add(StatusEffect statusEffect, float timeElapsed = 0f)
        {
            if (statusEffect == null) return;

            if (!this.m_Active.TryGetValue(statusEffect.ID.Hash, out RuntimeStatusEffectList list))
            {
                list = new RuntimeStatusEffectList(this.m_Traits, statusEffect);
                list.EventChange += this.ExecuteEventChange;
                
                this.m_Active.Add(statusEffect.ID.Hash, list);
            }
            
            list.Add(timeElapsed);
        }

        /// <summary>
        /// Removes the oldest active Status Effect of the specified type.
        /// </summary>
        /// <param name="statusEffect"></param>
        /// <param name="amount"></param>
        public void Remove(StatusEffect statusEffect, int amount = 1)
        {
            if (this.m_Active.TryGetValue(statusEffect.ID.Hash, out RuntimeStatusEffectList list))
            {
                list.Remove(amount);
            }
        }

        /// <summary>
        /// Removes all active Status Effects that match the type mask.
        /// </summary>
        /// <param name="maskType"></param>
        public void ClearByType(StatusEffectTypeMask maskType)
        {
            foreach (KeyValuePair<int, RuntimeStatusEffectList> entry in this.m_Active)
            {
                entry.Value?.RemoveByType((int) maskType);
            }
        }
        
        // GETTERS: -------------------------------------------------------------------------------
        
        /// <summary>
        /// Returns a list of all active status effects.
        /// </summary>
        /// <returns></returns>
        public List<IdString> GetActiveList()
        {
            List<IdString> active = new List<IdString>();

            if (this.m_Active == null) return active;
            foreach (KeyValuePair<int, RuntimeStatusEffectList> entry in this.m_Active)
            {
                if (entry.Value.Count == 0) continue;
                active.Add(entry.Value.ID);
            }

            return active;
        }

        /// <summary>
        /// Returns the Status Effect asset of an active status effects given a particular ID
        /// </summary>
        /// <param name="statusEffectID"></param>
        /// <returns></returns>
        public StatusEffect GetActiveStatusEffect(IdString statusEffectID)
        {
            return this.m_Active.TryGetValue(statusEffectID.Hash, out RuntimeStatusEffectList list)
                ? list.StatusEffect
                : null;
        }

        /// <summary>
        /// Returns the amount of active stacked status effects given a particular ID
        /// </summary>
        /// <param name="statusEffectID"></param>
        /// <returns></returns>
        public int GetActiveStackCount(IdString statusEffectID)
        {
            return this.m_Active.TryGetValue(statusEffectID.Hash, out RuntimeStatusEffectList list) 
                ? list.Count 
                : 0;
        }

        /// <summary>
        /// Returns the active stacked status effect at the specified index. 
        /// </summary>
        /// <param name="statusEffectID"></param>
        /// <param name="stackIndex"></param>
        /// <returns></returns>
        public RuntimeStatusEffectValue GetActiveAt(IdString statusEffectID, int stackIndex)
        {
            return this.m_Active.TryGetValue(statusEffectID.Hash, out RuntimeStatusEffectList list)
                ? list.GetValueAt(stackIndex) 
                : default;
        }
    }
}