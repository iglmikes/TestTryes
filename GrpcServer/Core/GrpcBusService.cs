

using Grpc.Core;
using GrpcBus.Core.Proto;
using Microsoft.Extensions.Logging;



namespace GrpcServer.Core
{
    public class GrpcBusService : DataBusService.DataBusServiceBase
    {
        private readonly ILogger<GrpcBusService> _logger;
        private readonly IMessageProcessor _messageProcessor;

        public GrpcBusService(ILogger<GrpcBusService> logger, IMessageProcessor messageProcessor)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
        }

        public override async Task EstablishDuplexConnection(
            IAsyncStreamReader<ClientMessage> requestStream,
            IServerStreamWriter<ServerMessage> responseStream,
            ServerCallContext context)
        {
            var connectionId = context.GetHttpContext().Connection.Id;
            _logger.LogInformation("Connection established: {ConnectionId}", connectionId);

            try
            {
                // Обработка входящих сообщений
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync())
                    {
                        await _messageProcessor.ProcessClientMessage(message, responseStream);
                    }
                });

                await readTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in duplex communication");
            }
            finally
            {
                _logger.LogInformation("Connection closed: {ConnectionId}", connectionId);
            }
        }
    }
}
