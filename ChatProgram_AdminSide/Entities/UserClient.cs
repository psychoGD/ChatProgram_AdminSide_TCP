namespace ChatProgram_AdminSide
{
    public class UserClient
    {
        public string EndPoint { get; set; }
        public string RemoteEndPoint { get; set; }
        public string UserName { get; set; }
        public bool IsConnected { get; set; } = false;
    }
}