using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppRestaurantesEF.Models;

namespace AppRestaurantesEF.Controllers
{
    public class RestaurantesController : Controller
    {
        private PratoDBContext pratoDb;
        private RestauranteDBContext db;

        public RestaurantesController()
        {
            pratoDb = new PratoDBContext();
            db = new RestauranteDBContext();
        }

        // GET: Restaurantes
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Index()
        {
            if(this.User.IsInRole("Gerente") || this.User.IsInRole("Colaborador"))
            {
                return RedirectToAction("MeusRestaurantes");
            }
            var restaurantes = db.Restaurantes.ToList().OrderBy(r => r.Nome);
            return View(restaurantes);
        }

        // GET: Restaurantes
        [Authorize (Roles = "Gerente, Colaborador")]
        public ActionResult MeusRestaurantes()
        {
            if (this.User.IsInRole("Gerente"))
            {
                var restaurantes = from Restaurante in db.Restaurantes
                                   where (Restaurante.Gerente == this.User.Identity.Name)
                                   orderby (Restaurante.Nome)
                                   select Restaurante;
                return View(restaurantes);
            }
            // Se for colaborador 
            else
            {
                var rest = db.Restaurantes.ToList();
                List<Restaurante> restaurantes = new List<Restaurante>();
                foreach(var r in rest)
                {
                    // percorre lista de restaurantes e verifica se usuario faz parte da lista de funcionarios
                    if (r.Funcionarios.Contains(this.User.Identity.Name))
                    {
                        restaurantes.Add(r);
                    }
                }
                return View(restaurantes);
            }
        }

        
        [Authorize(Roles = "Gerente")]
        public ActionResult Create()
        {
            //ViewBag.Categorias = db.Restaurantes.ToList();
            var model = new RestauranteViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public ActionResult Create(RestauranteViewModel model)
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
                var restaurante = new Restaurante();
                restaurante.Gerente = User.Identity.Name;
                restaurante.Nome = model.Nome;
                restaurante.EstiloGastronomico = model.EstiloGastronomico;
                restaurante.Endereco = model.Endereco;
                restaurante.ListaAvaliadores = "";
                restaurante.Funcionarios = "";
                //restaurante.ListaIdAvaliadores.Add("");
                
                //lemos a imagem e a seguir os bytes armazenados
                using (var binaryReader = new System.IO.BinaryReader(model.ImageUpload.InputStream))
                    restaurante.Imagem = binaryReader.ReadBytes(model.ImageUpload.ContentLength);
                /////////////////////////////////////////////////
                restaurante.LinkFacebook = model.LinkFacebook;
                db.Restaurantes.Add(restaurante);
                db.SaveChanges();
                return RedirectToAction("MeusRestaurantes");
            }
            // Se ocorrer um erro retorna para pagina
            ViewBag.Categories = db.Restaurantes.ToList();
            return View(model);
        }


        // GET: Restaurantes/Edit/5
        [Authorize(Roles = "Gerente")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(id);
            if(restaurante.Gerente != this.User.Identity.Name)
            {
                return RedirectToAction("MeusRestaurantes");
            }
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: Restaurantes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public ActionResult Edit([Bind(Include = "ID,Nome,EstiloGastronomico,Endereco")] Restaurante model)
        {
            if (ModelState.IsValid)
            {
                var restaurante = db.Restaurantes.Find(model.ID);
                restaurante.Nome = model.Nome;
                restaurante.EstiloGastronomico = model.EstiloGastronomico;
                restaurante.Endereco = model.Endereco;
                db.Entry(restaurante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MeusRestaurantes");
            }
            return View(model);
        }


        // GET: Restaurantes/Delete/5
        [Authorize(Roles = "Gerente")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(id);
            if (restaurante.Gerente != this.User.Identity.Name)
            {
                return RedirectToAction("MeusRestaurantes");
            }
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: Restaurantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente")]
        public ActionResult DeleteConfirmed(int id)
        {
            // primeiro exclui pratos deste restaurante
            var pratos = from prato in pratoDb.Pratos where (prato.RestauranteId == id) select prato;
            foreach(var prato in pratos)
            {
                pratoDb.Pratos.Remove(prato);
            }
            pratoDb.SaveChanges();
            Restaurante restaurante = db.Restaurantes.Find(id);
            db.Restaurantes.Remove(restaurante);
            db.SaveChanges();
            return RedirectToAction("MeusRestaurantes");
        }

        // GET: Restaurantes/Details/5
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(id);
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // GET: Restaurantes/RestauranteJaAvaliado/5
        [Authorize(Roles = "Cliente")]
        public ActionResult RestauranteJaAvaliado()
        {
            return View();
        }

        // GET: Restaurantes/Avaliar/5
        [Authorize(Roles = "Cliente")]
        public ActionResult Avaliar(int? RestauranteId)
        {
            if (RestauranteId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurante restaurante = db.Restaurantes.Find(RestauranteId);
            List<int> notas =  new List<int> {0,1,2,3,4,5,6,7,8,9,10};
            ViewBag.SomaDasNotas = new SelectList(notas);
            if (restaurante.ListaAvaliadores.Contains(this.User.Identity.Name))
            {
                return RedirectToAction("RestauranteJaAvaliado");
            }
            if (restaurante == null)
            {
                return HttpNotFound();
            }
            return View(restaurante);
        }

        // POST: Restaurantes/Avaliar/5
        [HttpPost, ActionName("Avaliar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public ActionResult Avaliar([Bind(Include = "ID,Nome,SomaDasNotas")] Restaurante model)
        {
            if (ModelState.IsValid)
            {
                Restaurante restaurante = db.Restaurantes.Find(model.ID);
                restaurante.ListaAvaliadores += " + " + this.User.Identity.Name;
                restaurante.SomaDasNotas += model.SomaDasNotas;
                restaurante.NroAvaliacoes++;
                db.Entry(restaurante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
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