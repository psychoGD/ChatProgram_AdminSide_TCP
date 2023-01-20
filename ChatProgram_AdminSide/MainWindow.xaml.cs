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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace ChatProgram_AdminSide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        static TcpListener listener = null;
        static BinaryWriter bw = null;
        static BinaryReader br = null;
        public static List<TcpClient> Clients { get; set; }
        //public static List<User> Users { get; set; } = new List<User>();

        private readonly object _locker = new object();

        private ObservableCollection<User> users;

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged();
                BindingOperations.EnableCollectionSynchronization(users, _locker);
            }
        }

        //Test
        //private ObservableCollection<UserUC> users;

        //public ObservableCollection<UserUC> Users
        //{
        //    get { return users; }
        //    set
        //    {
        //        users = value;
        //        OnPropertyChanged();
        //        BindingOperations.EnableCollectionSynchronization(users, _locker);
        //    }
        //}





        //UI Bind



        private string ipAdressUI;

        public string IpAdressUI
        {
            get { return ipAdressUI; }
            set { ipAdressUI = value; OnPropertyChanged(); }
        }

        private string portUI;

        public string PortUI
        {
            get { return portUI; }
            set { portUI = value; OnPropertyChanged(); }
        }

        //public string PortUI { get; set; }


        //public DispatcherTimer dispatcherTimer { get; set; }
        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //OLD Original
            Users = new ObservableCollection<User>();

            //TEST
            //Users = new ObservableCollection<UserUC>();
            Task.Run(() =>
            {
                ConnectAcceptor();
            });



            //Task.Run(() =>
            //{

            //    DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //    dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            //    dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            //    dispatcherTimer.Start();
            //});

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

        //This function for who is connect
        [Obsolete]
        public void ConnectAcceptor()
        {
            Clients = new List<TcpClient>();
            var ip = IPAddress.Parse(GetIpAdress(GetHostName()));
            var port = 27001;
            IpAdressUI = ip.ToString();
            portUI = port.ToString();
            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClientAsync().Result;
                Task.Run(() =>
                {

                    UserCreate(client);
                    Clients.Add(client);
                });
                //MessageBox.Show($"{client.Client.RemoteEndPoint} is connected");
            }
        }

        public void UserCreate(TcpClient client)
        {

            Task.Run(() =>
            {
                CreateUser(client);
                //Old
                foreach (var item in Users)
                {
                    MessageBox.Show(item.UserName + "   " + item.RemoteEndPoint);
                }

                //TESt
                //foreach (var item in Users)
                //{
                //    MessageBox.Show(item.user.UserName + "   " + item.user.RemoteEndPoint);
                //}
            });
        }

        //This is helper function for User Create. 
        //This Function get first string from user And If User Disconnect In 30 Second  
        public void CreateUser(TcpClient client)
        {
            var stream = client.GetStream();
            br = new BinaryReader(stream);
            while (true)
            {
                try
                {
                    var msg = br.ReadString();
                    var flag = msg == " " || msg == null || msg == "";
                    if (!flag)
                    {
                        //Test
                        //MessageBox.Show($"|Test| Flag: {flag}");

                        // your code
                        User user = new User();
                        user.UserName = msg;
                        user.RemoteEndPoint = client.Client.RemoteEndPoint.ToString();
                        user.IsConnected = true;

                        //UserUC userUC = new UserUC();
                        //userUC.user = user;
                        //Users.Add(userUC);
                        //Old
                        Users.Add(user);
                        return;
                    }

                }
                catch (Exception ex)
                {

                    //This is for UI WPF TEST
                    MessageBox.Show($"{client.Client.RemoteEndPoint} disconnected\n{ex.Message}");
                    UserDisconnected(client);
                    return;
                    //This is For Console TEST
                    //Console.WriteLine($"{item.Client.RemoteEndPoint}  disconnected");
                }
            }
        }

        public void UserDisconnected(TcpClient client)
        {
            Clients.Remove(client);
            //Old Original
            foreach (var item in Users)
            {
                if (item.RemoteEndPoint == client.Client.RemoteEndPoint.ToString())
                {
                    item.IsConnected = false;
                }
            }


            //TEST
            //foreach (var item in Users)
            //{
            //    if (item.user.RemoteEndPoint == client.Client.RemoteEndPoint.ToString())
            //    {
            //        item.user.IsConnected = false;
            //    }
            //}
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
