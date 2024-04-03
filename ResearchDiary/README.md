# Notes for Thesis Work
These notes act as progress report (research diary) throughout the thesis work. The note taking starts when the accompanying course starts (Begleitseminar), and ends when the thesis has been completed successfully. The notes should be visible for the course lectureres and the supervisor of the thesis.

## 03.03.2024
- Setup this repository and setup the diary.
- Setup latex already, I am planning to write stuff on the side already (so I wont forget about it, also I will try to finish the writing about a topic with every milestone as well eG once the room-setup is done I want to write the according chapter(s) for it as well). We'll see how well it works ^^'
- Planned a timeline in preparation for the kickoff with markus on tuesday.
   ![](images/masterTimelinePlan.png)

## 05.03.2024
- Meeting with Markus
- Recieved Zed Mini Cameras
- talked through my current plan and got some intel
    - should try to use different usb buses for cameras
    - need the check about creativity rules and what it means for my time plan
    - should make the VR prototype the main user study
        - also make sure to have some iterations for that before the big user test as well
    - make MR user study with room setup smaller, maybe only test the best navigation method decided via VR prototype
    - about theory for writing:
        - maybe set a specific day a week for research
        - mainly look into the lindlbauer stuff and transitional interfaces
        - check out **embodied interaction and body gestures and emobidment**
        - create an excel list for sources

## 07.03.2024
- updated this diary
- setup a excel list for sources -> will most likely spend first research day transfer my sources into it
- (wont do much more, weekend is MMArmalade game jam and have yet to find into a master-thesis workflow. plannning to start next week with full force)
    

## 12.03.2024
- updated this diary
- today is research day:
    - sorted through my sources and put them into the excel
    - organised overleaf doc a bit -> listed things for releated works

## 13.03.2024
- some first tinkering with zed mini
    - installed sdk -> v4.0
    - my current cuda version is 12.3.107 (didnt realise this needs cuda btw...)
    - my laptop: 16gb ram, 12th Gen Intel(R) Core(TM) i7-12700H   2.70 GHz, NVIDIA GeForce RTX 3070 Ti Laptop GPU
    - went through: https://support.stereolabs.com/hc/en-us/articles/207616785-Getting-Started-with-your-ZED-camera -> also has a few samples!
        - sample stuff:
        - ZED explorer: double camera feed
        - ZED depth viewer: live demo of created depth map
        - ZED calibration: for recalibrating a zed
        - ZED sensor viewer: live reading of sensor data -> gyro, accelerometer, orientation, ...
        - ZED360 -> for multi camera data fusion -> room setup?!
    - checkout out demo projects from stereolabs repo: https://github.com/stereolabs
    - zed has a unity plugin: https://github.com/stereolabs/zed-unity

- i am not yet completely in a workflow: everything seems more interesting than doing this right now^^ I intend to use the whole day tomorrow to checkout the unity plugin! (i hope baby steps work, some day after i will check out the roomSetupCode in detail, then i will add the zed plugin etc.)

## 14.03.2024
- tried out the zed mini plugin in unity -> **v4.0.7**
    - some basic setup for zed in unity:
        - https://www.stereolabs.com/docs/unity/basic-concepts
        - https://www.stereolabs.com/docs/unity/creating-mixed-reality-app
    - had a compiler error "The type or namespace name 'Management' does not exist in the namespace 'UnityEngine.XR' (are you missing an assembly reference?)"
        - just need to install the XR plugin management
        - https://community.stereolabs.com/t/getting-started-with-unity-and-zed-problems/2781
    - apparently the zed unity plugin only supports up to 4 cameras in multi-cam mode right now! -> at least readme says so, but in editor i see camera ids up to 8
    - dont think i need it, but: the zeds object detection feature is only available on the zed2 and NOT the zed or zed mini!
    - basically the plugin has two "cameras" the zed_rig_mono (for third person stuff, when not using a HMD) and the zed_rig_stereo (for when using an HMD with AR pass-through, renders stuff in stereo, therefore needs more performance)
    - base script to change stuff with is the **ZEDManager**
    - the plugin can get input via three ways:
        - USB -> cam directly connected
        - SVO -> SVO file format, recorded input from a previous live ZED session
        - Stream -> from a ZED on a remote device that is actively streaming its camera input, needs IP and port to broadcast
    - they also have some tutorials
    - maybe I could use multiple cameras and just make point clouds? for a "live 3d model"
    - tried the multicam -> my laptop only has one usb plug, used a usb hub. no problem so far.
        - https://github.com/stereolabs/zed-multi-camera
        - USB bandwidth: The ZED in 1080p30 mode generates around 250MB/s of image data. USB 3.0 maximum bandwidth is around 400MB/s, so the number of cameras, resolutions and framerates you can use on a single machine will be limited by the USB 3.0 controller on the motherboard. When bandwidth limit is exceeded, corrupted frames (green or purple frames, tearing) can appear.
    - https://community.stereolabs.com/t/fused-point-clouds/2926 gold!
    - https://github.com/stereolabs/zed-sdk/tree/master/fusion


