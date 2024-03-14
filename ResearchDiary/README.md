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
