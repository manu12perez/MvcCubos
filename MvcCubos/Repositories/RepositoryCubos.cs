using Microsoft.EntityFrameworkCore;
using MvcCubos.Data;
using MvcCubos.Models;

namespace MvcCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from datos in this.context.Cubos
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            var consulta = from datos in this.context.Cubos
                           where datos.IdCubo == idCubo
                           select datos;

            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> GetMaxIdCuboAsync()
        {
            if (!this.context.Cubos.Any())
            {
                return 1;
            }
            return await this.context.Cubos.MaxAsync(c => c.IdCubo);
        }

        public async Task CreateCuboAsync(Cubo cubo)
        {
            int newId = await GetMaxIdCuboAsync() + 1;
            cubo.IdCubo = newId;

            this.context.Cubos.Add(cubo);
            await this.context.SaveChangesAsync();
        }

        public async Task EditCuboAsync(Cubo cubo)
        {
            // Buscar el cubo existente en la base de datos
            var existingCubo = await this.context.Cubos.FindAsync(cubo.IdCubo);
            if (existingCubo != null)
            {
                // Actualizar los datos con los valores recibidos
                existingCubo.Nombre = cubo.Nombre;
                existingCubo.Modelo = cubo.Modelo;
                existingCubo.Marca = cubo.Marca;
                existingCubo.Precio = cubo.Precio;

                // Solo actualizar la imagen si se ha proporcionado una nueva
                if (!string.IsNullOrEmpty(cubo.Imagen))
                {
                    existingCubo.Imagen = cubo.Imagen;
                }

                // Guardar cambios en la base de datos
                await this.context.SaveChangesAsync();
            }
        }
    }
}
