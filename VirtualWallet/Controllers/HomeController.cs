using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VirtualWallet.Models;

namespace VirtualWallet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return await GetCards();
        }


        private async Task<IActionResult> GetCards()
        {
            List<Card> cardsList = new List<Card>();
            Query query = FirestoreDb.Create("virtualwallet-2a397").Collection("Cards");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();

                if (data.ContainsKey("Bank") && data["Bank"] != null &&
                    data.ContainsKey("Issuer") && data["Issuer"] != null &&
                    data.ContainsKey("Owner") && data["Owner"] != null &&
                    data.ContainsKey("CardNumber") && data["CardNumber"] != null &&
                    data.ContainsKey("CVV") && data["CVV"] != null &&
                    data.ContainsKey("ExpirationDate") && data["ExpirationDate"] != null)
                {
                    cardsList.Add(new Card
                    {
                        Id = item.Id,
                        Bank = data["Bank"].ToString(),
                        Issuer = data["Issuer"].ToString(),
                        Owner = data["Owner"].ToString(),
                        CardNumber = data["CardNumber"].ToString(),
                        CVV = data["CVV"].ToString(),
                        ExpirationDate = data["ExpirationDate"].ToString()
                    });
                }
            }

            ViewBag.Cards = cardsList;

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}