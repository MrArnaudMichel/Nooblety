using System;
using System.Globalization;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [Title("Last Attribute Change")]
    [Category("Stats/Last Attribute Change")]

    [Image(typeof(IconAttr), ColorTheme.Type.Yellow)]
    [Description("The difference between the new and the old value of the last Attribute changed")]

    [Serializable]
    public class GetStringAttributeLastChange : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetGameObject m_Traits = GetGameObjectPlayer.Create();

        public override string Get(Args args)
        {
            Traits traits = this.m_Traits.Get<Traits>(args);
            return traits != null 
                ? traits.RuntimeAttributes.LastChange.ToString("0", CultureInfo.InvariantCulture) 
                : string.Empty;
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringAttributeLastChange()
        );

        public override string String => this.m_Traits.ToString();
    }
}