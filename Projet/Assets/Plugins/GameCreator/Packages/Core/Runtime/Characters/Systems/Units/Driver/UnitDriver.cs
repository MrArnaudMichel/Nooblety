using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class UnitDriver
    {
        [SerializeReference] private IUnitDriver m_Driver = new UnitDriverController();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public IUnitDriver Wrapper => this.m_Driver;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public UnitDriver()
        { }

        public UnitDriver(IUnitDriver unit)
        {
            this.m_Driver = unit;
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Driver != null ? this.m_Driver.ToString() : "(none)";
        }
    }
}