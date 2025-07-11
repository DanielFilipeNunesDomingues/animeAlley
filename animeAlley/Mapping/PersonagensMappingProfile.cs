using AutoMapper;
using animeAlley.Models;
using animeAlley.Models.ViewModels.PersonagensDTO;
using animeAlley.Models.ViewModels.ShowsDTO;

namespace animeAlley.Mapping
{
    public class PersonagensMappingProfile : Profile
    {
        public PersonagensMappingProfile()
        {
            // Mapeamento de Personagem para PersonagemDTO
            CreateMap<Personagem, PersonagemDTO>()
                .ForMember(dest => dest.TipoPersonagem, opt => opt.MapFrom(src => src.TipoPersonagem.ToString()))
                .ForMember(dest => dest.Sexualidade, opt => opt.MapFrom(src => src.PersonagemSexualidade.HasValue ? src.PersonagemSexualidade.ToString() : null))
                .ForMember(dest => dest.DataNascimento, opt => opt.MapFrom(src => src.DataNasc))
                .ForMember(dest => dest.Shows, opt => opt.MapFrom(src => src.Shows));

            // Mapeamento de PersonagemCreateUpdateDTO para Personagem
            CreateMap<PersonagemCreateUpdateDTO, Personagem>()
                .ForMember(dest => dest.TipoPersonagem, opt => opt.MapFrom(src => Enum.Parse<TiposPersonagem>(src.TipoPersonagem)))
                .ForMember(dest => dest.PersonagemSexualidade, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Sexualidade) ? (Sexualidade?)null : Enum.Parse<Sexualidade>(src.Sexualidade)))
                .ForMember(dest => dest.DataNasc, opt => opt.MapFrom(src => src.DataNascimento))
                .ForMember(dest => dest.Sinopse, opt => opt.MapFrom(src => src.Sinopse ?? string.Empty))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Foto ?? string.Empty))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorar ID na criação
                .ForMember(dest => dest.Shows, opt => opt.Ignore()); // Shows serão tratados separadamente

            // Mapeamento de Personagem para PersonagemResumoDTO
            CreateMap<Personagem, PersonagemResumoDTO>()
                .ForMember(dest => dest.TipoPersonagem, opt => opt.MapFrom(src => src.TipoPersonagem.ToString()));

            // Mapeamento de Show para ShowResumoDTO (assumindo que existe)
            CreateMap<Show, ShowResumoDTO>()
                .ForMember(dest => dest.Imagem, opt => opt.MapFrom(src => src.Imagem));
        }
    }
}