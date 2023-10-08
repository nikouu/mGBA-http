namespace mGBAHttpServer.Models
{
    public record struct MessageModel(string Type, string Value)
    {
        public override string ToString()
        {
            return $"{Type},{Value}";
        }
    }
}
