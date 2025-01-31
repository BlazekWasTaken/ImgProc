using CommandLine;

namespace Task4;

public class Options
{
    [Option(longName: "input", Required = true, HelpText = "Input file. Format: path.")]
    public required string Input { get; set; }
    
    [Option(longName: "od-fourier", Required = false, HelpText = "One-dimensional Fourier transform")]
    public string? OneDimensionalFourier { get; set; }
    
    [Option(longName: "sd-fourier", Required = false, HelpText = "Standard Discrete Fourier transform")]
    public string? StandardDiscreteFourier { get; set; }
    
    [Option(longName: "in-sd-fourier", Required = false, HelpText = "Inverse Standard Discrete Fourier transform")]
    public string? InverseStandardFourier { get; set; }
    
    [Option(longName: "fourier", Required = false, HelpText = "Direct fast Fourier transform with decimation in frequency domain")]
    public string? FastFourier { get; set; }
    
    [Option(longName: "in-fourier", Required = false, HelpText = "Inverse fast Fourier transform with decimation in frequency domain")]
    public string? InverseFastFourier { get; set; }
    
    [Option(longName: "low-pass", Required = false, HelpText = "Low-pass filter (high-cut filter)")]
    public string? LowPassFilter { get; set; }
    
    [Option(longName: "high-pass", Required = false, HelpText = "High-pass filter (low-cut filter)")]
    public string? HighPassFilter { get; set; }
    
    [Option(longName: "band-pass", Required = false, HelpText = "Band-pass filter")]
    public string? BandPassFilter { get; set; }
    
    [Option(longName: "band-cut", Required = false, HelpText = "Band-cut filter")]
    public string? BandCutFilter { get; set; }
    
    [Option(longName: "high-pass-edge", Required = false, HelpText = "High-pass filter with detection of edge direction")]
    public string? HighPassEdgeFilter { get; set; }
    
    [Option(longName: "phase-modifying", Required = false, HelpText = "Phase modifying filter")]
    public string? PhaseModifyingFilter { get; set; }
}