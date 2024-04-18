using System;

namespace Yahv.Underly.Attributes
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class FixedAttribute : Attribute
    {
        public string ID { get; private set; }

        public FixedAttribute(string id)
        {
            this.ID = id;
        }
    }
}
