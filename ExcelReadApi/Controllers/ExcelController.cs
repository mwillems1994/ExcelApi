using ExcelReadApi.Extensions;
using ExcelReadApi.Models;
using ExcelReadApi.Services.Models;
using ExcelReadApi.Services.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExcelReadApi.Controllers
{
    public class ExcelController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IExcelService _excelService;
        public ExcelController(IWebHostEnvironment env, IExcelService excelService)
        {
            _env = env;
            _excelService = excelService;
        }
        [HttpGet("Example")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadExampleAsync() {
            var fileName = "VoorbeeldExcel.xlsx";

            // Could check for existing file instead of relying in a exception...
            /*var directoryInfo = new DirectoryInfo(_env.WebRootPath);
            if(!directoryInfo.GetFiles().Any(file => string.Compare(file.Name, fileName) == 0))
            {
                _log.LogError($"Example file cannot be found, requested file; ${_env.WebRootPath}");
                return StatusCode(StatusCodes.Status500InternalServerError); // Most fitting here i guess. 
            }
            */ 
            
            var filePath = Path.Combine(_env.ContentRootPath, fileName);

            var content = await System.IO.File.ReadAllBytesAsync(filePath);
            new FileExtensionContentTypeProvider()
                .TryGetContentType(fileName, out string contentType);
            try
            {
                return File(content, contentType, fileName);
            } 
            catch(IOException exception)
            {
                //TODO: log
            }
            catch(Exception exception)
            {
                // TODO: log
                // Maybe return a 500?
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPost("Read")]
        public async Task<IEnumerable<ExcelFormModel>> ReadExcelAsync([FromForm] ReadExcelInputModel model)
        {
            var filePath = await model.File.GetTempFilePath();
            var models = _excelService.ReadExcelToModel(filePath);

            return models;
        }

        [HttpPost("ReadDynamic")]
        public async Task<IEnumerable<Dictionary<string, object>>> ReadExcelDynamicAsync([FromForm] ReadExcelInputModel model)
        {
            var filePath = await model.File.GetTempFilePath();
            var models = _excelService.ReadExcelDynamic(filePath);

            return models;
        }
        
        [HttpPost("Upload")]
        public async Task UploadExcelAsync([FromForm] ReadExcelInputModel model)
        {
            var filePath = await model.File.GetTempFilePath();
            await _excelService.UploadAsync(filePath);
        }

        [HttpGet]
        public async Task<IEnumerable<ExcelFormModel>> GetAllAsync()
            => await _excelService.AllAsync();
    }
}
