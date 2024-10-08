using CommandLine;
using ImgProcTask1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

return 0;

static void RunOptions(Options opts)
{
    try
    {
        var input = Image.Load<Rgb24>(opts.Input);
        Image<Rgb24> output;
        
        if (opts.Brightness != 0) output = Operations.Brightness(input, opts.Brightness);
        else if (opts.Negative) output = Operations.Negative(input);
        else if (opts.HorizontalFlip) output = Operations.HorizontalFlip(input);
        else if (opts.Contrast != 0) output = Operations.Contrast(input, opts.Contrast);
        else output = input;
        
        output.SaveAsBmp(opts.Output);
    }
    catch (Exception)
    {
        Console.WriteLine("Something went wrong");
    }
}