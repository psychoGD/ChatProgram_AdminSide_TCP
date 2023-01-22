using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatProgram_AdminSide
{
    public class User
    {
        public string Username { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string RemoteEndPoint { get; set; }

        //New 
        //public TcpClient client { get; set; }
    }
}
