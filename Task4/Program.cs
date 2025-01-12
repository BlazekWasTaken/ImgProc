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
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        if (!string.IsNullOrEmpty(opts.OneDimensionalFourier)) {}

        if (!string.IsNullOrEmpty(opts.StandardDiscreteFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            Operations.StandardDiscreteFourier(image).SaveAsBmp(opts.StandardDiscreteFourier);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }
        if (!string.IsNullOrEmpty(opts.InverseStandardFourier)){}

        if (!string.IsNullOrEmpty(opts.FastFourier))
        {
            var image = Image.Load<L8>(opts.Input);
            var (mag, phase) = Operations.FastFourier(image);
            mag.SaveAsBmp(opts.FastFourier);
            var name = opts.FastFourier.Split('.')[0];
            var ext = opts.FastFourier.Split('.')[1];
            phase.SaveAsBmp(name + "_phase." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.InverseFastFourier))
        {
            var name = opts.Input.Split('.')[0];
            var ext = opts.Input.Split('.')[1];
            var magImage = Image.Load<L8>(opts.Input);
            var phaseImage = Image.Load<L8>(name + "_phase." + ext);
            Operations.InverseFastFourier(magImage, phaseImage).SaveAsBmp(opts.InverseFastFourier);
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
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}