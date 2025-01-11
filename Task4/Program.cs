using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Task4;

Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        if (!string.IsNullOrEmpty(opts.OneDimensionalFourier)) {}

        if (!string.IsNullOrEmpty(opts.StandardDiscreteFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.StandardDiscreteFourier(image).SaveAsBmp(opts.StandardDiscreteFourier);
            return;
        }
        if (!string.IsNullOrEmpty(opts.InverseStandardFourier)){}

        if (!string.IsNullOrEmpty(opts.FastFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.FastFourier(image).SaveAsBmp(opts.FastFourier);
            return;
        }
        if (!string.IsNullOrEmpty(opts.InverseFastFourier)){}
        if (!string.IsNullOrEmpty(opts.LowPassFilter)){}
        if (!string.IsNullOrEmpty(opts.HighPassFilter)){}
        if (!string.IsNullOrEmpty(opts.BandPassFilter)){}
        if (!string.IsNullOrEmpty(opts.BandCutFilter)){}
        if (!string.IsNullOrEmpty(opts.HighPassEdgeFilter)){}
        if (!string.IsNullOrEmpty(opts.PhaseModifyingFilter)){}
        
        s.Stop();
        Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}