using System.Data;
using Microsoft.Data.SqlClient;

namespace Annonssystem.Models
{
    public class adMethods
    {
        private readonly string _connectionstring;

        public adMethods(IConfiguration connectionstring)
        {
            _connectionstring = connectionstring.GetConnectionString("DefaultConnection");
        }

        public List<adDetails> GetAllAds(out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionstring))
            {
                String sqlQuery = "SELECT * FROM tbl_ads";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataSet = new DataSet();
                List<adDetails> adList = new List<adDetails>();

                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "adList");

                    int i = 0;
                    int count = 0;

                    count = dataSet.Tables["adList"].Rows.Count;

                    if (i < count)
                    {
                        while (i < count)
                        {
                            adDetails adDetails = new adDetails();
                            adDetails.ad_id = Convert.ToInt32(dataSet.Tables["adList"].Rows[i]["ad_id"]);
                            adDetails.ad_rubrik = dataSet.Tables["adList"].Rows[i]["ad_rubrik"].ToString();
                            adDetails.ad_innehall = dataSet.Tables["adList"].Rows[i]["ad_innehall"].ToString();
                            adDetails.ad_pris = Convert.ToInt32(dataSet.Tables["adList"].Rows[i]["ad_pris"]);
                            adDetails.ad_annonsPris = Convert.ToInt32(dataSet.Tables["adList"].Rows[i]["ad_annonsPris"]);

                            if(dataSet.Tables["adList"].Rows[i]["ad_pr_preNr"] != DBNull.Value)
                            {
                                adDetails.ad_pr_preNr = Convert.ToInt32(dataSet.Tables["adList"].Rows[i]["ad_pr_preNr"]);
                            }
                            else if (dataSet.Tables["adList"].Rows[i]["ad_an_orgNr"] != DBNull.Value) 
                            {
                                adDetails.ad_an_orgNr = Convert.ToInt32(dataSet.Tables["adList"].Rows[i]["ad_an_orgNr"]);
                            }

                                

                            adList.Add(adDetails);
                            i++;
                        }
                        errormsg = "";
                        return adList;
                    }
                    else
                    {
                        errormsg = "Inga annonser hittades.";
                        return adList;
                    }
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return adList;
                }
            }
        }

        // KOLLA UPP VAD FAN SELECT SCOPE_IDENTITY() GÖR??? Den autolades till av VS. Kanske tar ut bara en av prenr eller orgnr baserat på något? men identity idk
        public adDetails SkapaAnnons(adDetails ad, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionstring))
            {
                String sqlQuery = "INSERT INTO tbl_ads (ad_rubrik, ad_innehall, ad_pris, ad_annonsPris, ad_pr_preNr, ad_an_orgNr) " +
                                  "VALUES (@rubrik, @innehall, @pris, @annonsPris, @pr_preNr, @an_orgNr); " +
                                  "SELECT SCOPE_IDENTITY();";

                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@rubrik", ad.ad_rubrik);
                sqlCommand.Parameters.AddWithValue("@innehall", ad.ad_innehall);
                sqlCommand.Parameters.AddWithValue("@pris", ad.ad_pris);
                sqlCommand.Parameters.AddWithValue("@annonsPris", ad.ad_annonsPris);
                sqlCommand.Parameters.AddWithValue("@pr_preNr", (object?)ad.ad_pr_preNr ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@an_orgNr", (object?)ad.ad_an_orgNr ?? DBNull.Value);
                try
                {
                    sqlConnection.Open();
                    object result = sqlCommand.ExecuteScalar();
                    ad.ad_id = Convert.ToInt32(result);
                    errormsg = "";
                    return ad;
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return ad;
                }
            }
        }


    }
}
