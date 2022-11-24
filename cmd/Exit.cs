namespace cmd;

public class Exit: CommandBase
{
    public const string commandName = "exit";
    
    protected override string[] allowedArgs { get; } = Array.Empty<string>();

    protected override void PrintHelp(string stdout)
    {
        throw new NotImplementedException();
    }

    public override void Execute(string[] targets, string[] args, string[] stdout)
    {
        Console.WriteLine("Завершено успешно");
    }
}
