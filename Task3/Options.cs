using CommandLine;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Task3;

public class Options
{
    [Option(longName: "input", Required = true, HelpText = "Input file. Format: path.")]
    public required string Input { get; set; }
    
    #region basic operations
    [Option(longName: "dilation", Required = false, HelpText = "Dilation")]
    public string? Dilation { get; set; }
    
    //erosion
    [Option(longName: "erosion", Required = false, HelpText = "Erosion")]
    public string? Erosion { get; set; }
    //opening
    [Option(longName: "opening", Required = false, HelpText = "Opening")]
    public string? Opening { get; set; }
    //closing
    [Option(longName: "closing", Required = false, HelpText = "Closing")]
    public string? Closing { get; set; }
    //HMT transformation
    [Option(longName: "HMT transformation", Required = false, HelpText = "HMT transformation")]
    public string? HmtTransformation { get; set; }
    #endregion
    
    #region M6
    [Option(longName: "M6", Required = false, HelpText = "M6")]
    public string? M6 { get; set; }
    #endregion
    
    #region region growing (merging)
    [Option(longName: "region growing (merging)", Required = false, HelpText = "Region growing (merging)")]
    public string? RegionGrowing { get; set; }
    #endregion
}