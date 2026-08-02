using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelosDB.Inventario;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WA_StoreControl.Controllers;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.Tests
{
    [TestClass]
    public class MarcasControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(MarcasController controller, object model)
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
            var controller = new MarcasController();
            var marca = new Marca { Codigo = "", Descripcion = "" };
            controller.ModelState.AddModelError("Codigo", "El campo 'Código' es obligatorio");
            controller.ModelState.AddModelError("Descripcion", "El campo 'Descripción' es obligatorio");

            // Act
            var result = controller.Create(marca);

            // Assert: no debe redirigir a Index
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "Con ModelState inválido no debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "Con ModelState inválido no debe emitir una redirección.");

            // Assert: debe devolver el resultado JSON con los errores de validación
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Código' es obligatorio");
            StringAssert.Contains(requestResult.Message, "El campo 'Descripción' es obligatorio");
        }

        [TestMethod]
        public void Create_CodigoODescripcionVacio_DevuelveJsonConErroresYNoRedirige()
        {
            // Arrange
            var controller = new MarcasController();
            var marca = new Marca { Codigo = "", Descripcion = "" };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, marca);
            var result = controller.Create(marca);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index con datos inválidos.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Código' es obligatorio");
            StringAssert.Contains(requestResult.Message, "El campo 'Descripción' es obligatorio");
        }

        [TestMethod]
        public void Create_CodigoConEstructuraIncorrecta_DevuelveJsonConError()
        {
            // Arrange
            var controller = new MarcasController();
            var marca = new Marca { Codigo = "123", Descripcion = "Descripción válida" };

            // Act: el código no cumple la longitud exacta de 6 caracteres
            InvalidarModelStateConDataAnnotations(controller, marca);
            var result = controller.Create(marca);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud debe ser de 6 caracteres");
        }

        [TestMethod]
        public void Create_DescripcionDuplicada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que ya existe una marca con la misma descripción,
            // sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeMarcasService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new MarcasController(service);

            // Act: descripción en minúsculas y con acento
            var marca = new Marca { Codigo = "AAA111", Descripcion = "acerós" };
            var result = controller.Create(marca);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una descripción igual");
        }

        [TestMethod]
        public void Edit_CodigoODescripcionExistenteEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que el código o la descripción ya existen en otro registro
            var service = new FakeMarcasService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");
            var controller = new MarcasController(service);

            // Act
            var marca = new Marca { Id = 2, Codigo = "AAA111", Descripcion = "Aceros" };
            var result = controller.Edit(marca);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe un código igual");
        }

        [TestMethod]
        public void Edit_DescripcionDuplicada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la descripción ya existe en otro registro,
            // sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeMarcasService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");
            var controller = new MarcasController(service);

            // Act: descripción en mayúsculas y con acento
            var marca = new Marca { Id = 2, Codigo = "BBB222", Descripcion = "ACERÓS" };
            var result = controller.Edit(marca);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una descripción igual");
        }

        [TestMethod]
        public void Delete_MarcaPredeterminada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que es un registro predeterminado
            var service = new FakeMarcasService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar debido a que es un registro predeterminado");
            var controller = new MarcasController(service);

            // Act
            var result = controller.Delete(new Marca { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "registro predeterminado");
        }

        [TestMethod]
        public void Delete_MarcaEnUsoPorProductos_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la marca está siendo usada por productos
            var service = new FakeMarcasService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");
            var controller = new MarcasController(service);

            // Act
            var result = controller.Delete(new Marca { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no se puede eliminar");
            StringAssert.Contains(requestResult.Message, "usado por otros registros");
        }
    }
}
