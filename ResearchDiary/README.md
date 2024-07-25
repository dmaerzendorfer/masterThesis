# Notes for Thesis Work
These notes act as progress report (research diary) throughout the thesis work. The note taking starts when the accompanying course starts (Begleitseminar), and ends when the thesis has been completed successfully. The notes should be visible for the course lectureres and the supervisor of the thesis.

## 25.07.2024
- started at 11.30h ...
- testing the drone cams and fixing any bugs
    - forgot to make the meshes read/write (the outline needs that)
    - ![cam in face](images/camInFace.png)
        - spawning the cam at the left controller leads to the cam being stuck in your face -> instead do the same as with the viewpanel  -> spawn it a few feet in front (but of left hand) -> didnt work as well (maybe i overlooked something) -> instead every drone cam just finds the locomotion system in the scene and enables/disables the gameObject if need be

    - found the problem of walking and teleporting input still being enabled while controlling cam.
        - solution: disable/enable locomotion action map while a drone is active -> nvm cant really get a reference to an action map just single actions -> so i just disable the walk and teleport actions
    - also found the problem how would one unselect the drone if not visible?
        - solution: add an extra controller button to it. (left secondary)
    - they work now!!! \o/
- made some additions
    - all viewpanels now have proper names
    - viewpanels now have a button to select their cam (at least the drone one, oce does not support that)
- quick cleanup of code

- todos next time: 
    - trying to make avatar a bit better (fix hand positions, and make it so you can see yourself)
    - check if i can refactor code structure (viewManager is quite big...)
    - quick look into wim
    - define study tasks
    - setup a control table where users can set drone mode, see how many drones etc. -> also needs the controls somewhere
    - setup scene for task
    - setup documenting task measurements and save it to a file


- need to check if building works at some point as well!

## 24.07.2024
- break today. had housework todo in the morning and then a quick meeting at 13h
- and got stuff planned in the evening, so not worth it to spend 2h where i cant concentrate^^'
- but tomorrow, will start with full force!


## 23.07.2024
- did not start at 9h but 11h instead...
- finished up the drone part
- i did set the limit of 5 views for the orbit cams due to the virtual cam restriction of needing their own layers.
    - does not apply for drone views since they dont need virtual cams
    - but still left in the 5 max for consistency!
- drones could work in theory now, will test in detail and fix any bugs tomorrow (lest see if i manage 9h this time...)
- todos tomorrow:
    - test drones and fix any issues
    - look into WIM real quick to see how much work it would be
    - cleanup code
    - think about how the study could look like and prep a scene for the study and pilot test with ppl
    - if there is time -> cleanup the avatar a bit

## 22.07.2024
- I am back from vacation
- need to organize myself again on where to continue
    - work on the free drone cam view
    - todos: 
        - spawning of a drone-cam
        - make drone controllable + selectable
            - define what the controls are
        - make view changeable for drone flight
            - screw that for now, i dont have much time. just use the ground view.
    - then need to look into wim for the orbit cams
    - then need to review everything again
        - and eg fix up the avatar a bit
    - then need to design study
    - then need to make system for documenting study 
    - then need to show ppl for pilot test
    - then need to run study with ppl
    - then need to analyse result of study
    - then need to write everything down!
- work done:
    - organized myself
    - made a "drone" model -> just the normal cam model with two makeshift thrusters smacked onto it
    - added outline package for showing when a drone is selected
- plan for tomorrow:
    - start at 9h!
    - implement the drone with controls and spawning and selectable to move them again
        - 'X' button to spawn (just at location of left controller)
        - left joystick for moving (from player orientation so no matter where drone faces)
        - right joystick to yaw (x-axis) and pitch (y-axis) of drone
        - 'B' button for up
        - 'A' button for down

        - for selection -> just raycast and grab -> make own interactable just like with oce one -> make it somehow visible when selected -> eG green border around it

## 08.07.2024
- time to work again!
- fyi: bin von 10.-19. auf urlaub
- tested the oce cam spawning
    - now using controller buttons instead of handles
    - also showing the orbits path now
- fixed the anoying problem of my project always using steam VR in playmode
    - just hat do change stuff in the openXR runtime settings to use the correct runtime...
