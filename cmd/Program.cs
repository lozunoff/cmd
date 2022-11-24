using System.Text.RegularExpressions;

namespace cmd
{
    public class Program
    {
        private static bool running = false;
        
        private static Dictionary<string, string[]> PrepareArgs(string[] command)
        {
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();

            List<string> targets = new List<string>();
            List<string> stdout = new List<string>();
            List<string> args = new List<string>();

            if (command.Length > 1)
            {
                bool isStdoutPath = false;

                foreach (string item in command[1..(command.Length)])
                {
                    if (item.StartsWith("/"))
                    {
                        args.Add(item);
                    }
                    else if (item == ">")
                    {
                        isStdoutPath = true;
                    }
                    else
                    {
                        if (isStdoutPath == true)
                        {
                            stdout.Add(item);
                            isStdoutPath = false;
                        }
                        else
                        {
                            targets.Add(item);
                        }
                    }
                }
            }

            dict.Add("targets", targets.Distinct().ToArray());
            dict.Add("stdout", stdout.Distinct().ToArray());
            dict.Add("args", args.Distinct().ToArray());

            return dict;
        }

        public static void Main(string[] args)
        {
            Program.running = true;

            while (Program.running) {
                Console.WriteLine("Введите команду:");
                string? commandLine = Console.ReadLine();

                if (commandLine != null)
                {
                    Regex commandRegex = new Regex(@"\s+");
                    string[] command = commandRegex.Split(commandLine.Trim());

                    string commandName = command[0].ToLower();

                    Dictionary<string, string[]> commandArgs = PrepareArgs(command);

                    switch (commandName)
                    {
                        case Dir.commandName:
                            Dir dir = new Dir();
                            dir.Execute(commandArgs["targets"], commandArgs["args"], commandArgs["stdout"]);
                            break;
                        case Del.commandName:
                            Del del = new Del();
                            del.Execute(commandArgs["targets"], commandArgs["args"], commandArgs["stdout"]);
                            break;
                        case Sort.commandName:
                            Sort sort = new Sort();
                            sort.Execute(commandArgs["targets"], commandArgs["args"], commandArgs["stdout"]);
                            break;
                        case Type.commandName:
                            Type type = new Type();
                            type.Execute(commandArgs["targets"], commandArgs["args"], commandArgs["stdout"]);
                            break;
                        case Exit.commandName:
                            Program.running = false;
                            Exit exit = new Exit();
                            exit.Execute(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда");
                            break;
                    }
                }
            }
        }
    }

}
