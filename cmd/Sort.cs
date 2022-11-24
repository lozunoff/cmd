namespace cmd
{
    public class Sort: CommandBase
    {
        public const string commandName = "sort";

        protected override string[] allowedArgs { get; } = { "/r", "/?" };

        protected override void PrintHelp(string stdout)
        {
            List<string> help = new List<string>();

            help.Add("Считывает ввод, сортирует данные и записывает результаты на экран, в файл или на другое устройство.");
            help.Add("");
            help.Add("sort [/r] [[<drive>:][<path>]<filename>]");
            help.Add("");
            help.Add("      /r       Меняет порядок сортировки на обратный (т. е. Сортировка от Z к A и от 9 до 0).");

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
            
            if (targets.Length > 1)
            {
                if (stdoutPath == "")
                {
                    result.Add("");
                }
                    
                result.Add("Указано более одного файла");
            }
            else
            {
                List<string> text = new List<string>();

                if (targets.Length == 1)
                {
                    if (stdoutPath == "")
                    {
                        result.Add("");
                    }
                    
                    if (File.Exists(targets[0]))
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(targets[0]);

                            string? line = sr.ReadLine();

                            while (line != null)
                            {
                                text.Add(line);
                                line = sr.ReadLine();
                            }

                            sr.Close();
                        }
                        catch (Exception)
                        {
                            result.Add("Ошибка обработки файла");
                        }
                    }
                    else
                    {
                        result.Add($"Не удается найти указанный файл: {targets[0]}");
                    }
                }
                else
                {
                    string? str;

                    while (!String.IsNullOrWhiteSpace(str = Console.ReadLine()))
                    {
                        text.Add(str);
                    }
                }

                if (args.Contains("/r"))
                {
                    text = text.OrderByDescending(q => q).ToList();
                }
                else
                {
                    text = text.OrderBy(q => q).ToList();
                }

                if (text.Count() > 0)
                {
                    result.AddRange(text);
                }
            }

            result.Add("");
            
            PrintResult(result, stdoutPath);
        }
    }
}
