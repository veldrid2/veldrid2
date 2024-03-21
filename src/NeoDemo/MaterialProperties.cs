using System.Numerics;
using System.Runtime.InteropServices;

namespace Veldrid.NeoDemo
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MaterialProperties
    {
        public Vector3 SpecularIntensity;
        public float SpecularPower;
        private Vector3 _padding0;
        public float Reflectivity;
    }
}