- spawning of oce cam works now wiht controller
- decided to give WIM and viewpoint transition to 1st-person lower prio. not that important right now, better have it work at least at all.
- made the spawned cameras selectable so they can also be moved via the controller buttons
- first version of oce spawning is done then.

## 04.07.2024
- levelup is over and i have time for this again!
- todos:
    - fix oce cam handle
    - make variations on where the view-panel is (eG in worldspace or in left hand)
    - create free-cams (drones) version
        - and a way to swap the control views (first person, third person, ground)
    - create wim for changing oce cams
    - cleanup my messy code
- did some fixing on the oce-handle
    - the plane seems correct now but projecting onto it and reducint to 2d coordinates spells trouble
    - spent to much time on it, not worth it. scraping it.
    - now just using controller buttons
        - joystick left/right to spin it
        - joystick up/down to move up/down
        - A button to get closer
        - B button to get further away
    - have yet to test, my quest ran out of power since the cable to the pc doesnt seem to charge very well...
- since i pretty much used all the buttons on the controllers already will put the transition into the viewpoint into the view panel as a button.


## 26.06.2024
- besprechung mit markus
- vl mal mrtk ansehen -> für window management
- wenns mit mr wäre -> müsste man mit gesten machen -> wäre drone vermutlich schwierig (bräuchte WIM?) in arbeit reinschreiben.
- einzel obj vs exhibit + drone vs obj orientated
- aufbereiten warum mehrere views sinn machen -> zb bei museum exhibits -> weil tour mehrere pois am obj zeigen will für seine gäste
- vl in task was einbauen damit leute über die pois reflektieren müssen.
    - bzw leute dazu bringen damits nochmal in die viewpoints reinmüssen und damit fenster interaction sinn macht
- pilot study schonmal user study aufsetzen -> leuten geben und testen lassen -> dann anpassen.


## 25.06.2024
- added some logic on repositioning the viewpanel of a spawned view-pair
    - currently just casts a ray forward from the mainCamera for a certain distance and puts the view-panel wherever it hits or at the end of the ray
    - needs tweaking and leads to a real weird error where my view is off the map when i spawn a second view-pair
        - fixed it, problem was orbit viewpair 2-5 had an extra orbitCam1 in their prefab... 
- tested and fixed a small error with destroying view-pairs
    - still got a problem when deleting a view-pair that is currently in the hud and not in worldspace
        - fixed it

## 23.06.2024
- back to work :)
- started making OCE viewpoint spawning possible
    - certain objects are selectable
    - can spawn an oce cam already
        - wanted to make a fancy oce Handle that is used to set the height and radius of the cam.
            - doesnt work yet, got some weird stuff with plane projection going on...
        - also need to adjust the view panels position as well.
        - more stuff for next time, i am mush already...
- for the deletion of a view i decided to just put a button into the viewpanel :)
    - added one, needs testing tho
- added a circle to visualise the orbit :)


## 20.06.2024
- looked into the spawning of viewpoints in a OCE way.
- want to use cinemachine for its orbital camera script
- seems like each virtual cam needs to be on its own layer if i want them to not merge togehter
    - will limit the amount of virtual cams to 5
- made logic from the orbit cameras and made 5 prefabs for them (with the different layers)
    - might have worked with one prefab that just changes the layers etc but i was lazy
    - also might save on some logic for spawning them since i wont have to check for layers up etc. just will have a list of 5 prefabs and a bool if they have been spawned already
- work left to do: make objects selectable with controller and spawn such a new prefab. set its target. allow control of it. work out how/where to put its view panel
    - for the view panel: possibly just make a raycast 45° down from the HMD. where ever it hits move the view panel to.
    - also figure out some way to display the orbit cricle whilst placing it and make the camera ghostly :)


## 19.06.2024
- originally planned to tinker around a bit in unity
    - but only have 4ish hours right now
    - so will do some research and write stuff in overleaf

## 17.06.2024
- wrote down my plan a bit more consise already as intro in overleaf
- sent to markus to update him on my plan and also get some feedback :)

