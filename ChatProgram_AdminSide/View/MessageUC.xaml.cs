using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MessageUC.xaml
    /// </summary>
    public partial class MessageUC : UserControl
    {
        public Message message { get; set; }

        public string BackGroundColor { get; set; }

        public string ShortTime { get; set; }

        public string UsernameColor { get; set; } = "Black";
        public MessageUC()
        {
            InitializeComponent();
            this.DataContext = this;
            //this.message = message;
            //BackGroundColor = backGroundColor;

        }
    }
}
