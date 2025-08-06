namespace ImSharp;

public unsafe struct InputStringHandlerBuffer : IStringHandlerBuffer
{
    public static int Size
        => ImSharpConfiguration.Context->InputBufferSize;

    public static byte* Buffer
        => ImSharpConfiguration.Context->InputBuffer;

    public static Span<byte> Span
        => new(Buffer, Size);

    [MethodImpl(ImSharpConfiguration.OptInl)]
    public static bool Write(ReadOnlySpan<char> text, out byte* end)
        => IStringHandlerBuffer.Write<InputStringHandlerBuffer>(text, out end);
}
