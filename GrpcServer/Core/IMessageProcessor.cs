

using GrpcBus.Core.Proto;
using Grpc.Core;

namespace GrpcServer.Core
{
    public interface IMessageProcessor
    {
        Task ProcessClientMessage(ClientMessage message, IServerStreamWriter<ServerMessage> responseStream);
    }
}
