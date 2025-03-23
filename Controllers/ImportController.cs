using Microsoft.AspNetCore.Mvc;
using exchangeRateApi.Models;
namespace exchangeRateApi;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly CsvMigrate _csvMigrate;

    public ImportController(CsvMigrate csvMigrate)
    {
        _csvMigrate = csvMigrate;
    }

    [HttpPost("import-currencies")]
    public IActionResult ImportCurrencies()
    {
        _csvMigrate.ImportCurrenciesFromCsv("joined_currency_data.csv");
        return Ok("Importation réussie");
    }
}