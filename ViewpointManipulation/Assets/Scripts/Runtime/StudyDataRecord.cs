using Runtime.View.Manager;

namespace Runtime
{
    public class Task
    {
        public ViewMode
            Mode = ViewMode.Hover; //hover or drone -> for task one (not the exploration one, there its flipped!)
        
        //all times are in seconds
        public float Duration = 0f; //pressed start (of task one with pois)

        public float TimeForPoiOne = 0f; //time needed to find poi one for first time
        public string PoiNameOne = "null"; //what poi this is
        public long LostTrackOfPoiOne = 0; //how often the user lost track of the poi one
        public float TimeForPoiTwo = 0f; //time needed to find poi two for first time
        public string PoiNameTwo = "null"; //what poi this is
        public long LostTrackOfPoiTwo = 0; //how often the user lost track of the poi two
        public float TimeForPoiThree = 0f; //time needed to find poi three for first time
        public string PoiNameThree = "null"; //what poi this is
        public long LostTrackOfPoiThree = 0; //how often the user lost track of the poi three
        public float TimeForPoiFour = 0f; //time needed to find poi four for first time
        public string PoiNameFour = "null"; //what poi this is
        public long LostTrackOfPoiFour = 0; //how often the user lost track of the poi four
        public float TimeForPoiFive = 0f; //time needed to find poi five for first time
        public string PoiNameFive = "null"; //what poi this is
        public long LostTrackOfPoiFive = 0; //how often the user lost track of the poi five
        public float TimeForPoiSix = 0f; //time needed to find poi six for first time
        public string PoiNameSix = "null"; //what poi this is
        public long LostTrackOfPoiSix = 0; //how often the user lost track of the poi one

        public int SpawnedCamCount = 0; //how often the user spawned a cam
        public int DeletedCamCount = 0; //how often the user deleted a cam
        public int DockCount = 0; //how often the user docked a view-panel to the hud
        public int UnDockCount = 0; //how often the user undocked a view-panel from the hud

        public float TimeInUserRelativeMode = 0f; //time spent with an active drone cam in user relative mode
        public float TimeInDroneRelativeMode = 0f; //time spend with an active drone cam in drone relative mode

        public override string ToString()
        {
            return
                $"{(Mode == ViewMode.Drone ? "Drone" : (Mode == ViewMode.Hover ? "Hover" : "OCE"))};{Duration:F};" +
                $"{TimeForPoiOne:F};" +
                $"{PoiNameOne};" +
                $"{LostTrackOfPoiOne:D};" +
                $"{TimeForPoiTwo:F};" +
                $"{PoiNameTwo};" +
                $"{LostTrackOfPoiTwo:D};" +
                $"{TimeForPoiThree:F};" +
                $"{PoiNameThree};" +
                $"{LostTrackOfPoiThree:D};" +
                $"{TimeForPoiFour:F};" +
                $"{PoiNameFour};" +
                $"{LostTrackOfPoiFour:D};" +
                $"{TimeForPoiFive:F};" +
                $"{PoiNameFive};" +
                $"{LostTrackOfPoiFive:D};" +
                $"{TimeForPoiSix:F};" +
                $"{PoiNameSix};" +
                $"{LostTrackOfPoiSix:D};" +
                $"{SpawnedCamCount:D};" +
                $"{DeletedCamCount:D};" +
                $"{DockCount:D};" +
                $"{UnDockCount:D};" +
                $"{TimeInUserRelativeMode:F};" +
                $"{TimeInDroneRelativeMode:F}";
        }

        public string GetHeader()
        {
            return "Mode;Duration;" +
                   "TimeForPoiOne;" +
                   "PoiNameOne;" +
                   "LostTrackOfPoiOne;" +
                   "TimeForPoiTwo;" +
                   "PoiNameTwo;" +
                   "LostTrackOfPoiTwo;" +
                   "TimeForPoiThree;" +
                   "PoiNameThree;" +
                   "LostTrackOfPoiThree;" +
                   "TimeForPoiFour;" +
                   "PoiNameFour;" +
                   "LostTrackOfPoiFour;" +
                   "TimeForPoiFive;" +
                   "PoiNameFive;" +
                   "LostTrackOfPoiFive;" +
                   "TimeForPoiSix;" +
                   "PoiNameSix;" +
                   "LostTrackOfPoiSix;" +
                   "SpawnedCamCount;" +
                   "DeletedCamCount;" +
                   "DockCount;" +
                   "UnDockCount;" +
                   "TimeInUserRelativeMode;" +
                   "TimeInDroneRelativeMode";
        }
    }

    public class StudyDataRecord
    {
        public Task[] Tasks = new Task[2];

        public StudyDataRecord()
        {
            Tasks[0] = new Task();
            Tasks[1] = new Task();
        }
    }
}