## 12.06.2024 / 13.06.2024
- did some research and planning after meeting with markus
- the **microsoft rocket lib for rigged avatars**
    - looks like its really just for rigged avatars
    - will test out later but will create a generic male and female avatar
        - will take the models from there :)
        - they also have a paper i can cite!
        - will clean up the avatars IK a bit and make it so one sees oneself (will just get rid of the controllers, just leave in the ray?) -> maybe some more research on how to best achieve this.
- i need to **narrow** down what it want to look at (in relation to museums), so here we go:
    - I will look at a VR prototype of a how viewpoint **manipulation and spawning of viewpoints in MR** could work **in a museum setting with restricted physical movement**
        - the user is physically at a museum but wants greater freedom on looking at a exhibit from any angle
        - see as reference: the lindlbauer remixed reality for a setup that would allow viewpoints in a MR setting
        - so camera views are not restricted to specific locations and can be freely placed.
            - will look at **HOW to place and manipulate these views** however
    - I **will not look into detail on transitions between views**
    - focus on museum exhibits
        - see https://www.icom-italia.org/wp-content/uploads/2018/02/ICOMItalia.KeyconceptsofMuseology.Pubblicazioni.2010.pdf for exhibit and museum object definition
            - "... These two levels – presentation and
exhibition – explain the difference
between exhibition design and exhibit display. In the fi rst case the designer starts with the space and uses
the exhibits to furnish the space,
while in the second he starts with
the exhibits and strives to fi nd the
best way to express them, the best
language to make the exhibits speak. ... "
        - often museums are either designen around a space and filled with objects or otherwise an object is in the center and it is tried to show it of the best way possible
        - this thesis tries to focus on the second here. a single object in the center (or a room with multiple objects) with the caviate of given the visitor the possibility to look at the object from any object he/she wants.
    - tomorrow morning will define in detail on context eg hard focus on exhibits or more free viewpoint choice and then define three ways of spawning views, and manipulating them (not focus on transitions between them and not focus on selection of view, just use normal ray cast for that or via hud panel)
    - tomorrow now:
        - i think i will look at two ways of doing it and pitting it against each other:
            - 1. exhibit focused
                - the views are focused on the exhibit (orbit cam)
                    -  object-centric exploration (OCE)
                    - spawning -> first select the exhibit with raycast. via button press then show ghostly image of view that focuses on the exhibit. via joystick control the camera can be circled around the object and moved whilst placing
                    - for manipulation -> kind of a voodoo doll and world in miniature -> have wim of exhibit in palm and use other hand to move view-camera
            - 2. free placement, drone approach
                - uses a "drone" metapher to seed the correct way of thinking on how the system works to the user
                    - not related to real drones!
                    - see don normann on mental models!
                - users can place view-cams wherever they want
                - for spawning: spawn a view-point in left hand
                    - the viewpoint can be selected and then moved either in first-person, third-person or from normal ground view (as if controlling a real drone from the ground)
        - users can place as many views as they want
        - in addition ppl can just use raycast to select a cam and transit to its view.
            - once done they can move and rotate all right, but their change wont effect the view-cams position/rotation after they leave
        - for deletion same goes, just raycast and stuff
        
        - the research questions also change a bit:
            - in a museum setting: what method is more effective in placing and manipulating viewpoints. an OCE based approach or a more free approach?
            - in general and not related to the two examined methods: what effect do multiple viewpoints have on the user? do they lead to greater simulator sickness do they enhance the spatial awareness and orientation?

            - i would scrapt the vr perspective continuum questions for now. maybe just talk about it in the paper as a minor detail somewhere. will think about it again once its time for the user study.
                - does the VR perspective continuum still exist here as well? do users feel like spectators even if they only manipulate views. And what system minimizes this feeling?
        - todo: for sunday -> write this down more concise and send to markus for review.

