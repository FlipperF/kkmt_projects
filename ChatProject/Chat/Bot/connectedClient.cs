using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    class connectedClient
    {
        public TcpClient Client { get; set; }
        public String Name { get; set; }

        public connectedClient(TcpClient client, string name)
        {
            Client = client;
            Name = name;
        }
    }
}
