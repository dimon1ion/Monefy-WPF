using Monefy_WPF.Service;
using Monefy_WPF.ViewModel;
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
using System.Windows.Shapes;

namespace Monefy_WPF.View
{
    /// <summary>
    /// Interaction logic for Calculate.xaml
    /// </summary>
    public partial class Calculate : Window
    {
        public Calculate()
        {
            InitializeComponent();
            DataContext = new MonefyViewModel(new JsonFileService());
        }
        public Calculate(MonefyViewModel monefyViewModel)
        {
            InitializeComponent();
            DataContext = monefyViewModel;
        }
    }
}
