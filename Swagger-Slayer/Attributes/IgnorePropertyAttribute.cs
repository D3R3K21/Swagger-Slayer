using System;

namespace Swagger_Slayer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
        public IgnorePropertyAttribute()
        {
        }
    }
}
