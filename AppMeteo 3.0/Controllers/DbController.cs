using AppMeteo_3._0.Models;
using AppMeteo_3._0.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppMeteo_3._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbController : ControllerBase
    {
        private readonly DbServices _services;

        public DbController(DbServices services)
        {
            _services = services;
        }

        [HttpPost("RegistraUtente")]
        public IActionResult RegistraUtente([FromBody] Utente utente)
        {
            try
            {
                // Validazione dell'input
                if (utente == null)
                {
                    return BadRequest("Dati utente non validi.");
                }

                var result = _services.RegistrazioneUtente(utente.Email, utente.Password, utente.NomeUtente, utente.Stato);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante la registrazione dell'utente: {ex.Message}");
            }
        }

        [HttpPost("AccessoUtente")]
        public IActionResult AccessoUtente([FromBody] Utente utente)
        {
            try
            {
                if (utente == null)
                {
                    return BadRequest("Dati utente non validi.");
                }

                // Controlla se l'utente esiste nel sistema e se le credenziali sono corrette
                int idUtente = _services.OttieniId(utente.NomeUtente, utente.Password);
                if (idUtente != -1)
                {
                    // Se l'utente è stato autenticato correttamente, restituisci l'ID dell'utente
                    return Ok(new { id = idUtente });
                }
                else
                {
                    // Se le credenziali sono errate, restituisci un messaggio di errore
                    return BadRequest("Nome utente o password non validi.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'accesso dell'utente: {ex.Message}");
            }
        }



        [HttpDelete("EliminaUtente")]
        public IActionResult EliminaUtente([FromBody] Utente utente)
        {
            try
            {
                if (utente == null)
                {
                    return BadRequest("Dati utente non validi.");
                }
                var result = _services.EliminaUtente(utente.NomeUtente, utente.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante l'eliminazione dell'utente: {ex.Message}");
            }
        }


        [HttpPost("CambiaPassword")]

        public IActionResult CambiaPassword([FromBody] Utente utente)
        {
            try
            {
                if (utente == null)
                {
                    return BadRequest("email o password non validi.");
                }
                var result = _services.CambiaPassword(utente.Email, utente.Password);
                return Ok(result);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost("InserisciDatiUtente")]
        public IActionResult InserisciDatiUtente([FromBody] InfoMeteo info)
        {
            try
            {
                bool inserito = _services.InserisciDatiUtente(info.Citta, info.Temperatura, info.Meteo, info.IdUtente);
                if (inserito)
                {
                    return Ok("Dati utente inseriti correttamente.");
                }
                else
                {
                    return BadRequest("Impossibile inserire i dati utente.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Errore durante l'inserimento dei dati utente: " + ex.Message);
            }
        }

        [HttpGet("OttieniId")]
        public IActionResult OttieniId([FromBody] Utente utente)
        {
            try
            {
                int id = _services.OttieniId(utente.NomeUtente, utente.Password);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Errore durante l'ottenimento dell'id " + ex.Message);
            }
        }

    }
}