## 12.06.2024
- meeting with markus! :)
    - gave update on what i have done so far and what i plan to do
    - avatar -> rocket lib
        - https://www.microsoft.com/en-us/research/blog/microsoft-rocketbox-avatar-library-now-available-for-research-and-academic-use/
        - fix the avatar, ppl should be able to see themselves if looking down!
            - maybe male/female version
            - look into embodiment papers!
    - possiblites für simulation un transition -> in relation to the museum usecase
        - for spawning vl focus on the exhibit -> ray-cast then circle on it
    -  bevor is selection etc mal genauer definieren was ich mir in museums ansehen will
        - mal nen ramen finden
            - dann genau definieren was möglichkeiten für spawning, selection, manipulation and transition
        - erst mal ansehen wasses für exhibits im museum gibt
        - einfach einen exhibit typ wählen -> dann eG 3 types of spawning/manipulation/transition -> give ppl to test -> then design main study -> clarify questions -> design task etc.
        - also think about how to delete views
    - or not focus on exhibits -> allow free movement!
        - could be more interesting
            - think about it
            - see what ppl like in pilot study
- other exhibit focus thingy -> voodoo doll version
    - WIM of exhibit in hand

- todos: check papers sent by markus
    - review stuff written here, eg research on museum exhibits
    - and what type of exhibit to focus on
    - dont focus on transitions -> focus on spawning and manipulation


## 11.06.2024
- lets continue!
- making the viewpoint camera + window work!
![viewCamAndViewPanel](images/ViewCam.png)
    - the camera renders its view into a renderTexture that is displayed on its linked viewPanel
    - the view panel can be docked to the hud
    - they are connected via a line-renderer (is only shown if not docked or hovered over the panel when docked)
    - i also allowed the cam to be moved by grabbing it. this is only a quick and dirty test tho. didnt put much effort in.
- quick note: noticed my IK avatar is even more missaligned as i thought. but ignoring for now :)
- todos for tomorrow: reviewing what to do next (most likely will look at ways to spawn the cam first) aka look at the list further down below 

## 07.06.2024
- gave the objects in the demo museum scene interactables
- updated the restriction of walking/teleporting etc
- next up: either on weekend or monday:
    - make camera actually show its viewpoint
    - then work on spawning and destroying viewpoints
    - then on manipulation of viewpoints :)

## 06.06.2024
- sadly couldnt work on stuff the last two days...
- finished up the avatar
    - fixed the hands and removed the controller models
    - tested on HMD
    - the hand tracking is not perfect at all
        - i decided do just wing it so its roughly correct (also had trouble disabling the controllers of the xr interaction toolkit. i actually like them and dont know how to combine them with the hands)
            - https://www.youtube.com/watch?v=f_jHGNxwN2g
            - this looks more accurate but meh, not worth it
        - i will just disable the avatar from the main-camera view. it should only be visible via mirrors or from external viewpoints. for that the accuracy should be enough :)
            - just adjusted the culling mask of the main cam to not include the avatar layer :)
- i also saw there is a newer version of the xr interaction toolkit with some hand tracking samples
    - but updating my project broke stuff, not spending time on it to fix it. imma just stay on 2.5.0 and not go to 2.5.4

- made simple demo museum scene with synty poly asset pack i have
    - ![demo museum](images/demoMuseum.png)
    - just first draft. planning to add some random interactables in there as well to make it interesting
        - those i still need to setup the interactables
    - for now made a small, medium and large exhibit placeholder
        - also hid a little extra detail to each exhibit because i wanted to and to give ppl something to find when they look at stuff from different angles
    - also made little table for future use
        - want to put the config of the test env there, eg where users can configure how they can place cams etc.
- a friend told be there are studies in psychology focusing on museeum exhibits and how ppl view it and how to best display them -> maybe interesting to look into as well :)


- next up (hopefully tomorrow)
    - make sure the walking and stuff works here. want to restrict teleporting somewhat
    - give stuff interactables that need it
    - i found a camera model i want to use for my viewpoint indicators -> hook it up to actually work in combination with the hud window
        - the window should just display the camera stream
        - the window should point to the camera with a line renderer (aka that strange xr interaction toolkit line stuff)


