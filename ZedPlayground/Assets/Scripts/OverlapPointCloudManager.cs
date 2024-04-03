using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class OverlapPointCloudManager : MonoBehaviour
{
    public string calibrationJsonPath;

    private void Awake()
    {
        //read the json
        var jsonString = File.ReadAllText(calibrationJsonPath);
        //https://www.newtonsoft.com/json/help/html/t_newtonsoft_json_linq_jobject.htm
        var jsonData = JObject.Parse(jsonString);
        //iterate over cameras
        foreach (var cam in jsonData)
        {
            var id = cam.Key;
            JToken camValue = cam.Value;
            var world = camValue["world"];
            //create camera+point cloud manager for each
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //todo: replace cube with the actual mono rigs and point cloud renderers :)
            //align them according to the calibration
            var rot = world["rotation"];
            cube.transform.eulerAngles = new Vector3((float)rot[0], (float)rot[1], (float)rot[2]);
            var trans = world["translation"];
            cube.transform.position = new Vector3((float)trans[0], (float)trans[1], (float)trans[2]);
        }
    }
}