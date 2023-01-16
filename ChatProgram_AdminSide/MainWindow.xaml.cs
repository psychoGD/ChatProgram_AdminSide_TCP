using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatProgram_AdminSide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static TcpListener listener = null;
        static BinaryWriter bw = null;
        static BinaryReader br = null;
        public static List<TcpClient> Clients { get; set; }

        public MainWindow()
        {
            InitializeComponent();

        }

        public string GetHostName()
        {
            string hostName = Dns.GetHostName();
            return hostName;
        }

        [Obsolete]
        public string GetIpAdress(string HostName)
        {
            string IP = Dns.GetHostByName(HostName).AddressList[0].ToString();
            return IP;
        }

        [Obsolete]
        public void TestFunc()
        {
            

            Clients = new List<TcpClient>();
            var ip = IPAddress.Parse(GetIpAdress(GetHostName()));
            var port = 27001;

            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Clients.Add(client);
                Console.WriteLine($"{client.Client.RemoteEndPoint}");
                Task.Run(() =>
                {
                    var reader = Task.Run(() =>
                    {
                        foreach (var item in Clients)
                        {
                            Task.Run(() =>
                            {
                                var stream = item.GetStream();
                                br = new BinaryReader(stream);
                                while (true)
                                {
                                    try
                                    {
                                        var msg = br.ReadString();
                                        Console.WriteLine($"CLIENT : {client.Client.RemoteEndPoint} : {msg}");

                                    }
                                    catch (Exception)
                                    {

                                        //This is for UI WPF TEST
                                        MessageBox.Show($"{item.Client.RemoteEndPoint} disconnected");

                                        //This is For Console TEST
                                        //Console.WriteLine($"{item.Client.RemoteEndPoint}  disconnected");

                                        Clients.Remove(item);
                                    }
                                }
                            }).Wait(50);
                        }

                        //var stream = client.GetStream();
                        //br = new BinaryReader(stream);
                        //while (true)
                        //{
                        //    var msg = br.ReadString();
                        //    Console.WriteLine($"CLIENT : {client.Client.RemoteEndPoint} : {msg}");
                        //}
                    });

                    var writer = Task.Run(() =>
                    {
                        //var stream = client.GetStream();
                        //bw= new BinaryWriter(stream);
                        //while (true)
                        //{
                        //    var msg = Console.ReadLine();
                        //    bw.Write(msg);
                        //}

                        while (true)
                        {
                            var msg = Console.ReadLine();
                            foreach (var item in Clients)
                            {
                                var stream = item.GetStream();
                                bw = new BinaryWriter(stream);
                                bw.Write(msg);
                            }
                            foreach (var item in Clients)
                            {
                                if (item.Connected)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                }
                                Console.WriteLine($"item : {item.Client.RemoteEndPoint}");
                                Console.ResetColor();
                            }
                        }
                    });
                });
            }

        }
    }
}