## 03.06.2024
- lets get stuff done!
- planning stuff again:
    - deadline erstabgabe **4.9.24**
    - i am scrapping the MR stuff and updating the research questions a bit
    - new focus: **viewpoint manipulation in VR in a Museum setting**
    - will look at ways of placing viewpoints, transitioning into them and manipulating if the viewpoints are located somewhere where user is not physically able to move
        - note that the idea is still to also have this in MR but will only focus on a VR setting
        - first ideas for new research questions are:
            - what are effective systems for remote viewpoint manipulation in a museeum setting with limited physical moveability.
            - what effect do multiple viewpoints have on the user? do they lead to greater simulator sickness do they enhance the spatial awareness and orientation?
            - does the VR perspective continuum still exist here as well? do users feel like spectators even if they only manipulate views. And what system minimizes this feeling?

    - for this i will create a little playground with interaction possibilites and in a museeum environment.
        - this playground i will give to ppl, gather some feedback and then create a userstudy once i have more understanding on how ppl use the toys i give them
        - things to implement: 
            - a system to enable/disable the different ways of interaction
            - a virtual avatar
            - virtual viewpoints/cameras connected to a window that can be docked to the hud
            - ways of spawning the viewpoint:
                - mask gesture
                - spawn a virtual drone and controlling it remotely
                - spawn virtual drone and take flight yourself (first person and third person)
                - maybe some way of instantly spawning with a world in minature
            - ways of transitioning to viewpoints: (maybe??)
                - instant teleport
                - blink animation
                - etc. need to research and decide if i want to do stuff like that
            - also look into selection of the viewpoints?
                - research!
            - ways of manipulating the viewpoints:
                - moving while taking on the view (physical or joystick+head movement) (3rd person and first person?)
                - world in miniature
                - selecting and moving it from actual position (like steering a drone from the ground, but you have its pov in the hud as well)

    - goal: end of month have all this done, so i can test with ppl!

- stuff i did today: 
    - wrote down my plan. 
    - implemented a virtual avatar
        - https://www.youtube.com/watch?v=v47lmqfrQ9s
        - the avatar uses a humanoid model with inverse kinematics for its hands/arms, legs and head
            - using the unity animation rigging package
            - using this for the ik feets walking
                - https://www.youtube.com/watch?v=acMK93A-FSY

            - did not finish with the tutorial completely, need to fine tune and adjust the head position and hand positions
                - couldnt do that right now since my HMD is out of power ^^'
                - will finish tomorrow morning and also build a quick placeholder test scene of a museum
            
    - did some asset search for a museum
        - didnt find a free one (at least not in the unity asset store)
        - there are some random free 3d models of museums etc. but didnt spend time on checking them in detail
        - for now will most likely just use a synty polygon asset to build a placeholder for now.
            - might update later.
            - i have this: https://assetstore.unity.com/packages/3d/environments/polygon-sampler-pack-207048
            
                



## 15.05.2024
- actually continuing to work on it for once ^^'
- windows can now be selected and docked to hud
    - change color to indicate if in hud or in world
    - have small pop animation
    - would like to add something so the hud windows are always same distance from mainCamera. need to rescale them however so their size looks ok when the swap from world to hud
        - tried but failed to do so for now
    - want to make windows in the hud scalable
        - cant really find how to do that
        - no idea how to read the interactors primary2dAxis while held
- todos: next time, look into the scaling real quick. if it doenst work ignore and focus on avatar 

