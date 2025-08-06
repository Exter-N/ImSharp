using ImSharp;

namespace ImSharp;

public unsafe struct ImSharpContext : IDisposable
{
    public static readonly ImSharpContext  Empty;
    public static readonly ImSharpContext* EmptyPointer = (ImSharpContext*)Unsafe.AsPointer(ref Empty);

    public long Version;

    public byte* HintBuffer;
    public byte* LabelBuffer;
    public byte* TextBuffer;
    public byte* InputBuffer;

    public int HintBufferSize;
    public int LabelBufferSize;
    public int TextBufferSize;
    public int InputBufferSize;

    public void* ImGuiContext;
    public void* MonoFont;

    public static ImSharpContext* SetupDefault()
    {
        var ret = (ImSharpContext*)Marshal.AllocHGlobal(sizeof(ImSharpContext));
        ret->Version        = 1;
        ret->HintBuffer     = (byte*)Marshal.AllocHGlobal(128 * 1024 - 1);
        ret->HintBufferSize = 128 * 1024 - 1;

        ret->InputBuffer     = (byte*)Marshal.AllocHGlobal(8 * 1024 * 1024 - 1);
        ret->InputBufferSize = 8 * 1024 * 1024 - 1;

        ret->LabelBuffer     = (byte*)Marshal.AllocHGlobal(128 * 1024 - 1);
        ret->LabelBufferSize = 128 * 1024 - 1;

        ret->TextBuffer     = (byte*)Marshal.AllocHGlobal(4 * 1024 * 1024 - 1);
        ret->TextBufferSize = 4 * 1024 * 1024 - 1;
        ret->ImGuiContext   = Im.Context.Pointer;
        ret->MonoFont       = null;

        return ret;
    }

    public static void TearDownDefault(ImSharpContext* context)
    {
        Marshal.FreeHGlobal((nint)context->HintBuffer);
        Marshal.FreeHGlobal((nint)context->LabelBuffer);
        Marshal.FreeHGlobal((nint)context->TextBuffer);
        Marshal.FreeHGlobal((nint)context->InputBuffer);
        Marshal.FreeHGlobal((nint)context);
    }

    public void Dispose()
    {
        ImGuiContext    = null;
        HintBuffer      = null;
        LabelBuffer     = null;
        TextBuffer      = null;
        InputBuffer     = null;
        MonoFont        = null;
        HintBufferSize  = 0;
        LabelBufferSize = 0;
        TextBufferSize  = 0;
        InputBufferSize = 0;
    }
}
