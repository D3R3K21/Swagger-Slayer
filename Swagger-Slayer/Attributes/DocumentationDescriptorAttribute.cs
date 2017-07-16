using System;

namespace Swagger_Slayer
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DocumentationDescriptorAttribute : Attribute
    {
        public string BasePath { get; }
        public DocumentationDescriptorAttribute(string basePath)
        {
            BasePath = basePath;
        }
    }
}
