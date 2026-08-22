using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WA_StoreControl.Controllers;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Tests
{
    [TestClass]
    public class EntradasControllerTests
    {
        private static void InvalidarModelStateConDataAnnotations(EntradasController controller, object model)
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

        private static Entrada CrearEntradaValida()
        {
            return new Entrada
            {
                Codigo = "ENT001000000001",
                FechaEntrada = DateTime.Now.Date,
                ProveedorId = 1,
                TotalEntrada = 100
            };
        }

        // ------------------------------------------------------------------
        // CREAR
        // ------------------------------------------------------------------

        [TestMethod]
        public void Create_ModelStateInvalido_DevuelveJsonConErroresYNoRedirigeAIndex()
        {
            // Arrange
            var controller = new EntradasController();
            var entrada = new Entrada();
            controller.ModelState.AddModelError("Codigo", "El campo 'Código' es obligatorio");
            controller.ModelState.AddModelError("FechaEntrada", "El campo 'Fecha de entrada' es obligatorio");

            // Act
            var result = controller.Create(entrada);

            // Assert: no debe redirigir a Index
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "Con ModelState inválido no debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "Con ModelState inválido no debe emitir una redirección.");

            // Assert: debe devolver el resultado JSON con los errores de validación
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "El campo 'Código' es obligatorio");
            StringAssert.Contains(requestResult.Message, "El campo 'Fecha de entrada' es obligatorio");
        }

        [TestMethod]
        public void Create_CodigoConLongitudIncorrecta_DevuelveJsonConErrorYNoRedirige()
        {
            // Arrange: el código debe tener una longitud exacta de 15 caracteres
            var controller = new EntradasController();
            var entrada = new Entrada
            {
                Codigo = "ENT00100000000",
                FechaEntrada = DateTime.Now.Date,
                ProveedorId = 1,
                TotalEntrada = 100
            };

            // Act: ejecuta la validación por DataAnnotations y luego el action
            InvalidarModelStateConDataAnnotations(controller, entrada);
            var result = controller.Create(entrada);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index con datos inválidos.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "La longitud debe ser de 15 dígitos");
        }

        [TestMethod]
        public void Create_SinProveedor_DevuelveJsonConErrorYNoRedirige()
        {
            // Arrange: el servicio exige un proveedor para la entrada
            var controller = new EntradasController();
            var entrada = CrearEntradaValida();
            entrada.ProveedorId = 0;

            // Act
            var result = controller.Create(entrada);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");

            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ingrese un proveedor para la entrada");
        }

        [TestMethod]
        public void Create_DetalleConProductoInvalido_DevuelveJsonConError()
        {
            // Arrange: el servicio rechaza detalles con ProductoId menor o igual a cero
            var controller = new EntradasController();
            var entrada = CrearEntradaValida();
            entrada.DetallesEntrada.Add(new DetalleEntrada { ProductoId = 0, Cantidad = 5, Precio = 10 });

            // Act
            var result = controller.Create(entrada);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Existen detalles de productos inválidos");
        }

        [TestMethod]
        public void Create_DetalleConCantidadInvalida_DevuelveJsonConError()
        {
            // Arrange: el servicio rechaza detalles con cantidad menor o igual a cero
            var controller = new EntradasController();
            var entrada = CrearEntradaValida();
            entrada.DetallesEntrada.Add(new DetalleEntrada { ProductoId = 1, Cantidad = 0, Precio = 10 });

            // Act
            var result = controller.Create(entrada);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Existen detalles de productos inválidos");
        }

        [TestMethod]
        public void Create_DetalleConPrecioInvalido_DevuelveJsonConError()
        {
            // Arrange: el servicio rechaza detalles con precio menor o igual a cero
            var controller = new EntradasController();
            var entrada = CrearEntradaValida();
            entrada.DetallesEntrada.Add(new DetalleEntrada { ProductoId = 1, Cantidad = 5, Precio = 0 });

            // Act
            var result = controller.Create(entrada);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Existen detalles de productos inválidos");
        }

        [TestMethod]
        public void Create_DetallesNulos_DevuelveJsonConErrorDeProveedor()
        {
            // Arrange: una entrada sin detalles y sin proveedor es rechazada antes de procesar detalles
            var controller = new EntradasController();
            var entrada = new Entrada
            {
                Codigo = "ENT001000000001",
                FechaEntrada = DateTime.Now.Date,
                ProveedorId = 0,
                TotalEntrada = 100,
                DetallesEntrada = new HashSet<DetalleEntrada>()
            };

            // Act
            var result = controller.Create(entrada);

            // Assert
            var requestResult = ObtenerRequestResult(result);
            Assert.IsFalse(requestResult.Success, "La operación debe reportarse como fallida.");
            StringAssert.Contains(requestResult.Message, "Ingrese un proveedor para la entrada");
        }

        // ------------------------------------------------------------------
        // CLONAR (CrearEntrada / ObtenerParaCrearOClonar)
        // ------------------------------------------------------------------

        [TestMethod]
        public void CrearEntrada_SinId_RetornaVistaConEntradaNuevaYNoRedirige()
        {
            // Arrange: sin identificación se prepara una nueva entrada en lugar de clonar
            var controller = new EntradasController();

            // Act
            var result = controller.CrearEntrada(0);

            // Assert: no debe emitir una redirección
            Assert.IsNotInstanceOfType(result, typeof(RedirectToRouteResult), "No debe redirigir a Index.");
            Assert.IsNotInstanceOfType(result, typeof(RedirectResult), "No debe emitir una redirección.");

            // Assert: debe devolver la vista con una entrada nueva sin identificar
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Se esperaba un ViewResult para la pantalla de crear/clonar entradas.");

            var viewModel = viewResult.Model as IndexCrearEntradasVM;
            Assert.IsNotNull(viewModel, "El modelo de la vista debe ser un IndexCrearEntradasVM.");
            Assert.IsNotNull(viewModel.Entrada, "La vista debe recibir una entrada para trabajar.");
            Assert.AreEqual(0, viewModel.Entrada.Id, "Una nueva entrada no debe tener Id asignado.");
        }

        [TestMethod]
        public void CrearEntrada_ConIdInvalido_RetornaVistaConEntradaNueva()
        {
            // Arrange: identificadores negativos se tratan como creación de una entrada nueva
            var controller = new EntradasController();

            // Act
            var result = controller.CrearEntrada(-5);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Se esperaba un ViewResult para la pantalla de crear/clonar entradas.");

            var viewModel = viewResult.Model as IndexCrearEntradasVM;
            Assert.IsNotNull(viewModel, "El modelo de la vista debe ser un IndexCrearEntradasVM.");
            Assert.AreEqual(0, viewModel.Entrada.Id, "Con un id inválido no debe clonarse ningún registro.");
        }
    }
}
