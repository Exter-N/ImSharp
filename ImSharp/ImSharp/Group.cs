namespace ImSharp;

public static partial class Im
{
    /// <inheritdoc cref="GroupDisposable(bool)"/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static GroupDisposable Group()
        => new(true);
}
