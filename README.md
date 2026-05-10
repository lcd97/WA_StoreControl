# WA_StoreControl

Sistema web de gestión de inventario desarrollado con ASP.NET MVC y Knockout.js, diseñado para administrar productos, movimientos 
y futuras operaciones de ventas mediante una arquitectura escalable basada en MVC + MVVM.

---

## Objetivo del proyecto

Construir una solución modular y mantenible para la gestión de inventario, aplicando buenas prácticas de desarrollo web en .NET Framework
y patrones de arquitectura como MVC y MVVM.

---


## Demo

🔗 http://accemas.runasp.net/

> Nota: El sistema se encuentra en desarrollo.
> Actualmente no cuenta con autenticación (login).

---

## ⚙️ Tecnologías utilizadas

* **Backend:** C# + ASP.NET MVC (.NET Framework 4.8.1)
* **ORM:** Entity Framework 6
* **Frontend:**
  * Knockout.js (patrón MVVM)
  * jQuery
  * Bootstrap v5
* **Base de datos:** Microsoft SQL Server
* **Control de versiones:** Git + GitHub

---

## Arquitectura

El sistema utiliza una combinación de:

* **MVC (Model-View-Controller)** en el servidor
* **MVVM (Model-View-ViewModel)** en el cliente mediante Knockout.js

Esto permite separar la lógica de presentación de la lógica de negocio, manteniendo una estructura organizada y escalable.

---

## Modelo de datos

El sistema está estructurado en torno a entidades principales de inventario y datos generales:

### Inventario

* Categorías
* Subcategorías
* Productos
* Marcas
* Entradas
* Detalle de entrada

### General

* Personas
* Tipos de identificación
* Compañías telefónicas
* Detalle de teléfonos
* Identidades

---

## Funcionalidades actuales

* Gestión de categorías
* Gestión de subcategorías
* Gestión de productos
* Gestión de marcas
* Gestión de proveedores
* Gestión de tipos de identificación
* Gestión de compañías telefónicas
* GEstión de teléfonos
* Gestión de Identidades
* Estructura base para entradas de inventario
---

## Características técnicas

- Arquitectura MVC + MVVM
- Entity Framework 6 como ORM
- Separación de capas
- Formularios dinámicos con Knockout.js
- Persistencia relacional con SQL Server
- Diseño preparado para escalabilidad

---

## Estado del proyecto

* Catálogos principales del módulo de inventario finalizados
* Estructura base para movimientos de inventario
* Modelo de datos relacional implementado
* Arquitectura MVC + MVVM integrada

Actualmente se completó la primera fase del módulo de inventario, enfocada en la construcción de catálogos
y estructura de entidades para soportar futuras operaciones de stock, ventas y arqueo.

---

## Instalación local

1. Clonar el repositorio:
  ```bash id="clonex1"
  git clone https://github.com/lcd97/WA_StoreControl.git
  ```
2. Abrir el proyecto en Visual Studio
3. Configurar la cadena de conexión en `Web.config`
4. Ejecutar migraciones (si aplica)
5. Ejecutar el proyecto (IIS Express)
---

## Configuración

* Configurar conexión a SQL Server en `Web.config`
* No incluir credenciales reales en el repositorio
* Se recomienda usar archivos de configuración separados para entorno local

---

## Decisiones de diseño

* Se implementaron catálogos base como fundamento del sistema de inventario
* Se utilizó Entity Framework para facilitar el mapeo objeto-relacional
* Se aplicó MVVM en el frontend para mejorar la separación de responsabilidades
* Se estructuró el modelo pensando en la futura implementación de movimientos de inventario

---

## Estructura del proyecto

```plaintext
Controllers/
DTO/
Models/
Views/
Scripts/
Services/
Content/
Scripts/
ViewModels/
```

---

## Autora

**Daniela Cordero Leiva**
Desarrolladora Web


![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-purple)
![ASP.NET MVC](https://img.shields.io/badge/ASP.NET-MVC-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-red)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-purple)
![Status](https://img.shields.io/badge/status-in%20development-yellow)



