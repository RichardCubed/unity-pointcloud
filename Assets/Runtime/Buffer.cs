using Unity.Collections;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Assets.Runtime
{
    public class Buffer
    {
        public readonly int Size;
        
        public NativeArray<int> Indices;
        public NativeArray<Vertex> Vertices;

        public Buffer(int size)
        {
            Size = size;
            
            Indices = new NativeArray<int>(size, Allocator.Persistent);
            Vertices = new NativeArray<Vertex>(size, Allocator.Persistent);
        }


        ~Buffer()
        {
            Indices.Dispose();
            Vertices.Dispose();
        }


        public void Randomise()
        {
            for (var i = 0; i < Vertices.Length; i++)
            {
                Indices[i] = i;
                Vertices[i] = new Vertex
                {
                    Position = new float3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f)), 
                    Color = new float4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
                };
            }
        }


        public void updateBuffer(NativeArray<int> newInds, NativeArray<Vertex> newVerts)
        {
            Indices = newInds;
            Vertices = newVerts;
        }
    }
}