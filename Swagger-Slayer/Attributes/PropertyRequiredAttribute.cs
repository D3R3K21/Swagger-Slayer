using System;

namespace Swagger_Slayer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyRequiredAttribute : Attribute
    {
        public PropertyRequiredAttribute()
        {
        }
    }
}
