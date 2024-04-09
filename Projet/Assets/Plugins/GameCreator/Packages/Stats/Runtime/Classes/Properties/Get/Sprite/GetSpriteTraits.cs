using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [Title("Traits Sprite")]
    [Category("Stats/Traits Sprite")]
    
    [Image(typeof(IconTraits), ColorTheme.Type.Pink)]
    [Description("A reference to the Sprite value of a Class assigned to a Traits component")]

    [Serializable] [HideLabelsInEditor]
    public class GetSpriteTraits : PropertyTypeGetSprite
    {
        [SerializeField] protected PropertyGetGameObject m_Traits = GetGameObjectPlayer.Create();

        public override Sprite Get(Args args)
        {
            Traits traits = this.m_Traits.Get<Traits>(args);
            return traits != null ? traits.Class.GetSprite(args) : null;
        }

        public override string String => $"{this.m_Traits} Sprite";
    }
}