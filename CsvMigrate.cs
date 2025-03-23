using CsvHelper;
using CsvHelper.Configuration;
using exchangeRateApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;

namespace exchangeRateApi
{
    public class CsvMigrate
    {
        private readonly ApplicationDbContext _context;

        public CsvMigrate(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ImportCurrenciesFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Currency>().ToList();

                _context.Currencies.AddRange(records);
                _context.SaveChanges();
            }
        }
    }
}