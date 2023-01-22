namespace ChatProgram_AdminSide
{
    public class UserClient
    {
        public string RemoteEndPoint { get; set; }
        public string UserName { get; set; }
        public bool IsConnected { get; set; } = false;
    }
}