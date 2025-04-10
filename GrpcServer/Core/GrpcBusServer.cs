using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;




namespace GrpcServer.Core
{
    public static class GrpcBusServer
    {
        public static void MapGrpcBusService(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<GrpcBusService>();
        }
    }
}