## 05.05.2024
- working on the vr prototype
    - once again quest link doesnt want to work...
        - fuck that shit, stuff is plugged in but not recognised
        - plugging in and out a few times and moving the cable arround worked?
    - looking into avatars with the xr interactive toolkit
        - found this https://www.youtube.com/watch?v=tfpIXlGor2E
            - apparently with "ready player me" you can generate avatars online and import them to unity
                - maybe testers can create their own avatars later?
        - you can also rig full body characters like this https://www.youtube.com/watch?v=Iya-gKJ0V74
            - i want to go with full body ones if possible. only upper body and hands might be off-putting
        - or https://www.youtube.com/watch?v=v47lmqfrQ9s
            - this looks most promising (combine it with a ready player me model)
            - IK walking might be weird (anmiations are smoother...) but its good enough :)
        - other option: meta movement sdk
            - uses the quests body tracking 
            - was not able to find out if it works on quest 2 (but didnt search for long)
            - didnt bother too much with it, seems overkill and too much effort to use

    - looking into interactive windows/views in HUD
        - read up on what the xr interactive toolkit actually can do
        - for my HUD stuff **spatial UI** seems to be the most useful
        - i can make the windows just children of the main cam and they follow (could be smoother but good enough for now)
            - FOV quite small so they take in lots of space
            - plan: 
                - when creating a cam view -> creates a spatial UI near it
                - user can grab the UI, via button press it can be docked to the HUD -> once in the hud user can grab it and move it about. via the joystick forward/backward can resize it. Usually this would be for moving it further away and closer when in world space
                - want to visualise via icon or smth if window is in hub or world. also add small animation (tween?) for when it swaps between them.
                - add these tutorial indicators as well to tell users what buttons do :)

    - script for objcets that tries to stay in view
        - https://www.youtube.com/watch?v=ler5ffTJnrk

    - not important right now but maybe useful for WIM later down the line
        - https://www.youtube.com/watch?v=6PSLfRsN89g

    - if i want to use gestures for eg mask taking on/off stuff -> MiVRy could be an option?
        - https://assetstore.unity.com/packages/add-ons/mivry-3d-gesture-recognition-143176

    - for now: little break then i shall look into more research (not feeling the implementing right now, need to learn first!)

    - todos: tomorrow -> create a system for picking up spatial UIs and putting them in HUD. Implement the resizing controls. add indicators if UI is in world or in hud. add small transition animation to window (tween?). add control indicators (xr seems to have a system for that!)
        - once this works i can look into spawning the cameras (just make the grabbable for now)
            - make it sow that windows show where their camera is when selected
        - then add avatar support
        - then add the different ways of interacting/manipulating the view (most likely hud to mask transition will need some kind of transition! read about it!) 




## 29.04.2024
- trying out the initial setup of the quest 2
    - created unity project(2022.3.9f1) with VR-template
        - featuring XR interaction toolkit
        - https://www.youtube.com/watch?v=tGZgJ5XtOXo
        - added my default file structure
        - changed the systems i want to deploy on
            ![settings info](images/questSettings.png)
            -> set everything to oculus
            - dont! setting thigs to open XR fixed it and it just magically works
    - had the oculus pc app already installed (quest link)
        - my quest doesnt have a cable
            - trying the air link
                - had trouble with it, doesnt want to connect
                - had it running for a bit but when i start quest link in the HMD it says cant connect or smth
                - maybe wifi settings fault? at first my pc was not discoverable in network. also tried with hotspot form the pc and connect quest to that so no funny wifi settings
                - it does work if i make a hotspot on the pc and connect the quest but then i have the problem of the pc not being connected via ethernet so it just says missing requirements and doesnt connect...
                - also the right controllers battery is empty...
                - have tried it with my phones hotspot but then the quest link is really laggy
            - will try the setup in the fh, maybe ist just a wifi problem?
            - will ask david for a cable...
            - and will need to buy some batteries...
    - got a cable from david. also bought some batteries (AA ones)
        - not sure if important but cable straight bit is in HMD oculus logo facing up. bent part is in the usb adapter.
    - https://www.youtube.com/watch?v=zbqHNwDpi6Y
        - had the use open XR in the project settings for it to work...
    - last time i did vr stuff we used steam vr (now i always get a popup from steam for steam VR)
        - dont want to use it right now tho.
        - ignore it think its just steam trying to do convenient shit
    

## 24.04.2024
- got a Quest 2 from david today
- noticed that the order of these diary entries are messed up. most recent should be on top...


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
- not done yet, for now i only implemented the spawning of objects according to the calibration json


