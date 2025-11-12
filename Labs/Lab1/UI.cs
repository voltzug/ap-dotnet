namespace Lab1;

public class UIException : Exception
{
  public UIException(string message)
      : base(message)
  {
  }

  public UIException(string message, Exception innerException)
      : base(message, innerException)
  {
  }
}

public class UI
{
  public const string SIG_QUIT = "!";

  static void CatchSIGs(string input)
  {
    if (input.StartsWith(SIG_QUIT))
    {
      throw new UIException("operation quit");
    }
  }

  public static void PrintTaskTitle(string title)
  {
    Console.WriteLine("\n# " + title);
  }

  public static bool ReadIntLine(out int res, bool verbose = true)
  {
    res = 0;
    string? input = Console.ReadLine();
    if (input == null)
    {
      if (verbose) Console.WriteLine("Nie wprowadzono liczby");
      return false;
    }
    CatchSIGs(input);
    if (int.TryParse(input, out res) == false)
    {
      if (verbose) Console.WriteLine("To nie jest liczba calkowita");
      return false;
    }
    return true;
  }
}
