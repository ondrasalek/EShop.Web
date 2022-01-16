using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using IronPdf;
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
        EShopDbContext eShopDbContext;

        public CustomerInvoiceController(EShopDbContext eShopDbContext)
        {
            this.eShopDbContext = eShopDbContext;
        }

        public static MemoryStream RenderInvoiceFile(Order? order)
        {
            
            var htmlPage = new StringBuilder();
            htmlPage.Append(
                $@"
            <html>
            <body style=""margin:1rem"">
                <section class=""container-fluid"">
                    <div id=""ui-view"" data-select2-id=""ui-view"">
                        <div>
                            <div class=""card"">
                                <div class=""card-header"">
                                    <h4 class=""float-left"">Invoice #{order.ID}</h4>
                                </div>
                                <div class=""card-body"">
                                    <div class=""row mb-4"">
                                        <div class=""col-sm-4"">
                                            <h6 class=""mb-3"">From:</h6>
                                            <div>
                                                <strong>Robots.EShop</strong>
                                            </div>
                                            <div>Address: ...</div>
                                            <div>Email: <a href=""mailto:robotseshop@gmail.com"">robotseshop@gmail.com</a></div>
                                            <div>Phone: ...</div>
                                        </div>
                                        <div class=""col-sm-4"">
                                            <h6 class=""mb-3"">To:</h6>
                                            <strong>{order.User.FirstName} {order.User.LastName}</strong>
                                            <div>Address: ...</div>
                                            <div>Email: {order.User.Email}</div>
                                            <div>Phone: {order.User.PhoneNumber}</div>
                                        </div>
                                        <div class=""col-sm-4"">
                                            <h6 class=""mb-3"">Details:</h6>
                                            <div>Invoice:<strong> #{order.ID}</strong></div>
                                            <div>Date: {order.DateTimeCreated}</div>
                                            <div>Account: {order.User.UserName}</div>
                                        </div>
                                    </div>
                                    <table class=""table table-striped"">
                                        <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>{nameof(ProductItem.Name)}</th>
                                            <th>{nameof(OrderItem.Amount)}</th>
                                            <th>Base {nameof(OrderItem.Product.Price)}</th>
                                            <th>Total {nameof(OrderItem.Price)}</th>
                                        </tr>
                                        </thead>
                                        <tbody>
            ");
            int i = 0;
            foreach (var item in order.OrderItems)
            {
                i++;
                htmlPage.Append(
                    $@"
                            <tr>
                                <td>{i}</td>
                                <td>{item.Product.Name}</td>
                                <td>{item.Amount}</td>
                                <td>{item.Product.Price.ToString("C2")}</td>
                                <td>{item.Price.ToString("C2")}</td>
                            </tr>
                        ");
            }
            htmlPage.Append(
                    $@"
            </tbody>
                    </table>
                    <div class=""row"">
                        <div class=""col-lg-4 col-sm-5 ml-auto"">
                            <table class=""table table-clear"">
                                <tbody>
                                <tr>
                                    <td class=""left"">
                                        <strong>Total Price</strong>
                                    </td>
                                    <td class=""right"">
                                        <strong>{order.TotalPrice.ToString("C2")}</strong>
                                    </td>
                                </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class=""card-footer"">
                    <div>Thanks for ordering from us.</div>
                </div>
            </div>
        </div>
    </div>
</section>
</body>
</html>
            ");
            
            Installation.DefaultRenderingEngine = IronPdf.Rendering.PdfRenderingEngine.Chrome;
            var renderer = new ChromePdfRenderer();
            renderer.RenderingOptions.PrintHtmlBackgrounds = true;
            renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Screen;
            renderer.RenderingOptions.CustomCssUrl = "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css";
            renderer.RenderingOptions.ViewPortWidth = 1280;
            renderer.RenderingOptions.MarginTop = 0;
            renderer.RenderingOptions.MarginBottom = 0;
            renderer.RenderingOptions.MarginLeft = 0;
            renderer.RenderingOptions.MarginRight = 0;
            renderer.RenderingOptions.Title = $"Invoice #{order.ID}";
            renderer.RenderingOptions.FitToPaperWidth = true;
            renderer.RenderingOptions.FitToPaper = true;
            renderer.RenderingOptions.CreatePdfFormsFromHtml = false;
            
            var file = renderer.RenderHtmlAsPdf(htmlPage.ToString());

            Console.WriteLine("PDF was created");
            return file.Stream;
        }
        
        public static async Task  MailSender(MemoryStream file , int id, User currentUser)
        {
            var userName = currentUser.FirstName + " " + currentUser.LastName;
            var userEmail = currentUser.Email;
            
            var adminMail = "robotseshop@gmail.com";
            var adminMailPass = "Admin1#admin";
            var subject = $"Your order was successful - Invoice #{id}";
            
            var body = $@"
<html lang=""en"">
<head>
    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous"">
</head>
<body>
<div class=""card"">
                       <div class=""card-header"">Hello {userName}.<div>
                       <div class=""card-body"">Your order war successful.</div>
                       <div class=""card-footer"">Thank you for ordering from Robots.Eshop</div>
</div>
</body>
</html>
";
            var server = "smtp.gmail.com";
            var port = 587;
            
            MailAddress to = new MailAddress(userEmail);
            MailAddress from = new MailAddress(adminMail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Attachments.Add(new Attachment(file, $"Invoice #{id}.pdf", MediaTypeNames.Application.Pdf));

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
            var foundItem = eShopDbContext.Orders
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