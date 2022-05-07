using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController( ApplicationDbContext context,
            IMapper mapper) : base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //var generosDb = await context.Generos.ToListAsync();

            //var generos = mapper.Map<List<GeneroDTO>>(generosDb);

            //return generos;

            return await Get<Genero, GeneroDTO>();

        }

        [HttpGet("{id:int}", Name ="obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            //var generoDb = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            //if (generoDb == null)
            //{
            //    return NotFound();
            //}

            //var genero = mapper.Map<GeneroDTO>(generoDb);

            //return genero;

            return await Get<Genero, GeneroDTO>(id);


        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //var generoDd = mapper.Map<Genero>(generoCreacionDTO);
            //context.Add(generoDd);
            //await context.SaveChangesAsync();
            //var generoDTO = mapper.Map<GeneroDTO>(generoDd);

            //return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);

            return await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoCreacionDTO, "obteneroGenero");
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            //var existe = await context.Generos.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}


            //var generoDd = mapper.Map<Genero>(generoCreacionDTO);
            //generoDd.Id = id;

            ////context.Update(generoDd);

            //context.Entry(generoDd).State = EntityState.Modified;

            //await context.SaveChangesAsync();

            //return NoContent();

            return await Put<GeneroCreacionDTO, Genero>(generoCreacionDTO, id);



        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            //var existe = await context.Generos.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Genero() { Id = id});
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Delete<Genero>(id);


        }



    }
}
