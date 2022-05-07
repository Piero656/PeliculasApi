using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PeliculasApi.Controllers;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;
using PeliculasApi.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApi.Test.PruebasUnitarias
{
    [TestClass]
    public class ActoresControllerTest : BasePruebas
    {

        [TestMethod]
        public async Task ObtenerPersonasPaginadas()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            contexto.Actores.Add(new Actor() { Nombre = "Actor 1" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 2" });
            contexto.Actores.Add(new Actor() { Nombre = "Actor 3" });

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new ActoresController(mapper,contexto2,null);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina1 = await controller.Get(new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 2 });

            var actoresPagina1 = pagina1.Value;

            Assert.AreEqual(2, actoresPagina1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina2 = await controller.Get(new PaginacionDTO() { Pagina = 2, CantidadRegistrosPorPagina = 2 });
            var actoresPagina2 = pagina2.Value;
            Assert.AreEqual(1, actoresPagina2.Count);


            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina3 = await controller.Get(new PaginacionDTO() { Pagina = 3, CantidadRegistrosPorPagina = 2 });
            var actoresPagina3 = pagina3.Value;
            Assert.AreEqual(0, actoresPagina3.Count);


        }


        [TestMethod]
        public async Task CrearActorSinFoto()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var actor = new ActorCreacionDTO() { Nombre = "Felipe", FechaNacimiento = DateTime.Now };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(null, null, null, null))
                .Returns(Task.FromResult("url"));

            var controller = new ActoresController(mapper, contexto, null);

            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);

            var contexto2 = ConstruirContext(nombreDB);
            var listado = await contexto2.Actores.ToListAsync();

            Assert.AreEqual(1, listado.Count);
            Assert.IsNull(listado[0].Foto);

            Assert.AreEqual(0, mock.Invocations.Count);



        }

        [TestMethod]
        public async Task CrearActorConFoto()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var content = Encoding.UTF8.GetBytes("Imagen de prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");

            archivo.Headers = new HeaderDictionary();

            var actor = new ActorCreacionDTO() { Nombre = "Felipe", FechaNacimiento = DateTime.Now, Foto = archivo};

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(content, ".jpg", "actores", archivo.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new ActoresController(mapper, contexto, mock.Object);

            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.AreEqual(201, resultado.StatusCode);


            var contexto2 = ConstruirContext(nombreDB);
            var listado = await contexto2.Actores.ToListAsync();

            Assert.AreEqual(1, listado.Count);
            Assert.AreEqual("url", listado[0].Foto);
            Assert.AreEqual(1, mock.Invocations.Count);



        }


        [TestMethod]
        public async Task PatchRetorna404siActorNoExiste()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var controller = new ActoresController(mapper, contexto, null);

            var PatchDoc = new JsonPatchDocument<ActorPatchDTO>();

            var respuesta = await controller.Patch(1, PatchDoc);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);


        }


        [TestMethod]
        public async Task PatchActualizaUnSoloCampo()
        {

            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurationAutoMapper();

            var fechaNacimiento = DateTime.Now;

            var actor = new Actor() { Nombre = "Felipe", FechaNacimiento = fechaNacimiento };

            contexto.Actores.Add(actor);

            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new ActoresController(mapper, contexto2, null);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
               It.IsAny<ValidationStateDictionary>(),
               It.IsAny<string>(),
               It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;

            var PatchDoc = new JsonPatchDocument<ActorPatchDTO>();
            PatchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/nombre", null, "Claudia"));

            var respuesta = await controller.Patch(1, PatchDoc);

            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var actorDB = await contexto3.Actores.FirstAsync();
            Assert.AreEqual("Claudia", actorDB.Nombre);
            Assert.AreEqual(fechaNacimiento, actorDB.FechaNacimiento);


        }





    }
}
