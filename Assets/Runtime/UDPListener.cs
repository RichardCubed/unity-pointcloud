using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

using System.Collections;
using System.Net.Sockets;


namespace Assets.Runtime
{
    public class UDPListener : MonoBehaviour
    {
        public Renderer pointRenderer;

        // public string IP = "127.0.0.1"; default local
        public int port = 9001;
        private JobHandle jHandle;
        private UDPJob persistentJob;

        public NativeArray<int> buffer_indices;
        public NativeArray<Vertex> buffer_vertices;


        public IEnumerator Start()
        {
            yield return new WaitUntil(() => pointRenderer._buffer != null);
            buffer_indices = new NativeArray<int>(pointRenderer._buffer.Vertices.Length, Allocator.Persistent);
            buffer_vertices = new NativeArray<Vertex>(pointRenderer._buffer.Vertices.Length, Allocator.Persistent);
        }


        public void Update()
        {
            persistentJob = new UDPJob()
            {
                _indices = buffer_indices,
                _vertices = buffer_vertices,
                port = port,
                run = true
            };

            jHandle = persistentJob.Schedule();
        }


        public void LateUpdate()
        {
            //persistentJob.run = false;
            jHandle.Complete();
            persistentJob._vertices.CopyTo(pointRenderer._buffer.Vertices);
            persistentJob._indices.CopyTo(pointRenderer._buffer.Indices);
            //pointRenderer.updateBuffer(persistentJob._indices, persistentJob._vertices);
        }


        struct UDPJob : IJob
        {
            //public UdpClient client;
            public int port;
            public bool run;

            public NativeArray<int> _indices;
            public NativeArray<Vertex> _vertices;


            public void Execute()
            {
                //client = new UdpClient(port);
                var rand = new System.Random();

                //while (run)
                //{
                    /*
                    try
                    {
                        // Bytes empfangen.
                        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = client.Receive(ref anyIP);
                    }
                    catch (Exception err)
                    {
                        Debug.Log(err.ToString());
                    }
                    */

                    //_indices = new NativeArray<int>(500000, Allocator.Persistent);
                    //_vertices = new NativeArray<Vertex>(500000, Allocator.Persistent);

                    for (var i = 0; i < _vertices.Length; i++)
                    {
                        var point = new Vertex();
                        point.Position = new float3(((float)rand.NextDouble()*100f)-50f, ((float)rand.NextDouble() * 100f) - 50f, ((float)rand.NextDouble() * 100f) - 50f);
                        point.Color = new float4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1);
                        _vertices[i] = point;
                        _indices[i] = i;
                    }
                //}
            }

        }

    }
}


