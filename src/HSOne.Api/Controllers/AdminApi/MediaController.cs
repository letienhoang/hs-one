using HSOne.Core.ConfigOptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace HSOne.Api.Controllers.AdminApi
{
    [Route("api/admin/media")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly MediaSettings _mediaSettings;

        public MediaController(IWebHostEnvironment webHostEnvironment, IOptions<MediaSettings> mediaSettings)
        {
            _hostingEnvironment = webHostEnvironment;
            _mediaSettings = mediaSettings.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult UploadImage(string type, string newFileName)
        {
            var allowImageTypes = _mediaSettings.AllowImageFileTypes?.Split(",");

            var now = DateTime.Now;
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return BadRequest("No file uploaded");
            }

            var file = files[0];
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)?.FileName?.Trim('"');
            if (allowImageTypes?.Any(x => fileName?.EndsWith(x, StringComparison.OrdinalIgnoreCase) == true) == false)
            {
                return BadRequest("Non-image file uploads are not allowed.");
            }

            var fileExtension = Path.GetExtension(fileName);
            var finalFileName = !string.IsNullOrEmpty(newFileName) ? $"{newFileName}{fileExtension}" : fileName;

            var imageFolder = $@"\{_mediaSettings.ImageFolder}\images\{type}\{now:yyyyMM}";

            var folder = _hostingEnvironment.WebRootPath + imageFolder;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = Path.Combine(folder, finalFileName!);
            using (var fs = global::System.IO.File.Create(filePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            var path = Path.Combine(imageFolder, finalFileName!).Replace(@"\", @"/");
            return Ok(new { path });
        }
    }
}