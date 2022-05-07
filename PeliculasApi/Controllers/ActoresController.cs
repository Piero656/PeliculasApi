using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;
using PeliculasApi.Servicios;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(IMapper mapper, ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos) : base (context, mapper)
        {
            this.mapper = mapper;
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //var queryable = context.Actores.AsQueryable();

            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);

            //var actores = await queryable.Paginar(paginacionDTO).ToListAsync();

            //var dto = mapper.Map<List<ActorDTO>>(actores);

            //return Ok(dto);

            return await Get<Actor, ActorDTO>(paginacionDTO);


        }

        [HttpGet("{id}", Name = "ObtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            //var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (actor == null)
            //{
            //    return NotFound();
            //}

            //var dto = mapper.Map<ActorDTO>(actor);

            //return Ok(dto);

            return await Get<Actor, ActorDTO>(id);


        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memorySream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memorySream);
                    var contenido = memorySream.ToArray();

                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    var contentType = actorCreacionDTO.Foto.ContentType;

                    actor.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, contentType);

                }
            }

            context.Add(actor);
            await context.SaveChangesAsync();

            var dto = mapper.Map<ActorDTO>(actor);

            return new CreatedAtRouteResult("ObtenerActor", new { id = actor.Id }, dto);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromForm] ActorCreacionDTO actorCreacionDTO, int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memorySream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memorySream);
                    var contenido = memorySream.ToArray();

                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    var contentType = actorCreacionDTO.Foto.ContentType;

                    actor.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actor.Foto, contentType);

                }
            }

            //autordb.Id = id;

            //context.Entry(autordb).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            //if (patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if(entidadDB == null) { return NotFound(); }

            //var entidadDTO = mapper.Map<ActorPatchDTO>(entidadDB);

            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //var esValido = TryValidateModel(entidadDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(entidadDTO, entidadDB);
            
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Patch<Actor,ActorPatchDTO>(id, patchDocument);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var existe = await context.Actores.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Actor() { Id = id });
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Delete<Actor>(id);


        }


    }
}
