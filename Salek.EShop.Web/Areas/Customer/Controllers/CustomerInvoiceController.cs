using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salek.EShop.Web.Models.Database;
using Salek.EShop.Web.Models.Entity;
using Salek.EShop.Web.Models.Identity;

namespace Salek.EShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = nameof(RolesEnum.Customer))]
    public class CustomerInvoiceController : Controller
    {
        EShopDbContext EshopDbContext;

        public CustomerInvoiceController(EShopDbContext eshopDbContext)
        {
            EshopDbContext = eshopDbContext;
        }
        
        //TODO RenderPDF()
        private static void RenderInvoiceFile()
        {
            var htmlPage = @"
<html>
<head>
</head>
<body>
                <table class=""table table-clear"">
                <tbody>
                <tr>
                <td class=""left"">
                <strong>Total Price</strong>
                </td>
                </tr>
                </tbody>
                </table>
</body>
</html>
                ";
            var renderer = new IronPdf.ChromePdfRenderer();
            renderer.RenderHtmlAsPdf(htmlPage).SaveAs("invoice.pdf");

            Console.WriteLine("Invoice Rendered");
        }
        public static async Task MailSender(Order order)
        {
            RenderInvoiceFile();
            
            var currentUser = order.User;
            var adminMail = "robotseshop@gmail.com";
            var adminMailPass = "Admin1#admin";
            
            var subject = $"Your order was successful - Invoice #{order.ID}";
            
            var body = $@"
<html lang=""en"">
<head>
    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous"">
</head>
<body>
<div class=""card"">
                       <div class=""card-header"">Hello {order.User.FirstName} {order.User.LastName}.<div>
                       <div class=""card-body"">Your order war successful, you can find your Invoice #{order.ID} at our website in section My Orders</div>
                       <div class=""card-footer"">Thank you for ordering from Robots.Eshop</div>
</div>
</body>
</html>
";
            var server = "smtp.gmail.com";
            var port = 587;
            
            MailAddress to = new MailAddress(currentUser.Email);
            MailAddress from = new MailAddress(adminMail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Attachments.Add(new Attachment("invoice.pdf")); //TODO
            
            SmtpClient client = new SmtpClient(server, port)
            {
                Credentials = new NetworkCredential(adminMail, adminMailPass),
                EnableSsl = true
            };
            try
            { 
                client.Send(mail);
                Console.WriteLine("Email was sended");
            }
            catch (SmtpException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        public async Task<IActionResult> Invoice(int id)
        {
            var foundItem = EshopDbContext.Orders
                .Include(o=>o.User)
                .Include(o=>o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.ID == id);

            if (foundItem != null)
            {
                return View(foundItem);
            }
            return NotFound();
        }
    }
}