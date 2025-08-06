namespace ImSharp;

public static partial class ImEx
{
    /// <summary> Configuration to control the visuals of an extended button. </summary>
    public readonly struct ButtonConfiguration()
    {
        /// <summary> The size of the button in pixels. </summary>
        public Vector2 Size { get; init; } = default;

        /// <summary> The button's background color. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Button"/> is used. </summary>
        public ColorParameter ButtonColor { get; init; } = ColorParameter.Default;

        /// <summary> The button's background color when hovered by the mouse. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.ButtonHovered"/> is used. </summary>
        public ColorParameter HoveredColor { get; init; } = ColorParameter.Default;

        /// <summary> The button's background color when hovered by the mouse while the associated mouse button is held. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.ButtonActive"/> is used. </summary>
        public ColorParameter ActiveColor { get; init; } = ColorParameter.Default;

        /// <summary> The button's label color. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Text"/> is used. </summary>
        public ColorParameter TextColor { get; init; } = ColorParameter.Default;

        /// <summary> The button's border color. If this is <see cref="ColorParameter.Default"/>, <see cref="ImGuiColor.Border"/> is used. If this is not transparent, borders will be enabled. </summary>
        public ColorParameter BorderColor { get; init; } = ColorParameter.Default;

        /// <summary> Additional flags to control the button's behaviour. </summary>
        public ButtonFlags Flags { get; init; } = ButtonFlags.None;

        /// <summary> Whether the button should be disabled. </summary>
        public bool Disabled { get; init; } = false;
    }
}
