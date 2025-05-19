using Grpc.Core;
using GrpcClient.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebClientApp.GrpcProvider;
using WebClientApp.Models;

namespace WebClientApp.Controllers
{
    public class HomeController : Controller
    {
       // private readonly ILogger<HomeController> _logger;
        private readonly IGrpcBusClient _grpcClient;
        // Внедрение Singleton-зависимости
        public HomeController(IGrpcBusClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        //public HomeController(IGrpcBusClient grpcClient, ILogger<HomeController> logger)
        //{
        //   // _logger = logger;
        //}
        //public HomeController(ILogger<HomeController> logger)
        //{
        //   // _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Test()
        {
            return View();
        }

        public  IActionResult Privacy()
        {
            try
            {
                 _grpcClient.SendTextAsync($"New order: RUN BABY");
                //return Ok();
            }
            catch (RpcException ex)
            {
              //  return StatusCode(503, "GRPC service unavailable");
            }


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] String order)
        {
            try
            {
                await _grpcClient.SendTextAsync($"New order: {order}");
                return Ok();
            }
            catch (RpcException ex)
            {
                return StatusCode(503, "GRPC service unavailable");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
