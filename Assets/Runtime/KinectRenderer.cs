using System.Collections.Generic;
using UnityEngine;

namespace Assets.Runtime
{
    public class KinectRenderer : MonoBehaviour
    {
        private Mesh _mesh;
        private MeshFilter _filter;

        private List<Vector3> _vertices;
        private List<int> _indices;

        public void Start()
        {
            // Fetch local references for speed
            _filter = GetComponent<MeshFilter>();

            // Programatically create a new mesh 
            _mesh = new Mesh
            {
                // 32Bit indexs allow for meshes with greater than 65k vertices
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            
            // Optimize the mesh for frequent updates.
            _mesh.MarkDynamic();
            
            // Create some random vertices
            _vertices = new List<Vector3>();
            _indices = new List<int>();
            
            for (var i = 0; i < 1000000; i++)
            {
                var point = new Vector3(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));
                _vertices.Add(point);
                _indices.Add(i);
            }            
            
            // Instantiate
            _filter.mesh = _mesh;
        }

        public void Update()
        {
            // Update the mesh
            _mesh.SetVertices(_vertices);
            _mesh.SetIndices(_indices, MeshTopology.Points, 0);
        }
    }
}
