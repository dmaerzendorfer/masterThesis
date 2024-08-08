using Runtime.View.Manager;

namespace Runtime
{
    public class StudyDataRecord
    {
        public ViewMode
            Mode = ViewMode.OCE; //OCE or drone -> for task one (not the exploration one, there its flipped!)

        //all times are in milliseconds
        public long StartTime = 0; //pressed start (of task one with pois)
        public long EndTime = 0; //all pois found (of task one with pois)

        public long
            TimeInExploration =
                0; // after the first task in Mode, users get the chance to explore with the other method, this is how long they explored for

        public long TimeForPoiOne = 0; //time needed to find poi one for first time
        public long LostTrackOfPoiOne = 0; //how often the user lost track of the poi one
        public long TimeForPoiTwo = 0; //time needed to find poi two for first time
        public long LostTrackOfPoiTwo = 0; //how often the user lost track of the poi two
        public long TimeForPoiThree = 0; //time needed to find poi three for first time
        public long LostTrackOfPoiThree = 0; //how often the user lost track of the poi three
        public long TimeForPoiFour = 0; //time needed to find poi four for first time
        public long LostTrackOfPoiFour = 0; //how often the user lost track of the poi four
        public long TimeForPoiFive = 0; //time needed to find poi five for first time
        public long LostTrackOfPoiFive = 0; //how often the user lost track of the poi five
        public long TimeForPoiSix = 0; //time needed to find poi six for first time
        public long LostTrackOfPoiSix = 0; //how often the user lost track of the poi one
        public int SpawnedCamCount = 0; //how often the user spawned a cam
        public int DeletedCamCount = 0; //how often the user deleted a cam
        public int DeleteAllCamsCount = 0; //how often the user used the 'delete-all-cams' feature of the control table
        public int DockCount = 0; //how often the user docked a view-panel to the hud
        public int UnDockCount = 0; //how often the user undocked a view-panel from the hud

        //these are drone exclusive tracked data
        //during task 1: poi
        public long TimeInUserRelativeMode = 0; //time spent with an active drone cam in user relative mode

        public long TimeInDroneRelativeMode = 0; //time spend with an active drone cam in drone relative mode

        //during task 2: free exploration
        public long TimeInUserRelativeModeExplore = 0; //time spent with an active drone cam in user relative mode
        public long TimeInDroneRelativeModeExplore = 0; //time spend with an active drone cam in drone relative mode


        public override string ToString()
        {
            return $"{(Mode == ViewMode.Drone ? "Drone" : "OCE")};{StartTime:D};{EndTime:D};{TimeInExploration:D};" +
                   $"{TimeForPoiOne:D};" +
                   $"{LostTrackOfPoiOne:D};" +
                   $"{TimeForPoiTwo:D};" +
                   $"{LostTrackOfPoiTwo:D};" +
                   $"{TimeForPoiThree:D};" +
                   $"{LostTrackOfPoiThree:D};" +
                   $"{TimeForPoiFour:D};" +
                   $"{LostTrackOfPoiFour:D};" +
                   $"{TimeForPoiFive:D};" +
                   $"{LostTrackOfPoiFive:D};" +
                   $"{TimeForPoiSix:D};" +
                   $"{LostTrackOfPoiSix:D};" +
                   $"{SpawnedCamCount:D};" +
                   $"{DeletedCamCount:D};" +
                   $"{DeleteAllCamsCount:D};" +
                   $"{DockCount:D};" +
                   $"{UnDockCount:D};" +
                   $"{TimeInUserRelativeMode:D};" +
                   $"{TimeInDroneRelativeMode:D};" +
                   $"{TimeInUserRelativeModeExplore:D};" +
                   $"{TimeInDroneRelativeModeExplore:D}";
        }

        public string GetHeader()
        {
            return "Mode;" +
                   "TimeForPoiOne;" +
                   "LostTrackOfPoiOne;" +
                   "TimeForPoiTwo;" +
                   "LostTrackOfPoiTwo;" +
                   "TimeForPoiThree;" +
                   "LostTrackOfPoiThree;" +
                   "TimeForPoiFour;" +
                   "LostTrackOfPoiFour;" +
                   "TimeForPoiFive;" +
                   "LostTrackOfPoiFive;" +
                   "TimeForPoiSix;" +
                   "LostTrackOfPoiSix;" +
                   "SpawnedCamCount;" +
                   "DeletedCamCount;" +
                   "DeleteAllCamsCount;" +
                   "DockCount;" +
                   "UnDockCount;" +
                   "TimeInUserRelativeMode;" +
                   "TimeInDroneRelativeMode;" +
                   "TimeInUserRelativeModeExplore;" +
                   "TimeInDroneRelativeModeExplore";
        }
    }
}