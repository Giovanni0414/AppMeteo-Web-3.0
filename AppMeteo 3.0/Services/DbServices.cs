using AppMeteo_3._0.Models;
using System.Data;
using System.Data.SqlClient;

namespace AppMeteo_3._0.Services
{
    public class DbServices
    {
        private readonly IConfiguration _configuration;

        public DbServices(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        #region Registrazione Utente
        public bool RegistrazioneUtente(string email, string password, string nomeUtente, int stato)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO Utenti (" +
                        "NomeUtente, Email, Password, Stato) VALUES (@NomeUtente, @Email, @Password, @Stato)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@NomeUtente", nomeUtente);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Stato", stato);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception($"Errore durante la registrazione dell'utente: {ex.Message}");
                }
            }
        }
        #endregion

        #region Accesso utente
        public bool AccessoUtente(string nomeUtente, string password)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Utenti WHERE Password = @Password AND NomeUtente = @NomeUtente AND Stato = 0";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@NomeUtente", nomeUtente);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception($"Errore durante l'accesso dell'utente: {ex.Message}");
                }

            }
        }
        #endregion

        #region ottieni id

        public int OttieniId(string nomeUtente, string password)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");
            int idUtente = -1;

            using (SqlConnection conn = new SqlConnection(stringaConnessione))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT Id FROM Utenti WHERE NomeUtente = @NomeUtente AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NomeUtente", nomeUtente);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idUtente = reader.GetInt32(reader.GetOrdinal("Id"));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Errore durante il recupero dell'ID dell'utente: {ex.Message}");
                }
            }

            return idUtente;
        }


        #endregion


        #region eliminare utente dal database
        public bool EliminaUtente(string nomeUtente, string password)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");


            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    // Verifica che l'utente esista prima di eliminare
                    string verificaQuery = "SELECT COUNT(*) FROM Utenti WHERE NomeUtente = @NomeUtente AND Password = @Password";
                    using (SqlCommand verificaCommand = new SqlCommand(verificaQuery, connection))
                    {
                        verificaCommand.Parameters.AddWithValue("@Password", password);
                        verificaCommand.Parameters.AddWithValue("@NomeUtente", nomeUtente);

                        int count = (int)verificaCommand.ExecuteScalar();

                        if (count == 0)
                        {
                            throw new Exception("Utente non trovato.");
                        }
                    }
                    //UPDATE Utenti Set Stato = 1 WHERE NomeUtente = @NomeUtente AND Password = @Password
                    string eliminaQuery = "UPDATE Utenti Set Stato = 1 WHERE NomeUtente = @NomeUtente AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(eliminaQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@NomeUtente", nomeUtente);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception($"Errore durante l'eliminazione dell'utente: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion


        #region cambio password

        public bool CambiaPassword(string email, string nuovaPassword)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();


                    string queryVerifica = "SELECT COUNT(*) FROM Utenti WHERE Email = @Email";

                    using (SqlCommand verificaCmd = new SqlCommand(queryVerifica, connection))
                    {
                        verificaCmd.Parameters.AddWithValue("@Email", email);

                        int count = (int)verificaCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            throw new Exception("Utente non esistente.");
                        }
                    }


                    string queryUpdate = "UPDATE Utenti SET Password = @NuovaPassword WHERE Email = @Email";

                    using (SqlCommand updateCmd = new SqlCommand(queryUpdate, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@NuovaPassword", nuovaPassword);
                        updateCmd.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Errore durante il cambio password: " + ex.Message);

                }
                finally
                {
                    connection.Close();
                }
            }
        }


        #endregion


        #region lista utenti
        public List<Utente> OttieniListaUtenti()
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");
            List<Utente> listaUtenti = new List<Utente>();

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Id, NomeUtente, Email, Password, Stato FROM Utenti";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int intStato = 0;
                            bool? bstato = reader["Stato"] as bool?;
                            if (bstato.HasValue)
                            {
                                intStato = bstato.Value == true ? 1 : 0;
                            }


                            Utente utente = new Utente
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NomeUtente = reader["NomeUtente"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Stato = intStato

                            };

                            listaUtenti.Add(utente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception($"Errore durante l'ottenimento della lista degli utenti: {ex.Message}");
                }
                finally
                {
                    connection.Close();

                }
                return listaUtenti;
            }
        }
        #endregion


        #region lista di informazioni
        public List<InfoMeteo> OttieniListaInformazioni()
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");
            List<InfoMeteo> listaInformazioni = new List<InfoMeteo>();

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Id, Citta, Temperatura, Meteo, DataRicerca, IdUtente FROM TabellaMeteo ORDER BY DataRicerca DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            InfoMeteo info = new InfoMeteo();
                            {
                                info.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                info.Citta = reader.IsDBNull(reader.GetOrdinal("Citta")) ? null : reader.GetString(reader.GetOrdinal("Citta"));
                                info.Temperatura = reader.IsDBNull(reader.GetOrdinal("Temperatura")) ? null : reader.GetString(reader.GetOrdinal("Temperatura"));
                                info.Meteo = reader.IsDBNull(reader.GetOrdinal("Meteo")) ? null : reader.GetString(reader.GetOrdinal("Meteo"));
                                info.DataRicerca = reader.IsDBNull(reader.GetOrdinal("DataRicerca")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("DataRicerca"));
                                info.IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente"));
                            };

                            listaInformazioni.Add(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Errore durante l'ottenimento della lista delle informazioni: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return listaInformazioni;
        }
        #endregion


        #region inserire i dati cercati

        public bool InserisciDatiUtente(string citta, string temperatura, string meteo, int idUtente)
        {
            string stringaConnessione = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(stringaConnessione))
            {
                try
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO TabellaMeteo (Citta, Temperatura, Meteo, DataRicerca, IdUtente) VALUES (@Citta, @Temperatura, @Meteo, @DataRicerca, @IdUtente)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                    {

                        insertCmd.Parameters.Add("@Citta", SqlDbType.VarChar).Value = citta;
                        insertCmd.Parameters.Add("@Temperatura", SqlDbType.VarChar).Value = temperatura;
                        insertCmd.Parameters.Add("@Meteo", SqlDbType.VarChar).Value = meteo;
                        insertCmd.Parameters.Add("@DataRicerca", SqlDbType.DateTime).Value = DateTime.Now;
                        insertCmd.Parameters.Add("@IdUtente", SqlDbType.Int).Value = idUtente;

                        int rowsAffected = insertCmd.ExecuteNonQuery();


                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }


        #endregion


    }
}
