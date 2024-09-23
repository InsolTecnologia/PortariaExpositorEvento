using System.ComponentModel;

namespace PortariaExpositorEvento.ViewModels;

public partial class MainViewModel : INotifyPropertyChanged
{
    private string _iconeInsol = "Plataforma de Marca_easy_tech-01.jpg";

    public string IconeInsol
    {
        get => _iconeInsol;
        set
        {
            _iconeInsol = value;
            OnPropertyChanged(nameof(IconeInsol));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
