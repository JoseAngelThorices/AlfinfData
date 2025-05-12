using CommunityToolkit.Mvvm.ComponentModel;

namespace AlfinfData.Models.SQLITE
{
    public partial class JornaleroConCajas : ObservableObject
    {
        public int IdJornalero { get; set; }
        public string Nombre { get; set; }
        public int IdCuadrilla { get; set; }

        [ObservableProperty]
        private int totalCajas;
    }
}
