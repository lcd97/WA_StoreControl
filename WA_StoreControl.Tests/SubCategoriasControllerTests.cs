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
    public class SubCategoriasControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(SubCategoriasController controller, object model)
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
            var controller = new SubCategoriasController();
            var subCategoria = new SubCategoria { Codigo = "", Descripcion = "" };
            controller.ModelState.AddModelError("Codigo", "El campo 'Código' es obligatorio");
            controller.ModelState.AddModelError("Descripcion", "El campo 'Descripción' es obligatorio");

            // Act
            var result = controller.Create(subCategoria);

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
            var controller = new SubCategoriasController();
            var subCategoria = new SubCategoria { Codigo = "", Descripcion = "" };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, subCategoria);
            var result = controller.Create(subCategoria);

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
            var controller = new SubCategoriasController();
            var subCategoria = new SubCategoria { Codigo = "123", Descripcion = "Descripción válida" };

            // Act: el código no cumple la longitud exacta de 6 caracteres
            InvalidarModelStateConDataAnnotations(controller, subCategoria);
            var result = controller.Create(subCategoria);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud debe ser de 6 caracteres");
        }

        [TestMethod]
        public void Create_MismaCategoriaYDescripcion_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que ya existe una subcategoría con la misma
            // categoría y descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeSubCategoriasService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una subcategoría con la misma descripción en la categoría seleccionada. Modifique y vuelva a intentar");
            var controller = new SubCategoriasController(service);

            // Act: descripción en minúsculas y con acento
            var subCategoria = new SubCategoria { Codigo = "AAA111", Descripcion = "acerós", CategoriaId = 1 };
            var result = controller.Create(subCategoria);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una subcategoría con la misma descripción");
            StringAssert.Contains(requestResult.Message, "categoría seleccionada");
        }

        [TestMethod]
        public void Edit_CodigoODescripcionExistenteEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que el código o la descripción ya existen en otro registro
            var service = new FakeSubCategoriasService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");
            var controller = new SubCategoriasController(service);

            // Act
            var subCategoria = new SubCategoria { Id = 2, Codigo = "AAA111", Descripcion = "Aceros", CategoriaId = 1 };
            var result = controller.Edit(subCategoria);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe un código igual");
        }

        [TestMethod]
        public void Edit_DescripcionExistenteEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la descripción ya existe en otro registro
            var service = new FakeSubCategoriasService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una subcategoría con la misma descripción en la categoría seleccionada. Modifique y vuelva a intentar");
            var controller = new SubCategoriasController(service);

            // Act
            var subCategoria = new SubCategoria { Id = 2, Codigo = "BBB222", Descripcion = "aceros", CategoriaId = 1 };
            var result = controller.Edit(subCategoria);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una subcategoría con la misma descripción");
        }

        [TestMethod]
        public void Edit_MismaCategoriaYDescripcionEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que otro registro tiene la misma categoría y
            // descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeSubCategoriasService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe una subcategoría con la misma descripción en la categoría seleccionada. Modifique y vuelva a intentar");
            var controller = new SubCategoriasController(service);

            // Act: descripción en mayúsculas y con acento
            var subCategoria = new SubCategoria { Id = 3, Codigo = "CCC333", Descripcion = "ACERÓS", CategoriaId = 1 };
            var result = controller.Edit(subCategoria);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe una subcategoría con la misma descripción");
        }

        [TestMethod]
        public void Delete_SubCategoriaEnUsoPorProductos_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que la subcategoría está siendo usada por productos
            var service = new FakeSubCategoriasService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");
            var controller = new SubCategoriasController(service);

            // Act
            var result = controller.Delete(new SubCategoria { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no se puede eliminar");
            StringAssert.Contains(requestResult.Message, "usado por otros registros");
        }
    }

    [TestClass]
    public class PersonaHelperTests
    {
        [TestMethod]
        public void BuscarCoincidencias_IgnoraMayusculasYMinusculas()
        {
            Assert.AreEqual("ACEROS", PersonaHelper.BuscarCoincidencias("aceros"));
            Assert.AreEqual("ACEROS", PersonaHelper.BuscarCoincidencias("Aceros"));
            Assert.AreEqual("ACEROS", PersonaHelper.BuscarCoincidencias("ACEROS"));
        }

        [TestMethod]
        public void BuscarCoincidencias_IgnoraAcentos()
        {
            Assert.AreEqual("ACEROS", PersonaHelper.BuscarCoincidencias("Acerós"));
            Assert.AreEqual("ACEROS", PersonaHelper.BuscarCoincidencias("ácerós"));
        }

        [TestMethod]
        public void BuscarCoincidencias_IgnoraComasYEspaciosExtra()
        {
            Assert.AreEqual("ACEROS INOXIDABLES", PersonaHelper.BuscarCoincidencias("Aceros,   Inoxidables"));
        }
    }
}
