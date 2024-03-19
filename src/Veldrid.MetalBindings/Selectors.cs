namespace Veldrid.MetalBindings
{
    internal static class Selectors
    {
        internal static readonly Selector texture = "texture"u8;
        internal static readonly Selector setTexture = "setTexture:"u8;
        internal static readonly Selector loadAction = "loadAction"u8;
        internal static readonly Selector setLoadAction = "setLoadAction:"u8;
        internal static readonly Selector storeAction = "storeAction"u8;
        internal static readonly Selector setStoreAction = "setStoreAction:"u8;
        internal static readonly Selector resolveTexture = "resolveTexture"u8;
        internal static readonly Selector setResolveTexture = "setResolveTexture:"u8;
        internal static readonly Selector slice = "slice"u8;
        internal static readonly Selector setSlice = "setSlice:"u8;
        internal static readonly Selector level = "level"u8;
        internal static readonly Selector setLevel = "setLevel:"u8;
        internal static readonly Selector objectAtIndexedSubscript = "objectAtIndexedSubscript:"u8;
        internal static readonly Selector setObjectAtIndexedSubscript = "setObject:atIndexedSubscript:"u8;
        internal static readonly Selector pixelFormat = "pixelFormat"u8;
        internal static readonly Selector setPixelFormat = "setPixelFormat:"u8;
        internal static readonly Selector alloc = "alloc"u8;
        internal static readonly Selector init = "init"u8;
        internal static readonly Selector pushDebugGroup = "pushDebugGroup:"u8;
        internal static readonly Selector popDebugGroup = "popDebugGroup"u8;
        internal static readonly Selector insertDebugSignpost = "insertDebugSignpost:"u8;
    }
}
