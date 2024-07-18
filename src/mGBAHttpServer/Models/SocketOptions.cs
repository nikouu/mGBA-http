namespace mGBAHttpServer.Models
{
    public class SocketOptions
    {
        public static readonly string Section = "mgba-http:Socket";
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8888;
        public int ReadTimeout { get; set; } = 2000; 
        public int WriteTimeout { get; set; } = 2000; 
    }
}
