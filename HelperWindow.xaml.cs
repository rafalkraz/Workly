using System;
using Microsoft.UI.Xaml;
using WinUIEx;
using Workly.Interfaces;
using Workly.Structure;

namespace Workly;

public sealed partial class HelperWindow : WinUIEx.WindowEx
{
    public enum Action
    {
        Add,
        Duplicate,
        Edit
    }

    public HelperWindow(IDataViewPage parentPage, Entry templateEntry, Action action)
    {
        InitializeWindow();
        templateEntry ??= new Entry(0, 0, DateTime.Now.Date, DateTime.Now.Date, "", "", 0);
        SetupContent(parentPage, templateEntry, action);
    }

    public HelperWindow(IDataViewPage parentPage, EntryMileage templateEntry, Action action)
    {
        InitializeWindow();
        templateEntry ??= new EntryMileage(0, 0, DateOnly.Parse(DateTime.Now.Date.ToString("dd.MM.yyyy")), "", "", "", 0, 0);
        SetupContent(parentPage, templateEntry, action);
    }

    private void InitializeWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "Wpis";
        this.SetIcon(@"Assets\Calendar.ico");

        LogPage.editLock = true;
    }

    private void SetupContent(IDataViewPage parentPage, object templateEntry, Action action)
    {
        TitleBarTextBlock.Text = action switch
        {
            Action.Add => "Dodawanie nowego wpisu",
            Action.Duplicate => "Duplikowanie wpisu",
            Action.Edit => "Edycja wpisu",
            _ => throw new NotImplementedException(),
        };

        var mode = action switch
        {
            Action.Add => EntryEditPage.EditMode.Create,
            Action.Duplicate => EntryEditPage.EditMode.Duplicate,
            Action.Edit => EntryEditPage.EditMode.Edit,
            _ => throw new NotImplementedException()
        };

        if (templateEntry is Entry entry)
        {
            ContentFrame.Content = new EntryEditPage(this, parentPage, mode, entry);
        }
        else if (templateEntry is EntryMileage mileage)
        {
            ContentFrame.Content = new EntryEditPage(this, parentPage, mode, mileage);
        }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        LogPage.editLock = false;
    }
}
