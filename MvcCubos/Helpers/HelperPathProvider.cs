using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace MvcNetCoreUtilidades.Helpers
{
    //VAMOS A OFRECER EN PROGRAMACION UNA ENUMERACION 
    //CON LAS CARPETAS DE NUESTRO SERVIDOR
    public enum Folders { Images, Facturas, Uploads, Temporal }

    public class HelperPathProvider
    {
        private IHttpContextAccessor httpContextAccessor;
        private IWebHostEnvironment hostEnvironment;
        private IServer server;

        public HelperPathProvider(IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, IServer server)
        {
            this.hostEnvironment = hostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.server = server;
        }

        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.Images)
            {
                carpeta = "images";
            }
            else if (folder == Folders.Facturas)
            {
                carpeta = "facturas";
            }
            else if (folder == Folders.Uploads)
            {
                carpeta = "uploads";
            }
            else if (folder == Folders.Temporal)
            {
                carpeta = "temp";
            }
            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);

            return path;
        }

        public string MapUrlPath(string fileName, Folders folder)
        {
            var request = this.httpContextAccessor.HttpContext.Request;
            string url = $"{request.Scheme}://{request.Host}";
            string carpeta = "";
            if (folder == Folders.Images)
            {
                carpeta = "images";
            }
            else if (folder == Folders.Facturas)
            {
                carpeta = "facturas";
            }
            else if (folder == Folders.Uploads)
            {
                carpeta = "uploads";
            }
            else if (folder == Folders.Temporal)
            {
                carpeta = "temp";
            }
            var adresses = this.server.Features.Get<IServerAddressesFeature>().Addresses;
            string serverUrl = adresses.FirstOrDefault();
            string urlPath = serverUrl + "/" + carpeta + "/" + fileName;

            return urlPath;
        }
    }
}
