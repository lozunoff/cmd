namespace cmd
{
  public abstract class CommandBase
  {
    protected abstract string[] allowedArgs { get; }

    protected bool CheckArgs(string[] args)
    {
      bool isValidArgs = true;

      foreach (string arg in args)
      {
        if (!allowedArgs.Contains(arg))
        {
          Console.WriteLine("Неизвестные аргументы");
          isValidArgs = false;
          break;
        }
      }

      return isValidArgs;
    }

    protected void PrintResult(List<string> result, string stdout)
    {
      if (stdout != "")
      {
        Stdout.Write(stdout, result);
      }
      else
      {
        Console.WriteLine(String.Join(Environment.NewLine, result));
      }
    }

    protected abstract void PrintHelp(string stdout);

    public abstract void Execute(
        string[] targets,
        string[] args,
        string[] stdout
    );
  }
}
