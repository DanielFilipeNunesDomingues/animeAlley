using animeAlley.Models;
using animeAlley.Models.ViewModels.EstatisticasDTO;
using animeAlley.Models.ViewModels.GenerosAPI;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;
using AutoMapper;

namespace animeAlley.Mapping
{
    public class EstatisticasMappingProfile : Profile
    {
        public EstatisticasMappingProfile()
        {
            // Mapeamento para GeneroPopularDTO
            CreateMap<Genero, GeneroPopularDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.GeneroNome))
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.Shows.Count))
                .ForMember(dest => dest.MediaNotas, opt => opt.MapFrom(src =>
                    src.Shows.Where(s => s.Nota > 0).Select(s => s.Nota).DefaultIfEmpty(0).Average()));

            // Mapeamento para ShowPopularDTO
            CreateMap<Show, ShowPopularDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Nota, opt => opt.MapFrom(src => src.Nota));

            // Mapeamento para StudioPopularDTO
            CreateMap<Studio, StudioPopularDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.ShowsDesenvolvidos.Count))
                .ForMember(dest => dest.NotaMedia, opt => opt.MapFrom(src =>
                    src.ShowsDesenvolvidos.Where(s => s.Nota > 0).Select(s => s.Nota).DefaultIfEmpty(0).Average()));

            // Mapeamento para EstatisticaDTO (caso precise mapear de uma entidade agregada)
            CreateMap<object, EstatisticaDTO>()
                .ForMember(dest => dest.TotalShows, opt => opt.Ignore())
                .ForMember(dest => dest.TotalUtilizadores, opt => opt.Ignore())
                .ForMember(dest => dest.TotalGeneros, opt => opt.Ignore())
                .ForMember(dest => dest.TotalStudios, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAutores, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPersonagens, opt => opt.Ignore())
                .ForMember(dest => dest.NotaMediaGeral, opt => opt.Ignore())
                .ForMember(dest => dest.GenerosPopulares, opt => opt.Ignore())
                .ForMember(dest => dest.ShowsPopulares, opt => opt.Ignore())
                .ForMember(dest => dest.StudiosPopulares, opt => opt.Ignore());

            // Mapeamento para EstatisticaResumoDTO (caso precise mapear de uma entidade agregada)
            CreateMap<object, EstatisticaResumoDTO>()
                .ForMember(dest => dest.TotalShows, opt => opt.Ignore())
                .ForMember(dest => dest.TotalUtilizadores, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPersonagens, opt => opt.Ignore())
                .ForMember(dest => dest.NotaMediaGeral, opt => opt.Ignore())
                .ForMember(dest => dest.GeneroMaisPopular, opt => opt.Ignore())
                .ForMember(dest => dest.ShowMaisPopular, opt => opt.Ignore());

            // Mapeamentos reversos (caso necessário)
            CreateMap<GeneroPopularDTO, Genero>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GeneroNome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Shows, opt => opt.Ignore());

            CreateMap<ShowPopularDTO, Show>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Nota, opt => opt.MapFrom(src => src.Nota));

            CreateMap<StudioPopularDTO, Studio>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.ShowsDesenvolvidos, opt => opt.Ignore());
        }
    }
}