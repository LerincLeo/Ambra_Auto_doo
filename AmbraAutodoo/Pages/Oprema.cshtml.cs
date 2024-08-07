using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class OpremaModel : PageModel
{
    private readonly IConfiguration _configuration;

    public OpremaModel(IConfiguration configuration)
    {
        _configuration = configuration;
        Brisaci = new List<Item>
        {
            new Item { Name = "Brisač 1", ImagePath = "~/images/item1.jpg", Price="11.59$"},
            new Item { Name = "Brisač 2", ImagePath = "~/images/item2.jpg", Price="11.59$"},
            new Item { Name = "Brisač 3", ImagePath = "~/images/item3.jpg", Price="11.59$"},
            new Item { Name = "Brisač 4", ImagePath = "~/images/item4.jpg", Price="11.59$"}
        };
        Amortizeri = new List<Item>
        {
            new Item { Name = "Amortizer 1", ImagePath = "~/images/item1.jpg", Price="11.59$"},
            new Item { Name = "Amortizer 2", ImagePath = "~/images/item2.jpg", Price="11.59$"},
            new Item { Name = "Amortizer 3", ImagePath = "~/images/item3.jpg", Price="11.59$"},
            new Item { Name = "Amortizer 4", ImagePath = "~/images/item4.jpg", Price="11.59$"}
        };
    }

    public List<Item> Brisaci { get; set; }
    public List<Item> Amortizeri { get; set; }

    public class Item
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public string Price { get; set; }
    }

    [BindProperty]
    public string ItemName { get; set; }

    [BindProperty]
    public int Quantity { get; set; }

    [BindProperty]
    public string PhoneNumber { get; set; }

    [BindProperty]
    public string Address { get; set; }

    [BindProperty]
    public string Town { get; set; }

    [BindProperty]
    public string PostalCode { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["FromEmail"]),
                    Subject = "Naruči - Ambra Autodoo",
                    Body = $"Naziv artikla: {ItemName}\nKoličina: {Quantity}\nBroj telefona: {PhoneNumber}\nAdresa: {Address}\nGrad: {Town}\nPoštanski broj: {PostalCode}",
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
                ViewData["Message"] = "Naručba poslana uspješno!";
            }
            catch (SmtpException smtpEx)
            {
                // Log the detailed SMTP exception
                ViewData["Message"] = "Dogodila se pogreška prilikom slanja e-pošte.";
            }
        }

        return Page();
    }
}
