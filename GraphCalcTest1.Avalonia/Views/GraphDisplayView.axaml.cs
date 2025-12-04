using Avalonia.Controls;
using GraphCalcTest1.Avalonia.ViewModels;

namespace GraphCalcTest1.Avalonia.Views
{
    public partial class GraphDisplayView : Window
    {
        public GraphDisplayView()
        {
            InitializeComponent();
            DataContext = new GraphDisplayViewModel();
        }
    }
}
