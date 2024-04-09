using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Title("Node Type")]
    
    [Serializable]
    public abstract class TNodeType : TPolymorphicItem<TNodeType>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected NodeTypeData m_Options = NodeTypeData.FromSkin;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action<int> EventStartChoice;
        public event Action<int> EventFinishChoice;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public abstract bool IsBranch { get; }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public abstract Task Run(int id, Story story, Args args);
        public abstract List<int> GetNext(int id, Story story, Args args);
        
        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void InvokeEventStartChoice(int id) => this.EventStartChoice?.Invoke(id);
        protected void InvokeEventFinishChoice(int id) => this.EventFinishChoice?.Invoke(id);
    }
}