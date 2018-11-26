using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppRestaurantesEF.Models
{
    public class ReservaDBContext : DbContext
    {
        public DbSet<Reserva> Reservas { get; set; }
    }

    public class Reserva
    {
        public int ID { get; set; }

        public string IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public int IdRestaurante { get; set; }

        public string NomeRestaurante { get; set; }

        public int IdPrato { get; set; }

        public string NomePrato { get; set; }

        [Display(Name = "Data da reserva")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime? DataReserva { get; set; }

        public string HoraReserva { get; set; }

        public int Quantidade { get; set; }

    }

    public class ReservaViewModel
    {
        public Int32 ID { get; set; }

        public String IdCliente { get; set; }

        public String NomeCliente { get; set; }

        public Int32 IdRestaurante { get; set; }

        public String NomeRestaurante { get; set; }

        public Int32 IdPrato { get; set; }

        public String NomePrato { get; set; }

        [Display(Name = "Data da reserva")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime? DataReserva { get; set; }

        public String HoraReserva { get; set; }

        public Int32 Quantidade { get; set; }

    }

}