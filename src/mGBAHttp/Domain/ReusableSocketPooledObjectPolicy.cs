using mGBAHttp.Models;
using Microsoft.Extensions.ObjectPool;

namespace mGBAHttp.Domain
{
    public class ReusableSocketPooledObjectPolicy : IPooledObjectPolicy<ReusableSocket>
    {
        private readonly SocketOptions _options;

        public ReusableSocketPooledObjectPolicy(SocketOptions options)
        {
            _options = options;
        }

        public ReusableSocket Create()
        {
            return new ReusableSocket(_options);
        }

        public bool Return(ReusableSocket obj)
        {
            return true;
        }
    }
}
