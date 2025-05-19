using GrpcClient.Core;

namespace WebClientApp.GrpcProvider
{
    public class OrderService(IGrpcBusClient grpcClient)
    {
        public async Task PlaceOrder()
        {
            await grpcClient.SendTextAsync("Order placed");
        }
    }
}
