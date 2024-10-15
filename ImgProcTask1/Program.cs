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
        if (opts.MeanSquaredError || opts.PeakMeanSquaredError || opts.SignalToNoiseRatio || opts.PeakSignalToNoiseRatio || opts.MaximumDifference)
        {
            var input = Image.Load<Rgb24>(opts.Input);
            var output = Image.Load<Rgb24>(opts.Output);
            if (opts.MeanSquaredError) Console.WriteLine("mse: " + Operations.MeanSquaredError(ref input, ref output));
            if (opts.PeakMeanSquaredError) Console.WriteLine("pmse: " + Operations.PeakMeanSquaredError(ref input,ref output));
            if (opts.SignalToNoiseRatio) Console.WriteLine("snr: " + Operations.SignalToNoiseRatio(ref input, ref output));
            if (opts.PeakSignalToNoiseRatio) Console.WriteLine("psnr: " + Operations.PeakSignalToNoiseRatio(ref input, ref output));
            if (opts.MaximumDifference) Console.WriteLine("md: " + Operations.MaximumDifference(ref input, ref output));
        }
        else
        {
            var input = Image.Load<Rgb24>(opts.Input);
            Image<Rgb24> output;
            if (opts.Brightness != 0) output = Operations.Brightness(ref input, opts.Brightness);
            else if (opts.Contrast != 0) output = Operations.Contrast(ref input, opts.Contrast);
            else if (opts.Negative) output = Operations.Negative(ref input);
            else if (opts.HorizontalFlip) output = Operations.HorizontalFlip(ref input);
            else if (opts.VerticalFlip) output = Operations.VerticalFlip(ref input);
            else if (opts.DiagonalFlip) output = Operations.DiagonalFlip(ref input);
            else if (opts.Shrink != 0) output = Operations.Shrink(ref input, opts.Shrink);
            else if (opts.Enlarge != 0) output = Operations.Enlarge(ref input, opts.Enlarge);
            else if (opts.Midpoint) output = Operations.MidpointFilter(ref input);
            else if (opts.Mean) output = Operations.ArithmeticMeanFilter(ref input);
            else throw new ArgumentException();
            
            output.SaveAsBmp(opts.Output);
        }
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}