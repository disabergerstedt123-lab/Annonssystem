using Annonssystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


//HEJ DISA
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

            for(int i = 0; i < adList.Count; i++)
            {
                if(adList[i].ad_an_orgNr != null)
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

                    if(response.IsSuccessStatusCode)
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
    }
}
