using System.Collections.Generic;

namespace Swagger_Slayer
{
    public class DocumentationObject
    {
        public string BasePath { get; set; }
        public string Path { get; set; }
        public string HttpMethod { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }
        public Dictionary<string, object> RequestModel { get; set; }
        public Dictionary<string, object> ResponseModel { get; set; }

        public DocumentationObject()
        {
            BasePath = "/";
        }
    }
}
