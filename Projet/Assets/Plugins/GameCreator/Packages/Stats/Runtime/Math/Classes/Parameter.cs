using System;

namespace GameCreator.Runtime.Stats
{
    internal class Parameter
    {
        [field:NonSerialized] public string Name { get; }
        [field:NonSerialized] public InputHandle Function { get; }

        public Parameter(string name, InputHandle function)
        {
            this.Name = name;
            this.Function = function;
        }
    }
}