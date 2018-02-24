using UnityEngine;
using Intel.RealSense;
using System.Collections.Generic;

public class AlignImagesOriginal : MonoBehaviour
{

    Align aligner;
    public RealsenseStreamTextureOriginal from;
    public RealsenseStreamTextureOriginal to;
    public RealsenseDeviceOriginal realSenseDeviceOriginal;

    // Use this for initialization
    void Start()
    {
        aligner = new Align(Stream.Color);
        realSenseDeviceOriginal.Instance.onNewSampleSet += OnFrameSet;
    }

    private void OnFrameSet(FrameSet frames)
    {
        using (var aligned = aligner.Process(frames))
        {
            from.OnFrame(aligned.DepthFrame);
            to.OnFrame(aligned.ColorFrame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO:
    }
}