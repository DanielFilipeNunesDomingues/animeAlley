using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace animeAlley.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly SendGridClient _client;
        private readonly AuthMessageSenderOptions _options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            _options = optionsAccessor.Value;
            _logger = logger;

            // Correção: verificar se a chave existe antes de criar o client
            if (string.IsNullOrEmpty(_options.SendGridKey))
            {
                _logger.LogError("SendGrid API Key não configurada!");
                throw new ArgumentException("SendGrid API Key é obrigatória");
            }

            _client = new SendGridClient(_options.SendGridKey);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogError("Email de destino não pode ser vazio");
                    return;
                }

                if (string.IsNullOrEmpty(_options.SendGridKey))
                {
                    _logger.LogError("Chave do SendGrid não configurada!");
                    throw new Exception("Chave do SendGrid não configurada!");
                }

                if (string.IsNullOrEmpty(_options.FromEmail))
                {
                    _logger.LogError("Email remetente não configurado!");
                    throw new Exception("Email remetente não configurado!");
                }

                await Execute(subject, message, toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar email para {toEmail}");
                throw; // Re-throw para que o erro seja tratado pela aplicação
            }
        }

        private async Task Execute(string subject, string message, string toEmail)
        {
            try
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_options.FromEmail, _options.FromName ?? "AnimeAlley"),
                    Subject = subject,
                    PlainTextContent = StripHtml(message), // Versão texto sem HTML
                    HtmlContent = GetEmailTemplate(subject, message)
                };

                msg.AddTo(new EmailAddress(toEmail));

                // Configurações adicionais do SendGrid
                msg.SetClickTracking(false, false);
                msg.SetOpenTracking(false);
                msg.SetGoogleAnalytics(false);
                msg.SetSubscriptionTracking(false);

                _logger.LogInformation($"Tentando enviar email para {toEmail} com assunto: {subject}");

                var response = await _client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email enviado com sucesso para {toEmail}. Status: {response.StatusCode}");
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Falha ao enviar email para {toEmail}. Status: {response.StatusCode}, Response: {responseBody}");

                    // Lançar exceção para que o erro seja propagado
                    throw new Exception($"Falha no envio do email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro interno ao executar envio de email para {toEmail}");
                throw;
            }
        }

        private string GetEmailTemplate(string subject, string message)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>{subject}</title>
            </head>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <h1 style='color: #333; margin: 0;'>
                            <span style='color: #007bff;'>Anime</span><span style='color: #333;'>Alley</span>
                        </h1>
                    </div>
                    
                    <div style='background-color: #f8f9fa; padding: 25px; border-radius: 8px; margin-bottom: 20px;'>
                        <h2 style='color: #333; margin-top: 0; margin-bottom: 20px;'>{subject}</h2>
                        <div style='color: #555; line-height: 1.6; font-size: 16px;'>
                            {message}
                        </div>
                    </div>
                    
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    
                    <p style='text-align: center; color: #666; margin: 0; font-size: 14px;'>
                        Este email foi enviado automaticamente pelo sistema AnimeAlley.<br>
                        Não responda a este email.
                    </p>
                </div>
            </body>
            </html>";
        }

        private string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // Remove tags HTML básicas para criar versão texto
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty)
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Trim();
        }
    }
}