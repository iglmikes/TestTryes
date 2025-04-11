using Grpc.Core;
using GrpcBus.Core.Proto;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;



namespace GrpcServer.Core
{
    public class GrpcBusService : DataBusService.DataBusServiceBase
    {
        private readonly ILogger<GrpcBusService> _logger;
        private readonly IMessageProcessor _messageProcessor;
        private readonly ConcurrentDictionary<string, IServerStreamWriter<ServerMessage>> _activeConnections = new();


        public GrpcBusService(ILogger<GrpcBusService> logger, IMessageProcessor messageProcessor)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task EstablishDuplexConnection( IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            var connectionId = context.GetHttpContext().Connection.Id;
            var peer = context.Peer;

            _logger.LogInformation("New connection: {ConnectionId} from {Peer}", connectionId, peer);

            try
            {
                // Регистрируем соединение
                _activeConnections.TryAdd(connectionId, responseStream);

                // Отправляем уведомление о подключении
                await responseStream.WriteAsync(new ServerMessage
                {
                    Notification = ServerNotification.ConnectionEstablished
                });

                // Обработка входящих сообщений
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    try
                    {
                        await _messageProcessor.ProcessClientMessage(message, responseStream, context);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from {ConnectionId}", connectionId);
                    }
                }
            }
            catch (IOException ex) when (ex.InnerException is IOException ioEx &&
                                        ioEx.Message.Contains("The client reset the request stream"))
            {
                _logger.LogInformation("Client {ConnectionId} disconnected abruptly", connectionId);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("Client {ConnectionId} cancelled the connection", connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in duplex communication with {ConnectionId}", connectionId);
            }
            finally
            {
                _activeConnections.TryRemove(connectionId, out _);
                _logger.LogInformation("Connection closed: {ConnectionId}", connectionId);
            }
        }


        public async Task BroadcastMessage(ServerMessage message)
        {
            foreach (var connection in _activeConnections)
            {
                try
                {
                    await connection.Value.WriteAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to broadcast to {ConnectionId}", connection.Key);
                }
            }
        }



    }
}
