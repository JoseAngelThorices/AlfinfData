using CommunityToolkit.Mvvm.ComponentModel;

namespace AlfinfData.Models.SQLITE
{
    public partial class JornaleroConHoras : ObservableObject
    {
        public int IdJornalero { get; set; }
        public int IdCuadrilla { get; set; }
        public string Nombre { get; set; }
        

        [ObservableProperty] private double hn;
        [ObservableProperty] private double he1;
        [ObservableProperty] private double he2;
        [ObservableProperty] private bool falta;
    }
}
