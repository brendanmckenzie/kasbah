using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kasbah.Web.Controllers.Management
{
    public class StaticContentController : Controller
    {
        static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            { ".js", "application/x-javascript" },
            { ".html", "text/html" },
            { ".txt", "text/plain" },
            { ".css", "text/css" },
            { ".ico", "image/x-icon" },
            { ".eot", "application/octet-stream" },
            { ".ttf", "application/octet-stream" },
        };

        readonly ILogger<StaticContentController> _logger;

        public StaticContentController(ILogger<StaticContentController> logger)
        {
            _logger = logger;
        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        [Route("/{*path}")]
        public ActionResult StaticContent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "index.html";
            }

            var data = MapPathToResource(path);

            if (data == null)
            {
                path = "index.html";
                data = MapPathToResource(path);
            }

            if (data == null)
            {
                return NotFound();
            }

            return new FileContentResult(data, GetMimeType(path.Split('.').Last()));
        }

        byte[] MapPathToResource(string path)
        {
            var assembly = typeof(StaticContentController).GetTypeInfo().Assembly;
            const string Prefix = "Kasbah.Web.Management.Ui";
            var resources = assembly.GetManifestResourceNames();
            var resPath = $"{Prefix}.{path}".Replace("/", ".");

            if (resources.Contains(resPath))
            {
                using (var stream = assembly.GetManifestResourceStream(resPath))
                {
                    var buffer = new byte[stream.Length];

                    stream.Read(buffer, 0, buffer.Length);

                    return buffer;
                }
            }
            else
            {
                _logger.LogInformation($"Resource {resPath} not found");
                _logger.LogInformation("Available resources");
                _logger.LogInformation(string.Join(Environment.NewLine, resources));
            }

            return null;
        }
    }
}
