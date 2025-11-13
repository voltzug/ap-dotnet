using System;
using System.Collections.Generic;
using System.Globalization;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab2
{
  public class USDCourse
  {
    public static decimal Current = 1;

    public async static Task<decimal> GetUsdCourseAsync()
    {
      var wc = new HttpClient();
      var response = await wc.GetAsync("https://api.nbp.pl/api/exchangerates/tables/a/?format=xml");
      if (!response.IsSuccessStatusCode)
        throw new InvalidOperationException();
      XDocument doc = XDocument.Parse(await response.Content.ReadAsStringAsync());
      string? midUsdValue = doc.Descendants("Rate")
                .Where(rate => rate.Element("Code")?.Value == "USD")
                .Select(rate => rate.Element("Mid")?.Value)
                .FirstOrDefault();
      return (decimal)Convert.ToSingle(midUsdValue, System.Globalization.CultureInfo.InvariantCulture);
      throw new InvalidOperationException();
    }

    public static string FormatUsdPrice(decimal price)
    {
      var usc = new CultureInfo("en-us");
      return price.ToString("C2", usc);
    }
  }
}
