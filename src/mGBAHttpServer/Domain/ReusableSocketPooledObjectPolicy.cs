using Microsoft.Extensions.ObjectPool;

namespace mGBAHttpServer.Domain
{
    public class ReusableSocketPooledObjectPolicy : IPooledObjectPolicy<ReusableSocket>
    {
        private readonly string _ipAddress;
        private readonly int _port;

        public ReusableSocketPooledObjectPolicy(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public ReusableSocket Create()
        {
            // stampeding cache problem here too?
            var socket = new ReusableSocket(_ipAddress, _port);
            socket.Connect();
            return socket;
        }

        public bool Return(ReusableSocket obj)
        {
            return true;
        }
    }
}
