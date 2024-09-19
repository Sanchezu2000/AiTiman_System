using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(SmtpClient client, ILogger<EmailSender> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var mailMessage = new MailMessage("aitiman.soc@gmail.com", email, subject, message);
            await _client.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "An error occurred while sending email.");
            throw;
        }
    }
}
