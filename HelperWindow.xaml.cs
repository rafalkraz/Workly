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
using WinUIEx;
using Workly.Interfaces;
using Workly.Structure;

namespace Workly;

public sealed partial class HelperWindow : WinUIEx.WindowEx
{
    public enum Action
    {
        Add,
        Edit
    }
    public HelperWindow(IDataViewPage parentPage, object entry, Action action)
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "Wpis";
        this.SetIcon(@"Assets\Calendar.ico");

            LogPage.editLock = true;
        
            if (action == Action.Add)
            {
                TitleBarTextBlock.Text = "Dodawanie nowego wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, null, this);
            }
            else
            {
                TitleBarTextBlock.Text = "Edycja wpisu";
                ContentFrame.Content = new EntryEditPage(parentPage, entry, this);
            }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        LogPage.editLock = false;
    }
}
