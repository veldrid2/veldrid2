using System;
using System.IO;
using Android.Content.Res;
using Veldrid.Tests.Utilities;

namespace Veldrid.Tests.Android
{
    internal class AssetsShaderProvider : IShaderProvider
    {
        private readonly AssetManager _assets;

        public AssetsShaderProvider(AssetManager assets)
        {
            _assets = assets ?? throw new ArgumentNullException(nameof(assets));
        }

        public string GetPath(string name)
        {
            return Path.Combine("Shaders", name);
        }

        public Stream OpenRead(string path)
        {
            return _assets.Open(path);
        }
    }
}
