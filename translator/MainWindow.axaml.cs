using Avalonia.Controls;
using translator.ViewModels;

namespace translator;

/// <summary>
/// Main window of the application.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Set the data context to the view model
        DataContext = new MainWindowViewModel();
    }
}