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
    public class CompaniasTelefonicaControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(CompaniasTelefonicaController controller, object model)
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
            var controller = new CompaniasTelefonicaController();
            var companiaTelefonica = new CompaniaTelefonica { Descripcion = "" };
            controller.ModelState.AddModelError("Descripcion", "El campo Descripción es obligatorio");

            // Act
            var result = controller.Create(companiaTelefonica);

            // Assert: no debe redirigir a Index
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "Con ModelState inválido no debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "Con ModelState inválido no debe emitir una redirección.");

            // Assert: debe devolver el resultado JSON con los errores de validación
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo Descripción es obligatorio");
        }

        [TestMethod]
        public void Create_DescripcionVacia_DevuelveJsonConErroresYNoRedirige()
        {
            // Arrange
            var controller = new CompaniasTelefonicaController();
            var companiaTelefonica = new CompaniaTelefonica { Descripcion = "" };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, companiaTelefonica);
            var result = controller.Create(companiaTelefonica);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index con datos inválidos.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo Descripción es obligatorio");
        }

        [TestMethod]
        public void Create_DescripcionExcedeLongitudMaxima_DevuelveJsonConError()
        {
            // Arrange
            var controller = new CompaniasTelefonicaController();
            var companiaTelefonica = new CompaniaTelefonica { Descripcion = new string('A', 81) };

            // Act: la descripción supera los 80 caracteres permitidos
            InvalidarModelStateConDataAnnotations(controller, companiaTelefonica);
            var result = controller.Create(companiaTelefonica);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no debe exceder los 80 caracteres");
        }

        [TestMethod]
        public void Create_DescripcionDuplicada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que ya existe una compañía telefónica con la
            // misma descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeCompaniasTelefonicaService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new CompaniasTelefonicaController(service);

            // Act: descripción en minúsculas y con acento
            var companiaTelefonica = new CompaniaTelefonica { Descripcion = "movistár" };
            var result = controller.Create(companiaTelefonica);

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
            var service = new FakeCompaniasTelefonicaService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new CompaniasTelefonicaController(service);

            // Act: descripción en mayúsculas y con acento
            var companiaTelefonica = new CompaniaTelefonica { Id = 2, Descripcion = "MOVISTÁR" };
            var result = controller.Edit(companiaTelefonica);

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
            var controller = new CompaniasTelefonicaController();
            var companiaTelefonica = new CompaniaTelefonica { Descripcion = "" };
            controller.ModelState.AddModelError("Descripcion", "El campo Descripción es obligatorio");

            // Act
            var result = controller.Edit(companiaTelefonica);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo Descripción es obligatorio");
        }

        [TestMethod]
        public void Delete_CompaniaTelefonicaEnUsoPorDetallesTelefono_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la compañía telefónica está siendo usada por detalles de teléfono
            var service = new FakeCompaniasTelefonicaService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");
            var controller = new CompaniasTelefonicaController(service);

            // Act
            var result = controller.Delete(new CompaniaTelefonica { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no se puede eliminar");
            StringAssert.Contains(requestResult.Message, "usado por otros registros");
        }
    }
}
