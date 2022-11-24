namespace cmd
{
    public class Del: CommandBase
    {
        public const string commandName = "del";

        protected override string[] allowedArgs { get; } = { "/?" };

        protected override void PrintHelp(string stdout)
        {
            List<string> help = new List<string>();

            help.Add("Удаление одного или нескольких файлов.");
            help.Add("");
            help.Add("del names");
            help.Add("");
            help.Add("    names       Список из одного или нескольких файлов или каталогов.");
            help.Add("                Для удаления группы файлов можно использовать подстановочные знаки. Если");
            help.Add("                указан каталог, будут удалены все файлы в этом");
            help.Add("                каталоге.");

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

            foreach (string target in targets)
            {
                if (Directory.Exists(target))
                {
                    Directory.Delete(target, true);
                    result.Add($"Удален каталог: {target}");
                    result.Add("");
                    continue;
                }

                if (File.Exists(target))
                {
                    File.Delete(target);
                    result.Add($"Удален файл: {target}");
                    result.Add("");
                    continue;
                }

                result.Add($"Не удается найти: {target}");
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
