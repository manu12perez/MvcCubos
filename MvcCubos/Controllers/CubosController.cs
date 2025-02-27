using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCubos.Extensions;
using MvcCubos.Models;
using MvcCubos.Repositories;
using MvcNetCoreUtilidades.Helpers;

namespace MvcCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos repo;
        private HelperPathProvider helperPath;
        private IMemoryCache memoryCache;

        public CubosController(RepositoryCubos repo, HelperPathProvider helperPath, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.helperPath = helperPath;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(int? idCubo, int? idFavorito)
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            List<Cubo> cubosFavoritos;            

            if (idCubo != null)
            {
                List<int> idsCubos;

                if (HttpContext.Session.GetObject<List<int>>("IDSCUBOS") == null)
                {
                    idsCubos = new List<int>();
                }
                else
                {
                    idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");
                }
                idsCubos.Add(idCubo.Value);
                HttpContext.Session.SetObject("IDSCUBOS", idsCubos);

                ViewData["MENSAJE"] = "Cubos en el carrito: " + idsCubos.Count();
            }
            cubos = await this.repo.GetCubosAsync();
            return View(cubos);
        }

        public async Task<IActionResult> Details(int id)
        {
            Cubo cubo = await this.repo.FindCuboAsync(id);
            return View(cubo);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int idCubo, string nombre, string modelo, string marca, IFormFile imagen, int precio )
        {
            string fileName = imagen.FileName;
            string path = this.helperPath.MapPath(fileName, Folders.Images);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            Cubo cubo = new Cubo()
            {
                Nombre = nombre,
                Modelo = modelo,
                Marca = marca,
                Imagen = fileName,
                Precio = precio
            };

            await this.repo.CreateCuboAsync(cubo);
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            
            Cubo cubo = await this.repo.FindCuboAsync(id);            
            return View(cubo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int idCubo, string nombre, string modelo, string marca, IFormFile nuevaImagen, int precio)
        {
            // Obtener el cubo original para mantener la imagen si no se cambia
            Cubo existingCubo = await this.repo.FindCuboAsync(idCubo);
            if (existingCubo == null)
            {
                return NotFound();
            }

            string fileName = existingCubo.Imagen; // Mantener la imagen original por defecto

            // Si el usuario sube una nueva imagen, la guardamos en la carpeta
            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                fileName = nuevaImagen.FileName;
                string path = this.helperPath.MapPath(fileName, Folders.Images);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }
            }

            // Crear objeto actualizado
            Cubo cuboActualizado = new Cubo()
            {
                IdCubo = idCubo,
                Nombre = nombre,
                Modelo = modelo,
                Marca = marca,
                Imagen = fileName, // Imagen original o nueva
                Precio = precio
            };

            // Actualizar en la base de datos
            await this.repo.EditCuboAsync(cuboActualizado);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {            
            await this.repo.DeleteCuboAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CubosAlmacenados(int? idCubo, int? precio)
        {
            // Inicializar lista de IDs de cubos en sesión
            List<int> idsCubos = HttpContext.Session.GetObject<List<int>>("IDSCUBOS") ?? new List<int>();

            // Inicializar precio total almacenado en sesión
            int precioTotal = HttpContext.Session.GetObject<int>("precioTotal");

            // Si se recibe un idCubo, se elimina del carrito y se ajusta el precio
            if (idCubo != null)
            {
                // Buscar el cubo en la base de datos para obtener su precio
                Cubo cubo = await this.repo.FindCuboAsync(idCubo.Value);
                if (cubo != null)
                {
                    // Restar el precio del cubo eliminado
                    precioTotal -= cubo.Precio;
                }

                // Eliminar el cubo de la lista
                idsCubos.Remove(idCubo.Value);

                // Si la lista queda vacía, eliminamos la sesión
                if (!idsCubos.Any())
                {
                    HttpContext.Session.Remove("IDSCUBOS");
                    HttpContext.Session.Remove("precioTotal");
                }
                else
                {
                    // Guardamos la lista actualizada en la sesión
                    HttpContext.Session.SetObject("IDSCUBOS", idsCubos);
                    HttpContext.Session.SetObject("precioTotal", precioTotal);
                }
            }

            // Si no hay cubos en el carrito, mostramos un mensaje
            if (!idsCubos.Any())
            {
                ViewData["MENSAJE"] = "No hay cubos en el carrito";
                return View();
            }

            // Obtener los cubos almacenados en sesión
            List<Cubo> cubos = await this.repo.GetCubosSessionAsync(idsCubos);

            // Calcular el precio total si no se ha eliminado ningún cubo en esta petición
            if (idCubo == null)
            {
                precioTotal = cubos.Sum(c => c.Precio);
                HttpContext.Session.SetObject("precioTotal", precioTotal);
            }

            ViewData["PRECIO_TOTAL"] = precioTotal;

            return View(cubos);
        }
    }
}
