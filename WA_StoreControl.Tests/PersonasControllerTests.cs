using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WA_StoreControl.Controllers;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.Tests
{
    [TestClass]
    public class PersonasControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(PersonasController controller, object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            foreach (var result in results)
            {
                foreach (var memberName in result.MemberNames)
                    controller.ModelState.AddModelError(memberName, result.ErrorMessage);
            }
        }

        private static RequestResult ObtenerRequestResult(ActionResult result)
        {
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult, "Se esperaba un JsonResult con el resultado de la validación.");

            var requestResult = jsonResult.Data as RequestResult;
            Assert.IsNotNull(requestResult, "El Data del JsonResult debe ser un RequestResult.");

            return requestResult;
        }

        // ------------------------------------------------------------------
        // CREAR
        // ------------------------------------------------------------------

        [TestMethod]
        public void Create_ModelStateInvalido_DevuelveJsonConErroresYNoRedirigeAIndex()
        {
            // Arrange
            var controller = new PersonasController();
            var persona = new Persona { EsPersonaNatural = true };
            controller.ModelState.AddModelError("Nombres", "El campo 'Nombres' es obligatorio");
            controller.ModelState.AddModelError("Apellidos", "El campo 'Apellidos' es obligatorio");

            // Act
            var result = controller.Create(persona);

            // Assert: no debe redirigir a Index
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "Con ModelState inválido no debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "Con ModelState inválido no debe emitir una redirección.");

            // Assert: debe devolver el resultado JSON con los errores de validación
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Nombres' es obligatorio");
            StringAssert.Contains(requestResult.Message, "El campo 'Apellidos' es obligatorio");
        }

        [TestMethod]
        public void Create_NombresExcedenLongitudMaxima_DevuelveJsonConErrorYNoRedirige()
        {
            // Arrange: los nombres no deben exceder los 100 caracteres según DataAnnotations
            var controller = new PersonasController();
            var persona = new Persona
            {
                EsPersonaNatural = true,
                Nombres = new string('A', 101),
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.Date.AddYears(-30)
            };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, persona);
            var result = controller.Create(persona);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index con datos inválidos.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud no debe exceder los 100 caracteres");
        }

        [TestMethod]
        public void Create_IdentidadConLongitudInvalida_DevuelveJsonConError()
        {
            // Arrange: la identificación no debe exceder los 14 caracteres según DataAnnotations
            var controller = new PersonasController();
            var persona = new Persona
            {
                EsPersonaNatural = true,
                Nombres = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.Date.AddYears(-30)
            };
            var identidad = new Identidad
            {
                Identificacion = new string('1', 15),
                TipoIdentificacionId = 1,
                PersonaId = 1
            };

            // Act: ejecuta la validación por DataAnnotations sobre la identidad y luego el action
            InvalidarModelStateConDataAnnotations(controller, identidad);
            var result = controller.Create(persona);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "debe exceder los 14 caracteres");
        }

        [TestMethod]
        public void Create_PersonaNaturalSinNombresOApellidos_DevuelveJsonConError()
        {
            // Arrange: el servicio exige nombres y apellidos para personas naturales
            var controller = new PersonasController();
            var persona = new Persona
            {
                EsPersonaNatural = true,
                Nombres = null,
                Apellidos = null,
                FechaNacimiento = DateTime.Now.Date.AddYears(-30)
            };

            // Act
            var result = controller.Create(persona);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Para personas naturales");
            StringAssert.Contains(requestResult.Message, "los campos de nombres y apellidos son obligatorios");
        }

        [TestMethod]
        public void Create_PersonaNaturalConFechaNacimientoFutura_DevuelveJsonConError()
        {
            // Arrange: la fecha de nacimiento no puede ser posterior a la fecha actual
            var controller = new PersonasController();
            var persona = new Persona
            {
                EsPersonaNatural = true,
                Nombres = "Juan",
                Apellidos = "Pérez",
                FechaNacimiento = DateTime.Now.Date.AddDays(1)
            };

            // Act
            var result = controller.Create(persona);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Para personas naturales");
            StringAssert.Contains(requestResult.Message, "no puede ser una fecha futura");
        }

        [TestMethod]
        public void Create_PersonaComercialSinNombreComercial_DevuelveJsonConError()
        {
            // Arrange: el servicio exige nombre comercial para personas jurídicas
            var controller = new PersonasController();
            var persona = new Persona
            {
                EsPersonaNatural = false,
                NombreComercial = null
            };

            // Act
            var result = controller.Create(persona);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Para personas jurídicas");
            StringAssert.Contains(requestResult.Message, "nombre comercial es obligatorio");
        }

        // ------------------------------------------------------------------
        // EDITAR
        // ------------------------------------------------------------------

        [TestMethod]
        public void Edit_ApellidosExcedenLongitudMaxima_DevuelveJsonConError()
        {
            // Arrange: los apellidos no deben exceder los 100 caracteres según DataAnnotations
            var controller = new PersonasController();
            var persona = new Persona
            {
                Id = 2,
                EsPersonaNatural = true,
                Nombres = "Juan",
                Apellidos = new string('A', 101),
                FechaNacimiento = DateTime.Now.Date.AddYears(-30)
            };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, persona);
            var result = controller.Edit(persona);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud no debe exceder los 100 caracteres");
        }

        // ------------------------------------------------------------------
        // NORMALIZACIÓN DE NOMBRES PARA DETECCIÓN DE COINCIDENCIAS
        // ------------------------------------------------------------------

        [TestClass]
        public class PersonaNormalizacionNombresTests
        {
            [TestMethod]
            public void BuscarCoincidencias_IgnoraMayusculasYMinusculas()
            {
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("juan perez"));
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("Juan Perez"));
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("JUAN PEREZ"));
            }

            [TestMethod]
            public void BuscarCoincidencias_IgnoraAcentos()
            {
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("Juán Pérez"));
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("juàn péréz"));
                Assert.AreEqual("CORPORACION MARIA S.A", PersonaHelper.BuscarCoincidencias("Corporación María S.A"));
            }

            [TestMethod]
            public void BuscarCoincidencias_IgnoraComasYEspaciosExtra()
            {
                Assert.AreEqual("JUAN PEREZ", PersonaHelper.BuscarCoincidencias("Juan,   Pérez"));
            }
        }
    }
}
