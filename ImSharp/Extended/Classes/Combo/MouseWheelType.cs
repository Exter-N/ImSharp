using ImSharp;

namespace OtterGui.Widgets;

/// <summary> The different types of mousewheel scrolling supported for combo previews. </summary>
/// <remarks> The modifiers can be combined as flags. </remarks>
[Flags]
public enum MouseWheelType : byte
{
    /// <summary> Do not interact with the mousewheel. </summary>
    None = 0,

    /// <summary> React to any mousewheel interaction while hovering the preview. </summary>
    Unmodified = 1,

    /// <summary> Only react to mousewheel interaction while hovering the preview if Shift is held. </summary>
    Shift = 2,

    /// <summary> Only react to mousewheel interaction while hovering the preview if Control is held. </summary>
    Control = 4,

    /// <summary> Only react to mousewheel interaction while hovering the preview if Alt is held. </summary>
    Alt = 8,
}

public static class MouseWheelTypeExtensions
{
    /// <summary> Check the modifiers for the mousewheel check. </summary>
    public static bool CheckMouseWheel(this MouseWheelType type)
        => type switch
        {
            MouseWheelType.None                                                => false,
            MouseWheelType.Unmodified                                          => true,
            MouseWheelType.Shift                                               => Im.Io.KeyShift,
            MouseWheelType.Control                                             => Im.Io.KeyControl,
            MouseWheelType.Alt                                                 => Im.Io.KeyAlt,
            MouseWheelType.Shift | MouseWheelType.Control                      => Im.Io.KeyShift && Im.Io.KeyControl,
            MouseWheelType.Shift | MouseWheelType.Alt                          => Im.Io.KeyShift && Im.Io.KeyAlt,
            MouseWheelType.Control | MouseWheelType.Alt                        => Im.Io.KeyControl && Im.Io.KeyAlt,
            MouseWheelType.Shift | MouseWheelType.Control | MouseWheelType.Alt => Im.Io.KeyShift && Im.Io.KeyControl && Im.Io.KeyAlt,
            _                                                                  => true,
        };
}
