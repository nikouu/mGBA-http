using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Domain
{
    public static class PooledSocketHelper
    {
        public static async Task<string> SendMessageAsync(ObjectPool<ReusableSocket> socketPool, string message)
        {
            var socket = socketPool.Get();
            try
            {
                return await socket.SendMessageAsync(message);
            }
            finally
            {
                socketPool.Return(socket);
            }
        }
    }
}