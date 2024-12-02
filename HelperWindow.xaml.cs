using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WorkLog.Structure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkLog
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelperWindow : WinUIEx.WindowEx
    {
        public enum Action
        {
            Add,
            Edit
        }
        public HelperWindow(LogPage parentPage, Entry entry, Action action)
        {
            this.InitializeComponent();
            LogPage.editLock = true;
            
            if (action == Action.Add)
            {
                this.Title = "Dodawanie nowego wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, null, this);
            }
            else
            {
                this.Title = "Edycja wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, entry, this);
            }
            
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            LogPage.editLock = false;
        }
    }
}
