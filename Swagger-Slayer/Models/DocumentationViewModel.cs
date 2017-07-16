using System.Collections.Generic;

namespace Swagger_Slayer
{
    public class DocumentationViewModel
    {
        public List<DocumentationObject> DocumentationObjects { get; set; }
        public DocumentationViewModel(List<DocumentationObject> documentationObjects)
        {
            DocumentationObjects = documentationObjects;
        }
    }
}
