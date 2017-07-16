using System.Linq;
using Nancy;
using Nancy.Routing;

namespace Swagger_Slayer.Modules
{
    public class DocumentationModule : NancyModule
    {
        public DocumentationModule(IRouteCacheProvider routeCacheProvider) : base("/documentation")
        {
            Get["/"] = parameters =>
            {
                var model = new DocumentationViewModel(routeCacheProvider.GetCache().SelectMany(p => p.Value)
                .SelectMany(p => p.Item2.Metadata.Raw.Select(g => (DocumentationObject)g.Value).ToList()).Where(p => p != null).ToList());
                return Response.AsJson(model);
            };
        }
    }
}
