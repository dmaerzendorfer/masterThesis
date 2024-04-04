using System.IO;
using Newtonsoft.Json.Linq;
using sl;
using UnityEngine;

public class OverlapPointCloudManager : MonoBehaviour
{
    public string calibrationJsonPath;
    public ZEDManager[] pointCloudPrefabs;

    private void Awake()
    {
        //read the json
        var jsonString = File.ReadAllText(calibrationJsonPath);
        //https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jobject.htm
        var jsonData = JObject.Parse(jsonString);
        int i = 0;
        //iterate over cameras
        foreach (var cam in jsonData)
        {
            var id = cam.Key;
            JToken camValue = cam.Value;
            var world = camValue["world"];
            //create camera+point cloud manager for each
            var zed = Instantiate(pointCloudPrefabs[i]);
            //align them according to the calibration
            var rot = world["rotation"];
            zed.transform.eulerAngles = new Vector3((float)rot[0], (float)rot[1], (float)rot[2]);
            var trans = world["translation"];
            zed.transform.position = new Vector3((float)trans[0], (float)trans[1], (float)trans[2]);
            i++;
        }
    }
}