## 04.04.2024
- continued with the zed pointcloud overlapping
    - it now instantiates zed mono rigs with point cloud managers attached to them
        - found some issues where zed prefabs and gameObjects can no longer be dragged and dropped in unity... (current consistent fix is to restart unity, then it works again)
        - had some trouble with instantiating the cameras during runtime. at first i instantiated them and then set the camera id. but zed doesnt like it(during awake it still thinks its cam 01?). solution was to set the id in the prefab and instantiate the correct prefab.
        - for now only works for two cameras
    - need to test it with a better setup soon (camera positions that actually make sense. ), then i can evaluate if its good enough or if in need to use the lindlbauer thingy (we'll see when that will be, hope i will have time/motivation on saturday or sunday...)

## 10.04.2024
- testing the overlapping point clouds with some better positions
    - did the calib first
<video controls src="images/zed360_calibration.mp4" title="Title"></video>
    - then tested in unity with overlapping point clouds

        - my setup btw:
            - ![setup1](images/setup1.jpg)
            - ![setup2](images/setup2.jpg)
            - ![setup3](images/setup3.jpg)
            - ![setup4](images/setup4.jpg)


        - from its normal viewing point its nicely visible
        - ![in unity with one cam](images/oneCamInUnity.png)
        - however from the oposite side stuff is inverted
        - ![from other side](images/fromOtherSide.png)

        - so if we have two cameras facing each other we get a mess (at least so i thought! my setup was incorrect!)

        - seems like my transform and rotation setting was wrong?
            - some hints about the config file https://community.stereolabs.com/t/multi-camera-point-cloud-fusion-using-room-calibration-file/3640
            - rotation seems to be fixed with the code from the post
            - but cameras seem to reset their position when they connect?
                - changed the instantiating so that the transform is set once the cameras are ready (got a nifty onZedReady event :3 )

            - also calibration seems to be off!
                - even there is a huge visual gap!
                - did adjust it by hand for a comparisson:
                - with adjustment
                ![adjusted 1](images/adjusted1.png)
                ![adjusted 2](images/adjusted2.png)

                - without adjustment 
                ![without adjustment](images/withoutAdjust.png)
                    - little sidenote, i was using on of the cams upside down (but dont think it should do any harm, there is the calibration anyway. and no videos ever looked upsidedown)

                - **found the problem!**
                    - the coordinates for the cameras was swapped!
                        - in the callib file they get the cameras serial id? or smth
                        - in unity its just an enum with Camera_ID_02 etc
                    - i fixed it to match its cameras serial id but still is swapped?!
                    - example if i swap it by hand (not perfectly alligned but i blame that on shitty calibration in a too small area)
                    ![swappedByHand](images/swappedByHand.png)

    - stoping here for now, need to fix that swapped coordinates at some point 
        - the point cloud is messy but might have potential (doubt it will be very pleasing in vr/mr tho...)
        - however: i dont want to work much further on this. I want to focus on the VR prototype!
        - what does markus say?
            - one thing i would like to have is for the points of the point cloud to only be visible from their "front" +/- 90° (but that seems complex to do...)
                - might help with the problem of seeing the back of a point cloud (since they are not perfectly aligned)
        - i could also try to get the lindlbauer setup running (the calibration should already be done by zed anyway, only need to change the stuff to use zed sdk and maybe get a obj of the environment or rather check if its really needed since zed meshes are messy.)

## 11.04.24
- for point clouds might be able to check normals
- lindlbauer -> read paper, check voxel stuff
- can go to vr prototype
    - get quest 2 (3 sind aus) bei david
    - get an avatar!
    - for usability testing with ppl, no need for questionnaire
- have asked david fasching for a hmd, he will let me know once one would be available (depends a bit on the ordering of quest 3s etc.)

## 13.04.24
- reading lindlbauer paper again
- doing some research

- planning to checking in on overleaf 
    - specifically: related work
    - tackling this next time

## 15.04.24
- checking in on overleaf
    - some really rough structure on chapters plan to cover
    - compared to a masters thesis from library -> Alexander Auberger Enhancing Player Engagement through Adaptive Difficulty using Implicit Biofeedback in Simulation & Real-Time Strategy Games (mainly just for structure, not content)
- more research regarding the topics i have identified for the thesis structure


## 17.04.24
- trying to fix the mixed up spawning of point cloud coordinates
    - from code pov the correct cam id gets the corresponding coordinates of the json file
    - maybe applying the translation etc is wrong? i am using some code i found for it, this one converts the rotation somehow (radians to deg)
        - didnt find any documentation if the calibration file rotation is in deg or radians
            - i just tried it real quick: seems they are radians. when read as deg the cameras face roughly the same direction which does not allign with the setup of the calibration.
        - https://www.stereolabs.com/docs/fusion/zed360
        - "fun" fact, cam 0 is always at 0 0 0, except if they detect ankles they assume the floor level then its 0 H 0

    - this might help in the future: https://community.stereolabs.com/
- will continue my research for viewpoints (related work stuff) tomorrow? (we'll see about that...)
    

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
