using InvokerTraining.Application.Abstractions;
using InvokerTraining.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InvokerTraining.Controllers
{

    public class HomeController(IInfoService infoService) : Controller
    {
        private readonly IInfoService _infoService = infoService;

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> LeaderBoard()
        {
            var leaderboard = new List<(string PhoneOrEmail, int GameCount, TimeSpan? PlayerRecord)>();
            await foreach (var player in _infoService.GetLeaderBoardAsync())
            {
                leaderboard.Add((player.PhoneOrEmail, player.GameCount, player.playerRecord));
            }
            return View(leaderboard);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
