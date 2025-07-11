using animeAlley.Models;
using animeAlley.Models.ViewModels.AutoresDTO;
using AutoMapper;

namespace animeAlley.Mapping
{
    public class AutoresMappingProfile : Profile
    {
        public AutoresMappingProfile()
        {
            // Autor para AutorDTO
            CreateMap<Autor, AutorDTO>()
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.AutorSexualidade.ToString()))
                .ForMember(dest => dest.TotalObras, opt => opt.MapFrom(src => src.ShowsCriados != null ? src.ShowsCriados.Count : 0));

            // AutorCreateUpdateDTO para Autor
            CreateMap<AutorCreateUpdateDTO, Autor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateNasc, opt => opt.MapFrom(src => src.DataNascimento))
                .ForMember(dest => dest.AutorSexualidade, opt => opt.MapFrom(src => src.Sexo))
                .ForMember(dest => dest.ShowsCriados, opt => opt.Ignore())
                .ForMember(dest => dest.Sobre, opt => opt.MapFrom(src => src.Sobre ?? string.Empty))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto ?? string.Empty));

            // Autor para AutorPopularDTO
            CreateMap<Autor, AutorPopularDTO>()
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.ShowsCriados != null ? src.ShowsCriados.Count : 0))
                .ForMember(dest => dest.NotaMedia, opt => opt.MapFrom(src =>
                    src.ShowsCriados != null && src.ShowsCriados.Any() ?
                    src.ShowsCriados.Average(s => s.Nota) : 0));

            // Mapeamento reverso
            CreateMap<AutorDTO, Autor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateNasc, opt => opt.MapFrom(src => src.DataNascimento))
                .ForMember(dest => dest.AutorSexualidade, opt => opt.ConvertUsing<SexualidadeValueConverter, string>(src => src.Sexo))
                .ForMember(dest => dest.ShowsCriados, opt => opt.Ignore())
                .ForMember(dest => dest.Sobre, opt => opt.MapFrom(src => src.Sobre ?? string.Empty))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto ?? string.Empty));
        }
    }

    // Conversor customizado para converter string para enum Sexualidade
    public class SexualidadeValueConverter : IValueConverter<string, Sexualidade?>
    {
        public Sexualidade? Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
                return null;

            if (Enum.TryParse<Sexualidade>(sourceMember, true, out var result))
                return result;

            return null;
        }
    }
}