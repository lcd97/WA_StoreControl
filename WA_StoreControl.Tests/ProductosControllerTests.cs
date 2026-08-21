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
    public class ProductosControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(ProductosController controller, object model)
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
            var controller = new ProductosController();
            var producto = new Producto { Codigo = "", Descripcion = "" };
            controller.ModelState.AddModelError("Codigo", "El campo 'Código' es obligatorio");
            controller.ModelState.AddModelError("Descripcion", "El campo 'Descripción' es obligatorio");

            // Act
            var result = controller.Create(producto);

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
            var controller = new ProductosController();
            var producto = new Producto { Codigo = "", Descripcion = "" };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, producto);
            var result = controller.Create(producto);

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
            var controller = new ProductosController();
            var producto = new Producto { Codigo = "123", Descripcion = "Descripción válida" };

            // Act: el código no cumple la longitud exacta de 6 caracteres
            InvalidarModelStateConDataAnnotations(controller, producto);
            var result = controller.Create(producto);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud debe ser de 3 caracteres");
        }

        [TestMethod]
        public void Create_StockNegativo_DevuelveJsonConError()
        {
            // Arrange
            var controller = new ProductosController();
            var producto = new Producto { Codigo = "AAA111", Descripcion = "Descripción válida", Stock = -1 };

            // Act: el stock no puede ser negativo según la validación del modelo
            InvalidarModelStateConDataAnnotations(controller, producto);
            var result = controller.Create(producto);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo Stock debe ser mayor a 0");
        }

        [TestMethod]
        public void Create_MismaSubCategoriaMarcaYDescripcion_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que ya existe un producto con la misma
            // subcategoría, marca y descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeProductosService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Ya existe un producto con la misma descripción, marca y subcategoría. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act: descripción en minúsculas y con acento
            var producto = new Producto { Codigo = "AAA111", Descripcion = "tornillós", SubCategoriaId = 1, MarcaId = 1 };
            var result = controller.Create(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe un producto con la misma descripción");
            StringAssert.Contains(requestResult.Message, "subcategoría");
        }

        [TestMethod]
        public void Create_ProductoNoInventariableConStock_DevuelveJsonConError()
        {
            // Arrange: un producto no inventariable no debe registrar stock, siempre debe ser 0
            var service = new FakeProductosService(
                validateBeforeCreateResult: $"{SystemMessage.ValidateOperationError} : Un producto no inventariable no debe tener stock. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act: producto no inventariable con stock mayor a 0
            var producto = new Producto { Codigo = "AAA111", Descripcion = "Servicio de instalación", EsInventariable = false, Stock = 10 };
            var result = controller.Create(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no inventariable");
            StringAssert.Contains(requestResult.Message, "no debe tener stock");
        }

        [TestMethod]
        public void Edit_CodigoODescripcionExistenteEnOtroRegistro_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que el código o la descripción ya existen en otro registro
            var service = new FakeProductosService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act
            var producto = new Producto { Id = 2, Codigo = "AAA111", Descripcion = "Tornillos", SubCategoriaId = 1, MarcaId = 1 };
            var result = controller.Edit(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe un código igual");
        }

        [TestMethod]
        public void Edit_MismaSubCategoriaMarcaYDescripcion_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que otro registro tiene la misma subcategoría, marca
            // y descripción, sin importar mayúsculas/minúsculas ni acentos
            var service = new FakeProductosService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Ya existe un producto con la misma descripción, marca y subcategoría. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act: descripción en mayúsculas y con acento
            var producto = new Producto { Id = 3, Codigo = "CCC333", Descripcion = "TORNILLÓS", SubCategoriaId = 1, MarcaId = 1 };
            var result = controller.Edit(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ya existe un producto con la misma descripción");
            StringAssert.Contains(requestResult.Message, "subcategoría");
        }

        [TestMethod]
        public void Edit_StockNoModificable_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que se intenta modificar el stock al editar
            var service = new FakeProductosService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : El stock no se puede modificar al editar un producto. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act
            var producto = new Producto { Id = 2, Codigo = "AAA111", Descripcion = "Tornillos", SubCategoriaId = 1, MarcaId = 1, Stock = 25 };
            var result = controller.Edit(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El stock no se puede modificar");
        }

        [TestMethod]
        public void Edit_ProductoNoInventariableConStock_DevuelveJsonConError()
        {
            // Arrange: un producto no inventariable no debe registrar stock, siempre debe ser 0
            var service = new FakeProductosService(
                validateBeforeUpdateResult: $"{SystemMessage.ValidateOperationError} : Un producto no inventariable no debe tener stock. Modifique y vuelva a intentar");
            var controller = new ProductosController(service);

            // Act: producto no inventariable con stock mayor a 0
            var producto = new Producto { Id = 2, Codigo = "AAA111", Descripcion = "Servicio de instalación", SubCategoriaId = 1, MarcaId = 1, EsInventariable = false, Stock = 10 };
            var result = controller.Edit(producto);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no inventariable");
            StringAssert.Contains(requestResult.Message, "no debe tener stock");
        }

        [TestMethod]
        public void Delete_ProductoEnUsoPorDetallesEntrada_DevuelveJsonConError()
        {
            // Arrange: el servicio detecta que el producto está siendo usado por detalles de entrada
            var service = new FakeProductosService(
                validateBeforeDeleteResult: $"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");
            var controller = new ProductosController(service);

            // Act
            var result = controller.Delete(new Producto { Id = 1 });

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "no se puede eliminar");
            StringAssert.Contains(requestResult.Message, "usado por otros registros");
        }
    }
}
