using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Distance")]
    [Description("Calculates the distance between two points in space and saves the result")]

    [Category("Math/Geometry/Distance")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Point 1", "The first operand of the geometric operation that represents a point in space")]
    [Parameter("Point 2", "The second operand of the geometric operation that represents a point in space")]

    [Keywords("Magnitude")]
    [Keywords("Position", "Location", "Variable")]
    [Image(typeof(IconCompass), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryDistance : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberNone.Create;
        
        [SerializeField]
        private PropertyGetPosition m_Point1 = new PropertyGetPosition();
        
        [SerializeField]
        private PropertyGetPosition m_Point2 = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => string.Format(
            "Set {0} = distance from {1} to {2}", 
            this.m_Set,
            this.m_Point1,
            this.m_Point2
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            float value = Vector3.Distance(
                this.m_Point1.Get(args),
                this.m_Point2.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}