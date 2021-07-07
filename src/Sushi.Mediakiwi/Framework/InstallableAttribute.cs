using System;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InstallableAttribute : Attribute
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }

        public InstallableAttribute(string identifierGuid, int version, string name, string description)
        {
            ID = new Guid(identifierGuid);
            Name = name;
            Version = version;
            Description = description;
        }
    }
}
