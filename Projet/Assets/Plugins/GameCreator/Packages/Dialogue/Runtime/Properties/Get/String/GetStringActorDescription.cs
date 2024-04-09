using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Title("Actor Description")]
    [Category("Dialogue/Actor Description")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Description("Returns the description of the Actor asset")]
    
    [Serializable]
    public class GetStringActorDescription : PropertyTypeGetString
    {
        [SerializeField] private Actor m_Actor;
        
        public override string Get(Args args) => this.m_Actor != null 
            ? this.m_Actor.GetDescription(args) 
            : string.Empty;

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringActorDescription()
        );

        public override string String => string.Format(
            "{0} Description", 
            this.m_Actor != null ? this.m_Actor.name : "(none)"
        );
    }
}