using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TRunner<TValue> : Runner
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected TValue m_Value;

        // PROPERTIES: ----------------------------------------------------------------------------

        public TValue Value => this.m_Value;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static GameObject CreateTemplate<TRunnerType>(TValue value)
            where TRunnerType : TRunner<TValue>
        {
            GameObject template = new GameObject
            {
                name = "Template",
                hideFlags = TEMPLATE_FLAGS
            };
                
            TRunnerType runner = template.Add<TRunnerType>();
            runner.m_Value = value;

            return template;
        }
        
        public static TRunnerType CreateRunner<TRunnerType>(GameObject template, RunnerConfig config)
            where TRunnerType : TRunner<TValue>
        {
            Vector3 position = config.Location.Position;
            Quaternion rotation = config.Location.Rotation;
            
            if (config.Location.Parent != null)
            {
                position = config.Location.Parent.TransformPoint(position);
                rotation = config.Location.Parent.rotation * rotation;
            }
            
            GameObject instance = Instantiate(
                template, 
                position, rotation, 
                config.Location?.Parent
            );

            instance.name = config.Name;
            instance.hideFlags = INSTANCE_FLAGS;
            return instance.GetComponent<TRunnerType>();
        }
    }
}