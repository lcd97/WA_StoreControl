using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelosDB.General;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WA_StoreControl.Controllers;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.Tests
{
    [TestClass]
    public class TiposIdentificacionControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(TiposIdentificacionController controller, object model)
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

        [TestMethod]
        public void Create_ModelStateInvalido_DevuelveJsonConErroresYNoRedirigeAIndex()
        {
            // Arrange
            var controller = new TiposIdentificacionController();
            var tipoIdentificacion = new TipoIdentificacion { Descripcion = "" };
            controller.ModelState.AddModelError("Descripcion", "El campo 'Tipo Identificación' es obligatorio");

            // Act
            var result = controller.Create(tipoIdentificacion);

            // Assert: no debe redirigir a Index
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "Con ModelState inválido no debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "Con ModelState inválido no debe emitir una redirección.");

            // Assert: debe devolver el resultado JSON con los errores de validación
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Tipo Identificación' es obligatorio");
        }

        [TestMethod]
        public void Create_DescripcionVacia_DevuelveJsonConErroresYNoRedirige()
        {
            // Arrange
            var controller = new TiposIdentificacionController();
            var tipoIdentificacion = new TipoIdentificacion { Descripcion = "" };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, tipoIdentificacion);
            var result = controller.Create(tipoIdentificacion);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index con datos inválidos.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Tipo Identificación' es obligatorio");
        }

        [TestMethod]
        public void Create_DescripcionExcedeLongitudMaxima_DevuelveJsonConError()
        {
            // Arrange
            var controller = new TiposIdentificacionController();
            var tipoIdentificacion = new TipoIdentificacion { Descripcion = new string('A', 101) };

            // Act: la descripción supera los 100 caracteres permitidos
            InvalidarModelStateConDataAnnotations(controller, tipoIdentificacion);
            var result = controller.Create(tipoIdentificacion);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no debe exceder los 100 caracteres");
        }

        [TestMethod]
        public void Create_DescripcionDuplicada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que ya existe un tipo de identificación con la
            // misma descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeTiposIdentificacionService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new TiposIdentificacionController(service);

            // Act: descripción en minúsculas y con acento
            var tipoIdentificacion = new TipoIdentificacion { Descripcion = "cédula de identidad" };
            var result = controller.Create(tipoIdentificacion);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una descripción igual");
        }

        [TestMethod]
        public void Edit_DescripcionExistenteEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la descripción ya existe en otro registro,
            // sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeTiposIdentificacionService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new TiposIdentificacionController(service);

            // Act: descripción en mayúsculas y con acento
            var tipoIdentificacion = new TipoIdentificacion { Id = 2, Descripcion = "CÉDULA DE IDENTIDAD" };
            var result = controller.Edit(tipoIdentificacion);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una descripción igual");
        }

        [TestMethod]
        public void Edit_ModelStateInvalido_DevuelveJsonConError()
        {
            // Arrange
            var controller = new TiposIdentificacionController();
            var tipoIdentificacion = new TipoIdentificacion { Descripcion = "" };
            controller.ModelState.AddModelError("Descripcion", "El campo 'Tipo Identificación' es obligatorio");

            // Act
            var result = controller.Edit(tipoIdentificacion);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Tipo Identificación' es obligatorio");
        }

        [TestMethod]
        public void Delete_TipoIdentificacionEnUsoPorIdentidades_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que el tipo de identificación está siendo usado por identidades
            var service = new FakeTiposIdentificacionService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");
            var controller = new TiposIdentificacionController(service);

            // Act
            var result = controller.Delete(new TipoIdentificacion { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no se puede eliminar");
            StringAssert.Contains(requestResult.Message, "usado por otros registros");
        }
    }
}
