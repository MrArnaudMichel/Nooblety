using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Stats
{
    [Title("Attribute Name")]
    [Category("Stats/Attribute Name")]

    [Image(typeof(IconAttr), ColorTheme.Type.Blue)]
    [Description("Returns the name of a Attribute")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringAttributeName : PropertyTypeGetString
    {
        [SerializeField] protected Attribute m_Attribute;

        public override string Get(Args args) => this.m_Attribute != null 
            ? this.m_Attribute.GetName(args) 
            : string.Empty;

        public override string String => this.m_Attribute != null
            ? this.m_Attribute.ID.String 
            : "(none)";
    }
}