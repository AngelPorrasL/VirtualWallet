using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Firebase.Storage;
using VirtualWallet.Models;
using Newtonsoft.Json;

namespace VirtualWallet.Controllers
{
    public class CardsController : Controller
    {
        // GET: CardsController
        public async Task<IActionResult> Index()
        {
            return await GetCards();
        }

        // GET: CardsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
                    data.ContainsKey("ExpirationDate") && data["ExpirationDate"] != null)
                {
                    cardsList.Add(new Card
                    {
                        Id = item.Id,
                        Bank = data["Bank"].ToString(),
                        Issuer = data["Issuer"].ToString(),
                        Owner = data["Owner"].ToString(),
                        CardNumber = data["CardNumber"].ToString(),
                        ExpirationDate = data["ExpirationDate"].ToString()
                    });
                }
            }

            ViewBag.Cards = cardsList;

            return View();
        }

        // GET: CardsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CardsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string bank, string issuer, string owner, string cardNumber,string cvv, string expirationDate)
        {
            try
            {
                DocumentReference addedDocRef =
                    await FirestoreDb.Create("virtualwallet-2a397")
                        .Collection("Cards").AddAsync(new Dictionary<string, object>
                            {
                                { "Bank", bank },
                                { "Issuer",  issuer },
                                { "Owner", owner },
                                { "CardNumber", cardNumber },
                                { "CVV", cvv },
                                { "ExpirationDate", expirationDate },
                            });

                // Después de agregar la tarjeta, actualiza la lista de tarjetas
                await GetCards();

                // Redirige a la vista
                return View("~/Views/Home/Index.cshtml");
            }
            catch
            {
                return View();
            }
        }

        // GET: CardsController/Edit/5
        public ActionResult Edit1(int id)
        {
            return View();
        }

        // POST: CardsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string cardId, string bank, string issuer, string owner, string cardNumber, string cvv, string expirationDate)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas editar en Firebase
                var cardDocRef = FirestoreDb.Create("virtualwallet-2a397")
                    .Collection("Cards")
                    .Document(cardId);

                // Actualiza los valores de la tarjeta
                var updateData = new Dictionary<string, object>
                {
                    { "Bank", bank },
                    { "Issuer", issuer },
                    { "Owner", owner },
                    { "CardNumber", cardNumber },
                    { "CVV", cvv },
                    { "ExpirationDate", expirationDate }
                };

                await cardDocRef.UpdateAsync(updateData);

                // Después de editar la tarjeta, actualiza la lista de tarjetas
                await GetCards();

                // Redirige a la vista principal (Index) después de editar la tarjeta
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: CardsController/Delete/5
        public ActionResult Delete1(int id)
        {
            return View();
        }

        // POST: CardsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string cardId)
        {
            try
            {
                // Primero, obtén la referencia al documento de la tarjeta que deseas eliminar en Firebase
                var cardDocRef = FirestoreDb.Create("virtualwallet-2a397")
                    .Collection("Cards")
                    .Document(cardId);

                // Borra el documento de la tarjeta
                await cardDocRef.DeleteAsync();

                // Redirige a la vista principal (Index) después de eliminar la tarjeta
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine("Error al eliminar tarjeta: " + ex.Message);
                return View();
            }
        }
    }
}
