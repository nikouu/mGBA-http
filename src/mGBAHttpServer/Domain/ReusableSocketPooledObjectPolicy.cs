using Microsoft.Extensions.ObjectPool;

namespace mGBAHttpServer.Domain
{
    public class ReusableSocketPooledObjectPolicy : IPooledObjectPolicy<ReusableSocket>
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private int _socketCount = 0;
        public ReusableSocketPooledObjectPolicy(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public ReusableSocket Create()
        {
            // stampeding cache problem here too?
            Interlocked.Increment(ref _socketCount);
            Console.WriteLine($"new socket ID: {Interlocked.Read(_socketCount)}");
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
