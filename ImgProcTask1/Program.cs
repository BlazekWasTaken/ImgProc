using CommandLine;
using ImgProcTask1;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return 0;

static void RunOptions(Options opts)
{
    try
    {
        
    }
    catch (Exception)
    {
        Console.WriteLine("Something went wrong");
    }
}