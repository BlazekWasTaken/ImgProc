using System.Diagnostics;
using CommandLine;
using ImgProcTask1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        var input = Image.Load<Rgb24>(opts.Input);
        Image<Rgb24> output;
        
        if (opts.Brightness != 0) output = Operations.Brightness(input, opts.Brightness);
        else if (opts.Negative) output = Operations.Negative(input);
        else if (opts.HorizontalFlip) output = Operations.HorizontalFlip(input);
        else if (opts.Shrink != 0) output = Operations.Shrink(input, opts.Shrink);
        else if (opts.Midpoint) output = Operations.MidpointFilter(input);
        else output = input;
        
        output.SaveAsBmp(opts.Output);
        
        s.Stop();
        Console.WriteLine(s.ElapsedMilliseconds);
    }
    catch (Exception)
    {
        Console.WriteLine("Something went wrong");
    }
}