using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasApi.Controllers;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApi.Test.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTest : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            //Preparacion

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Género 2" });

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);

            //Prueba

            var controller = new GenerosController(contexto2,mapper);
            var respuesta = await controller.Get();

            //Verificacion
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);


        }



        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            var resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);

        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Género 2" });

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);


            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get(1);

            var resultado = respuesta.Value;

            Assert.AreEqual(1, resultado.Id);

        }

        [TestMethod]
        public async Task CrearGenero()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();


            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "nuevo Genero" };

            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Post(nuevoGenero);

            var resultado = respuesta as CreatedAtRouteResult;

            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreDB);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1,cantidad);

        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GenerosController(contexto2, mapper);

            var generoCreacionDTO = new GeneroCreacionDTO() { Nombre = "Nuevo Genero" };

            var id = 1;
            var respuesta = await controller.Put(generoCreacionDTO, id);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "Nuevo Genero");

            Assert.IsTrue(existe);

        }


        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Delete(1);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);


        }


        [TestMethod]
        public async Task IntentaBorrarGeneroExistente()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GenerosController(contexto2, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync();

            Assert.IsFalse(existe);

        }
    }
}
