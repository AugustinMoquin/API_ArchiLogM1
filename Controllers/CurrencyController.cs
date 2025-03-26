using System.Collections.Generic;
using System.Globalization;
using exchangeRateApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace exchangeRateApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ApplicationDbContext context, ILogger<CurrencyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            _logger.LogInformation("GetCurrencies called");
            return await _context.Currencies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            _logger.LogInformation("GetCurrency called with id: {Id}", id);
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
            {
                _logger.LogWarning("Currency with id: {Id} not found", id);
                return NotFound();
            }

            return currency;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchCurrencies(
            int? id,
            string? country_Code,
            string? Currency_Code,
            string? range,
            string? desc,
            string? asc,
            string? fields
            )
        {
            _logger.LogInformation("SearchCurrencies called with parameters: id={Id}, country_Code={CountryCode}, Currency_Code={CurrencyName}, range={Range}, desc={Desc}, asc={Asc}, fields={Fields}",
                id, country_Code, Currency_Code, range, desc, asc, fields);

            var query = _context.Currencies.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(country_Code))
            {
                query = query.Where(c => c.country_Code == country_Code);
            }

            if (!string.IsNullOrEmpty(Currency_Code))
            {
                query = query.Where(c => c.Currency_Code == Currency_Code);
            }

            if (!string.IsNullOrEmpty(range))
            {
                var rangeParts = range.Split('-');
                if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                {
                    query = query.Skip(start).Take(end - start + 1);
                }
            }

            if (!string.IsNullOrEmpty(desc))
            {
                query = query.OrderByDescending(e => EF.Property<object>(e, desc));
            }

            if (!string.IsNullOrEmpty(asc))
            {
                query = query.OrderBy(e => EF.Property<object>(e, asc));
            }

            var result = await query.ToListAsync();

            if (!result.Any())
            {
                _logger.LogWarning("No currencies found with the given parameters");
                return NotFound();
            }

            if (!string.IsNullOrEmpty(fields))
            {
                var selectedFields = fields.Split(',');
                var partialResult = result.Select(currency =>
                {
                    var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                    foreach (var field in selectedFields)
                    {
                        var propertyInfo = typeof(Currency).GetProperty(field, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (propertyInfo != null)
                        {
                            expando[field] = propertyInfo.GetValue(currency);
                        }
                    }
                    return expando;
                }).ToList();

                return partialResult;
            }

            return result;
        }

        [HttpGet("compare")]
        public async Task<ActionResult<object>> CompareCurrencies(
            int? id1, 
            int? id2, 
            string? country_Code1, 
            string? country_Code2, 
            string? Currency_Code1, 
            string? Currency_Code2)
        {
            _logger.LogInformation("CompareCurrencies called with parameters: id1={Id1}, id2={Id2}, country_Code1={CountryCode1}, country_Code2={CountryCode2}, Currency_Code1={CurrencyCode1}, Currency_Code2={CurrencyCode2}",
                id1, id2, country_Code1, country_Code2, Currency_Code1, Currency_Code2);

            var currency1 = await GetCurrencyByParameters(id1, country_Code1, Currency_Code1);
            var currency2 = await GetCurrencyByParameters(id2, country_Code2, Currency_Code2);

            if (currency1 == null || currency2 == null)
            {
                _logger.LogWarning("One or both currencies not found");
                return NotFound();
            }

            var value1ToDollar = currency1.Value;
            var value2ToDollar = currency2.Value;

            var Value1ToValue2 = value1ToDollar / value2ToDollar;
            var Value2ToValue1 = value2ToDollar / value1ToDollar;

            var comparison = new Dictionary<string, Dictionary<string, float>>
            {
                [currency1.Currency_Code + " to " + currency2.Currency_Code] = new Dictionary<string, float>
                {
                    [currency1.Currency_Code] = 1,
                    [currency2.Currency_Code] = Value2ToValue1
                },
                [currency2.Currency_Code + " to " + currency1.Currency_Code] = new Dictionary<string, float>
                {
                    [currency2.Currency_Code] = 1,
                    [currency1.Currency_Code] = Value1ToValue2
                }
            };

            return new { currency1, currency2, comparison };
        }

        private async Task<Currency?> GetCurrencyByParameters(int? id, string? country_Code, string? Currency_Code)
        {
            var query = _context.Currencies.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(country_Code))
            {
                query = query.Where(c => c.country_Code == country_Code);
            }

            if (!string.IsNullOrEmpty(Currency_Code))
            {
                query = query.Where(c => c.Currency_Code == Currency_Code);
            }

            return await query.FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Currency>> CreateCurrency([FromBody] Currency currency)
        {
            _logger.LogInformation("CreateCurrency called with currency: {Currency}", currency);

            if (currency == null)
            {
                _logger.LogWarning("CreateCurrency called with null currency");
                return BadRequest();
            }

            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            _logger.LogInformation("DeleteCurrency called with id: {Id}", id);

            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                _logger.LogWarning("Currency with id: {Id} not found", id);
                return NotFound();
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, [FromBody] Currency updatedCurrency)
        {
            _logger.LogInformation("UpdateCurrency called with id: {Id} and currency: {Currency}", id, updatedCurrency);

            if (id != updatedCurrency.Id)
            {
                _logger.LogWarning("UpdateCurrency called with mismatched id: {Id} and currency id: {CurrencyId}", id, updatedCurrency.Id);
                return BadRequest();
            }

            var existingCurrency = await _context.Currencies.FindAsync(id);
            if (existingCurrency == null)
            {
                _logger.LogWarning("Currency with id: {Id} not found", id);
                return NotFound();
            }

            existingCurrency.country_Code = updatedCurrency.country_Code;
            existingCurrency.Country_number = updatedCurrency.Country_number;
            existingCurrency.Country = updatedCurrency.Country;
            existingCurrency.Currency_Code = updatedCurrency.Currency_Code;
            existingCurrency.Currency_Code = updatedCurrency.Currency_Code;
            existingCurrency.Currency_Number = updatedCurrency.Currency_Number;
            existingCurrency.Value = updatedCurrency.Value;

            _context.Entry(existingCurrency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
                {
                    _logger.LogWarning("Concurrency exception: Currency with id: {Id} not found", id);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CurrencyExists(int id)
        {
            return _context.Currencies.Any(e => e.Id == id);
        }
    }
}
