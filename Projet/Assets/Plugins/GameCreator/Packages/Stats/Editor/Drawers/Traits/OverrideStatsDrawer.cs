using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEditor;

namespace GameCreator.Editor.Stats
{
    [CustomPropertyDrawer(typeof(OverrideStats))]
    public class OverrideStatsDrawer : TOverrideDrawer
    {
        public static readonly IIcon ICON = new IconStat(ColorTheme.Type.Red);
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Label => "Stats";
        protected override IIcon Icon => ICON;

        // IMPLEMENT METHODS: ---------------------------------------------------------------------
        
        protected override SerializedProperty GetKeys(SerializedProperty property)
        {
            return property.FindPropertyRelative(OverrideStats.NAME_KEYS);
        }

        protected override SerializedProperty GetValues(SerializedProperty property)
        {
            return property.FindPropertyRelative(OverrideStats.NAME_VALUES);
        }
    }
}