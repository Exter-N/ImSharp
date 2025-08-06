namespace ImSharp;

/// <summary> A wrapper around an arbitrary ID type for textures. Could be changed when compiling ImGui. </summary>
public readonly record struct ImTextureId(nint Value) : ISpanFormattable, IUtf8SpanFormattable
{
    public static readonly ImTextureId Zero = new(nint.Zero);

    [MethodImpl(ImSharpConfiguration.OptInl)]
    public override string ToString()
        => Value.ToString("X");

    /// <inheritdoc/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => Value.ToString(format, formatProvider);

    /// <inheritdoc/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public bool TryFormat(Span<char> destination, out int charsWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
        ReadOnlySpan<char> format, IFormatProvider? provider)
        => Value.TryFormat(destination, out charsWritten, format, provider);

    /// <inheritdoc/>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    public bool TryFormat(Span<byte> destination, out int bytesWritten,
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
        ReadOnlySpan<char> format, IFormatProvider? provider)
        => Value.TryFormat(destination, out bytesWritten, format, provider);
};
