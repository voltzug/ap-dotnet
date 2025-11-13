using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
  public class Fruit
  {
    public string Name { get; set; }
    public bool IsSweet { get; set; }
    public decimal Price { get; set; }
    public decimal UsdPrice => Price / USDCourse.Current;



    public static Fruit Create()
    {
      Random r = new();
      string[] names = ["Apple", "Banana", "Cherry", "Durian", "Edelberry", "Grape", "Jackfruit"];
      return new Fruit
      {
        Name = names[r.Next(names.Length)],
        IsSweet = r.NextDouble() > 0.5,
        Price = (decimal)(r.NextDouble() * 10)
      };
    }
        public static Fruit CreateSame()
        {
            Random r = new();
            return new Fruit
            {
                Name = "Same",
                IsSweet = r.NextDouble() > 0.5,
                Price = (decimal)(r.NextDouble() * 10)
            };
        }

        public override string ToString()
    {
      return $"Fruit: Name={Name}, IsSweet={IsSweet}, Price={Price:C2}, PriceUSD={USDCourse.FormatUsdPrice(UsdPrice)} $";
    }
  }
}
