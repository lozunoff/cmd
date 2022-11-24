namespace cmd;

public class Stdout
{
    public static void Write(string path, List<string>  data)
    {
        if (path != "")
        {
            File.WriteAllText(path, String.Join(Environment.NewLine, data));
        }
    }
}