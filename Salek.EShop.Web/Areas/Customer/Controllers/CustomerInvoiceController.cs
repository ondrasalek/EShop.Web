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

        public static async Task MailSender(Order order)
        {
            var currentUser = order.User;
            var adminMail = "robotseshop@gmail.com";
            var adminMailPass = "Admin1#admin";
            
            var subject = $"Your order was succesful - Invoice #{order.ID}";
            var body = "Thank you for ordering from Robots.Eshop" +
                       ("");
            
            
            var filename = $"/Customer/CustomerOrders/Invoice/{order.ID}";
            // Attachment data = new Attachment(filename, MediaTypeNames.Application.Octet);
            
            var server = "smtp.gmail.com";
            var port = 587;
            
            MailAddress to = new MailAddress(currentUser.Email);
            MailAddress from = new MailAddress(adminMail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            // message.Attachments.Add(data);
            
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
        
        public Task<IActionResult> Invoice(int id)
        {
            var foundItem =  EshopDbContext.Orders
                .Include(o=>o.User)
                .Include(o=>o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.ID == id);
            return foundItem != null ? Task.FromResult<IActionResult>(View(foundItem)) : Task.FromResult<IActionResult>(NotFound());
        }
    }
}