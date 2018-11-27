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
    public class ReservasController : Controller
    {
        private ReservaDBContext db = new ReservaDBContext();
        private RestauranteDBContext restDb = new RestauranteDBContext();
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: Reservas
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Reservas.OrderBy(reserva => reserva.DataReserva).ToList());
            }
            else if (User.IsInRole("Gerente"))
            {
                var reservas = db.Reservas.ToList();
                var restaurantes = from Restaurante in restDb.Restaurantes
                                   where (Restaurante.Gerente == this.User.Identity.Name)
                                   orderby (Restaurante.Nome)
                                   select Restaurante;
                List<Reserva> reservasMeusRestaurantes = new List<Reserva>();
                foreach (var reserva in reservas)
                {
                    foreach (var restaurante in restaurantes)
                    {                        
                        if (reserva.IdRestaurante == restaurante.ID)
                        {
                            reservasMeusRestaurantes.Add(reserva);
                        }
                    }
                }
                reservasMeusRestaurantes.OrderBy(c => c.NomeRestaurante);
                return View(reservasMeusRestaurantes);
            }
            else if (User.IsInRole("Colaborador"))
            {
                var reservas = db.Reservas.ToList();
                var restaurantes = restDb.Restaurantes.Where(r => r.Funcionarios.Contains(this.User.Identity.Name));
                List<Reserva> reservasMeusRestaurantes = new List<Reserva>();
                foreach (var reserva in reservas)
                {
                    foreach (var restaurante in restaurantes)
                    {
                        if (reserva.IdRestaurante == restaurante.ID)
                        {
                            reservasMeusRestaurantes.Add(reserva);
                        }
                    }
                }
                reservasMeusRestaurantes.OrderBy(c => c.NomeRestaurante);
                return View(reservasMeusRestaurantes);
            }
            else if (User.IsInRole("Cliente"))
            {
                var reservas = from Reserva in db.Reservas
                                where (Reserva.NomeCliente == this.User.Identity.Name)
                                orderby(Reserva.DataReserva)
                                select Reserva;
                
                return View(reservas);
            }
            // se nenhum perfil estiver cadastrado retorna lista com todas as reservas
            return View(db.Reservas.ToList());
        }

        // GET: Reservas
        [Authorize(Roles = "Admin, Gerente, Colaborador")]
        public ActionResult ReservasRestaurante(int? RestauranteId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Gerente") || User.IsInRole("Colaborador"))
            {
                var reservas = db.Reservas.Where(c => c.IdRestaurante == RestauranteId).OrderBy(c => c.DataReserva).ToList();

                return View(reservas);
            }

            // se nenhum perfil estiver cadastrado retorna lista com todas as reservas
            return View(db.Reservas.ToList());
        }

        // GET: Reservas/Details/5
        [Authorize(Roles = "Admin, Gerente, Colaborador, Cliente")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reservas.Find(id);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            return View(reserva);
        }

        // GET: Reservas/Create
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        public ActionResult Create(int? PratoId)
        {
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        //public ActionResult Create([Bind(Include = "ID,IdCliente,IdRestaurante,IdPrato,DataReserva,HoraReserva,Quantidade")] Reserva reserva)
        public ActionResult Create([Bind(Include = "ID,IdCliente,IdRestaurante,IdPrato,DataReserva,HoraReserva,Quantidade")] Reserva reserva, int? PratoId)
        {
            
            if (ModelState.IsValid)
            {
                PratoDBContext dbPratos = new PratoDBContext();
                Prato prato = dbPratos.Pratos.Find(PratoId);
                reserva.IdCliente = this.User.Identity.Name;
                reserva.NomeCliente = this.User.Identity.Name;
                reserva.IdPrato = prato.ID;
                reserva.NomePrato = prato.Nome;
                reserva.IdRestaurante = prato.RestauranteId;
                reserva.NomeRestaurante = restDb.Restaurantes.Find(prato.RestauranteId).Nome;
                
                db.Reservas.Add(reserva);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reservas.Find(id);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        public ActionResult Edit([Bind(Include = "ID,IdCliente,IdRestaurante,IdPrato,DataReserva,HoraReserva,Quantidade")] Reserva model)
        {
            if (ModelState.IsValid)
            {
                var reserva = db.Reservas.Find(model.ID);
                reserva.DataReserva = model.DataReserva;
                reserva.HoraReserva = model.HoraReserva;
                reserva.Quantidade = model.Quantidade;
                db.Entry(reserva).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reservas.Find(id);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente, Colaborador, Cliente")]
        public ActionResult DeleteConfirmed(int id)
        {
            Reserva reserva = db.Reservas.Find(id);
            db.Reservas.Remove(reserva);
            db.SaveChanges();
            return RedirectToAction("Index");
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
