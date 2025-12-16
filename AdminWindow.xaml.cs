using System.Windows;
using FurnitureShop.ViewModels;

namespace FurnitureShop
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            DataContext = new AdminViewModel();
        }
    }
}
