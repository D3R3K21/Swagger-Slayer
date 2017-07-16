using Nancy;
using Nancy.ModelBinding;

namespace Sample
{
    public class TestModule : NancyModule
    {
        public TestModule(IModuleController controller) : base(EndpointPaths.TestModuleBasePath)
        {

            Post[EndpointPaths.GetSingleResourcePath, true] = async (context, cancel) =>
            {
                return await controller.GetResource(this.Bind<RequestModel>());
            };
        }
    }
}
