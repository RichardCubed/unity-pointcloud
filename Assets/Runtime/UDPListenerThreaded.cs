using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;



namespace Assets.Runtime
{
    public class UDPListenerThreaded : MonoBehaviour
    {
        public Renderer pointRenderer;

        // public string IP = "127.0.0.1"; default local
        public int port = 9001;

        public NativeArray<int>[] buffer_indices = new NativeArray<int>[2];
        public NativeArray<Vertex>[] buffer_vertices = new NativeArray<Vertex>[2];
        private int bufferSize;

        private Thread ListenerThread;
        private Thread ControlThread;

        private readonly object _lock = new object();
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);
        private readonly AutoResetEvent _listenerContinue = new AutoResetEvent(false);
        private readonly AutoResetEvent _controlContinue = new AutoResetEvent(false);

        private volatile bool running = true;
        private volatile int arrayToUse = 0;
        private const float frameTime = 1000f / 15f;

        private Buffer transferBuffer;
        private volatile bool transferReady = false;


        private IEnumerator Start()
        {
            yield return new WaitUntil(() => pointRenderer._buffer != null);
            bufferSize = pointRenderer._buffer.Vertices.Length;
            //buffer_indices = new NativeArray<int>[2];
            //buffer_vertices = new NativeArray<Vertex>[2];
            buffer_indices[0] = new NativeArray<int>(bufferSize, Allocator.Persistent);
            buffer_indices[1] = new NativeArray<int>(bufferSize, Allocator.Persistent);
            buffer_vertices[0] = new NativeArray<Vertex>(bufferSize, Allocator.Persistent);
            buffer_vertices[1] = new NativeArray<Vertex>(bufferSize, Allocator.Persistent);

            ListenerThread = new Thread(new ThreadStart(Listener));
            ListenerThread.Start();
            ControlThread = new Thread(new ThreadStart(Controller));
            ControlThread.Start();
        }


        private void Update()
        {
            if (transferReady)
            {
                pointRenderer.swapBuffer(transferBuffer);
                transferReady = false;
                _controlContinue.Set();
            }
        }


        private void OnDestroy()
        {
            running = false;
            _signal.Set();
            _listenerContinue.Set();
            _controlContinue.Set();
            ListenerThread.Join();
            ControlThread.Join();
            buffer_indices[0].Dispose();
            buffer_indices[1].Dispose();
            buffer_vertices[0].Dispose();
            buffer_vertices[1].Dispose();
        }


        private void Listener()
        {
            int i = 0;
            var rand = new System.Random();
            float thisFrameTime = DateTime.Now.Ticks / 10000f;

            while (running)
            {
                thisFrameTime = thisFrameTime = DateTime.Now.Ticks / 10000f;
                i = 0;

                while (i < buffer_vertices[arrayToUse].Length)// && ((DateTime.Now.Ticks / 10000f) - thisFrameTime) < frameTime)
                {
                    var point = new Vertex();
                    point.Position = new float3(((float)rand.NextDouble() * 100f) - 50f, ((float)rand.NextDouble() * 100f) - 50f, ((float)rand.NextDouble() * 100f) - 50f);
                    point.Color = new float4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1);
                    lock (_lock)
                    {
                        buffer_vertices[arrayToUse][i] = point;
                        buffer_indices[arrayToUse][i] = i;
                    }
                    i++;
                }

                _signal.Set();
                _listenerContinue.WaitOne();
            }
        }


        private void Controller()
        {
            while (running)
            {
                _signal.WaitOne();
                int arrayToCopy = arrayToUse;
                if (arrayToUse == 0) arrayToUse = 1;
                else arrayToUse = 0;
                _listenerContinue.Set();
                transferBuffer = new Buffer(bufferSize);
                buffer_vertices[arrayToCopy].CopyTo(transferBuffer.Vertices);
                buffer_indices[arrayToCopy].CopyTo(transferBuffer.Indices);
                transferReady = true;
                _controlContinue.WaitOne();
                //pointRenderer.swapBuffer(transferBuffer);
            }
        }
    }
}
