﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [Route("api/salasdecine")]
    [ApiController]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public SalasDeCineController(ApplicationDbContext context, IMapper mapper,
            GeometryFactory geometryFactory): base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
        }


        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine,SalaDeCineDTO>();
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);

        }

        [HttpGet("cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDTO filtro)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));

            var salasDeCine = await context.SalasDeCine.OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, filtro.DistanciaEnKms * 1000))
                .Select(x => new SalaDeCineCercanoDTO
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Latitud = x.Ubicacion.Y,
                    Longitud = x.Ubicacion.X,
                    DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                }).ToListAsync();

            return salasDeCine;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO generoCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(generoCreacionDTO, "obtenerSalaDeCine");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody] SalaDeCineCreacionDTO generoCreacionDTO, int id)
        {
            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(generoCreacionDTO, id);

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

    }
}
