using Intel.RealSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

using OpenCVForUnity;

namespace OpenCVForUnityExample
{
    public class ArucoPointCloud : MonoBehaviour
    {

        [Header("Device Settings")]
        [SerializeField]
        Stream[] sourceStreamTypes;
        [SerializeField]
        RealsenseDeviceOriginal realSenseDeviceOriginal;
        [SerializeField]
        bool fetchFramesFromDevice = true;

        int dataLength;

        //PointCloud data
        PointCloud pointCloud;
        Points.Vertex[] vertices;
        ComputeBuffer pointCloudBuffer;
        int pointStride;

        //Color data
        byte[] byteColorData;
        uint[] uintColorData;
        ComputeBuffer colorBuffer;
        int colorStride;

        //Frame
        FrameSet aligned;
        VideoFrame vidFrame;
        Frame depthFrame;

        [Header("Visualizer")]
        [SerializeField]
        Material pointCloudVisualizer;

        Align aligner;
        Texture2D imgTexture;


        [Header("Aruco Setting")]
        [SerializeField]
        bool applyAruco = true;
        [SerializeField] bool isStatic = false;
        [SerializeField] bool drawMesh = true;
        [SerializeField] int frameCount = 5;
        [SerializeField] int dictionaryId = Aruco.DICT_4X4_50;
        [SerializeField] float markerLength = 0.064f;
        bool matrixCofigured = false;
        Dictionary dictionary;
        Mat rgbMat;
        Mat ids;
        List<Mat> corners;
        List<Mat> rejected;
        Mat camMatrix;
        MatOfFloat distCoeffs;
        DetectorParameters detectorParams;
        Mat rvecs;
        Mat rvec;
        Mat tvecs;
        double[] tvec;
        Mat rotMat;
        Matrix4x4 invertYM;
        Matrix4x4 invertZM;
        Matrix4x4 transformationM;
        Matrix4x4 alignM;
        Range range;
        int width;
        int height;

        Matrix4x4 trs;

        [SerializeField] Camera viewCamera;
        [SerializeField] GameObject markerOrigin;
        [SerializeField] MeshDrawer meshDrawer;

