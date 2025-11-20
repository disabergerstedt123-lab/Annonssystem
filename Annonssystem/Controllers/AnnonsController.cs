using Annonssystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Annonssystem.Controllers
{
    public class AnnonsController : Controller
    {
        private readonly adMethods adMethods;
        private readonly annonsorMethods annonsorMethods;

        public AnnonsController(IConfiguration configuration)
        {
            adMethods = new adMethods(configuration);
            annonsorMethods = new annonsorMethods(configuration);
        }

        public async Task<IActionResult> VisaAnnonserAsync()
        {

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5072/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            List<adDetails> adList = new List<adDetails>();

            List<adForListDetails> adsForList = new List<adForListDetails>();

            adList = adMethods.GetAllAds(out string errormsg);

            for (int i = 0; i < adList.Count; i++)
            {
                if (adList[i].ad_an_orgNr != null)
                {
                    annonsorDetails annonsor = new annonsorDetails();

                    annonsor = annonsorMethods.GetOneAnnonsor(adList[i].ad_an_orgNr.Value, out string annonsorErrormsg);

                    adForListDetails adForList = new adForListDetails
                    {
                        ad_id = adList[i].ad_id,
                        ad_rubrik = adList[i].ad_rubrik,
                        ad_innehall = adList[i].ad_innehall,
                        ad_pris = adList[i].ad_pris,
                        ad_annonsPris = adList[i].ad_annonsPris,
                        ad_an_orgNr = adList[i].ad_an_orgNr,
                        an_namn = annonsor.an_namn,
                        an_teleNr = annonsor.an_teleNr
                    };

                    adsForList.Add(adForList);
                }
                else if (adList[i].ad_pr_preNr != null)
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"Prenumeranter/prenumerant/{adList[i].ad_pr_preNr}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        PrenumerantDetails prenumerant = JsonConvert.DeserializeObject<PrenumerantDetails>(apiResponse);

                        adForListDetails adForList = new adForListDetails
                        {
                            ad_id = adList[i].ad_id,
                            ad_rubrik = adList[i].ad_rubrik,
                            ad_innehall = adList[i].ad_innehall,
                            ad_pris = adList[i].ad_pris,
                            ad_annonsPris = adList[i].ad_annonsPris,
                            ad_pr_preNr = adList[i].ad_pr_preNr,
                            an_namn = prenumerant.pr_namn,
                            an_teleNr = prenumerant.pr_teleNr
                        };

                        adsForList.Add(adForList);
                    }
                }

            }


            return View(adsForList);
        }
        [HttpGet]
        public IActionResult ValjKundTyp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> valjTyp(string kundTyp, string prenumerantId)
        {
            if (kundTyp == "Prenumerant")
            {
                if (string.IsNullOrWhiteSpace(prenumerantId))
                {
                    ViewBag.ErrorMessage = "Du måste skriva ett prenumerantnummer";
                    return View("ValjKundTyp");
                }

 
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://localhost:5072/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync($"Prenumeranter/prenumerant/{prenumerantId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Ej prenumerant";
                    return View("ValjKundTyp");
                }

                return RedirectToAction("SkapaAnnons", new { preNr = prenumerantId });
            }
            else if (kundTyp == "Foretag")
            {
                return RedirectToAction("SkapaForetag");
            }
            else
            {
                return RedirectToAction("ValjKundTyp");
            }
        }

        [HttpGet]
        public IActionResult SkapaForetag()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SkapaForetag(annonsorDetails annonsor)
        {
            annonsorDetails annonsorFinns = annonsorMethods.GetOneAnnonsor(annonsor.an_orgNr, out string errormsg);

            if(annonsorFinns.an_orgNr == 0)
            {
                annonsorMethods.createAnnonsor(annonsor, out string CreateErrormsg);
            }
            

                return RedirectToAction("SkapaAnnons", new { orgNr = annonsor.an_orgNr });
        }
        
        [HttpGet]
        public IActionResult SkapaAnnons(int? preNr, int? orgNr)
        {
            var adNr = new adDetails
            {
                ad_pr_preNr = preNr,
                ad_an_orgNr = orgNr
            };

            annonsorDetails annonsor = new annonsorDetails();
            if (orgNr != null)
            {
                annonsor = annonsorMethods.GetOneAnnonsor(orgNr.Value, out string errormsg);
                ViewBag.Annonsor = annonsor;
            }
            else if (preNr != null)
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://localhost:5072/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync($"Prenumeranter/prenumerant/{preNr}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    PrenumerantDetails prenumerant = JsonConvert.DeserializeObject<PrenumerantDetails>(apiResponse);
                    ViewBag.Prenumerant = prenumerant;
                }
            }

            return View(adNr);
        }

        [HttpPost]
        public IActionResult SkapaAnnons(adDetails ad)
        {
            string errormsg;
            adMethods.SkapaAnnons(ad,out errormsg);

            return RedirectToAction("VisaAnnonser");

        }   

        [HttpGet]
        public IActionResult EditPrenumerant(int preNr)
        {
            return View();
        }
    }
}
