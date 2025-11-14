namespace Annonssystem.Models
{
    public class adForListDetails
    {
        public int ad_id { get; set; }
        public string ad_rubrik { get; set; }
        public string ad_innehall { get; set; }
        public int ad_pris { get; set; }
        public int ad_annonsPris { get; set; }
        public int? ad_pr_preNr { get; set; }
        public int? ad_an_orgNr { get; set; }
        public string an_namn { get; set; }
        public string an_teleNr { get; set; }
    }
}
