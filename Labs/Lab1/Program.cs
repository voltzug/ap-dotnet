// notes
// !!! use tryParse
using Lab1;

Console.WriteLine(UI.SIG_QUIT + " - dla przervania operacji");

UI.PrintTaskTitle("1.3");
{
  const string BUZZ = "Buzz";

  for (int i = 0; i <= 100; i++)
  {
    bool isMod3 = i % 3 == 0;
    bool isMod5 = i % 5 == 0;

    if (isMod3)
    {
      Console.Write("Fizz");
      if (isMod5)
      {
        Console.WriteLine(BUZZ);
      }
      else
      {
        Console.WriteLine();
      }
    }
    else if (isMod5) Console.WriteLine(BUZZ);
    else Console.WriteLine(i);
  }
}

UI.PrintTaskTitle("1.4-5");
{
  var rand = new Random();
  int value = rand.Next(1, 101), guess = 0, tries = 0;
  try
  {
    Console.Write("Sproboj zgadnonc: ");
    while (guess != value && tries++ < 9)
    {
      UI.ReadIntLine(out guess);
      if (value < guess) Console.Write("mniejsza");
      else if (value > guess) Console.Write("wieksza");
    }
  }
  catch (UIException)
  {
    Console.WriteLine("Force quit");
  }
  Console.WriteLine();
  if (tries > 9) Console.WriteLine("Przegrales");
  else Console.WriteLine("Zgadnoles z " + tries + " razu");
}