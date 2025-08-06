namespace ImSharp;

public static partial class Im
{
    /// <summary> A reference to a viewport. </summary>
    /// <param name="pointer"> The native pointer to the viewport. </param>
    public readonly unsafe ref struct Viewport(Native.Viewport* pointer)
    {
        /// <summary> The address of the native object. </summary>
        public readonly Native.Viewport* Pointer = pointer;

        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static implicit operator Viewport(Native.Viewport* pointer)
            => new(pointer);

        /// <summary> Get the primary viewport. This can never be invalid. </summary>
        public static Viewport Main
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Native.Methods.DrawList.GetMainViewport();
        }

        /// <summary> Get the background draw list for the given viewport. </summary>
        /// <remarks> The background draw list is the first that renders, so anything else is rendered on top of it. </remarks>
        public static DrawList GetBackgroundDrawList(Viewport viewport)
            => Native.Methods.DrawList.GetBackgroundDrawList(viewport.Pointer);

        /// <summary> Get the foreground draw list for the given viewport. </summary>
        /// <remarks> The foreground draw list is the last that renders, so it renders on top of everything else. </remarks>
        public static DrawList GetForegroundDrawList(Viewport viewport)
            => Native.Methods.DrawList.GetForegroundDrawList(viewport.Pointer);

        /// <summary> Helper function for backends to get a specific viewport by ID. </summary>
        /// <param name="id"> The Viewport ID. </param>
        /// <returns> A reference to the viewport that may be invalid. </returns>
        public static Viewport FindById(ImGuiId id)
            => Native.Methods.Viewport.FindViewportById(id);

        /// <summary> Helper function for backends to get a specific viewport by platform-specific handle. </summary>
        /// <param name="handle"> The handle to query. </param>
        /// <returns> A reference to the viewport that may be invalid. </returns>
        /// <remarks> Typical handles are <c>HWND</c>, <c>MyWindow*</c> or <c>GLFWwindow*</c> and similar. </remarks>
        public static Viewport FindByPlatformHandle(nint handle)
            => Native.Methods.Viewport.FindViewportByPlatformHandle((void*)handle);
    }
}
