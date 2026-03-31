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

            CreateMap<EntidadDTO, Entidad>()
                .ForMember(d => d.FechaNacimiento, d => d.MapFrom(s => s.FechaNacimiento.ToString()));

            CreateMap<Entidad, EntidadDTO>()
                .ForMember(d => d.FechaNacimiento, d => d.MapFrom(s => s.FechaNacimiento))
                //.ForMember(d => d.TipoIdentificacion, d => d.MapFrom(s => s.Identidades..Descripcion))
                .ForMember(d => d.Descripcion, d => d.MapFrom(s => s.NombreComercial == null ? string.Concat(s.Nombres, " ", s.Apellidos) : s.NombreComercial));

            CreateMap<SubCategoria, SubCategoriaDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.Categoria.Descripcion));

            CreateMap<Producto, ProductoDTO>()
                .ForMember(d => d.DescripcionCategoria, d => d.MapFrom(s => s.SubCategoria.Categoria.Descripcion))
                .ForMember(d => d.DescripcionSubCategoria, d => d.MapFrom(s => s.SubCategoria.Descripcion));
        }
    }
}