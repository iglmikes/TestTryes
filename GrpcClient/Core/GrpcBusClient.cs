using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using GrpcBus.Core.Proto;
using System.Threading.Channels;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using GrpcClient.Support;

namespace GrpcClient.Core
{
    public class GrpcBusClient : IDisposable
    {
        private readonly ILogger<GrpcBusClient> _logger;
        private readonly GrpcBusClientOptions _options;
        private readonly Channel<ClientMessage> _outgoingMessages = Channel.CreateUnbounded<ClientMessage>();
        private readonly CancellationTokenSource _cts = new();
        private AsyncDuplexStreamingCall<ClientMessage, ServerMessage>? _duplexStream;
        private DataBusService.DataBusServiceClient? _client;
        private GrpcChannel? _channel;
        private Task? _receivingTask;
        private Task? _sendingTask;

        public event Action<ServerMessage>? OnMessageReceived;
        public event Action<Exception>? OnError;
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public GrpcBusClient(ILogger<GrpcBusClient> logger)
        {
            _logger = logger;
        }

        public GrpcBusClient(ILogger<GrpcBusClient> logger, GrpcBusClientOptions options)
        {
            _logger = logger;
            _options = options;
        }


        public async Task ConnectAsync(string serverUrl, bool useHttps = true)
        {
            try
            {
                _logger.LogInformation("Connecting to {ServerUrl}...", serverUrl);

                var options = new GrpcChannelOptions
                {
                    HttpHandler = new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                        EnableMultipleHttp2Connections = true
                    },
                    MaxReceiveMessageSize = 16 * 1024 * 1024, // 16MB
                    MaxSendMessageSize = 16 * 1024 * 1024 // 16MB
                };

                _channel = useHttps ?
                    GrpcChannel.ForAddress(serverUrl, options) :
                    GrpcChannel.ForAddress(serverUrl, new GrpcChannelOptions
                    {
                        HttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }
                    });

                _client = new DataBusService.DataBusServiceClient(_channel);

                _duplexStream = _client.EstablishDuplexConnection(
                    cancellationToken: _cts.Token);

                _receivingTask = Task.Run(ReceiveMessagesAsync);
                _sendingTask = Task.Run(SendMessagesAsync);

                OnConnected?.Invoke();
                _logger.LogInformation("Connected to {ServerUrl}", serverUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection failed");
                OnError?.Invoke(ex);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting...");

                // Завершаем отправку сообщений
                _outgoingMessages.Writer.Complete();

                // Отменяем операции
                _cts.Cancel();

                // Ждем завершения задач
                if (_sendingTask != null) await _sendingTask;
                if (_receivingTask != null) await _receivingTask;

                // Закрываем соединение
                //if (_duplexStream != null) await _duplexStream.DisposeAsync();
                if (_channel != null) await _channel.ShutdownAsync();
                if (_duplexStream != null)  _duplexStream.Dispose();//почему то DisposeAsynk не нашолса


                OnDisconnected?.Invoke();
                _logger.LogInformation("Disconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Disconnection error");
                OnError?.Invoke(ex);
            }
        }

        public async Task SendTextAsync(string text)
        {
            var message = new ClientMessage { TextMessage = text };
            await _outgoingMessages.Writer.WriteAsync(message);
        }

        public async Task SendBinaryAsync(byte[] data)
        {
            var message = new ClientMessage
            {
                BinaryData = Google.Protobuf.ByteString.CopyFrom(data)
            };
            await _outgoingMessages.Writer.WriteAsync(message);
        }

        public async Task SendCommandAsync(ClientCommand command)
        {
            var message = new ClientMessage { Command = command };
            await _outgoingMessages.Writer.WriteAsync(message);
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                await foreach (var response in _duplexStream!.ResponseStream.ReadAllAsync(_cts.Token))
                {
                    try
                    {
                        OnMessageReceived?.Invoke(response);
                        ProcessServerMessage(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing server message");
                    }
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("Receiving cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Receiving error");
                OnError?.Invoke(ex);
            }
        }

        private async Task SendMessagesAsync()
        {
            try
            {
                await foreach (var message in _outgoingMessages.Reader.ReadAllAsync(_cts.Token))
                {
                    try
                    {
                        await _duplexStream!.RequestStream.WriteAsync(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending message");
                        OnError?.Invoke(ex);
                    }
                }

                await _duplexStream!.RequestStream.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sending error");
                OnError?.Invoke(ex);
            }
        }

        private void ProcessServerMessage(ServerMessage message)
        {
            switch (message.ContentCase)
            {
                case ServerMessage.ContentOneofCase.TextResponse:
                    _logger.LogInformation("Server text: {Text}", message.TextResponse);
                    break;

                case ServerMessage.ContentOneofCase.BinaryResponse:
                    _logger.LogInformation("Server binary data, length: {Length}",
                        message.BinaryResponse.Length);
                    break;

                case ServerMessage.ContentOneofCase.Notification:
                    _logger.LogInformation("Server notification: {Notification}",
                        message.Notification);
                    break;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _duplexStream?.Dispose();
            _channel?.Dispose();
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
