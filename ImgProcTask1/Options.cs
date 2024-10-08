using CommandLine;

namespace ImgProcTask1;

public class Options
{
    [Option(longName: "input", Required = true, HelpText = "Input file.")]
    public string Input { get; set; }
    
    [Option(longName: "output", Required = true, HelpText = "Output file.")]
    public string Output { get; set; }
    
    #region Elementary operations (B)

    [Option(longName: "brightness", Required = false, HelpText = "Brightness value.")]
    public int Brightness { get; set; }
    
    [Option(longName: "contrast", Required = false, HelpText = "Contrast value.")]
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
    
}