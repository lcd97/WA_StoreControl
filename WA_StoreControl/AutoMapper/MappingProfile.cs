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
                .ForMember(d => d.Identidades, d => d.MapFrom(s => s.Identidades));

            CreateMap<SubCategoria, SubCategoriaDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.Categoria.Descripcion));

            CreateMap<Producto, ProductoDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.SubCategoria.Categoria.Descripcion))
                .ForMember(d => d.DescripcionSubCategoria, d => d.MapFrom(s => s.SubCategoria.Descripcion));

            CreateMap<CompaniaTelefonica, CompaniaTelefonicaDTO>();
            CreateMap<Identidad, IdentidadDTO>()
                .ForMember(d => d.DescripcionTipoIdentificacion, d => d.MapFrom(s => s.TipoIdentificacion.Descripcion));

            CreateMap<DetalleTelefono, DetalleTelefonoDTO>()
                .ForMember(d => d.DescripcionCompania, d => d.MapFrom(s => s.CompaniaTelefonica.Descripcion));

        }
    }
}