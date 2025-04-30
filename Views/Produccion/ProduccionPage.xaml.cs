using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;


namespace AlfinfData.Views.Produccion
{
    public partial class ProduccionPage : ContentPage
    {
        public ObservableCollection<Jornalero> Jornaleros { get; set; }

        public ProduccionPage()
        {
            InitializeComponent();

            // Inicializar datos de ejemplo (todos con cajas = 0)
            Jornaleros = new ObservableCollection<Jornalero>
            {
                new Jornalero { IdJornalero = 1, Nombre = "Juan Pérez", Cajas = 0, },
                new Jornalero { IdJornalero = 2, Nombre = "María García", Cajas = 0,},
                new Jornalero { IdJornalero = 3, Nombre = "Carlos López", Cajas = 0, },
                new Jornalero { IdJornalero = 4, Nombre = "Ana Martínez", Cajas = 0,},
                new Jornalero { IdJornalero = 5, Nombre = "Pedro Sánchez", Cajas = 0,}
            };

            ListaDeJornaleros.ItemsSource = Jornaleros;

            // Asignar eventos a los botones
            Btn1.Clicked += (s, e) => AddBoxes(1);
            Btn2.Clicked += (s, e) => AddBoxes(2);
            Btn3.Clicked += (s, e) => AddBoxes(3);
            Btn4.Clicked += (s, e) => AddBoxes(4);
            Btn5.Clicked += (s, e) => AddBoxes(5);
            BtnN.Clicked += OnBtnNClicked;
        }

        private void AddBoxes(int numberOfBoxes)
        {
            var seleccionados = ListaDeJornaleros.SelectedItems.Cast<Jornalero>().ToList();

            if (seleccionados.Any())
            {
                foreach (var jornalero in seleccionados)
                {
                    jornalero.Cajas += numberOfBoxes;
                }

                DisplayAlert("Éxito", $"Añadidas {numberOfBoxes} cajas a {seleccionados.Count} jornalero(s)", "OK");
            }
            else
            {
                DisplayAlert("Error", "Selecciona al menos un jornalero", "OK");
            }
        }


        private async void OnBtnNClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync(
                "Número de cajas",
                "Introduce el número de cajas:",
                "Aceptar",
                "Cancelar",
                "0",
                maxLength: 2,
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int numberOfBoxes))
            {
                AddBoxes(numberOfBoxes);
            }
        }
    }

    public class Jornalero : INotifyPropertyChanged
    {
        private int _cajas;

        public int IdJornalero { get; set; }
        public string Nombre { get; set; }

        public int Cajas
        {
            get => _cajas;
            set
            {
                if (_cajas != value)
                {
                    _cajas = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool TarjetaNFC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