- guess next step will be to try out the multi cam tutorial, let them make a point cloud and interpolate a viewpoint? https://www.stereolabs.com/docs/tutorials/spatial-mapping
    - will most likely first look into fusing possibilites!

- other sidenote: will be on **vacation from 10.07. - 19.07** -> thats during the "implement in MR" period
- maybe dont even need the lindlbauer setup? zed sdk might be able to real time map by itself: https://www.youtube.com/watch?v=AFH2yN3rM78

## 20.03.2024
- Begleitseminar mit Markus:
    - my previous stuff durchgehen
    - look into gaussian splattering -> maybe ust needs a point cloud and does the magic
        - maybe not realtime tho
    - maybe i wont need meshes if point cloud is enough
        - vergleich mit lindlbauer und deren meshes!
        - zed sdk meshes are not that smooth, so maybe dont use them if not needed
            - lindlbauer makes some meshes? how?
        - neural depth mode for zed?
    
## 28.03.2024
- actually working on the project again after a small hiatus (motivation is still low, hope it gets better once I can do stuff with the HMD...)
- looked into the lindlbauer setup
    - https://github.com/microsoft/RoomAliveToolkit
    - consists of two part-projects:
        - ProCamCalibration
            - https://github.com/microsoft/RoomAliveToolkit/tree/master/ProCamCalibration
            - seems to need lots of callibration for the projection stuff (which i dont need!)
                - the callibration setp uses the projector to project gray code patterns and uses the (in this case) kinect cameras to track them. The gray code patterns are used to map the kinects color images pixel to the detph images coordinates. 
                - with the gained info on the 3d depth points and their coresponding 2d projector coordinates the needed callibration parameters can be calculated

            - all in all this does not seem to usefull to me. lots of stuff on projection mapping and solving parameters to make it possible, I dont need that!
        - RoomAliveToolkitForUnity
            - https://github.com/microsoft/RoomAliveToolkit/tree/master/RoomAliveToolkitForUnity
            - this uses the proCamCalibration (needs the callibration with projector...)

    - the roomalive toolkit focuses a lot on projection mapping (projecting imagery to non-planar object eG 3d boxes/non flat objects) which i do not care about.
- my current opinion: fuck lindlbauer and its setup. Just use the zed sdk and make a solution myself. it should be able to fuse multiple cam viewpoints and create point clouds. I will disregard colliders right now, they are not needed.
    - also: gives option to try out gaussian splat!
        - https://github.com/aras-p/UnityGaussianSplatting there is a unity plugin for it :3
        - for quick and easy 3d captures: https://poly.cam/ (tho low quality!)
        - if i want to train gaussian splats myself: https://www.reshot.ai/3d-gaussian-splatting

- compare: https://www.youtube.com/@LeeVermeulen he also got magic portals! uses some kind of room setup to interpolate viewpoints!
    - look through his blog posts: https://www.3delement.com/?p=850
    - Lee Vermeulen is a magician!
    - looks like zed mini can do some fancy stuff! https://twitter.com/Alientrap/status/1671578720249716736
    - he had a talk in vienna a while ago! https://www.youtube.com/watch?v=v2402mxJpHQ


- **future todos aka tomorrow at 17h:** tinker about with the zed sdk and the zed unity plugin to make mutli cam fusing work!

