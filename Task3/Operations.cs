using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Task3;

public static class Operations
{
    #region basic operations
    #endregion
    
    #region M6
    #endregion
    
    #region region growing (merging)
    #endregion
    
    private static byte ToByte(this int value)
    {
        return value switch
        {
            < byte.MinValue => 0,
            > byte.MaxValue => byte.MaxValue,
            _ => (byte)value
        };
    }
}