using System.Diagnostics;
using CommandLine;
using Task4;

Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);

return;

static void RunOptions(Options opts)
{
    try
    {
        var s = new Stopwatch();
        s.Start();
        
        if (!string.IsNullOrEmpty(opts.Fourier)){}
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