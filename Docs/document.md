# About this project
This project is now handed over to [Inami Hiyama Laboratory](https://star.rcast.u-tokyo.ac.jp/) from [Ajingu](https://github.com/ajingu) since April 2018.

# How to try each demo
## MultiConnection
### Add RealSenseDeviceOriginal component
### Duplicate RealSenseDeviceOriginal Object and Images Object
### Insert serial number of each device into Requested Series label


## ColorPointCloud
### Add PointCloudOriginal component

## ArUco
### Install OpenCV for Unity Asset
### Add ArUcoPointCloud component
### Add DrawMesher component

## ViveViewer
### Install SteamVR Plugin Asset
### Add \[CameraRig\] component and \[SteamVR\] component
### Add ViveInteractionManager component

# Left to do

### Complete registration of different RealSense coordinates
Although I use ArUco marker of [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088) to adjust different RealSense coordinates, each coordinate always shifts over a little from other coordinates.

### Putting the multiple draw calls from each devices into one draw call
At this moment, every time one RealSense device is added, one drawcall to create point cloud is also added. Ideally, multiple point cloud data should be merged and only one drawcall should be sent to create merged point cloud.
  
### Exception handling when the ArUco marker can't be found
Now, homography transformation matrix can be measured using first five frames of scene as long as the ArUco marker is in sight of RealSense. It will be thoughtful to implement Exception handling for the case of not being able to find any markers.

# Reference
・[Intel RealSense official Unity wrapper](https://github.com/IntelRealSense/librealsense/tree/development/wrappers/unity)  
・[How to setup RealSense(d400 series)](https://software.intel.com/en-us/realsense/d400/get-started)  
・[RealSense Camera Configuration](https://github.com/IntelRealSense/librealsense/wiki/Projection-in-RealSense-SDK-2.0)  
・[How ArUco marker works](https://docs.opencv.org/3.1.0/d5/dae/tutorial_aruco_detection.html)  
・[OpenCV for Unity ArUco document](https://enoxsoftware.github.io/OpenCVForUnity/3.0.0/doc/html/class_open_c_v_for_unity_1_1_aruco.html)  
・[UnityEngine.XR module document](https://docs.unity3d.com/ScriptReference/XR.InputTracking.html)
