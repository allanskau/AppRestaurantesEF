using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using AppRestaurantesEF.Models;
using System.Net;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace AppRestaurantesEF.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext context;
        UserManager<ApplicationUser> _userManager;
        private RestauranteDBContext restDb = new RestauranteDBContext();

        public RolesController()
        {
            context = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        }

        //[Authorize(Roles = "Admin, Gerente")]
        public ActionResult RoleUsers()
        {
            if (this.User.IsInRole("Admin"))
            {
                var usuariosComPerfis = (from user in context.Users
                                         select new
                                         {
                                             UserId = user.Id,
                                             Username = user.UserName,
                                             Email = user.Email,
                                             RoleNames = (from userRole in user.Roles
                                                          join role in context.Roles on userRole.RoleId
                                                          equals role.Id
                                                          select role.Name).ToList()
                                         }).ToList().Select(p => new UsersInRoleViewModel()
                                         {
                                             UserId = p.UserId,
                                             Username = p.Username,
                                             Email = p.Email,
                                             Role = string.Join(",", p.RoleNames)
                                         });
                return View(usuariosComPerfis);
            }
            // Se for Gerente mostra apenas usuarios com perfil de cliente e colaborador
            else
            {
                var usuariosComPerfis = (from user in context.Users
                                         select new
                                         {
                                             UserId = user.Id,
                                             Username = user.UserName,
                                             Email = user.Email,
                                             RoleNames = (from userRole in user.Roles
                                                          join role in context.Roles on userRole.RoleId
                                                          equals role.Id
                                                          select role.Name).ToList()
                                         }).ToList().Where(r => !(r.RoleNames.Contains("Admin")) && !(r.RoleNames.Contains("Gerente"))).Select(p => new UsersInRoleViewModel()
                                         {
                                             UserId = p.UserId,
                                             Username = p.Username,
                                             Email = p.Email,
                                             Role = string.Join(",", p.RoleNames)
                                         });
                return View(usuariosComPerfis);
            }
            
        }

        //[Authorize(Roles = "Admin, Gerente")]
        public ActionResult Clientes()
        {
            var clientes = (from user in context.Users
                                     select new
                                     {
                                         UserId = user.Id,
                                         Username = user.UserName,
                                         Email = user.Email,
                                         RoleNames = (from userRole in user.Roles
                                                      join role in context.Roles on userRole.RoleId
                                                      equals role.Id
                                                      select role.Name).ToList()
                                     }).ToList().Where(r => !(r.RoleNames.Contains("Admin")) 
                                                        && !(r.RoleNames.Contains("Gerente")) 
                                                        && !(r.RoleNames.Contains("Colaborador"))).Select(p => new UsersInRoleViewModel()
                                     {
                                         UserId = p.UserId,
                                         Username = p.Username,
                                         Email = p.Email,
                                         Role = string.Join(",", p.RoleNames)
                                     });
            return View(clientes);           
        }

        //[Authorize(Roles = "Admin, Gerente")]
        public ActionResult Colaboradores()
        {
            if(restDb == null || restDb.Restaurantes.Where(r => r.Gerente.Equals(this.User.Identity.Name)) == null)
            {
                List<UsersInRoleViewModel> c = new List<UsersInRoleViewModel>();
                return View(c); // retorna lista vazia
            }
            else
            {
                var restaurantes = restDb.Restaurantes.Where(r => r.Gerente.Equals(this.User.Identity.Name));
                var colaboradores = (from user in context.Users
                                     select new
                                     {
                                         UserId = user.Id,
                                         Username = user.UserName,
                                         Email = user.Email,
                                         RoleNames = (from userRole in user.Roles
                                                      join role in context.Roles on userRole.RoleId
                                                      equals role.Id
                                                      select role.Name).ToList()
                                     }).ToList().Where(r => (r.RoleNames.Contains("Colaborador"))).Select(p => new UsersInRoleViewModel()
                                     {
                                         UserId = p.UserId,
                                         Username = p.Username,
                                         Email = p.Email,
                                         Role = string.Join(",", p.RoleNames)
                                     });
                List<UsersInRoleViewModel> c = new List<UsersInRoleViewModel>();
                foreach (var colaborador in colaboradores)
                {
                    foreach (var restaurante in restaurantes)
                    {
                        if (restaurante.Funcionarios.Contains(colaborador.Username))
                        {
                            c.Add(colaborador); // adiciona colaborador para mostrar na view
                        }
                    }
                }

                return View(c);
            }
            
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var roles = context.Roles.ToList().OrderBy(r => r.Name);
            ViewBag.Roles = roles;
            return View(roles);
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Role/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole Role = context.Roles.Find(id);
            if (Role == null)
            {
                return HttpNotFound();
            }
            return View(Role);
        }

        // POST: Role/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var role = context.Roles.Find(model.Id);
                role.Name = model.Name;
                context.Entry(role).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Role/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole Role = context.Roles.Find(id);
            if (Role == null)
            {
                return HttpNotFound();
            }
            return View(Role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(IdentityRole model)
        {
            IdentityRole Role = context.Roles.Find(model.Id);
            context.Roles.Remove(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UsuariosComPerfis()
        {
            var usuariosComPerfis = (from user in context.Users
                                     select new
                                     {
                                         UserId = user.Id,
                                         Username = user.UserName,
                                         Email = user.Email,
                                         RoleNames = (from userRole in user.Roles
                                                      join role in context.Roles on userRole.RoleId
                                                      equals role.Id
                                                      select role.Name).ToList()
                                     }).ToList().Select(p => new UsersInRoleViewModel()
                                     {
                                         UserId = p.UserId,
                                         Username = p.Username,
                                         Email = p.Email,
                                         Role = string.Join(",", p.RoleNames)
                                     });
            return View(usuariosComPerfis);
        }

        //[Authorize(Roles = "Admin, Gerente")]
        public ActionResult TelaAtribuicao(string id)
        {
            if (this.User.IsInRole("Admin"))
            { 
                var Usuario = context.Users.Find(id);
                List<string> user = new List<string>();
                user.Add(Usuario.UserName);
                ViewBag.UserName = new SelectList(user);
                ViewBag.Roles = new SelectList(context.Roles.ToList(), "Name", "Name");
                return View();
            }
            //Se for gerente
            else
            {
                var Usuario = context.Users.Find(id);
                List<string> user = new List<string>();
                user.Add(Usuario.UserName);
                ViewBag.UserName = new SelectList(user);
                // Seleciona as roles, menos admin
                ViewBag.Roles = new SelectList(context.Roles.Where(r => !(r.Name.Equals("Admin")) && !(r.Name.Equals("Gerente"))).ToList(), "Name", "Name");
                return View();
            }

        }

        [Authorize(Roles = "Admin, Gerente")]
        public ActionResult TelaRemocao(string id)
        {
            var Usuario = context.Users.Find(id);
            var roles = Usuario.Roles.ToList();
            var nomePerfis = new List<string>();
            var aux1 = new List<string>();
            aux1.Add(Usuario.UserName);
            foreach (var role in roles)
            {
                IdentityRole aux = context.Roles.Find(role.RoleId);
                nomePerfis.Add(aux.Name); 
            }
            ViewBag.UserName =  new SelectList(aux1);
            ViewBag.Roles = new SelectList(nomePerfis);
           
            return View();
        }

        [HttpPost, ActionName("AtribuirPerfil")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin, Gerente")]
        public ActionResult AtribuirPerfil(string UserName, string Roles)
        {
            if (this.User.IsInRole("Admin"))
            {
                var Usuario = _userManager.FindByName(UserName);
                _userManager.AddToRoleAsync(Usuario.Id, Roles);
                context.SaveChanges();
                return RedirectToAction("RoleUsers");
            }
            // Se for gerente
            else
            {
                var restaurantes = restDb.Restaurantes.Where(r => r.Gerente.Equals(this.User.Identity.Name));
                foreach(var restaurante in restaurantes)
                {
                    // adiciona usuario na lista de funcionarios dos restaurantes que este gerente for responsavel
                    restaurante.Funcionarios += " + " + UserName;
                    restDb.Entry(restaurante).State = EntityState.Modified;
                    
                }
                var Usuario = _userManager.FindByName(UserName);
                _userManager.AddToRoleAsync(Usuario.Id, Roles);
                restDb.SaveChanges();
                context.SaveChanges();
                return RedirectToAction("RoleUsers");
            }
            
        }

        [HttpPost, ActionName("RemoverPerfil")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Gerente")]
        public ActionResult RemoverPerfil(string UserName, string Roles)
        {
            var Usuario = _userManager.FindByName(UserName);
            _userManager.RemoveFromRoleAsync(Usuario.Id, Roles);
            context.SaveChanges();
            return RedirectToAction("RoleUsers");
        }

    }
}