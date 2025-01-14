using System.Diagnostics;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using Task4;

Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    // try
    // {
        var s = new Stopwatch();
        s.Start();
        
        if (!string.IsNullOrEmpty(opts.OneDimensionalFourier)) {}

        if (!string.IsNullOrEmpty(opts.StandardDiscreteFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.StandardDiscreteFourier(image).SaveAsPng(opts.StandardDiscreteFourier);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.InverseStandardFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.StandardInverseFourier(image).SaveAsPng(opts.InverseStandardFourier);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.FastFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.FastFourier(image).SaveAsPng(opts.FastFourier);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.InverseFastFourier))
        {
            var magImage = Image.Load<L8>(opts.Input);
            Operations.InverseFastFourier(magImage).SaveAsPng(opts.InverseFastFourier);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }
        if (!string.IsNullOrEmpty(opts.LowPassFilter)){}
        if (!string.IsNullOrEmpty(opts.HighPassFilter)){}
        if (!string.IsNullOrEmpty(opts.BandPassFilter)){}
        if (!string.IsNullOrEmpty(opts.BandCutFilter)){}
        if (!string.IsNullOrEmpty(opts.HighPassEdgeFilter)){}
        if (!string.IsNullOrEmpty(opts.PhaseModifyingFilter)){}
    // }
    // catch (Exception e)
    // {
    //     Console.WriteLine("Something went wrong: " + e.Message);
    // }
}