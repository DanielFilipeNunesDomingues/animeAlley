using animeAlley.Models;
using animeAlley.DTOs;

namespace animeAlley.Services
{
    // ServiÃ§o para mapeamento entre Models e DTOs
    public interface IMappingService
    {
        ShowResumoDto MapToResumoDto(Show show);
        ShowDetalheDto MapToDetalheDto(Show show);
        ListaDto MapToListaDto(Lista lista);
        PaginatedResponseDto<T> CreatePaginatedResponse<T>(List<T> data, int page, int pageSize, int totalItems);
    }

    public class MappingService : IMappingService
    {
        public ShowResumoDto MapToResumoDto(Show show)
        {
            return new ShowResumoDto
            {
                Id = show.Id,
                Nome = show.Nome,
                Descricao = show.Sinopse,
                ImagemUrl = show.Imagem,
                Tipo = show.Tipo.ToString(),
                Rating = show.Nota,
                DataLancamento = show.Ano,
                Status = show.Status.ToString(),
                Generos = show.GenerosShows?.Select(g => g.GeneroNome).ToList() ?? new List<string>(),
                Studios = show.Studio.ToString()
            };
        }

        public ShowDetalheDto MapToDetalheDto(Show show)
        {
            return new ShowDetalheDto
            {
                Id = show.Id,
                Nome = show.Nome,
                Descricao = show.Sinopse,
                ImagemUrl = show.Imagem,
                Tipo = show.Tipo.ToString(),
                Rating = show.Nota,
                DataLancamento = show.Ano,
                //DataFim = show.DataFim,
                Status = show.Status.ToString(),
                //NumeroEpisodios = show.NumeroEpisodios,
                //NumeroCapitulos = show.NumeroCapitulos,
                Generos = show.GenerosShows?.Select(g => new GeneroDto
                {
                    Id = g.Id,
                    Nome = g.GeneroNome,
                }).ToList() ?? new List<GeneroDto>(),
                Studios = show.Studio.ToString(),
                Personagens = show.Personagens?.Select(p => new PersonagemDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descricao = p.Sinopse,
                    ImagemUrl = p.Foto,
                    Papel = p.TipoPersonagem.ToString(),
                    //VoiceActor = p.VoiceActor
                }).ToList() ?? new List<PersonagemDto>()
            };
        }

        public ListaDto MapToListaDto(Lista lista)
        {
            return new ListaDto
            {
                Id = lista.Id,
                UtilizadorId = lista.UtilizadorId,
                NomeUtilizador = lista.Utilizador.ToString(),
                //Descricao = lista.Descricao,
                //Publica = lista.Publica,
                //DataCriacao = lista.DataCriacao,
                //DataAtualizacao = lista.DataAtualizacao,
                Shows = lista.ListaShows?.Select(ls => new ListaShowDto
                {
                    //Show = ls.ShowId,
                    //DataAdicao = ls.DataAdicao,
                    //Nota = ls.Nota,
                    //Comentario = ls.Comentario,
                    Show = MapToResumoDto(ls.Show)
                }).ToList() ?? new List<ListaShowDto>(),
                //TotalShows = lista.ListaShows?.Count ?? 0
            };
        }

        public PaginatedResponseDto<T> CreatePaginatedResponse<T>(List<T> data, int page, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PaginatedResponseDto<T>
            {
                Data = data,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                //HasNextPage = page < totalPages,
                //HasPreviousPage = page > 1
            };
        }
    }
}