﻿namespace mGBAHttp.Models
{
    public class SocketOptions
    {
        public const string Section = "mgba-http:Socket";
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8888;
        public int ReadTimeout { get; set; } = 3000;
        public int WriteTimeout { get; set; } = 3000;
    }
}
