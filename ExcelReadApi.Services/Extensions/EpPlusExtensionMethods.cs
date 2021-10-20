using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelReadApi.Services.Extensions
{
    public record ColNumberAndStringValue(int ColNumber, string Value);
    public static class EpPlusExtensionMethods
    {
        public static int? GetColumnByName(this ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells["1:1"]
                .FirstOrDefault(c => c.Value.ToString() == columnName)?.Start?.Column;
        }

        public static IEnumerable<ColNumberAndStringValue> GetNotEmptyColumns(this ExcelWorksheet ws)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells["1:1"]
                .Where(cell => !string.IsNullOrEmpty(cell.Value.ToString()))
                .Select(cell => new ColNumberAndStringValue(cell.Start.Column, cell.Value.ToString()!));
        }
    }
}
