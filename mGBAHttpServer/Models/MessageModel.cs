namespace mGBAHttpServer.Models
{
    public record struct MessageModel(string Type, string Value1 = "", string Value2 = "", string Value3 = "")
    {
        public override readonly string ToString()
        {
            return $"{Type},{Value1},{Value2},{Value3}";
        }
    }
}
