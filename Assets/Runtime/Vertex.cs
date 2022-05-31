using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Assets.Runtime
{
    /// <summary>
    /// Defines a vertex.
    /// </summary>
    public struct Vertex
    {
        public float3 Position;
        public float4 Color;
    }
    
    /// <summary>
    /// Defines the underlying structure of a vertex, used to map data from managed to unmanaged memory.
    /// </summary>
    static class Descriptor
    {
        public static readonly VertexAttributeDescriptor[] Layout =
        {
            new()
            {
                attribute = VertexAttribute.Position,
                format = VertexAttributeFormat.Float32,
                dimension = 3
            },
            new()
            {
                attribute = VertexAttribute.Color,
                format = VertexAttributeFormat.Float32,
                dimension = 4
            }
        };
    }    
}