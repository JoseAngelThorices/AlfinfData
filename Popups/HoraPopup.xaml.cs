using CommunityToolkit.Maui.Views;

namespace AlfinfData.Popups;

public partial class HoraPopup : Popup
{
    public TimeSpan HoraSeleccionada { get; private set; }

    public HoraPopup()
    {
        InitializeComponent();
    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        HoraSeleccionada = HoraPicker.Time;
        Close(HoraSeleccionada); // Devuelve la hora al cerrar el popup
    }
}
