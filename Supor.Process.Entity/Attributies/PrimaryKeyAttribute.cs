using System;

namespace Supor.Process.Entity.Attributies
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute:Attribute
    {
        public readonly string Name;

        public PrimaryKeyAttribute(string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }
        }
    }
}
