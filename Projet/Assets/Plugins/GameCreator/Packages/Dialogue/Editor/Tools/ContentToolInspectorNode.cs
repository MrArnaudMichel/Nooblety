using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public class ContentToolInspectorNode : VisualElement
    {
        private const string PROPERTY_DATA = TTreeDataItem<Node>.NAME_VALUE;

        private const string PROPERTY_NODE_TYPE = "m_NodeType";
        private const string PROPERTY_ACTING = "m_Acting";
        private const string PROPERTY_TEXT = "m_Text";
        private const string PROPERTY_AUDIO = "m_Audio";
        
        private const string PROPERTY_ANIMATION = "m_Animation";
        private const string PROPERTY_ANIM_DATA = "m_AnimationData";
        private const string PROPERTY_SEQUENCE = "m_Sequence";

        private const string PROPERTY_CONDITIONS = "m_Conditions";
        private const string PROPERTY_ON_START = "m_OnStart";
        private const string PROPERTY_ON_FINISH = "m_OnFinish";

        private const string PROPERTY_DURATION = "m_Duration";
        private const string PROPERTY_TIMEOUT = "m_Timeout";
        public const string PROPERTY_TAG = "m_Tag";
        private const string PROPERTY_JUMP = "m_Jump";

        // PROPERTIES: ----------------------------------------------------------------------------
        
        private ContentTool ContentTool { get; }
        private SerializedProperty Property { get; }
        
        public NodeSequenceTool NodeSequence { get; }

        public Actor Actor
        {
            get
            {
                SerializedProperty actor = this.Property
                    .FindPropertyRelative(PROPERTY_ACTING)
                    .FindPropertyRelative(ActingDrawer.PROPERTY_ACTOR);
                
                return actor.objectReferenceValue as Actor;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ContentToolInspectorNode(ContentTool contentTool, SerializedProperty property)
        {
            this.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            this.AddToClassList(AlignLabel.CLASS_UNITY_MAIN_CONTAINER);
            
            this.ContentTool = contentTool;
            this.Property = property.FindPropertyRelative(PROPERTY_DATA);

            SerializedProperty nodeType = this.Property.FindPropertyRelative(PROPERTY_NODE_TYPE);
            SerializedProperty conditions = this.Property.FindPropertyRelative(PROPERTY_CONDITIONS);
            
            NodeTypePropertyElement fieldNodeType = new NodeTypePropertyElement(nodeType);
            PropertyField fieldConditions = new PropertyField(conditions);
            
            this.Add(fieldNodeType);
            this.Add(new SpaceSmall());
            this.Add(fieldConditions);

            SerializedProperty acting = this.Property.FindPropertyRelative(PROPERTY_ACTING);
            PropertyField fieldActor = new PropertyField(acting);
            
            this.Add(new SpaceSmall());
            this.Add(fieldActor);
            
            SerializedProperty text = this.Property.FindPropertyRelative(PROPERTY_TEXT);
            SerializedProperty audio = this.Property.FindPropertyRelative(PROPERTY_AUDIO);
            
            this.Add(new SpaceSmall());
            this.Add(new PropertyField(text));
            this.Add(new SpaceSmall());
            this.Add(new PropertyField(audio));
            
            SerializedProperty animation = this.Property.FindPropertyRelative(PROPERTY_ANIMATION);
            SerializedProperty animationData = this.Property.FindPropertyRelative(PROPERTY_ANIM_DATA);
            SerializedProperty sequence = this.Property.FindPropertyRelative(PROPERTY_SEQUENCE);
            
            PropertyField fieldAnimation = new PropertyField(animation);
            PropertyField fieldAnimationData = new PropertyField(animationData);

            this.NodeSequence = new NodeSequenceTool(sequence);
            
            this.Add(new SpaceSmall());
            this.Add(fieldAnimation);
            this.Add(fieldAnimationData);
            this.Add(new SpaceSmall());
            this.Add(this.NodeSequence);

            fieldActor.RegisterValueChangeCallback(changeEvent =>
            {
                SerializedPropertyType changeType = changeEvent.changedProperty.propertyType; 
                if (changeType != SerializedPropertyType.ObjectReference) return;
                
                Object newValue = changeEvent.changedProperty.objectReferenceValue;
                Actor actorAsset = newValue as Actor;
                
                this.NodeSequence.Target =
                    this.ContentTool.Settings.FindSceneReferenceForActor(actorAsset);
            });

            fieldAnimation.RegisterValueChangeCallback(changeEvent =>
            {
                Object newValue = changeEvent.changedProperty.objectReferenceValue;
                AnimationClip animationClip = newValue as AnimationClip;

                this.NodeSequence.AnimationClip = animationClip;
                this.NodeSequence.style.display = animationClip != null
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            this.NodeSequence.AnimationClip = animation.objectReferenceValue as AnimationClip;
            this.NodeSequence.style.display = animation.objectReferenceValue != null
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            
            SerializedProperty onStart = this.Property.FindPropertyRelative(PROPERTY_ON_START);
            SerializedProperty onFinish = this.Property.FindPropertyRelative(PROPERTY_ON_FINISH);

            PropertyField fieldOnStart = new PropertyField(onStart);
            PropertyField fieldOnFinish = new PropertyField(onFinish);
            
            fieldNodeType.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
            {
                bool showInstructions = 
                    !nodeType.managedReferenceFullTypename.Contains(nameof(NodeTypeChoice)) ||
                    !nodeType.FindPropertyRelative(NodeTypeChoice.NAME_SKIP_CHOICE).boolValue;
                
                fieldOnStart.SetEnabled(showInstructions);
                fieldOnFinish.SetEnabled(showInstructions);
            });

            bool showInstructions =
                !nodeType.managedReferenceFullTypename.Contains(nameof(NodeTypeChoice)) || 
                !nodeType.FindPropertyRelative(NodeTypeChoice.NAME_SKIP_CHOICE).boolValue;
                
            fieldOnStart.SetEnabled(showInstructions);
            fieldOnFinish.SetEnabled(showInstructions);
            
            this.Add(new SpaceSmall());
            this.Add(new LabelTitle("On Start"));
            this.Add(fieldOnStart);
            this.Add(new SpaceSmall());
            this.Add(new LabelTitle("On Finish"));
            this.Add(fieldOnFinish);

            SerializedProperty duration = this.Property.FindPropertyRelative(PROPERTY_DURATION);
            SerializedProperty timeout = this.Property.FindPropertyRelative(PROPERTY_TIMEOUT);

            PropertyField durationField = new PropertyField(duration);
            PropertyField timeoutField = new PropertyField(timeout);

            this.Add(new SpaceSmall());
            this.Add(durationField);
            this.Add(timeoutField);

            durationField.RegisterValueChangeCallback(eventChange =>
            {
                int index = eventChange.changedProperty.enumValueIndex;
                timeoutField.style.display = index == (int) NodeDuration.Timeout
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            });
            
            timeoutField.style.display = duration.enumValueIndex == (int) NodeDuration.Timeout
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            
            SerializedProperty jump = this.Property.FindPropertyRelative(PROPERTY_JUMP);
            
            this.Add(new SpaceSmall());
            this.Add(new NodeJumpTool(jump, this.ContentTool));
            
            this.Bind(this.ContentTool.SerializedObject);
        }
    }
}