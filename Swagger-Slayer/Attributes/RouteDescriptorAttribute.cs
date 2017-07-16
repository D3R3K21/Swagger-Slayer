using System;

namespace Swagger_Slayer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteDescriptorAttribute : Attribute
    {
        public string Path { get; }
        public HttpMethodTypes HttpMethodType { get; }
        public string Description { get; set; }
        public RouteDescriptorAttribute(string path, HttpMethodTypes type, string description)
        {
            Path = path;
            HttpMethodType = type;
            Description = description;
        }
    }
}
