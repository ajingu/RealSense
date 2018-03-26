# Extension of RealSense Unity Wrapper
## ViveViewer
![viveviewer](https://user-images.githubusercontent.com/20081122/37893129-0bd83e4e-3115-11e8-8fcb-9f377b79edde.PNG)
Using HTC Vive, you can view pointcloud in free viewpoint.  
This demo requires [Steam VR Plugin](https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647) and the asset should be set like `Assets/SteamVR`

### Setup
Adding **\[SteamVR\] prefab** and **\[CameraRig\] prefab** to the scene, components should be set like below
![device1](https://user-images.githubusercontent.com/20081122/37893125-0946addc-3115-11e8-856a-54e8d49c2179.PNG)
![steamvr](https://user-images.githubusercontent.com/20081122/37893133-0d28affe-3115-11e8-9e81-06c30989853b.PNG)

## ArUco  
![aruco](https://user-images.githubusercontent.com/20081122/37756053-534381bc-2deb-11e8-88d8-45247ec93b5b.PNG)
Using ArUco marker(OpenCVForUnity), the position and the pose of the device are estimated.  
  
![mesh](https://user-images.githubusercontent.com/20081122/37756058-5a5fc2e4-2deb-11e8-936d-8d2eb42c8caa.PNG)
  
The mesh or the box is created by checking the bool value of **MeshDrawer** component.
![meshdrawer](https://user-images.githubusercontent.com/20081122/37756069-67db7634-2deb-11e8-823d-bd43807ec7f3.PNG)
  
ArUcoPointCloud object should be set like below.
![arucopointcloud](https://user-images.githubusercontent.com/20081122/37756061-63731a70-2deb-11e8-8481-be7f238016fa.PNG)

### caution
・[OpenCVForUnity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088) asset is required to try this demo. The asset should be set like `Assets/OpenCVForUnity`  
  
・**When some part of the marker can't be seen by the RealSense, this demo won't work well**. 

## PointCloud(RGBD)
![pointcloud](https://user-images.githubusercontent.com/20081122/36625678-89cf749a-1967-11e8-933a-bf39d626b4d5.PNG)

## Multiple connection
![multi](https://user-images.githubusercontent.com/20081122/36625690-ac519250-1967-11e8-8205-9482284d6106.PNG)

### Caution
Insert the serial number of your device into **RequestedSerialNumber** of **RealSenseDevice Prefab**.  
[Learn More](https://medium.com/@aratajingu/realsense%E3%81%AEunity%E3%83%A9%E3%83%83%E3%83%91%E3%83%BC%E3%82%92%E6%8B%A1%E5%BC%B5%E3%81%97%E3%81%A6%E8%A4%87%E6%95%B0%E6%8E%A5%E7%B6%9A%E3%81%A7%E3%81%8D%E3%82%8B%E3%82%88%E3%81%86%E3%81%AB%E3%81%97%E3%81%A6%E3%81%BF%E3%81%9F-e5e8ebf34b9f)

# Environment
Unity 2017.3.1f1(64-bit)  
Visual Studio 2017  
RealSense d415/d435  
