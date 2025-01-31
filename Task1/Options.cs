using CommandLine;

namespace Task1;

public class Options
{
    [Option(longName: "input", Required = true, HelpText = "Input file.")]
    public required string Input { get; set; }
    
    [Option(longName: "output", Required = true, HelpText = "Output file.")]
    public required string Output { get; set; }
    
    #region Elementary operations (B)
    [Option(longName: "brightness", Required = false, HelpText = "Brightness change value.")]
    public int Brightness { get; set; }
    
    [Option(longName: "contrast", Required = false, HelpText = "Contrast change value.")]
    public double Contrast { get; set; }
    
    [Option(longName: "negative", Required = false, HelpText = "Negative.", Default = false)]
    public bool Negative { get; set; }
    #endregion

    #region Geometric operations (G)
    [Option(longName: "hflip", Required = false, HelpText = "Horizontal flip.", Default = false)]
    public bool HorizontalFlip { get; set; }
    
    [Option(longName: "vflip", Required = false, HelpText = "Vertical flip.", Default = false)]
    public bool VerticalFlip { get; set; }
    
    [Option(longName: "dflip", Required = false, HelpText = "Diagonal flip.", Default = false)]
    public bool DiagonalFlip { get; set; }
    
    [Option(longName: "shrink", Required = false, HelpText = "Shrink value.")]
    public int Shrink { get; set; }
    
    [Option(longName: "enlarge", Required = false, HelpText = "Enlarge value.")]
    public int Enlarge { get; set; }
    #endregion

    #region Noise removal (N)
    [Option(longName: "mid", Required = false, HelpText = "Window size for midpoint filter noise removal.")]
    public int Midpoint { get; set; }
    
    [Option(longName: "amean", Required = false, HelpText = "Window size for arithmetic mean filter noise removal.")]
    public int Mean { get; set; }
    #endregion

    #region Analysis (E)
    [Option(longName: "mse", Required = false, HelpText = "Mean squared error.", Default = false)]
    public bool MeanSquaredError { get; set; }
    
    [Option(longName: "pmse", Required = false, HelpText = "Peak mean squared error.", Default = false)]
    public bool PeakMeanSquaredError { get; set; }
    
    [Option(longName: "snr", Required = false, HelpText = "Signal to noise ratio.", Default = false)]
    public bool SignalToNoiseRatio { get; set; }
    
    [Option(longName: "psnr", Required = false, HelpText = "Peak signal to noise ratio.", Default = false)]
    public bool PeakSignalToNoiseRatio { get; set; }
    
    [Option(longName: "md", Required = false, HelpText = "Maximum difference.", Default = false)]
    public bool MaximumDifference { get; set; }
    #endregion
}