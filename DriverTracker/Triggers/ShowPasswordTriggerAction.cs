using System.ComponentModel;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;

namespace DriverTracker.triggers;

public class ShowPasswordTriggerAction:TriggerAction<ImageButton>, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string HideIcon { get; set; }
    public string ShowIcon { get; set; }
    
    public bool _hidePassword = true;

    public bool HidePassword
    {
        get => _hidePassword;
        set
        {
            if (_hidePassword != value)
            {
                _hidePassword = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HidePassword)));
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected override void Invoke(ImageButton sender)
    {
        sender.Source = HidePassword? ShowIcon : HideIcon;
        HidePassword = !HidePassword;
    }
   
}