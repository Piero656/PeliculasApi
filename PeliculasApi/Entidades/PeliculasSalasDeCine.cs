namespace PeliculasApi.Entidades
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaId { get; set; }
        public int SalaDeCineId { get; set; }
        public SalaDeCine SalaDeCine { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
