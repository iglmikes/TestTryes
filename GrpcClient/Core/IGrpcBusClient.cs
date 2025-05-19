

using GrpcBus.Core.Proto;
using Grpc.Core;
using System.Threading.Channels;


namespace GrpcClient.Core
{

    /// <summary>
    /// Интерфейс GRPC клиента для двунаправленной потоковой связи
    /// </summary>
    public interface IGrpcBusClient : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Событие при получении сообщения от сервера
        /// </summary>
        event Action<ServerMessage> OnMessageReceived;

        /// <summary>
        /// Событие при ошибке соединения
        /// </summary>
        event Action<Exception> OnError;

        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        Task ConnectAsync(CancellationToken ct = default);

        /// <summary>
        /// Отключиться от сервера
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Отправить текстовое сообщение
        /// </summary>
        Task SendTextAsync(string text, CancellationToken ct = default);

        /// <summary>
        /// Отправить бинарные данные
        /// </summary>
        Task SendBinaryAsync(byte[] data, CancellationToken ct = default);

        /// <summary>
        /// Отправить команду
        /// </summary>
        Task SendCommandAsync(ClientCommand command, CancellationToken ct = default);

        /// <summary>
        /// Получить асинхронный поток ответов
        /// (альтернатива событию OnMessageReceived)
        /// </summary>
        IAsyncEnumerable<ServerMessage> GetResponseStream(CancellationToken ct = default);

        /// <summary>
        /// Текущий статус подключения
        /// </summary>
        bool IsConnected { get; }
    }
}
