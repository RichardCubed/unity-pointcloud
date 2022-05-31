using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

namespace Assets.Runtime
{
    public class Renderer : MonoBehaviour
    {
        private Mesh _mesh;
        private MeshFilter _filter;

        public Buffer _buffer;

        /// <summary>
        /// Initialise
        /// </summary>
        public void Start()
        {
            // Create a new buffer for our streamed vertex data
            _buffer = new Buffer(500000);
            _buffer.Randomise();
            
            // Create a mesh 
            _mesh = new Mesh
            {
                // A 32bit index allows for greater than 65k vertices
                indexFormat = IndexFormat.UInt32
            };
            
            // Optimize the mesh for frequent updates.
            _mesh.MarkDynamic();
            
            // We only have a single sub-mesh, skinned meshes can have more
            _mesh.subMeshCount = 1;
            
            // Assign the mesh the parent GameObject
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        /// <summary>
        /// Dynamically update our mesh
        /// </summary>
        public void Update()
        {
            // Set the new vertex data
            _mesh.SetVertexBufferParams(_buffer.Size, Descriptor.Layout);
            _mesh.SetVertexBufferData(_buffer.Vertices, 0, 0, _buffer.Size);
            
            // Set the new indexes
            _mesh.SetIndexBufferParams(_buffer.Size, IndexFormat.UInt32);
            _mesh.SetIndexBufferData(_buffer.Indices, 0, 0, _buffer.Size);

            // Set the sub-descriptor, use only point data
            var subDescriptor = new SubMeshDescriptor {
                topology = MeshTopology.Points,
                vertexCount = _buffer.Size,
                indexCount = _buffer.Size 
            };
            
            // Update
            _mesh.SetSubMesh(0, subDescriptor);
            _mesh.UploadMeshData(false);
        }


        public void updateBuffer(NativeArray<int> newInds, NativeArray<Vertex> newVerts)
        {
            _buffer.updateBuffer(newInds, newVerts);
        }
    }
}
