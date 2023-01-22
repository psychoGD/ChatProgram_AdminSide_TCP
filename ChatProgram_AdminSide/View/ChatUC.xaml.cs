using ChatProgram_ClientSide_Wpf.JsonHelper;
using ChatProgram_ClientSide_Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.IO;

namespace ChatProgram_AdminSide
{
    /// <summary>
    /// Interaction logic for ChatUC.xaml
    /// </summary>
    public partial class ChatUC : UserControl, INotifyPropertyChanged
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


        private ObservableCollection<Message> messages;

        public ObservableCollection<Message> Messages
        {
            get { return messages; }
            set { messages = value; OnPropertyChanged(); }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }

        public TcpClient CurrentUser { get; set; }
        public ChatUC()
        {
            InitializeComponent();
            this.DataContext = this;

        }
        public void AddMessageToUI(Message message)
        {
            MessageUC messageUC = new MessageUC();
            string bgColor = "";
            if (!message.FromClient)
            {
                bgColor = "LightBlue";
                messageUC.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                bgColor = "LightGray";
                string usernameColor = "orange";
                messageUC.UsernameColor = usernameColor;
                messageUC.HorizontalAlignment = HorizontalAlignment.Left;
            }
            messageUC.BackGroundColor = bgColor;
            messageUC.ShortTime = message.dateTime.ToShortTimeString();
            messageUC.message = message;
            MainStack.Children.Add(messageUC);
        }

        public Message CreateMessageClass(string text,bool FromClient)
        {
            Message message = new Message();
            message.FromClient = FromClient;
            message.message = text;
            message.dateTime = DateTime.Now;
            return message;
        }
        public void GetNewMessage(Message message)
        {
            var msg = CreateMessageClass(message.message,true);
            AddMessageToUI(msg);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Send Message To User Under Writing  
                var stream = CurrentUser.GetStream();
                var bw = new BinaryWriter(stream);
                bw.Write(text);
                var msg = CreateMessageClass(text,false);
                AddMessageToUI(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Text = string.Empty;


        }
    }
}
