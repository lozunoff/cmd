using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace cmd
{
    public class Dir: CommandBase 
	{
        public const string commandName = "dir";

        protected override string[] allowedArgs { get; } = { "/o:n", "/a:h", "/?" };

        private static async Task<long> GetDirSize(string? dirPath)
        {
            if (dirPath == null)
            {
                return 0;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(@dirPath);
            long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));

            return dirSize;
        }

        protected override void PrintHelp(string stdout)
        {
            List<string> help = new List<string>();

            help.Add("Вывод списка файлов и подкаталогов в указанном каталоге.");
            help.Add("");
            help.Add("dir [drive:][path][filename] [/a[[:]attributes]] [/o[[:]sortorder]]");
            help.Add("");
            help.Add("    [drive:][path][filename]");
            help.Add("");
            help.Add("                Диск, каталог или имена файлов для включения в список.");
            help.Add("");
            help.Add("    /a          Отображение файлов с указанными атрибутами.");
            help.Add("    атрибуты      h  Скрытые файлы");
            help.Add("    /o          Сортировка списка отображаемых файлов.");
            help.Add("    sortorder     n  По имени (по алфавиту) ");

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

            if (targets == null)
            {
                targets = Array.Empty<string>();
            }
            else if (targets.Length == 0)
            {
                targets = targets.Append(Directory.GetCurrentDirectory()).ToArray();
            }

            bool showHiddenItems = args.Contains("/a:h");

            List<string> result = new List<string>();

            for (int i = 0; i < targets.Length; i++)
            {
                string target = targets[i];

                int totalCount = 0;
                long totalSize = 0;

                int totalDirCount = 0;
                long totalDirSize = 0;

                int totalFileCount = 0;
                long totalFileSize = 0;

                int nameColSize = 0;
                int typeColSize = 0;
                int sizeColSize = 0;
                int dateColSize = 0;
                int timeColSize = 0;

                List<string> names = new List<string>();
                List<string> types = new List<string>();
                List<string> sizes = new List<string>();
                List<string> dates = new List<string>();
                List<string> times = new List<string>();

                if (i > 0 && i < targets.Length)
                {
                    result.Add("");
                }

                bool isExistsFile = File.Exists(@target);
                bool isExistsDirectory = Directory.Exists(target);

                if (!isExistsFile && !isExistsDirectory)
                {
                    result.Add($"Папка не найдена: {target}");
                    result.Add("");
                    continue;
                }

                List<string> items = new List<string>();

                if (isExistsFile)
                {
                    items.Add(target);
                } else
                {
                    items.AddRange(Directory.GetFileSystemEntries(target, "*"));
                }

                foreach (string item in items)
                {
                    
                    FileInfo fileInfo = new FileInfo(@item);

                    Boolean isHidden = fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

                    if ((isHidden && !showHiddenItems) || (!isHidden && showHiddenItems))
                    {
                        continue;
                    }

                    totalCount += 1;

                    string name = fileInfo.Name;
                    nameColSize = Math.Max(nameColSize, name.Length);
                    names.Add(name);

                    bool isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                    string type = isDirectory ? "<DIR>" : "";
                    typeColSize = Math.Max(typeColSize, type.Length);
                    types.Add(type);

                    long size = isDirectory ? GetDirSize(item).GetAwaiter().GetResult() : fileInfo.Length;
                    sizeColSize = Math.Max(sizeColSize, $"{size}".Length);
                    sizes.Add($"{size}");

                    totalSize += size;

                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

                    string date = fileInfo.LastWriteTime.ToShortDateString();
                    dateColSize = Math.Max(dateColSize, date.Length);
                    dates.Add(date);

                    string time = fileInfo.LastWriteTime.ToShortTimeString();
                    timeColSize = Math.Max(timeColSize, time.Length);
                    times.Add(time);

                    if (isDirectory)
                    {
                        totalDirCount += 1;
                        totalDirSize += size;
                    }
                    else
                    {
                        totalFileCount += 1;
                        totalFileSize += size;
                    }
                }

                if (isExistsFile)
                {
                    string? fileDirectory = new FileInfo(target).DirectoryName;
                    result.Add($"Содержимое папки: {fileDirectory}");
                }
                else
                {
                    string? fileDirectory = new FileInfo(target).FullName;
                    result.Add($"Содержимое папки: {fileDirectory}");
                }
                

                if (names.Count() > 0)
                {
                    result.Add("");
                }

                List<string> dirResult = new List<string>();

                for (int j = 0; j < totalCount; j++)
                {
                    string cell1 = $"{{0,-{nameColSize + 5}}}";
                    string cell2 = $"{{1,-{typeColSize + 5}}}";
                    string cell3 = $"{{2,-{sizeColSize + 5}}}";
                    string cell4 = $"{{3,-{dateColSize + 5}}}";
                    string cell5 = $"{{4,-{timeColSize + 5}}}";

                    string resultFormat = $"{cell1} {cell2} {cell3} {cell4} {cell5}";

                    dirResult.Add(String.Format(resultFormat, names[j], types[j], sizes[j], dates[j], times[j]));
                }

                if (args.Contains("/o:n"))
                {
                    dirResult = dirResult.OrderBy(q => q).ToList();
                }

                result.AddRange(dirResult);

                result.Add("");
                result.Add($"Всего объектов: {totalCount}, {totalSize} байт");
                if (totalDirCount > 0)
                    result.Add($"Каталогов: {totalDirCount}, {totalDirSize} байт");
                if (totalFileCount > 0)
                    result.Add($"Файлов: {totalFileCount}, {totalFileSize} байт");
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
