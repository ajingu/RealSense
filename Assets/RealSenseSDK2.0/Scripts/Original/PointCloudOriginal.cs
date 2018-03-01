using Intel.RealSense;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PointCloudOriginal : MonoBehaviour {

    [Header("Device Settings")]
    [SerializeField]
    Stream[] sourceStreamTypes;

    [SerializeField]
    RealsenseDeviceOriginal realSenseDeviceOriginal;

    [Header("Camera Size")]
    [SerializeField] int width;
    [SerializeField] int height;

    private PointCloud pointCloud;
    int dataLength;

    //PointCloud data
    Points.Vertex[] vertices;
    ComputeBuffer pointCloudBuffer;
    int pointStride;

    //Color data
    byte[] byteColorData;
    uint[] uintColorData;
    ComputeBuffer colorBuffer;
    int colorStride;

    //TRS Matrix
    Matrix4x4 m;

    [Header("Visualizer")]
    [SerializeField] Material pointCloudVisualizer;

    Align aligner;
    
    public bool fetchFramesFromDevice = true;


    void Start()
    {
        if (realSenseDeviceOriginal.Instance.ActiveProfile.Streams == null)
            return;

        int numStream = realSenseDeviceOriginal.Instance.ActiveProfile.Streams.Where(p => sourceStreamTypes.Contains(p.Stream)).Count();
        if (numStream < sourceStreamTypes.Length)
            return;

        if (fetchFramesFromDevice)
        {
            if (realSenseDeviceOriginal.Instance.processMode == RealsenseDeviceOriginal.ProcessMode.UnityThread)
            {
                // do nothing
            }
            else
            {
                realSenseDeviceOriginal.Instance.onNewSampleSet += onNewSampleSetThreading;
            }
        }

        pointCloud = new PointCloud();

        dataLength = width * height;
        
        pointStride = Marshal.SizeOf(typeof(Points.Vertex));
        colorStride = sizeof(uint) * 3;

        pointCloudBuffer = new ComputeBuffer(dataLength, pointStride);
        colorBuffer = new ComputeBuffer(dataLength, colorStride);
        
        aligner = new Align(Stream.Color);
        
    }

 
    
    void onNewSampleSetThreading(FrameSet frameSet)
    {
        
        using (FrameSet aligned = aligner.Process(frameSet)){

            VideoFrame vidFrame = aligned.Where(x => x.Profile.Stream == Stream.Color).First() as VideoFrame;
            Frame depthFrame = aligned.Where(x => x.Profile.Stream == Stream.Depth).First();


            byteColorData = byteColorData ?? new byte[vidFrame.Stride * vidFrame.Height];
            vidFrame.CopyTo(byteColorData);
            uintColorData = Array.ConvertAll(byteColorData, x => (uint)x);

            var points = pointCloud.Calculate(depthFrame);
            vertices = vertices ?? new Points.Vertex[dataLength];
            points.CopyTo(vertices);
        }
    }

    
    void OnRenderObject()
    {
        if (pointCloudVisualizer == null)
            return;

        if (vertices == null || uintColorData == null)
            return;

        pointCloudBuffer.SetData(vertices);

        colorBuffer.SetData(uintColorData);

        pointCloudVisualizer.SetBuffer("_PointCloudData", pointCloudBuffer);
        pointCloudVisualizer.SetBuffer("_ColorData", colorBuffer);

        m = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        
        pointCloudVisualizer.SetMatrix("_trs", m);
        
        pointCloudVisualizer.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, dataLength);
    }
    
    
    void OnApplicationQuit()
    {
        if(pointCloudBuffer != null)
            pointCloudBuffer.Dispose();

        if (colorBuffer != null)
            colorBuffer.Dispose();
    }
    
}
