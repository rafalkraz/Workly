using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace Workly;
internal class AppSettings : INotifyPropertyChanged
{
    private ApplicationDataContainer localSettings;
    private double _standardSalary;
    private double _overtimeSalary;
    private double _leaveSalary;
    private double _mileageSalary;

    public AppSettings() 
    {
        localSettings = ApplicationData.Current.LocalSettings;
        LoadFromLocalSettings();
    }

    public double StandardSalary
    {
        get => _standardSalary;
        set
        {
            
            if (_standardSalary != value)
            {
                _standardSalary = value;
                OnPropertyChanged();
                SaveToLocalSettings("StandardSalary", value);
            }
        }
    }

    public double OvertimeSalary
    {
        get => _overtimeSalary;
        set
        {
            if (_overtimeSalary != value)
            {
                _overtimeSalary = value;
                OnPropertyChanged();
                SaveToLocalSettings("OvertimeSalary", value);
            }
        }
    }

    public double LeaveSalary
    {
        get => _leaveSalary;
        set
        {
            if (_leaveSalary != value)
            {
                _leaveSalary = value;
                OnPropertyChanged();
                SaveToLocalSettings("LeaveSalary", value);
            }
        }
    }

    public double MileageSalary
    {
        get => _mileageSalary;
        set
        {
            if (_mileageSalary != value)
            {
                _mileageSalary = value;
                OnPropertyChanged();
                SaveToLocalSettings("MileageSalary", value);
            }
        }
    }

    private void LoadFromLocalSettings()
    {
        if (localSettings.Values.ContainsKey("StandardSalary"))
        {
            StandardSalary = Convert.ToDouble(localSettings.Values["StandardSalary"]);
        }
        if (localSettings.Values.ContainsKey("OvertimeSalary"))
        {
            OvertimeSalary = Convert.ToDouble(localSettings.Values["OvertimeSalary"]);
        }
        if (localSettings.Values.ContainsKey("LeaveSalary"))
        {
            LeaveSalary = Convert.ToDouble(localSettings.Values["LeaveSalary"]);
        }
        if (localSettings.Values.ContainsKey("MileageSalary"))
        {
            MileageSalary = Convert.ToDouble(localSettings.Values["MileageSalary"]);
        }
    }

    private void SaveToLocalSettings(string keyName, double value)
    {
        localSettings.Values[keyName] = value;
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
