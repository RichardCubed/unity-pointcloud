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


        public void disposeAssets()
        {
            Indices.Dispose();
            Vertices.Dispose();
        }

    }
}