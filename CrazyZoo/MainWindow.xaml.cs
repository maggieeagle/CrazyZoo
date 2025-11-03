using CrazyZoo.Properties;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CrazyZoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown; // listen at keys on window
        }



        // keys for owl wisdoms
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.DataContext is ZooViewModel zvm)
            {
                zvm.FollowKeysPressed(e);
            }
        }
    }
}