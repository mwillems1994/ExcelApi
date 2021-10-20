using ExcelReadApi.Database.Context;
using ExcelReadApi.Services.Extensions;
using ExcelReadApi.Services.Models;
using ExcelReadApi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelReadApi.Services.Services
{
    public interface IExcelService
    {
        IEnumerable<ExcelFormModel> ReadExcelToModel(string filePath);
        // TODO: find a better data format than a dictionary, it produces good json tho
        IEnumerable<Dictionary<string, object>> ReadExcelDynamic(string filePath);
        Task UploadAsync(string filePath, bool commit = true);
        Task<ExcelFormModel[]> AllAsync();
    }
    public class ExcelService : BaseService, IExcelService
    {
        private readonly ExcelFormRepository _excelFormRepository;
        public ExcelService(ExcelDbContext db, ExcelFormRepository excelFormRepository): base(db)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _excelFormRepository = excelFormRepository;
        }

        public IEnumerable<Dictionary<string, object>> ReadExcelDynamic(string filePath)
        {
            using var package = ReadFileToPackage(filePath);

            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            var columns = worksheet.GetNotEmptyColumns();
            var results = new List<Dictionary<string, object>>();

            // 1 are headers
            for (int rowId = 2; rowId <= rowCount; rowId++)
            {
                var dic = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    dic.Add(column.Value, worksheet.Cells[rowId, column.ColNumber].Value.ToString()!);
                }

                results.Add(dic);
            };

            return results;
        }

        public IEnumerable<ExcelFormModel> ReadExcelToModel(string filePath)
            => ReadExcel(filePath);

        public async Task UploadAsync(string filePath, bool commit = true)
        {
            var existingForms = await _excelFormRepository
                .AllExcelForms
                .ToListAsync();

            var excelForms = ReadExcel(filePath);

            // Could do filtering in db...
            var newExcelForms = excelForms
                .Where(item => !existingForms
                    .Any(item.Equals))
                .Select(item => new Database.Models.ExcelForm
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Address = item.Address
                });

            await _excelFormRepository.AddRangeAsync(newExcelForms);

            if (commit)
            {
                await CommitAsync();
            }
        }

        public async Task<ExcelFormModel[]> AllAsync()
            => await _excelFormRepository
                .AllExcelForms
                .Select(item => AsExcelFormModel(item))
                .ToArrayAsync();

        private ExcelPackage ReadFileToPackage(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File not found");
            }

            var package = new ExcelPackage(fileInfo);
            return package;
        }

        private IEnumerable<ExcelFormModel> ReadExcel(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File not found");
            }

            using var package = ReadFileToPackage(filePath);

            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            // TODO: Create custom exceptions
            // TODO: Create some form on configuration
            var firstNameCol = worksheet.GetColumnByName("Voornaam") ?? throw new Exception("Column not found");
            var lastNameCol = worksheet.GetColumnByName("Achternaam") ?? throw new Exception("Column not found");
            var addressCol = worksheet.GetColumnByName("Adres") ?? throw new Exception("Column not found");

            var results = new List<ExcelFormModel>();

            // 1 are headers
            for (int rowId = 2; rowId <= rowCount; rowId++)
            {
                results.Add(new ExcelFormModel(
                    worksheet.Cells[rowId, firstNameCol].Value?.ToString() ?? string.Empty,
                    worksheet.Cells[rowId, lastNameCol].Value?.ToString() ?? string.Empty,
                    worksheet.Cells[rowId, addressCol].Value?.ToString() ?? string.Empty
                ));
            }

            return results;
        }

        private static ExcelFormModel AsExcelFormModel(Database.Models.ExcelForm model)
            => new ExcelFormModel(model.FirstName, model.LastName, model.Address);
    }
}
