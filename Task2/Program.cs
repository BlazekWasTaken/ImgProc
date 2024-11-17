using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Task2;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        var input = Image.Load<L8>(opts.Input);
        Image<L8> output;

        if (!string.IsNullOrEmpty(opts.Histogram))
        {
            output = Operations.Histogram(ref input);
            output.SaveAsBmp(opts.Histogram);
        }
        if (!string.IsNullOrEmpty(opts.Hyper))
        {
            var values = opts.Hyper.Split(',');
            if (values.Length != 3) throw new Exception("Invalid parameter format.");
            var min = byte.Parse(values[1]);
            var max = byte.Parse(values[2]);
            output = Operations.Hyperbolic(ref input, min, max);
            var path = values[0].Split('.');
            output.SaveAsBmp($"{path[0]}_{min}_{max}.{path[1]}");
        }
        if (!string.IsNullOrEmpty(opts.LineIdentification))
        {
            output = Operations.LineIdentification(ref input);
            output.SaveAsBmp(opts.LineIdentification);
        }
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}