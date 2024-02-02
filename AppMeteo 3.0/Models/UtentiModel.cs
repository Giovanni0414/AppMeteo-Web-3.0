using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppMeteo_3._0.Models
{
    public class Utente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? NomeUtente { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Stato { get; set; }

        //publi virtual InfoMeteo

    }

    public class InfoMeteo
    {
        [Key]
        public int Id { set; get; }
        [Required]
        public string? Citta { get; set; }
        public string? Temperatura { get; set; }
        public string? Meteo { get; set; }
        public DateTime? DataRicerca { get; set; }

        [ForeignKey(nameof(Utente))]
        public int IdUtente { get; set; }
        public Utente? Utente { get; set; }
    }

    public class AmministratoreViewModel
    {
        public List<Utente> ListaUtenti { get; set; }
        public List<InfoMeteo> ListaInfo { get; set; }
    }
}
