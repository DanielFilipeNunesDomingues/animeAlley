using animeAlley.Models;
using animeAlley.Models.ViewModels.AutoresDTO;
using animeAlley.Models.ViewModels.GenerosDTO;
using animeAlley.Models.ViewModels.PersonagensDTO;
using animeAlley.Models.ViewModels.ShowsDTO;
using animeAlley.Models.ViewModels.StudiosDTO;
using AutoMapper;

namespace animeAlley.Mapping
{
    public class ShowsMappingProfile : Profile
    {
        public ShowsMappingProfile()
        {
            // Show para ShowResumoDTO
            CreateMap<Show, ShowResumoDTO>()
                .ForMember(dest => dest.Generos, opt => opt.MapFrom(src => src.GenerosShows.Select(g => g.GeneroNome)))
                .ForMember(dest => dest.Studio, opt => opt.MapFrom(src => src.Studio.Nome))
                .ForMember(dest => dest.Autor, opt => opt.MapFrom(src => src.Autor.Nome));

            // Show para ShowDetalheDTO
            CreateMap<Show, ShowDetalheDTO>()
                .ForMember(dest => dest.Generos, opt => opt.MapFrom(src => src.GenerosShows))
                .ForMember(dest => dest.Studio, opt => opt.MapFrom(src => src.Studio))
                .ForMember(dest => dest.Autor, opt => opt.MapFrom(src => src.Autor))
                .ForMember(dest => dest.Personagens, opt => opt.MapFrom(src => src.Personagens));

            // Show para ShowPopularDTO
            CreateMap<Show, ShowPopularDTO>()
                .ForMember(dest => dest.TotalNasListas, opt => opt.MapFrom(src => src.ListaShows.Count));

            // ShowCreateUpdateDTO para Show
            CreateMap<ShowCreateUpdateDTO, Show>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.GenerosShows, opt => opt.Ignore())
                .ForMember(dest => dest.Studio, opt => opt.Ignore())
                .ForMember(dest => dest.Autor, opt => opt.Ignore())
                .ForMember(dest => dest.Personagens, opt => opt.Ignore())
                .ForMember(dest => dest.ListaShows, opt => opt.Ignore());

            // Mapeamentos auxiliares 
            CreateMap<Genero, GeneroDTO>();
            CreateMap<Studio, StudioDTO>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Personagem, PersonagemResumoDTO>();
        }
    }
}