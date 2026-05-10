using AutoMapper;
using ModelosDB.General;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WA_StoreControl.DTO;

namespace WA_StoreControl.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Categoria, CategoriaDTO>();
            CreateMap<TipoIdentificacion, TipoIdentificacionDTO>();

            CreateMap<Persona, PersonaDTO>()
                .ForMember(d => d.FechaNacimiento, d => d.MapFrom(s => s.FechaNacimiento.ToString("dd/MM/yyyy")))
                .ForMember(d => d.DetallesTelefono, d => d.MapFrom(s => s.DetallesTelefono))
                .ForMember(d => d.Identidades, d => d.MapFrom(s => s.Identidades))
                .ForMember(d => d.Identificacion, d => d.MapFrom(s => s.Identidades.Count > 0
                                                                     ? string.Join(" || ", s.Identidades.Select(x => x.Identificacion))
                                                                     : "Sin identificaciones"));


            CreateMap<SubCategoria, SubCategoriaDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.Categoria.Descripcion));

            CreateMap<Producto, ProductoDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.SubCategoria.Categoria.Descripcion))
                .ForMember(d => d.DescripcionProducto, d => d.MapFrom(s => string.Concat(s.Descripcion.Trim(), s.MarcaId != 1 ? (" - " + s.Marca.Descripcion) : "")))
                .ForMember(d => d.DescripcionMarca, d => d.MapFrom(s => s.Marca.Descripcion))
                .ForMember(d => d.DescripcionSubCategoria, d => d.MapFrom(s => s.SubCategoria.Descripcion));

            CreateMap<CompaniaTelefonica, CompaniaTelefonicaDTO>();
            CreateMap<Identidad, IdentidadDTO>()
                .ForMember(d => d.DescripcionTipoIdentificacion, d => d.MapFrom(s => s.TipoIdentificacion.Descripcion));

            CreateMap<DetalleTelefono, DetalleTelefonoDTO>()
                .ForMember(d => d.DescripcionCompania, d => d.MapFrom(s => s.CompaniaTelefonica.Descripcion));

            CreateMap<Marca, MarcaDTO>();

            CreateMap<Entrada, EntradaDTO>()
                .ForMember(d => d.FechaEntrada, d => d.MapFrom(s => s.FechaEntrada.ToString("dd/MM/yyyy")))
                .ForMember(d => d.FechaEntrada, d => d.MapFrom(s => s.FechaEntrada.ToString("dd/MM/yyyy")))
                .ForMember(d => d.NombreProveedor, d => d.MapFrom(s =>
                        string.Concat(s.Proveedor.NombreComercial.Trim(),
                                        s.Proveedor.Identidades.Count > 0
                                            ? (" || " + s.Proveedor.Identidades.FirstOrDefault().Identificacion)
                                            : "|| Sin identificaciones")))
                .ForMember(d => d.DetallesEntrada, d => d.MapFrom(s => s.DetallesEntrada));

            CreateMap<DetalleEntrada, DetalleEntradaDTO>()
                .ForMember(d => d.DescripcionProducto, o => o.MapFrom(s => s.Producto.Descripcion));
        }
    }
}