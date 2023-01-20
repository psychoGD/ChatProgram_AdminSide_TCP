using System;
using System.Collections.Generic;
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

namespace ChatProgram_AdminSide
{
    /// <summary>
    /// Interaction logic for UserUC.xaml
    /// </summary>
    public partial class UserUC : UserControl, INotifyPropertyChanged
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

        private string backGroundColor;

        public string BackGroundColor
        {
            get { return backGroundColor; }
            set { backGroundColor = value;OnPropertyChanged(); }
        }

        public User user { get; set; }

        public UserUC()
        {
            InitializeComponent();
            BackGroundColor = "Black";
        }
        public void ChangeBackGroundColor(string Color)
        {
            BackGroundColor= Color;
        }
    }
}
