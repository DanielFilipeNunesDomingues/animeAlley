using AutoMapper;
using animeAlley.Models;
using animeAlley.Models.ViewModels.GenerosDTO;
using animeAlley.Models.ViewModels.GenerosAPI;

namespace animeAlley.Mapping
{
    public class GenerosMappingProfile : Profile
    {
        public GenerosMappingProfile()
        {
            // Mapeamento de Genero para GeneroDTO
            CreateMap<Genero, GeneroDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.GeneroNome))
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.Shows.Count()));

            // Mapeamento de GeneroCreateUpdateDTO para Genero
            CreateMap<GeneroCreateUpdateDTO, Genero>()
                .ForMember(dest => dest.GeneroNome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorar ID na criação
                .ForMember(dest => dest.Shows, opt => opt.Ignore()); // Shows serão tratados separadamente

            // Mapeamento de Genero para GeneroPopularDTO
            CreateMap<Genero, GeneroPopularDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.GeneroNome))
                .ForMember(dest => dest.TotalShows, opt => opt.MapFrom(src => src.Shows.Count()));
        }
    }
}