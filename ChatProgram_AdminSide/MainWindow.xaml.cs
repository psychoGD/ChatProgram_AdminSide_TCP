using ChatProgram_ClientSide_Wpf.JsonHelper;
using ChatProgram_ClientSide_Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private readonly object _locker = new object();

        private ObservableCollection<UserClient> users;

        public ObservableCollection<UserClient> Users
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
        private UserClient currentUser;

        public UserClient CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; OnPropertyChanged(); }
        }

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //OLD Original
            Users = new ObservableCollection<UserClient>();
            Clients = new List<TcpClient>();
            //TEST
            //Users = new ObservableCollection<UserUC>();
            var task = Task.Run(() =>
            {
                //MessageBox.Show("Acceptor");
                ConnectAcceptor();
            });

            //error
            //Task.Run(() =>
            //{
            //    //MessageBox.Show("Data Reader");
            //    Task.Run(() =>
            //    {

            //        DataReaderFromEveryone();
            //    });
            //});


            //Task.Run(() =>
            //{

            //    DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //    dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            //    dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            //    dispatcherTimer.Start();
            //});

        }
        #region Datareader Test

        //public void DataReaderFromEveryone()
        //{
        //    Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            foreach (var item in Clients)
        //            {
        //                Task.Run(() =>
        //                {
        //                    var user = GetUserByRemoteEndPoint(item.Client.RemoteEndPoint.ToString());
        //                    if (user == null)
        //                    {
        //                        var stream = item.GetStream();
        //                        br = new BinaryReader(stream);
        //                        while (true)
        //                        {
        //                            try
        //                            {
        //                                var msg = br.ReadString();
        //                                MessageBox.Show(msg);
        //                            }
        //                            catch (Exception)
        //                            {
        //                                UserDisconnected(item);
        //                                MessageBox.Show("Test");
        //                                break;
        //                            }
        //                        }
        //                    }
        //                });

        //            }
        //        }
        //    });
        //}

        #endregion


        #region Network

        public static string GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }
                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }
                    return address.Address.ToString();
                }
            }
            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }

        #endregion

        #region User Acceptor

        //This function for who is connect
        [Obsolete]
        public void ConnectAcceptor()
        {
            Clients = new List<TcpClient>();
            var ip = IPAddress.Parse(GetLocalIpAddress());
            var port = 27001;
            IpAdressUI = ip.ToString();
            portUI = port.ToString();
            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClientAsync().Result;
                MessageBox.Show(client.Client.RemoteEndPoint.ToString());
                Clients.Add(client);
                Task.Run(() =>
                {
                    //UserCreate(client);
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
                                        MessageBox.Show(msg);
                                        try
                                        {
                                            var settings = new JsonSerializerSettings();
                                            settings.Converters.Add(new IPAddressConverter());
                                            settings.Converters.Add(new IPEndPointConverter());
                                            settings.Formatting = Formatting.Indented;
                                            var user = JsonConvert.DeserializeObject<User>(msg, settings);

                                            if (user != null)
                                            {
                                                UserClient userClient = new UserClient();
                                                userClient.UserName = user.Username;
                                                userClient.RemoteEndPoint = user.EndPoint.ToString();
                                                userClient.IsConnected = true;
                                                Users.Add(userClient);
                                            }
                                            else
                                            {
                                                var message = JsonConvert.DeserializeObject(msg);
                                                MessageBox.Show(message.ToString());
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        MessageBox.Show($"CLIENT : {client.Client.RemoteEndPoint} :\n {msg}");

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"{item.Client.RemoteEndPoint}  disconnected");
                                        //Clients.Remove(item);
                                        break;
                                    }
                                }

                            });
                        }
                    });
                });
                //MessageBox.Show($"{client.Client.RemoteEndPoint} is connected");
            }
        }

        //private bool IsClientExist(TcpClient client)
        //{
        //    var ep = client.Client.RemoteEndPoint.ToString();
        //    foreach (var user in Users)
        //    {
        //        if (user.RemoteEndPoint == ep)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        #region Old Broken Func
        //public void UserCreate(TcpClient client)
        //{

        //    Task.Run(() =>
        //    {
        //        CreateUser(client);
        //        //Old
        //        foreach (var item in Users)
        //        {
        //            MessageBox.Show(item.UserName + "   " + item.RemoteEndPoint);
        //        }

        //        //TESt
        //        //foreach (var item in Users)
        //        //{
        //        //    MessageBox.Show(item.user.UserName + "   " + item.user.RemoteEndPoint);
        //        //}
        //    });
        //}

        //This is helper function for User Create. 
        //This Function get first string from user And If User Disconnect In 30 Second  
        //public void CreateUser(TcpClient client)
        //{
        //    var stream = client.GetStream();
        //    br = new BinaryReader(stream);
        //    while (true)
        //    {
        //        try
        //        {
        //            var msg = br.ReadString();
        //            var flag = msg == " " || msg == null || msg == "";
        //            if (!flag)
        //            {
        //                //Test
        //                MessageBox.Show($"|Test| Flag: {flag}");

        //                // your code
        //                User user = new User();
        //                user.UserName = msg;
        //                user.RemoteEndPoint = client.Client.RemoteEndPoint.ToString();
        //                user.IsConnected = true;

        //                //UserUC userUC = new UserUC();
        //                //userUC.user = user;
        //                //Users.Add(userUC);
        //                //Old
        //                Users.Add(user);
        //                client.Dispose();
        //                return;
        //            }

        //        }
        //        catch (Exception)
        //        {

        //            //This is for UI WPF TEST
        //            //MessageBox.Show($"{client.Client.RemoteEndPoint} disconnected\n{ex.Message}");
        //            UserDisconnected(client);
        //            return;
        //            //This is For Console TEST
        //            //Console.WriteLine($"{item.Client.RemoteEndPoint}  disconnected");
        //        }
        //    }
        //}
        #endregion



        #endregion

        #region Helper Functions

        public UserClient GetUserByRemoteEndPoint(string endPoint)
        {
            foreach (var user in Users)
            {
                if (user.RemoteEndPoint == endPoint)
                {
                    return user;
                }
            }
            return null;
        }

        public TcpClient GetClientByEndPoint(string endPoint)
        {

            foreach (var client in Clients)
            {
                if (client.Client.RemoteEndPoint.ToString() == endPoint)
                {
                    return client;
                }
            }
            return null;
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
        #endregion


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


        private void MainListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            foreach (var item in MainListBox.Items)
            {

                var text = item as TextBlock;
                text.Background = Brushes.Red;

            }
            //if (currentUser.IsConnected)
            //{

            //    ChatWindow chatWindow = new ChatWindow();
            //    chatWindow.User = CurrentUser;
            //    chatWindow.Client = GetClientByEndPoint(CurrentUser.RemoteEndPoint);
            //    chatWindow.Show();

            //}

        }

    }
}
