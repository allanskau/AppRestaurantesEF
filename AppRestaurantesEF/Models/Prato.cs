using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppRestaurantesEF.Models
{
    public class PratoDBContext : DbContext
    {
        public DbSet<Prato> Pratos { get; set; }
    }

    public class Prato
    {
        public int ID { get; set; }

        public int RestauranteId { get; set; }

        public List<string> DiasNoCardapio { get; set; }

        public string Nome { get; set; }

        public string Ingredientes { get; set; }

        public decimal Quantidade { get; set; }

        public int NroAvaliacoes { get; set; }

        public decimal SomaDasNotas { get; set; }

        public byte[] Imagem { get; set; }

        public string ListaAvaliadores { get; set; }


    }

    public class PratoViewModel
    {

        public Int32 ID { get; set; }

        public Int32 RestauranteId { get; set; }

        public List<String> DiasNoCardapio { get; set; }

        [Required(ErrorMessage = "O nome do prato é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome do Prato")]
        public String Nome { get; set; }

        [Required(ErrorMessage = "O descritivo dos ingredientes é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Ingredientes do Prato")]
        public String Ingredientes { get; set; }

        [Required(ErrorMessage = "A quantidade da porção do prato (em gramas) é obrigatória", AllowEmptyStrings = false)]
        [Display(Name = "Quantidade em gramas")]
        public Decimal Quantidade { get; set; }

        public Int32 NroAvaliacoes { get; set; }

        public Decimal SomaDasNotas { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Imagem do prato")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public String ListaAvaliadores { get; set; }


    }
}