        void Start()
        {
            if (realSenseDeviceOriginal.Instance.ActiveProfile.Streams == null)
                return;

            int numStream = realSenseDeviceOriginal.Instance.ActiveProfile.Streams.Where(p => sourceStreamTypes.Contains(p.Stream)).Count();
            if (numStream < sourceStreamTypes.Length)
                return;

            var videoProfile = realSenseDeviceOriginal.Instance.ActiveProfile.Streams.First(p => p.Stream == Stream.Color) as VideoStreamProfile;
            width = videoProfile.Width;
            height = videoProfile.Height;
            Debug.Log("width " + width + " height " + height + " fps " + videoProfile.Framerate + " format " + videoProfile.Format.ToString());

            imgTexture = new Texture2D(videoProfile.Width, videoProfile.Height, TextureFormat.RGB24, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            imgTexture.Apply();

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

            //PointCloud Setting
            pointCloud = new PointCloud();

            dataLength = width * height;

            pointStride = Marshal.SizeOf(typeof(Points.Vertex));
            colorStride = sizeof(uint) * 3;

            pointCloudBuffer = new ComputeBuffer(dataLength, pointStride);
            colorBuffer = new ComputeBuffer(dataLength, colorStride);

            aligner = new Align(Stream.Color);

            //Aruco Setting
            rgbMat = new Mat(height, width, CvType.CV_8UC3);


            Intrinsics intrinsics = videoProfile.GetIntrinsics();
            camMatrix = new Mat(3, 3, CvType.CV_64FC1);
            camMatrix.put(0, 0, intrinsics.fx);
            camMatrix.put(0, 1, 0);
            camMatrix.put(0, 2, intrinsics.ppx);
            camMatrix.put(1, 0, 0);
            camMatrix.put(1, 1, intrinsics.fy);
            camMatrix.put(1, 2, intrinsics.ppy);
            camMatrix.put(2, 0, 0);
            camMatrix.put(2, 1, 0);
            camMatrix.put(2, 2, 1.0f);
            Debug.Log("camMatrix " + camMatrix.dump());

            distCoeffs = new MatOfFloat(intrinsics.coeffs);
            Debug.Log("distCoeffs " + distCoeffs.dump());

            ids = new Mat();
            corners = new List<Mat>();
            rejected = new List<Mat>();
            rvecs = new Mat();
            tvecs = new Mat();


            detectorParams = DetectorParameters.create();
            #region ShowDetectorParams
            Debug.Log("adaptiveThreshConstant " + detectorParams.get_adaptiveThreshConstant() + "\n"
                      + "adaptiveThreshWinSizeMax " + detectorParams.get_adaptiveThreshWinSizeMax() + "\n"
                      + "adaptiveThreshWinSizeMin " + detectorParams.get_adaptiveThreshWinSizeMin() + "\n"
                      + "adaptiveThreshWinSizeStep " + detectorParams.get_adaptiveThreshWinSizeStep() + "\n"
                      + "minMarkerPerimeterRate " + detectorParams.get_minMarkerPerimeterRate() + "\n"
                      + "maxMarkerPerimeterRate " + detectorParams.get_maxMarkerPerimeterRate() + "\n"
                      + "polygonalApproxAccuracyRate " + detectorParams.get_polygonalApproxAccuracyRate() + "\n"
                      + "minCornerDistanceRate " + detectorParams.get_minCornerDistanceRate() + "\n"
                      + "minMarkerDistanceRate " + detectorParams.get_minMarkerDistanceRate() + "\n"
                      + "minDistanceToBorder " + detectorParams.get_minDistanceToBorder() + "\n"
                      + "markerBorderBits " + detectorParams.get_markerBorderBits() + "\n"
                      + "minOtsuStdDev " + detectorParams.get_minOtsuStdDev() + "\n"
                      + "perspectiveRemovePixelPerCell " + detectorParams.get_perspectiveRemovePixelPerCell() + "\n"
                      + "perspectiveRemoveIgnoredMarginPerCell " + detectorParams.get_perspectiveRemoveIgnoredMarginPerCell() + "\n"
                      + "maxErroneousBitsInBorderRate " + detectorParams.get_maxErroneousBitsInBorderRate() + "\n"
                      + "errorCorrectionRate " + detectorParams.get_errorCorrectionRate() + "\n");
            #endregion

            dictionary = Aruco.getPredefinedDictionary(dictionaryId);
            Debug.Log("markerSize: " + dictionary.get_markerSize() + " maxCorrectionbit: " + dictionary.get_maxCorrectionBits());

            rotMat = new Mat(3, 3, CvType.CV_64FC1);
            transformationM = new Matrix4x4();

            invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
            Debug.Log("invertYM: " + invertYM.ToString());

            Quaternion q = Quaternion.Euler(90f, 0f, 0f);
            alignM = Matrix4x4.TRS(new Vector3(0, 0, 0), q, new Vector3(1, 1, 1));
            Debug.Log("alignM: " + alignM.ToString());

            invertZM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            Debug.Log("invertZM: " + invertZM.ToString());

            range = new Range(0, 1);
        }



        void onNewSampleSetThreading(FrameSet frameSet)
        {

            using (aligned = aligner.Process(frameSet))
            {

                vidFrame = aligned.Where(x => x.Profile.Stream == Stream.Color).First() as VideoFrame;
                depthFrame = aligned.Where(x => x.Profile.Stream == Stream.Depth).First();


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



            //Aruco
            if (applyAruco)
            {
                //transformation Matrix is static
                if (isStatic)
                {
                    //using earlier frames, decide transformationM
                    if (!matrixCofigured)
                    {
                        while (frameCount > 0)
                        {
                            imgTexture.LoadRawTextureData(byteColorData);

                            Utils.texture2DToMat(imgTexture, rgbMat);

                            //Upside down
                            Core.flip(rgbMat, rgbMat, 0);
                            Aruco.detectMarkers(rgbMat, dictionary, corners, ids, detectorParams, rejected, camMatrix, distCoeffs);


                            if (ids.total() == 1)
                            {
                                Debug.Log("detected");

                                frameCount--;

                            }
                        }

                        Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

                        // position
                        tvec = tvecs.get(0, 0);

                        // rotation
                        rvec = new Mat(rvecs, range, range);
                        Calib3d.Rodrigues(rvec, rotMat);

                        //from marker coordinate(opencv) to camera coordinate(opencv)
                        transformationM.SetRow(0, new Vector4((float)rotMat.get(0, 0)[0], (float)rotMat.get(0, 1)[0], (float)rotMat.get(0, 2)[0], (float)tvec[0]));
                        transformationM.SetRow(1, new Vector4((float)rotMat.get(1, 0)[0], (float)rotMat.get(1, 1)[0], (float)rotMat.get(1, 2)[0], (float)tvec[1]));
                        transformationM.SetRow(2, new Vector4((float)rotMat.get(2, 0)[0], (float)rotMat.get(2, 1)[0], (float)rotMat.get(2, 2)[0], (float)tvec[2]));
                        transformationM.SetRow(3, new Vector4(0, 0, 0, 1));


                        // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                        // from camera coordinate(opencv) to camera coordinate(unity)
                        transformationM = invertYM * transformationM;

                        // Apply Z axis inverted matrix.
                        // from marker coordinate(Unity) to camera coordinate(Unity)
                        transformationM = transformationM * invertZM;

                        //from camera coordinate(unity) to marker coordinate(unity)
                        //rotate 90 degrees around the x-axis(alignM)
                        //from marker coordinate(unity) to world coordinate(unity)
                        transformationM = markerOrigin.transform.localToWorldMatrix * alignM * transformationM.inverse;

                        //transformationM: the matrix which transforms camera coordinate(unity) into world coordinate(unity)
                        //move RealSense model
                        ARUtils.SetTransformFromMatrix(viewCamera.transform, ref transformationM);

                        matrixCofigured = true;

                        Debug.Log("Transformation Matrix is configured");

                        if (drawMesh)
                            meshDrawer.draw(corners, vertices, ref transformationM, width, markerLength);

                    }

                }
                else
                {
                    imgTexture.LoadRawTextureData(byteColorData);

                    Utils.texture2DToMat(imgTexture, rgbMat);

                    Core.flip(rgbMat, rgbMat, 0);
                    Aruco.detectMarkers(rgbMat, dictionary, corners, ids, detectorParams, rejected, camMatrix, distCoeffs);


                    if (ids.total() > 0)
                    {
                        Debug.Log("detected: " + ids.total());

                        Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);

                        // position
                        tvec = tvecs.get(0, 0);

                        // rotation
                        rvec = new Mat(rvecs, range, range);
                        Calib3d.Rodrigues(rvec, rotMat);

                        transformationM.SetRow(0, new Vector4((float)rotMat.get(0, 0)[0], (float)rotMat.get(0, 1)[0], (float)rotMat.get(0, 2)[0], (float)tvec[0]));
                        transformationM.SetRow(1, new Vector4((float)rotMat.get(1, 0)[0], (float)rotMat.get(1, 1)[0], (float)rotMat.get(1, 2)[0], (float)tvec[1]));
                        transformationM.SetRow(2, new Vector4((float)rotMat.get(2, 0)[0], (float)rotMat.get(2, 1)[0], (float)rotMat.get(2, 2)[0], (float)tvec[2]));
                        transformationM.SetRow(3, new Vector4(0, 0, 0, 1));


                        transformationM = invertYM * transformationM;

                        transformationM = transformationM * invertZM;

                        transformationM = markerOrigin.transform.localToWorldMatrix * alignM * transformationM.inverse;

                        ARUtils.SetTransformFromMatrix(viewCamera.transform, ref transformationM);
                    }
                }

            }
            else
            {
                transformationM = Matrix4x4.TRS(viewCamera.transform.position, viewCamera.transform.rotation, viewCamera.transform.lossyScale);
            }

            pointCloudVisualizer.SetMatrix("_transformationM", transformationM);

            trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            pointCloudVisualizer.SetMatrix("_trs", trs);

            pointCloudVisualizer.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Points, dataLength);
            
        }


        void OnApplicationQuit()
        {
            if (pointCloudBuffer != null)
                pointCloudBuffer.Dispose();

            if (colorBuffer != null)
                colorBuffer.Dispose();
        }
    }
}

