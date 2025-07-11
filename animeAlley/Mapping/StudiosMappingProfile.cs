using animeAlley.Models;
using animeAlley.Models.ViewModels.StudiosDTO;
using AutoMapper;

namespace animeAlley.Mapping
{
    public class StudiosMappingProfile : Profile
    {
        public StudiosMappingProfile()
        {
            // Studio para StudioDTO
            CreateMap<Studio, StudioDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.ShowsDesenvolvidos != null ? src.ShowsDesenvolvidos.Count : 0));

            // StudioCreateUpdateDTO para Studio
            CreateMap<StudioCreateUpdateDTO, Studio>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.ConvertUsing<EstadoValueConverter, string>(src => src.Status))
                .ForMember(dest => dest.ShowsDesenvolvidos, opt => opt.Ignore())
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto ?? string.Empty));

            // Studio para StudioPopularDTO
            CreateMap<Studio, StudioPopularDTO>()
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.ShowsDesenvolvidos != null ? src.ShowsDesenvolvidos.Count : 0))
                .ForMember(dest => dest.NotaMedia, opt => opt.MapFrom(src =>
                    src.ShowsDesenvolvidos != null && src.ShowsDesenvolvidos.Any() ?
                    src.ShowsDesenvolvidos.Average(s => s.Nota) : 0));

            // Mapeamento reverso
            CreateMap<StudioDTO, Studio>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.ConvertUsing<EstadoValueConverter, string>(src => src.Status))
                .ForMember(dest => dest.ShowsDesenvolvidos, opt => opt.Ignore())
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto ?? string.Empty));
        }
    }

    // Conversor customizado para converter string para enum Estado
    public class EstadoValueConverter : IValueConverter<string, Estado?>
    {
        public Estado? Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
                return Estado.Ativo; // Valor padrão

            if (Enum.TryParse<Estado>(sourceMember, true, out var result))
                return result;

            return Estado.Ativo; // Valor padrão se não conseguir converter
        }
    }
}