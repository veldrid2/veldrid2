namespace Veldrid
{
    /// <summary>
    /// The kind of a <see cref="BindableResource"/> object.
    /// </summary>
    public enum ResourceKind : byte
    {
        /// <summary>
        /// A <see cref="DeviceBuffer"/> accessed as a uniform buffer. A subset of a buffer can be bound using a
        /// <see cref="DeviceBufferRange"/>.
        /// </summary>
        UniformBuffer,

        /// <summary>
        /// A <see cref="DeviceBuffer"/> accessed as a read-only storage buffer. A subset of a buffer can be bound using a
        /// <see cref="DeviceBufferRange"/>.
        /// </summary>
        StructuredBufferReadOnly,

        /// <summary>
        /// A <see cref="DeviceBuffer"/> accessed as a read-write storage buffer. A subset of a buffer can be bound using a
        /// <see cref="DeviceBufferRange"/>.
        /// </summary>
        StructuredBufferReadWrite,

        /// <summary>
        /// A read-only texture, accessed through a <see cref="Texture"/> or <see cref="TextureView"/>.
        /// <remarks>
        /// Binding a <see cref="Texture"/> to a resource slot expecting <see cref="TextureReadOnly"/> is equivalent to binding a
        /// <see cref="TextureView"/> that covers the full mip and array layer range, with the original <see cref="Texture.Format"/>.
        /// </remarks>
        /// </summary>
        TextureReadOnly,

        /// <summary>
        /// A read-write texture, accessed through a <see cref="Texture"/> or <see cref="TextureView"/>.
        /// </summary>
        /// <remarks>
        /// Binding a <see cref="Texture"/> to a resource slot expecting <see cref="TextureReadWrite"/> is equivalent to binding a
        /// <see cref="TextureView"/> that covers the full mip and array layer range, with the original <see cref="Texture.Format"/>.
        /// </remarks>
        TextureReadWrite,

        /// <summary>
        /// A <see cref="Veldrid.Sampler"/>.
        /// </summary>
        Sampler,
    }
}
