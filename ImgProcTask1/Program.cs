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
        
        if (opts.MeanSquaredError) Console.WriteLine(Operations.MeanSquaredError(Image.Load<Rgb24>(opts.Input), Image.Load<Rgb24>(opts.Output)));
        else if (opts.PeakMeanSquaredError) Console.WriteLine(Operations.PeakMeanSquaredError(Image.Load<Rgb24>(opts.Input),Image.Load<Rgb24>(opts.Output)));
        else if (opts.SignalToNoiseRatio) Console.WriteLine(Operations.SignalToNoiseRatio(Image.Load<Rgb24>(opts.Input), Image.Load<Rgb24>(opts.Output)));
        else if (opts.PeakSignalToNoiseRatio) Console.WriteLine(Operations.PeakSignalToNoiseRatio(Image.Load<Rgb24>(opts.Input), Image.Load<Rgb24>(opts.Output)));
        else if (opts.MaximumDifference) Console.WriteLine(Operations.MaximumDifference(Image.Load<Rgb24>(opts.Input), Image.Load<Rgb24>(opts.Output)));
        else
        {
            var input = Image.Load<Rgb24>(opts.Input);
            Image<Rgb24> output;
            if (opts.Brightness != 0) output = Operations.Brightness(input, opts.Brightness);
            else if (opts.Contrast != 0) output = Operations.Contrast(input, opts.Contrast);
            else if (opts.Negative) output = Operations.Negative(input);
            else if (opts.HorizontalFlip) output = Operations.HorizontalFlip(input);
            else if (opts.VerticalFlip) output = Operations.VerticalFlip(input);
            else if (opts.DiagonalFlip) output = Operations.DiagonalFlip(input);
            else if (opts.Shrink != 0) output = Operations.Shrink(input, opts.Shrink);
            else if (opts.Enlarge != 0) output = Operations.Enlarge(input, opts.Enlarge);
            else if (opts.Midpoint) output = Operations.MidpointFilter(input);
            else if (opts.Mean) output = Operations.ArithmeticMeanFilter(input);
            else throw new ArgumentException();
            
            output.SaveAsBmp(opts.Output);
        }
        
        s.Stop();
        Console.WriteLine(s.ElapsedMilliseconds);
    }
    catch (Exception)
    {
        Console.WriteLine("Something went wrong");
    }
}