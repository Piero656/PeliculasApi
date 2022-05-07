using PeliculasApi.Entidades;

namespace PeliculasApi.DTOs
{
    public class PeliculaDetallesDTO : PeliculaDTO
    {
        public List<Genero> Generos{ get; set; }
        public List<ActorPeliculaDetalleDTO> Actores { get; set; }

    }
}
