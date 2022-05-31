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
        private UDPJob frameUpdateJob;

        private float lastUpdateTime;
        private bool cycle_started = false;

        public NativeArray<int> buffer_indices;
        public NativeArray<Vertex> buffer_vertices;


        private void Start()
        {
            lastUpdateTime = Time.time;
        }


        public void Update()
        {
            if (Time.time - lastUpdateTime < 0.08)
            {
                if (!cycle_started)
                {
                    var rand = new System.Random();
                    buffer_indices = new NativeArray<int>(300000, Allocator.Persistent);
                    buffer_vertices = new NativeArray<Vertex>(buffer_indices.Length, Allocator.Persistent);
                    cycle_started = true;

                    frameUpdateJob = new UDPJob()
                    {
                        _indices = buffer_indices,
                        _vertices = buffer_vertices,
                        port = port,
                        run = true
                    };

                    jHandle = frameUpdateJob.Schedule();
                }
            }
            else
            {
                //frameUpdateJob.run = false;
                jHandle.Complete();
                Buffer transferBuffer = new Buffer(buffer_indices.Length);
                frameUpdateJob._vertices.CopyTo(transferBuffer.Vertices);
                frameUpdateJob._indices.CopyTo(transferBuffer.Indices);
                pointRenderer.swapBuffer(transferBuffer);
                buffer_indices.Dispose();
                buffer_vertices.Dispose();
                cycle_started = false;
                lastUpdateTime = Time.time;
            }
        }


        private void OnDestroy()
        {
            if (cycle_started)
            {
                jHandle.Complete();
                buffer_indices.Dispose();
                buffer_vertices.Dispose();
            }
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


