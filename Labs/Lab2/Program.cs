// notes
// !!! use decimal, formatowanie po lokalizacji (PL or USD)
// !!! Linq - xml value (Element()?.Value == "USD")
using Lab1;
using Lab2;

USDCourse.Current = await USDCourse.GetUsdCourseAsync();

UI.PrintTaskTitle("Lab2.3");
{
  List<Fruit> fruits = [];

  for (int i = 0; i < 15; i++)
  {
    fruits.Add(Fruit.Create());
  }

  var filtered = fruits
                .Where(fruit => fruit.IsSweet)
                .OrderByDescending(fruit => fruit.Price);
  foreach (var fruit in filtered)
  {
    Console.WriteLine(fruit);
  }
}

UI.PrintTaskTitle("Lab2.4");
{
  Console.WriteLine($"USD course = {USDCourse.Current}");
}