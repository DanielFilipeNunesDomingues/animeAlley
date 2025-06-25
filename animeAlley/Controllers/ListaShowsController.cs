using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace animeAlley.Controllers
{
    [Authorize]
    public class ListaShowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListaShowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um show à lista do utilizador com o status especificado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarShowALista(int showId, ListaStatus status)
        {
            try
            {
                // Obter o utilizador atual através do UserName
                var userName = User.Identity.Name;
                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (utilizador == null)
                {
                    return Json(new { success = false, message = "Utilizador não encontrado." });
                }

                // Verificar se o utilizador já tem uma lista (criar se não existir)
                var lista = await _context.Listas
                    .FirstOrDefaultAsync(l => l.UtilizadorId == utilizador.Id);

                if (lista == null)
                {
                    lista = new Lista
                    {
                        UtilizadorId = utilizador.Id
                    };
                    _context.Listas.Add(lista);
                    await _context.SaveChangesAsync();
                }

                // Verificar se o show já existe na lista do utilizador
                var showExistente = await _context.ListaShows
                    .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

                if (showExistente != null)
                {
                    // Atualizar o status se o show já existir
                    showExistente.ListaStatus = status;
                    _context.Update(showExistente);
                }
                else
                {
                    // Adicionar novo show à lista
                    var novoListaShow = new ListaShows
                    {
                        ShowId = showId,
                        ListaId = lista.Id,
                        ListaStatus = status
                    };
                    _context.ListaShows.Add(novoListaShow);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Show adicionado à lista com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao adicionar show à lista: " + ex.Message });
            }
        }

        /// <summary>
        /// Remove um show da lista do utilizador
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverShowDaLista(int showId)
        {
            try
            {
                var userName = User.Identity.Name;
                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (utilizador == null)
                {
                    return Json(new { success = false, message = "Utilizador não encontrado." });
                }

                var lista = await _context.Listas
                    .FirstOrDefaultAsync(l => l.UtilizadorId == utilizador.Id);

                if (lista == null)
                {
                    return Json(new { success = false, message = "Lista não encontrada." });
                }

                var listaShow = await _context.ListaShows
                    .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

                if (listaShow != null)
                {
                    _context.ListaShows.Remove(listaShow);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Show removido da lista com sucesso!" });
                }

                return Json(new { success = false, message = "Show não encontrado na lista." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao remover show da lista: " + ex.Message });
            }
        }

        /// <summary>
        /// Verifica se um show está na lista do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> VerificarShowNaLista(int showId)
        {
            try
            {
                var userName = User.Identity.Name;
                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (utilizador == null)
                {
                    return Json(new { inList = false });
                }

                var lista = await _context.Listas
                    .FirstOrDefaultAsync(l => l.UtilizadorId == utilizador.Id);

                if (lista == null)
                {
                    return Json(new { inList = false });
                }

                var listaShow = await _context.ListaShows
                    .FirstOrDefaultAsync(ls => ls.ListaId == lista.Id && ls.ShowId == showId);

                if (listaShow != null)
                {
                    return Json(new
                    {
                        inList = true,
                        status = listaShow.ListaStatus.ToString(),
                        statusDisplay = GetStatusDisplayName(listaShow.ListaStatus)
                    });
                }

                return Json(new { inList = false });
            }
            catch (Exception ex)
            {
                return Json(new { inList = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a lista de shows do utilizador filtrada por status
        /// </summary>
        public async Task<IActionResult> MinhaLista(ListaStatus? status = null)
        {
            var userName = User.Identity.Name;
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var query = _context.ListaShows
                .Include(ls => ls.Show)
                .Where(ls => ls.Lista.UtilizadorId == utilizador.Id);

            if (status.HasValue)
            {
                query = query.Where(ls => ls.ListaStatus == status.Value);
            }

            var listaShows = await query.ToListAsync();

            ViewBag.StatusFiltro = status;
            ViewBag.StatusOptions = Enum.GetValues(typeof(Status))
                .Cast<ListaStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = GetStatusDisplayName(s),
                    Selected = s == status
                }).ToList();

            return View(listaShows);
        }

        private string GetStatusDisplayName(ListaStatus status)
        {
            return status switch
            {
                ListaStatus.Assistir => "Assistindo",
                ListaStatus.Terminei => "Terminei",
                ListaStatus.Pausa => "Em Pausa",
                ListaStatus.Desisti => "Desisti",
                ListaStatus.Pensar_Assistir => "Planejo Assistir",
                _ => status.ToString()
            };
        }
    }
}