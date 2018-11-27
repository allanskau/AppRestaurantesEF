using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppRestaurantesEF.Models
{
    public class MensagemDBContext : DbContext
    {
        public DbSet<Mensagem> Mensagens { get; set; }
    }
    public class Mensagem
    {
        public int ID { get; set; }

        public string UserName { get; set; }

        public string Restaurante { get; set; }

        public string Texto { get; set; }

        public DateTime? DataUltimaAlteracao { get; set; }

    }
}