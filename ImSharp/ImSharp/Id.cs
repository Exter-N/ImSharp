namespace ImSharp;

public static partial class Im
{
    public static class Id
    {
        /// <inheritdoc cref="IdDisposable.Push(ImGuiId)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        [OverloadResolutionPriority(100)]
        public static IdDisposable Push(ImGuiId id)
            => new IdDisposable().Push(id);

        /// <inheritdoc cref="IdDisposable.Push(nint)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        [OverloadResolutionPriority(50)]
        public static IdDisposable Push(nint id)
            => new IdDisposable().Push(id);

        /// <inheritdoc cref="IdDisposable.Push(Utf8LabelHandler)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static IdDisposable Push(Utf8LabelHandler id)
            => new IdDisposable().Push(ref id);

        /// <inheritdoc cref="IdDisposable.Push(Utf8LabelHandler)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static IdDisposable Push<T>(ref Utf8StringHandler<T> id) where T : IStringHandlerBuffer
            => new IdDisposable().Push(ref id);

        /// <summary> Calculate an ID based on the current ID stack and the given ID string. </summary>
        /// <param name="id"> The ID string as text. Does not have to be null-terminated. </param>
        /// <returns> The calculated ID. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static unsafe ImGuiId Get(Utf8LabelHandler id)
            => Native.Methods.IdStack.GetId(id.Start(out var end), end);

        /// <inheritdoc cref="Get(Utf8LabelHandler)"/>
        /// <typeparam name="T"> The buffer type. </typeparam>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static unsafe ImGuiId Get<T>(ref Utf8StringHandler<T> id) where T : IStringHandlerBuffer
            => Native.Methods.IdStack.GetId(id.Start(out var end), end);

        /// <summary> Calculate an ID based on the current ID stack and the given pointer. </summary>
        /// <param name="pointer"> The pointer to be used as an ID. This does not access the content of the pointer. </param>
        /// <returns> The calculated ID. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static ImGuiId Get(nint pointer)
            => Native.Methods.IdStack.GetId(pointer);

        /// <summary> Get the last ID from the ID stack of the current window. </summary>
        public static unsafe ImGuiId Current
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Context.Pointer->CurrentWindow->IdStack[^1];
        }

        /// <summary> Get the ID of the currently active widget. </summary>
        public static ImGuiId Active
        {
            [MethodImpl(ImSharpConfiguration.OptInl)]
            get => Context.ActiveId;
        }

        /// <summary> Check whether an ID represents the currently active widget. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool IsActive(ImGuiId id)
            => Context.ActiveId == id;

        /// <summary> Check whether an ID represents the last drawn widget. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool IsCurrent(ImGuiId id)
            => Current == id;
    }
}
