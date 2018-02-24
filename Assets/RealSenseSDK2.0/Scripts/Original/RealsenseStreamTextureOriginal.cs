using Intel.RealSense;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class RealsenseStreamTextureOriginal : MonoBehaviour
{
    public Stream sourceStreamType;
    public TextureFormat textureFormat;
    public RealsenseDeviceOriginal realSenseDeviceOriginal;
    private Texture2D texture;

    public TextureProvider.TextureEvent textureBinding;

    [System.NonSerialized]
    byte[] data;

    AutoResetEvent f = new AutoResetEvent(false);
    int threadId;

    void Awake()
    {
        threadId = Thread.CurrentThread.ManagedThreadId;
    }
    /// <summary>
    /// Called per frame before publishing it
    /// </summary>
    /// <param name="f">The frame to process</param>
    /// <returns>The processed frame</returns>
    virtual protected Frame ProcessFrame(Frame f)
    {
        return f;
    }

    public bool fetchFramesFromDevice = true;

    void Start()
    {
        if (realSenseDeviceOriginal.Instance.ActiveProfile.Streams == null)
            return;
        var videoProfile = realSenseDeviceOriginal.Instance.ActiveProfile.Streams.First(p => p.Stream == sourceStreamType) as VideoStreamProfile;
        if (videoProfile == null)
            return;
        texture = new Texture2D(videoProfile.Width, videoProfile.Height, textureFormat, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        texture.Apply();
        textureBinding.Invoke(texture);
        if (fetchFramesFromDevice)
        {
            if (realSenseDeviceOriginal.Instance.processMode == RealsenseDeviceOriginal.ProcessMode.UnityThread)
                realSenseDeviceOriginal.Instance.onNewSample += onNewSampleUnityThread;
            else
                realSenseDeviceOriginal.Instance.onNewSample += onNewSampleThreading;
        }
    }
    public void OnFrame(Frame f)
    {
        if (realSenseDeviceOriginal.Instance.processMode == RealsenseDeviceOriginal.ProcessMode.UnityThread)
        {
            onNewSampleUnityThread(f);
        }
        else
        {
            onNewSampleThreading(f);
        }
    }

    private void onNewSampleThreading(Frame frame)
    {
        if (frame.Profile.Stream != sourceStreamType)
            return;
        var vidFrame = ProcessFrame(frame) as VideoFrame;
        data = data ?? new byte[vidFrame.Stride * vidFrame.Height];
        vidFrame.CopyTo(data);
        f.Set();
    }

    private void onNewSampleUnityThread(Frame frame)
    {
        UnityEngine.Assertions.Assert.AreEqual(threadId, Thread.CurrentThread.ManagedThreadId);
        if (frame.Profile.Stream != sourceStreamType)
            return;

        var vidFrame = ProcessFrame(frame) as VideoFrame;
        texture.LoadRawTextureData(frame.Data, vidFrame.Stride * vidFrame.Height);
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        if (f.WaitOne(0))
        {
            texture.LoadRawTextureData(data);
            texture.Apply();
        }
    }
}
