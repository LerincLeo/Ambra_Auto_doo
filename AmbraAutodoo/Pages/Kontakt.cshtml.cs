using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

public class KontaktModel : PageModel
{
    private readonly IConfiguration _configuration;

    public KontaktModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string Name { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Phone { get; set; }

    [BindProperty]
    public string Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Email),
                    Subject = "Kontakt forma - Ambra Autodoo",
                    Body = $"Ime i prezime: {Name}\nE-mail: {Email}\nTelefon: {Phone}\nPoruka: {Message}",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(emailSettings["FromEmail"]);

                var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["SmtpPort"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailSettings["FromEmail"], emailSettings["FromPassword"])
                };

                await smtpClient.SendMailAsync(mailMessage);
                ViewData["Message"] = "Poruka poslana uspješno!";
            }
            catch (SmtpException smtpEx)
            {
                // Log the detailed SMTP exception
                ViewData["Message"] = $"SMTP Error: {smtpEx.Message}";
            }
            catch (Exception ex)
            {
                // Log general exceptions
                ViewData["Message"] = $"Error: {ex.Message}";
            }
        }

        return Page();
    }
}