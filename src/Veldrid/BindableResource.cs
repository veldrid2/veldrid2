using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Veldrid
{
    /// <summary>
    /// A resource which can be bound in a <see cref="ResourceSet"/> and used in a shader.
    /// </summary>
    /// <remarks>
    /// See <see cref="DeviceBuffer"/>, <see cref="DeviceBufferRange"/>, <see cref="Texture"/>, <see cref="TextureView"/>
    /// and <see cref="Sampler"/>.
    /// </remarks>
    public readonly struct BindableResource : IEquatable<BindableResource>
    {
        public BindableResourceKind Kind { get; }

        public object? Resource { get; }

        private BindableResource(BindableResourceKind kind, object? resource)
        {
            Debug.Assert(kind != BindableResourceKind.Null || resource == null, "Non-null resource for Null kind.");

            Kind = resource != null ? kind : BindableResourceKind.Null;
            Resource = resource;
        }

        [MemberNotNullWhen(true, nameof(Resource))]
        private bool IsNotNull
        {
            get
            {
                if (Kind != BindableResourceKind.Null)
                {
                    Debug.Assert(Resource != null);
                    return true;
                }
                return false;
            }
        }

        public Texture GetTexture() => As<Texture>(BindableResourceKind.Texture);

        public TextureView GetTextureView() => As<TextureView>(BindableResourceKind.TextureView);

        public DeviceBuffer GetDeviceBuffer() => As<DeviceBuffer>(BindableResourceKind.DeviceBuffer);

        public DeviceBufferRange GetDeviceBufferRange() => Unbox<DeviceBufferRange>(BindableResourceKind.DeviceBufferRange);

        public Sampler GetSampler() => As<Sampler>(BindableResourceKind.Sampler);

        public bool Equals(BindableResource other)
        {
            // We can check Kind as a fast path,
            // which also saves us from doing actual null checks.
            return Kind == other.Kind && (IsNotNull && Resource.Equals(other.Resource));
        }

        public override int GetHashCode()
        {
            // Including the Kind in the hash is pointless
            // since Resource must be of the right kind.
            return Resource?.GetHashCode() ?? 0;
        }

        private T As<T>(BindableResourceKind kind)
            where T : class
        {
            if (Kind == kind)
            {
                Debug.Assert(Resource is T, "Resource is not of the correct Kind.");
                return Unsafe.As<T>(Resource)!;
            }
            ThrowMismatch(kind, Kind);
            return default!;
        }

        private T Unbox<T>(BindableResourceKind kind)
            where T : struct
        {
            if (Kind == kind)
            {
                Debug.Assert(Resource is T, "Resource is not of the correct Kind.");
                return Unsafe.Unbox<T>(Resource!);
            }
            ThrowMismatch(kind, Kind);
            return default!;
        }

        public static implicit operator BindableResource(Texture? texture) =>
            new(BindableResourceKind.Texture, texture);

        public static implicit operator BindableResource(TextureView? textureView) =>
            new(BindableResourceKind.TextureView, textureView);

        public static implicit operator BindableResource(DeviceBuffer? deviceBuffer) =>
            new(BindableResourceKind.DeviceBuffer, deviceBuffer);

        public static implicit operator BindableResource(DeviceBufferRange deviceBufferRange) =>
            new(BindableResourceKind.DeviceBufferRange, deviceBufferRange);

        public static implicit operator BindableResource(Sampler? sampler) =>
            new(BindableResourceKind.Sampler, sampler);

        private static void ThrowMismatch(BindableResourceKind expected, BindableResourceKind actual)
        {
            throw new VeldridException($"Resource type mismatch. Expected {expected} but got {actual}.");
        }
    }
}
