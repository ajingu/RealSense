# About this project
This project is now handed over to [Inami Hiyama Laboratory](https://star.rcast.u-tokyo.ac.jp/) from [Ajingu](https://github.com/ajingu) since April 2018.

# How to try each demo
## MultiConnection
### Add RealSenseDeviceOriginal component
![realsensedeviceoriginal](https://user-images.githubusercontent.com/20081122/38487682-f9bc2380-3c1b-11e8-9296-f63c702a24c1.PNG)

### Duplicate RealSenseDeviceOriginal Object and Images Object
![duplicate](https://user-images.githubusercontent.com/20081122/38487679-f810c9c8-3c1b-11e8-95a6-1a0a8c0466e6.PNG)

### Insert serial number of each device into Requested Series label
![serialnumber](https://user-images.githubusercontent.com/20081122/38487218-6ab82dce-3c1a-11e8-9cd7-31d742cb5bba.png)

## ColorPointCloud
### Add PointCloudOriginal component
![pointcloudmaterial](https://user-images.githubusercontent.com/20081122/38486891-52c02d94-3c19-11e8-828f-b57fa05e7f96.PNG)

## ArUco
### Install OpenCV for Unity Asset
[OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088)

### Add ArUcoPointCloud component
![arucopointcloud](https://user-images.githubusercontent.com/20081122/37756061-63731a70-2deb-11e8-8481-be7f238016fa.PNG)

### Add MeshDrawer component
![meshdrawer](https://user-images.githubusercontent.com/20081122/37756069-67db7634-2deb-11e8-823d-bd43807ec7f3.PNG)

## ViveViewer
### Install SteamVR Plugin Asset
[SteamVRPlugin](https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647)

### Add \[CameraRig\] component and \[SteamVR\] component
![device1](https://user-images.githubusercontent.com/20081122/37893125-0946addc-3115-11e8-856a-54e8d49c2179.PNG)

### Add ViveInteractionManager component
![viveinteractionmanager](https://user-images.githubusercontent.com/20081122/37894003-a2a8686a-3117-11e8-8359-0d72f85be6a7.PNG)

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
