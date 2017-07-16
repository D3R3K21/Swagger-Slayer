using System.Threading.Tasks;
using Swagger_Slayer;

namespace Sample
{
    [DocumentationDescriptor(EndpointPaths.TestModuleBasePath)]
    public interface IModuleController
    {
        [RouteDescriptor(EndpointPaths.GetSingleResourcePath, HttpMethodTypes.Post, "Description Goes Here")]
        Task<ResponseModel> GetResource(RequestModel model);
    }

    public class ModuleController : IModuleController
    {
        public Task<ResponseModel> GetResource(RequestModel model)
        {
            return Task.FromResult(new ResponseModel());
        }
    }
}
