public class RegistroHistorico
{
    public string NombreJornalero { get; set; }
    public DateTime Fecha { get; set; }

    // Datos de Producción
    public int Cajas { get; set; }

    // Datos de Horas
    public double HN { get; set; }
    public double HE1 { get; set; }
    public double HE2 { get; set; }
}
