using Microsoft.AspNetCore.Mvc;
using MvcCubos.Models;
using MvcCubos.Repositories;
using MvcNetCoreUtilidades.Helpers;

namespace MvcCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos repo;
        private HelperPathProvider helperPath;

        public CubosController(RepositoryCubos repo, HelperPathProvider helperPath)
        {
            this.repo = repo;
            this.helperPath = helperPath;
        }

        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
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
    }
}