## 30.03.2024
- soooo, didnt do the stuff yesterday. But today is the day!
- tinkering in with the zed plugin to make multi cam fusing work
    - little side note: even tho the zed mini cables are type-c they have an up and down side. make sure to plug them in correctly! (the arrows >> need to be up...)
    - one can use the zed360 tool to setup the cameras in a room, then move around in the room -> cameras will track the poses and callibrate that way
        - this exports a **JSON calibration file**
            - maybe i could plug this into the lindlbauer setup?
                - but i dont want to use it!
        - https://www.stereolabs.com/docs/fusion/zed360

    - as the lindlbauer roomalivetoolkit for unity would need a callibration(can be done with the zed360) and a obj file (can be done with zed as well? https://www.stereolabs.com/docs/tutorials/spatial-mapping)

    - possible ways for point cloud fusion could be Iterative Closest Point (ICP) algorithm
    https://community.stereolabs.com/t/fuse-point-cloud-from-multiple-cameras/3202

    - https://community.stereolabs.com/t/multi-camera-point-cloud-fusion-using-room-calibration-file/3640
    another post on point cloud fusion with multi cam
        - https://github.com/stereolabs/zed-sdk/tree/master/spatial%20mapping/multi%20camera/cpp
            - this is not fusing of point clouds but the spatial mapping!

- there also exsits Zedfu (https://support.stereolabs.com/hc/en-us/articles/213074149-Spatial-Mapping-Best-Practices) sample thingy for spatial mapping with single cam

- since point cloud fusion is not yet in the sdk (and it seems to be super hard to implement oneself), i can go with the fake approach of just overlapping some point clouds using the config from the zed360 calibration.json for the data to align them
    - https://www.youtube.com/watch?v=75plbxS9GV0 at least point clouds should be able to be realtime plausible
    - **trying to get a point cloud into unity!**
        - tomorrow it is (or later...)
        - vergleich zed DetphViewer
        - unity can create meshes and textures from point clouds?
            - https://docs.unity3d.com/Packages/com.unity.pixyz.plugin4unity@2.0/manual/import-point-clouds.html
            - pixyz plugin
        - used to be an example, but was removed now from zed plugin
            - https://github.com/stereolabs/zed-unity/issues/51
            - use the ZedPointCloudManager

- two approaches are possible now: 
    - try to overlap zed point clouds in unity or
    - try to use the zed360 callibration and obj-file from the zed spatial mapping in the roomToolkit
        could be troublesome, not sure if it will work, will need to update the toolkit code to use zed sdk

## 03.04.2024
- took a bit longer to continue...
- trying the overlapping zed point clouds in unity
    - multiple cameras have been calibrated via zed360
        - exports a calibration file with position, rotation etc of the cameras.
    - each camera in unity, displaying a pointcloud
    - cameras aligned as described in the calibration of zedFu
- the zed sdk seems to have a FusedZedPointCloudManager Script, didnt work for me tho :shrug: (also didnt find any documentation about this...)





<!--
The notes have the following minimal requirements:
- must be continously updated (at least once a week, even though there is nothing to report)
- must contain notes of discussions in the course units that accompany the thesis (Begleitseminare) 
- must contain the notes of meetings with supervisors 
- must be uploaded to the gitlab repository of the thesis
- are written in a single file starting with the newest date
- are written in markdown language

Complete notes are required to pass the accompanying courses for thesis work in Bachelor and Master. If the requirements stated above are not fulfilled, the course fails. If no notes have been taken, a compensation work will be defined that includes a written report representing the time frame of the thesis work, but can also include additional work. 


## 20.3.2024
- breakthrough with miniaturization

   ![](images/image3.jpg){width=600px}


## 13.3.2024
- set up prototype
    - experimented with hardware XXX
    - compiled libraries [LibraryImportantForProject](https://-(fakeurl)-linktolibrary.com/download)
- snapshot of currently set up system/software/...

    ![](images/image1.jpg){width=600px}

## 11.3.2024
- literature review
    - found more work on topic XXX, but still more research required
    - found an important paper: Apple, C. T. (1965, August). Evaluation and performance of computers: the program monitor—a device for program performance measurement. In Proceedings of the 1965 20th national conference (pp. 66-75).
    - read a relevant blog article on [microcontrollers](https://-(fakeurl)-importantblog.org/mc-controller)


## 4.3.2024
- no work on the thesis this week

## 27.2.2024 
- Meeting with supervisor
- Discussed first steps of thesis
    - continue literature review in area XXX
    - set up prototype using XXX harwarde/software
 -->
