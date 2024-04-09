using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public class ContentToolSettings : VisualElement
    {
        private const string KEY_STATE = "gc:dialogue:settings:state";
        private const string KEY_DISPLAY_MODE = "gc:dialogue:settings:display-mode";
        private const string KEY_DISPLAY_TAGS = "gc:dialogue:settings:display-tags";

        private const string NAME_SCROLL = "GC-Dialogue-Settings-Scroll";
        private const string NAME_TITLE = "GC-Dialogue-Settings-Title";
        private const string NAME_CONTAINER = "GC-Dialogue-Settings-Container";

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly ScrollView m_Scroll;
        
        private readonly VisualElement m_ConfigContent;
        private readonly VisualElement m_RolesContent;
        private readonly VisualElement m_EditorContent;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public bool State
        {
            get => EditorPrefs.GetBool(KEY_STATE, false);
            set
            {
                EditorPrefs.SetBool(KEY_STATE, value);
                this.EventState?.Invoke();
            }
        }
        
        public bool DisplayActors
        {
            get => EditorPrefs.GetBool(KEY_DISPLAY_MODE, true);
            set
            {
                EditorPrefs.SetBool(KEY_DISPLAY_MODE, value);
                this.EventDisplayActors?.Invoke();
            }
        }

        public bool DisplayTags
        {
            get => EditorPrefs.GetBool(KEY_DISPLAY_TAGS, true);
            set
            {
                EditorPrefs.SetBool(KEY_DISPLAY_TAGS, value);
                this.EventDisplayTags?.Invoke();
            }
        }

        private ContentTool ContentTool { get; }
        
        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action EventState;

        public event Action EventChangeActor;
        
        public event Action EventDisplayActors;
        public event Action EventDisplayTags;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public ContentToolSettings(ContentTool contentTool)
        {
            this.ContentTool = contentTool;
            
            this.AddToClassList(AlignLabel.CLASS_UNITY_INSPECTOR_ELEMENT);
            this.AddToClassList(AlignLabel.CLASS_UNITY_MAIN_CONTAINER);

            this.m_Scroll = new ScrollView { name = NAME_SCROLL };

            this.m_ConfigContent = new VisualElement { name = NAME_CONTAINER };
            this.m_RolesContent = new VisualElement { name = NAME_CONTAINER };
            this.m_EditorContent = new VisualElement { name = NAME_CONTAINER };
            
            this.m_Scroll.Add(new Label("Configuration") { name = NAME_TITLE });
            this.m_Scroll.Add(this.m_ConfigContent);
            
            this.m_Scroll.Add(new Label("Actors") { name = NAME_TITLE });
            this.m_Scroll.Add(this.m_RolesContent);
            
            this.m_Scroll.Add(new Label("Editor") { name = NAME_TITLE });
            this.m_Scroll.Add(this.m_EditorContent);
            
            this.Add(this.m_Scroll);
        }
        
        public void Setup()
        {
            this.ContentTool.Inspector.EventChange += this.RefreshRoles;
            this.ContentTool.Tree.EventChange += this.RefreshRoles;

            this.RefreshConfig();
            this.RefreshRoles();
            this.RefreshEditor();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ToggleState()
        {
            bool state = this.State;
            this.State = !state;
        }

        public GameObject FindSceneReferenceForActor(Actor actor)
        {
            this.ContentTool.SerializedObject.Update();
            
            return actor != null 
                ? this.ContentTool.Content.GetSceneReferenceFromActor(actor)
                : null;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshConfig()
        {
            if (this.m_ConfigContent == null) return;

            SerializedProperty dialogueSkin = this.ContentTool
                .Property
                .FindPropertyRelative("m_DialogueSkin");
            
            SerializedProperty time = this.ContentTool
                .Property
                .FindPropertyRelative("m_Time");
            
            ErrorMessage fieldError = new ErrorMessage("A 'Dialogue Skin' asset is required");
            PropertyField fieldDialogueSkin = new PropertyField(dialogueSkin);
            PropertyField fieldTime = new PropertyField(time);

            this.m_ConfigContent.Add(fieldError);
            this.m_ConfigContent.Add(fieldDialogueSkin);
            this.m_ConfigContent.Add(new SpaceSmallest());
            this.m_ConfigContent.Add(fieldTime);

            fieldDialogueSkin.RegisterValueChangeCallback(changeEvent =>
            {
                if (changeEvent.changedProperty.objectReferenceValue != null)
                {
                    fieldError.style.display = DisplayStyle.None;

                    UnityEngine.Object skin = changeEvent.changedProperty.objectReferenceValue;
                    Content.LAST_SKIN = skin as DialogueSkin;;
                }
                else
                {
                    fieldError.style.display = DisplayStyle.Flex;
                }
            });
            
            fieldError.style.display = dialogueSkin.objectReferenceValue != null
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }
        
        private void RefreshRoles()
        {
            if (this.m_RolesContent == null) return;
            
            this.m_RolesContent.Clear();
            this.ContentTool.SerializedObject.Update();
            
            SerializedProperty roles = this.ContentTool.Property.FindPropertyRelative("m_Roles");
            int rolesSize = roles.arraySize;

            for (int i = 0; i < rolesSize; ++i)
            {
                SerializedProperty role = roles.GetArrayElementAtIndex(i);
                Actor actor = role
                    .FindPropertyRelative(Role.NAME_ACTOR)
                    .objectReferenceValue as Actor;

                string title = actor != null ? actor.name : "(missing)";
                PropertyField fieldRole = new PropertyField(role, title);

                this.m_RolesContent.Add(fieldRole);
                this.m_RolesContent.Add(new SpaceSmaller());
                fieldRole.Bind(this.ContentTool.SerializedObject);

                fieldRole.RegisterValueChangeCallback(_ => this.OnChangeActor());
            }
        }

        private void RefreshEditor()
        {
            this.m_EditorContent.Clear();

            IntegerField editorHeight = new IntegerField("Height")
            {
                value = this.ContentTool.InspectorHeight,
            };

            editorHeight.RegisterValueChangedCallback(changeEvent =>
            {
                this.ContentTool.InspectorHeight = changeEvent.newValue;
            });

            Toggle editorDisplayActors = new Toggle("Display Actors")
            {
                value = this.DisplayActors
            };

            editorDisplayActors.RegisterValueChangedCallback(changeEvent =>
            {
                this.DisplayActors = changeEvent.newValue;
            });
            
            Toggle editorDisplayTags = new Toggle("Display Tags")
            {
                value = this.DisplayTags
            };

            editorDisplayTags.RegisterValueChangedCallback(changeEvent =>
            {
                this.DisplayTags = changeEvent.newValue;
            });
            
            this.m_EditorContent.Add(editorHeight);
            this.m_EditorContent.Add(editorDisplayActors);
            this.m_EditorContent.Add(editorDisplayTags);
        }
        
        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeActor()
        {
            EventChangeActor?.Invoke();
        }
    }
}