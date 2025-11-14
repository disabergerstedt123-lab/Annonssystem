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
    }
}
