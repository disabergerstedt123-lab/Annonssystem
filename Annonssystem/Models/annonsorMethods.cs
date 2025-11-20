using System.Data;
using Microsoft.Data.SqlClient;

namespace Annonssystem.Models
{
    public class annonsorMethods
    {
        private readonly string _connectionstring;

        public annonsorMethods(IConfiguration connectionstring)
        {
            _connectionstring = connectionstring.GetConnectionString("DefaultConnection");
        }

        public annonsorDetails GetOneAnnonsor(int id, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionstring))
            {
                String sqlQuery = "SELECT * FROM tbl_annonsorer WHERE an_orgNr=@id";

                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataSet = new DataSet();

                sqlCommand.Parameters.AddWithValue("@id", id);

                annonsorDetails annonsor = new annonsorDetails();

                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet);
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];


                        annonsor.an_orgNr = Convert.ToInt32(row["an_orgNr"]);
                        annonsor.an_namn = row["an_namn"].ToString();
                        annonsor.an_teleNr = row["an_teleNr"].ToString();
                        annonsor.an_utAdress = row["an_utAdress"].ToString();
                        annonsor.an_postNr = row["an_postNr"].ToString();
                        annonsor.an_ort = row["an_ort"].ToString();
                        annonsor.an_faktAdress = row["an_faktAdress"].ToString();
                        annonsor.an_faktPostNr = row["an_faktPostNr"].ToString();
                        annonsor.an_faktOrt = row["an_faktOrt"].ToString();
                        
                        errormsg = string.Empty;
                        return annonsor;
                    }
                    else
                    {
                        errormsg = "No records found.";
                        return annonsor;
                    }
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return annonsor;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public annonsorDetails createAnnonsor(annonsorDetails annonsor, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionstring))
            {
                String sqlQuery = "INSERT INTO tbl_annonsorer (an_orgNr, an_namn, an_teleNr, an_utAdress, an_postNr, an_ort, an_faktAdress, an_faktPostNr, an_faktOrt) " +
                                  "VALUES (@an_orgNr, @an_namn, @an_teleNr, @an_utAdress, @an_postNr, @an_ort, @an_faktAdress, @an_faktPostNr, @an_faktOrt)";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@an_orgNr", annonsor.an_orgNr);
                sqlCommand.Parameters.AddWithValue("@an_namn", annonsor.an_namn);
                sqlCommand.Parameters.AddWithValue("@an_teleNr", annonsor.an_teleNr);
                sqlCommand.Parameters.AddWithValue("@an_utAdress", annonsor.an_utAdress);
                sqlCommand.Parameters.AddWithValue("@an_postNr", annonsor.an_postNr);
                sqlCommand.Parameters.AddWithValue("@an_ort", annonsor.an_ort);
                sqlCommand.Parameters.AddWithValue("@an_faktAdress", annonsor.an_faktAdress);
                sqlCommand.Parameters.AddWithValue("@an_faktPostNr", annonsor.an_faktPostNr);
                sqlCommand.Parameters.AddWithValue("@an_faktOrt", annonsor.an_faktOrt);
                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    errormsg = string.Empty;
                    return annonsor;
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return annonsor;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

       
        
    }
}
