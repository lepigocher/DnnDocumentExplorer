using DotNetNuke.Web.Api;

namespace TidyModules.DocumentExplorer.Components
{
    public sealed class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(@"TidyModules/DocumentExplorer", "default", "{controller}/{action}", new[] { "TidyModules.DocumentExplorer.Components" });
        }
    }
}