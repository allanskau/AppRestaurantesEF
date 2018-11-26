using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppRestaurantesEF.Models;

namespace AppRestaurantesEF.Controllers
{
    public class PratosController : Controller
    {
        private PratoDBContext db = new PratoDBContext();
        private RestauranteDBContext restDb = new RestauranteDBContext();

        // GET: Pratos
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Index(int restauranteId)
        {
            DateTime data = DateTime.Today;
            int ano = data.Year;
            int mes = data.Month;
            List<int> dias = new List<int>();
            for(int i = 1; i <= DateTime.DaysInMonth(ano,mes); i++)
            {
                var diadasemana = data.DayOfWeek.ToString();
                data = data.AddDays(i);
                //if (data.DayOfWeek.ToString().Equals))
            }

            ViewBag.RestauranteId = restauranteId;
            var restaurante = restDb.Restaurantes.Find(restauranteId);            
            var pratos = from Prato in db.Pratos
                         where (restaurante.ID == Prato.RestauranteId)
                         orderby (Prato.Nome) select Prato;
            return View(pratos);
        }

        // GET: Pratos/Details/5
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prato prato = db.Pratos.Find(id);
            if (prato == null)
            {
                return HttpNotFound();
            }
            Restaurante restaurante = restDb.Restaurantes.Find(prato.RestauranteId);
            ViewBag.LinkFace = restaurante.LinkFacebook;
            return View(prato);
        }

        //// GET: Pratos/Create
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult Create(int RestauranteId)
        {
            //var Restaurante = from restaurante in restDb.Restaurantes
            //                  where (restaurante.Gerente == this.User.Identity.Name 
            //                  && restaurante.ID == RestauranteId)
            //                  select restaurante;
            //ViewBag.Restaurante = new SelectList(Restaurante, "Nome", "Nome");
            List<int> auxId = new List<int>();
            auxId.Add(RestauranteId);
            ViewBag.RestauranteId = new SelectList(auxId);
            var model = new PratoViewModel();
            return View(model);
        }

        // POST: Pratos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult Create(PratoViewModel model)
        {
            var imageTypes = new string[]{
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };
            if (model.ImageUpload == null || model.ImageUpload.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "Este campo é obrigatório");
            }
            else if (!imageTypes.Contains(model.ImageUpload.ContentType))
            {
                ModelState.AddModelError("ImageUpload", "Escolha uma iamgem GIF, JPG ou PNG.");
            }
            if (ModelState.IsValid)
            {
                // localiza restaurante e pega id para redirecionar para index
                //var restaurante = (Restaurante) from r in restDb.Restaurantes where (r.Nome == model.Restaurante) select r;
                var prato = new Prato();
                prato.RestauranteId = model.RestauranteId;
                prato.Nome = model.Nome;
                prato.Ingredientes = model.Ingredientes;
                prato.Quantidade = model.Quantidade;
                prato.ListaAvaliadores = "";
                
                //lemos a imagem e a seguir os bytes armazenados
                using (var binaryReader = new System.IO.BinaryReader(model.ImageUpload.InputStream))
                    prato.Imagem = binaryReader.ReadBytes(model.ImageUpload.ContentLength);
                db.Pratos.Add(prato);
                db.SaveChanges();
                return RedirectToAction("Index", new { restauranteId = model.RestauranteId });
                //return RedirectToAction("MeusRestaurantes", "Restaurantes", null);
            }
            // Se ocorrer um erro retorna para pagina
            return View(model);

        }

        // GET: Pratos/Edit/5
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prato prato = db.Pratos.Find(id);
            if (prato == null)
            {
                return HttpNotFound();
            }
            return View(prato);
        }

        // POST: Pratos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult Edit([Bind(Include = "ID,Nome,Ingredientes,Quantidade")] Prato model)
        {
            if (ModelState.IsValid)
            {
                var prato = db.Pratos.Find(model.ID);
                prato.Nome = model.Nome;
                prato.Ingredientes = model.Ingredientes;
                prato.Quantidade = model.Quantidade;
                db.Entry(prato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { RestauranteId = prato.RestauranteId});
            }
            return View(model);
        }

        // GET: Pratos/Delete/5
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prato prato = db.Pratos.Find(id);
            if (prato == null)
            {
                return HttpNotFound();
            }
            return View(prato);
        }

        // POST: Pratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador")]
        public ActionResult DeleteConfirmed(int id)
        {
            Prato prato = db.Pratos.Find(id);
            db.Pratos.Remove(prato);
            db.SaveChanges();
            return RedirectToAction("Index", new { RestauranteId = prato.RestauranteId });
        }


        // GET: Pratos/Avaliar/5
        [Authorize(Roles = "Cliente")]
        public ActionResult Avaliar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prato prato = db.Pratos.Find(id);
            List<int> notas = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ViewBag.SomaDasNotas = new SelectList(notas);
            if (prato.ListaAvaliadores.Contains(this.User.Identity.Name))
            {
                return RedirectToAction("PratoJaAvaliado");
            }
            if (prato == null)
            {
                return HttpNotFound();
            }
            return View(prato);
        }

        // POST: Pratos/Avaliar/5
        [HttpPost, ActionName("Avaliar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public ActionResult Avaliar([Bind(Include = "ID,Nome,SomaDasNotas")] Prato model)
        {
            if (ModelState.IsValid)
            {
                Prato prato = db.Pratos.Find(model.ID);
                prato.ListaAvaliadores += " + " + this.User.Identity.Name;
                prato.SomaDasNotas += model.SomaDasNotas;
                prato.NroAvaliacoes++;
                db.Entry(prato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { RestauranteId = prato.RestauranteId });
            }
            return View(model);
        }

        // GET: Pratos/PratoJaAvaliado/5
        [Authorize(Roles = "Cliente")]
        public ActionResult PratoJaAvaliado()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
