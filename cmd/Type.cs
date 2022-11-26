namespace cmd
{
  public class Type: CommandBase
  {
    public const string commandName = "type";

    protected override string[] allowedArgs { get; } = { "/?" };

    protected override void PrintHelp(string stdout)
    {
      List<string> help = new List<string>();

      help.Add("Вывод содержимого одного или нескольких текстовых файлов.");
      help.Add("");
      help.Add("type [<drive>:][<path>]<filename>");

      if (stdout == "")
      {
        help.Add("");
      }

      PrintResult(help, stdout);
    }

    public override void Execute(string[] targets, string[] args, string[] stdout)
    {
      string stdoutPath = stdout.Length > 0 ? stdout[0] : "";

      if (args.Contains("/?"))
      {
        PrintHelp(stdoutPath);
        return;
      }

      if (!CheckArgs(args))
      {
        return;
      }

      List<string> result = new List<string>();

      for (int i = 0; i < targets.Length; i++)
      {
        string target = targets[i];

        if (i > 0 && i < targets.Length)
        {
          result.Add("");
        }

        if (File.Exists(target))
        {
          try
          {
            result.Add($"Содержимое файла: {target}");
            result.Add("");

            StreamReader sr = new StreamReader(target);

            string? line = sr.ReadLine();

            while (line != null)
            {
              result.Add(line);
              line = sr.ReadLine();
            }

            sr.Close();
          } catch (Exception)
          {
            result.Add("Ошибка обработки файла");
          }
        }
        else
        {
          result.Add($"Не удается найти указанный файл: {target}");
        }

        result.Add("");
      }
      
      if (stdoutPath == "")
      {
        result.Insert(0, "");
      }

      PrintResult(result, stdoutPath);
    }
  }
}
