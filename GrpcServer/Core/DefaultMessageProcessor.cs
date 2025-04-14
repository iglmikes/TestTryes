using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GrpcBus.Core;
using GrpcBus.Core.Proto;
using Grpc.Core;
using Microsoft.Extensions.Logging;


namespace GrpcServer.Core
{
    public class DefaultMessageProcessor : IMessageProcessor
    {
        private readonly ILogger<DefaultMessageProcessor> _logger;

        private readonly GrpcBusService _busService;



        public DefaultMessageProcessor(ILogger<DefaultMessageProcessor> logger)
        {
            _logger = logger;
        }

        public DefaultMessageProcessor(  ILogger<DefaultMessageProcessor> logger, GrpcBusService busService)
        {
            _logger = logger;
            _busService = busService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="responseStream"></param>
        /// <returns></returns>
        public async Task ProcessClientMessage(   ClientMessage message,  IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            switch (message.ContentCase)
            {
                case ClientMessage.ContentOneofCase.TextMessage:
                    await HandleTextMessage(message.TextMessage, responseStream, context);
                    break;

                case ClientMessage.ContentOneofCase.BinaryData:
                    await HandleBinaryData(message.BinaryData.ToArray(), responseStream, context);
                    break;

                case ClientMessage.ContentOneofCase.Command:
                    await HandleCommand(message.Command, responseStream, context);
                    break;

                default:
                    _logger.LogWarning("Unknown message type from {Peer}", context.Peer);
                    break;
            }
        }

        private async Task HandleTextMessage( string text, IServerStreamWriter<ServerMessage> responseStream,  ServerCallContext context)
        {
            _logger.LogInformation("Received text from {Peer}: {Text}", context.Peer, text);

            // Эхо-ответ
            await responseStream.WriteAsync(new ServerMessage
            {
                TextResponse = $"Server received at {DateTime.UtcNow}: {text}"
            });

           // //Широковещательное сообщение всем клиентам -пока не реализовано -мб в.proto над добавить?
            
           //await _busService.BroadcastMessage(new ServerMessage
           //{
           //    TextResponse = $"User {context.Peer} sent: {text}"
           //});
        }

        private async Task HandleBinaryData(  byte[] data,  IServerStreamWriter<ServerMessage> responseStream,  ServerCallContext context)
        {
            _logger.LogInformation("Received binary data from {Peer}, length: {Length}",
                context.Peer, data.Length);

            // Пример обработки бинарных данных
            var processedData = ProcessData(data);

            await responseStream.WriteAsync(new ServerMessage
            {
                BinaryResponse = Google.Protobuf.ByteString.CopyFrom(processedData)
            });
        }

        private async Task HandleCommand( ClientCommand command,IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Received command {Command} from {Peer}", command, context.Peer);

            switch (command)
            {
                case ClientCommand.Ping:
                    await responseStream.WriteAsync(new ServerMessage
                    {
                        Notification = ServerNotification.Pong
                    });
                    break;

                case ClientCommand.Subscribe:
                    // Логика подписки
                    await responseStream.WriteAsync(new ServerMessage
                    {
                        TextResponse = "Subscribed to updates"
                    });
                    break;

                case ClientCommand.Unsubscribe:
                    // Логика отписки
                    await responseStream.WriteAsync(new ServerMessage
                    {
                        TextResponse = "Unsubscribed from updates"
                    });
                    break;
            }
        }

        private byte[] ProcessData(byte[] data)
        {
            // Пример обработки данных
            return data.Reverse().ToArray();
        }



    }
}
