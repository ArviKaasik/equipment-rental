using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EquipmentRentalClient.Models;
using EquipmentRentalClient.Services;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EquipmentRentalClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEquipmentService _equipmentService;
        
        public HomeController(ILogger<HomeController> logger, IEquipmentService equipmentService)
        {
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public async Task<IActionResult> RentEquipment()
        {
            try
            {
                var availableEquipment = await _equipmentService.GetAvailableEquipment();
                return View(new EquipmentViewModel {AvailableEquipment = availableEquipment});
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception occured while requesting available equipment");
                return RedirectError("Exception occured while requesting available equipment. Please try again later");
            }
        }

        [HttpPost]
        [Route("Home/GenerateInvoice")]
        public async Task<IActionResult> GenerateInvoice([Required] [FromBody] GetInvoiceRequest request)
        {
            Invoice invoice;
            try
            {
                invoice = await _equipmentService.GenerateInvoice(request.Equipment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occured while generating invoice");
                return RedirectError("Exception occured while generating invoice. Please try again later");
            }

            _logger.LogInformation($"Sending invoice: {JsonConvert.SerializeObject(invoice)}");
            
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, invoice);

            var cd = new ContentDisposition
            {
                FileName = invoice.Title,
                Inline = true
            };
            Response?.Headers?.Add("Content-Disposition", cd.ToString());
            stream.Seek(0, 0);
            return new FileStreamResult(stream, "application/octet-stream");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [Route("/Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message});
        }

        private IActionResult RedirectError([FromRoute]string message)
        {
            var encodedMessage = System.Web.HttpUtility.UrlEncode($"{message}");
            return Redirect($"Home/Error?message={encodedMessage}");
        }
    }
}