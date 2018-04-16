# About this project
This project is now handed over to [Inami Hiyama Laboratory](https://star.rcast.u-tokyo.ac.jp/) from [Ajingu](https://github.com/ajingu) since April 2018.

# How to try each demo
## MultiConnection
### Add RealSenseDeviceOriginal component
The component should be set like below.
![realsensedeviceoriginal](https://user-images.githubusercontent.com/20081122/38487682-f9bc2380-3c1b-11e8-9296-f63c702a24c1.PNG)

### Duplicate RealSenseDeviceOriginal Object and Images Object
In accordance with the situation, duplicate objects.  
![duplicate](https://user-images.githubusercontent.com/20081122/38487679-f810c9c8-3c1b-11e8-95a6-1a0a8c0466e6.PNG)

### Insert serial number of each device into Requested Series label
Serial number(12 digit) is wrote on the device.
![serialnumber](https://user-images.githubusercontent.com/20081122/38487218-6ab82dce-3c1a-11e8-9cd7-31d742cb5bba.png)

## ColorPointCloud
### Add PointCloudOriginal component
The component should be set like below.
![pointcloudmaterial](https://user-images.githubusercontent.com/20081122/38486891-52c02d94-3c19-11e8-828f-b57fa05e7f96.PNG)

## ArUco
### Install OpenCV for Unity Asset
Install [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088) asset and set it like `Assets/OpenCVforUnity`.

### Print ArUco marker
Print an ArUco marker(uploaded to `Assets/RealSenseSDK2.0/Resources`).  
This marker should be set on the **white paper**, or the marker won't be recognized correctly.  
![marker_id1](https://user-images.githubusercontent.com/20081122/38797611-00cb0640-419a-11e8-98b6-51db682930ff.png)

### Add ArUcoPointCloud component
The component should be set like below.  
**MarkerLength** is the length(meter) of one side of the ArUco marker printed.  
**Marker Id** is the ID of the ArUco marker.  

![arucopointcloud](https://user-images.githubusercontent.com/20081122/37756061-63731a70-2deb-11e8-8481-be7f238016fa.PNG)  
About the detail of the marker creation, please refer to [the official document](https://docs.opencv.org/3.1.0/d5/dae/tutorial_aruco_detection.html).


### Add MeshDrawer component
You can change Whether to draw mesh or box on the marker by toggling the check state.
![meshdrawer](https://user-images.githubusercontent.com/20081122/37756069-67db7634-2deb-11e8-823d-bd43807ec7f3.PNG)

## ViveViewer
### Install SteamVR Plugin Asset
Install [SteamVR Plugin](https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647) asset and set it like `Assets/SteamVRPlugin`.

### Add \[CameraRig\] component and \[SteamVR\] component
The component should be set like below.
![device1](https://user-images.githubusercontent.com/20081122/37893125-0946addc-3115-11e8-856a-54e8d49c2179.PNG)
![steamvr](https://user-images.githubusercontent.com/20081122/37893133-0d28affe-3115-11e8-9e81-06c30989853b.PNG)

### Add ViveInteractionManager component
The component should be set like below.
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
