using Microsoft.AspNetCore.SignalR;
using animeAlley.Data;
using animeAlley.Models;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Hubs
{
    /// <summary>
    /// Hub SignalR para gerenciar atualizações em tempo real das views dos shows
    /// </summary>
    public class ShowsHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ShowsHub(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona o usuário a um grupo específico do show para receber atualizações
        /// </summary>
        /// <param name="showId">ID do show</param>
        public async Task JoinShowGroup(string showId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Show_{showId}");
        }

        /// <summary>
        /// Remove o usuário do grupo do show
        /// </summary>
        /// <param name="showId">ID do show</param>
        public async Task LeaveShowGroup(string showId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Show_{showId}");
        }

        /// <summary>
        /// Incrementa a view do show e notifica todos os usuários conectados
        /// </summary>
        /// <param name="showId">ID do show</param>
        public async Task IncrementView(int showId)
        {
            try
            {
                var show = await _context.Shows.FindAsync(showId);
                if (show != null)
                {
                    show.Views++;
                    show.DataAtualizacao = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notificar todos os usuários no grupo do show sobre a atualização
                    await Clients.Group($"Show_{showId}").SendAsync("ViewUpdated", showId, show.Views);

                    // Também notificar a página de índice se houver usuários conectados
                    await Clients.Group("ShowsIndex").SendAsync("ShowViewUpdated", showId, show.Views);
                }
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao incrementar view do show {showId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Adiciona usuário ao grupo da página de índice
        /// </summary>
        public async Task JoinShowsIndexGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ShowsIndex");
        }

        /// <summary>
        /// Remove usuário do grupo da página de índice
        /// </summary>
        public async Task LeaveShowsIndexGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ShowsIndex");
        }

        /// <summary>
        /// Método chamado quando uma conexão é estabelecida
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Método chamado quando uma conexão é encerrada
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}