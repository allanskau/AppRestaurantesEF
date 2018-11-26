using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppRestaurantesEF.Models
{
    public class RestauranteDBContext : DbContext
    {
        public DbSet<Restaurante> Restaurantes { get; set; }
    }

    public class Restaurante
    {
        public int ID { get; set; }

        public string Gerente { get; set; }

        public string Nome { get; set; }

        public string EstiloGastronomico { get; set; }

        public string Endereco { get; set; }

        public int NroAvaliacoes { get; set; }

        public decimal SomaDasNotas { get; set; }

        public byte[] Imagem { get; set; }

        public string LinkFacebook { get; set; }

        public string ListaAvaliadores { get; set; }

        public string Funcionarios { get; set; }

    }

    public class RestauranteViewModel
    {
        public Int32 ID { get; set; }

        public String Gerente { get; set; }

        [Required(ErrorMessage = "O nome do restaurante é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome do Restaurante")]
        public String Nome { get; set; }

        [Required(ErrorMessage = "O nome do estilo gastronômico é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Estilo gastronômico do restaurante")]
        public String EstiloGastronomico { get; set; }

        [Required(ErrorMessage = "O endereço do restaurante é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Endereço")]
        public String Endereco { get; set; }

        public Int32 NroAvaliacoes { get; set; }

        public Decimal SomaDasNotas { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Imagem")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public String LinkFacebook { get; set; }

        public String ListaAvaliadores { get; set; }

        public String Funcionarios { get; set; }

    }
}