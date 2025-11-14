using Annonssystem.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult VisaAnnonser()
        {
            List<adDetails> adList = new List<adDetails>();

            List<adForList> adsForList = new List<adForList>();

            adList = adMethods.GetAllAds(out string errormsg);

            for(int i = 0; i < adList.Count; i++)
            {
                if(adList[i].ad_an_orgNr != null)
                {
                    annonsorDetails annonsor = new annonsorDetails();

                    annonsor = annonsorMethods.GetOneAnnonsor(adList[i].ad_an_orgNr.Value, out string annonsorErrormsg);

                    adForList adForList = new adForList
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
            }


            return View(adsForList);
        }
    }
}
