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

        if (!string.IsNullOrEmpty(opts.LowPassFilter))
        {
            var path = opts.LowPassFilter.Split('.');
            var magImage = Image.Load<L8>(opts.Input);
            var ext = path[1].Split(';')[0];
            var size = int.Parse(path[1].Split(';')[1]);
            var (aa, bb, cc) = Operations.Filter(magImage, d => d <= size);
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.HighPassFilter))
        {
            var path = opts.HighPassFilter.Split('.');
            var magImage = Image.Load<L8>(opts.Input);
            var ext = path[1].Split(';')[0];
            var size = int.Parse(path[1].Split(';')[1]);
            var (aa, bb, cc) = Operations.Filter(magImage, d => d >= size);
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.BandPassFilter))
        {
            var path = opts.BandPassFilter.Split('.');
            var magImage = Image.Load<L8>(opts.Input);
            var ext = path[1].Split(';')[0];
            var size1 = int.Parse(path[1].Split(';')[1]);
            var size2 = int.Parse(path[1].Split(';')[2]);
            var (aa, bb, cc) = Operations.Filter(magImage, d => d >= size1 && d <= size2);
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.BandCutFilter))
        {
            var path = opts.BandCutFilter.Split('.');
            var magImage = Image.Load<L8>(opts.Input);
            var ext = path[1].Split(';')[0];
            var size1 = int.Parse(path[1].Split(';')[1]);
            var size2 = int.Parse(path[1].Split(';')[2]);
            var (aa, bb, cc) = Operations.Filter(magImage, d => d <= size1 || d >= size2);
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.HighPassEdgeFilter))
        {
            var path = opts.HighPassEdgeFilter.Split('.');
            var image = Image.Load<L8>(opts.Input.Split(';')[0]);
            var mask = Image.Load<L8>(opts.Input.Split(';')[1]);
            var (aa, bb, cc) = Operations.HighPassEdgeFilter(image, mask);
            var ext = path[1].Split(';')[0];
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }

        if (!string.IsNullOrEmpty(opts.PhaseModifyingFilter))
        {
            var path = opts.PhaseModifyingFilter.Split('.');
            var image = Image.Load<L8>(opts.Input);
            var ext = path[1].Split(';')[0];
            var k = int.Parse(path[1].Split(';')[1]);
            var l = int.Parse(path[1].Split(';')[2]);
            var (aa, bb, cc) = Operations.PhaseFilter(image, k, l);
            aa.SaveAsPng(path[0] + "_magnitude." + ext);
            bb.SaveAsPng(path[0] + "_filter." + ext);
            cc.SaveAsPng(path[0] + "_result." + ext);
            s.Stop();
            Console.WriteLine("Time: " + s.ElapsedMilliseconds + "ms");
            return;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Something went wrong: " + e.Message);
    }
}