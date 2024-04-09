using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Serializable]
    public class Actant
    {
        [SerializeField] public PropertyGetString m_Name = new PropertyGetString();
        [SerializeField] private PropertyGetString m_Description = new PropertyGetString();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public string GetName(Args args) => this.m_Name.Get(args);
        
        
        public string GetDescription(Args args) => this.m_Description.Get(args);
    }
}