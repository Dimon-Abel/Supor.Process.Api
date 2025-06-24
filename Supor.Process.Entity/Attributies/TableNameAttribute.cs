using System;

namespace Supor.Process.Entity.Attributies
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute: Attribute
    {
        public readonly string Name;

        public TableNameAttribute(string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }
        }
    }
}
