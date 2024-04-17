using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CameraConfig
{
    public uint serialId;
    public float[] rotation;
    public float[] translation;
}

public class OverlapPointCloudManager : MonoBehaviour
{
    public string calibrationJsonPath;
    public ZEDManager[] pointCloudPrefabs;

    private List<CameraConfig> _camConfigs = new List<CameraConfig>();

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
            var id = UInt32.Parse(cam.Key);

            JToken camValue = cam.Value;
            // JObject room = JObject.Parse(roomCalibration.text);
            // JObject camera = (JObject)room[cameraSerial.ToString()];
            JObject world = (JObject)camValue["world"];
            JArray rotation = (JArray)world["rotation"];
            JArray translation = (JArray)world["translation"];
            var zed = Instantiate(pointCloudPrefabs[i]);
            // https://community.stereolabs.com/t/multi-camera-point-cloud-fusion-using-room-calibration-file/3640
            float[] r = rotation.Select(v => (float)v * 180.0f / Mathf.PI).ToArray();
            float[] t = translation.Select(v => (float)v).ToArray();
            _camConfigs.Add(new CameraConfig { serialId = id, rotation = r, translation = t });

            zed.OnZEDReady += () =>
            {
                var serialNum = zed.zedCamera.GetZEDSerialNumber();
                Debug.Log($"Camera with serial number: {serialNum} is ready :)", this);
                
                var config = _camConfigs.FirstOrDefault(x => x.serialId == serialNum);
                if (config == null)
                {
                    Debug.LogWarning(
                        $"Could not find config for camera with serial number: {zed.zedCamera.GetZEDSerialNumber()}",
                        this);
                    return;
                }

                zed.transform.SetLocalPositionAndRotation(
                    new Vector3(config.translation[0], config.translation[1], config.translation[2]),
                    Quaternion.Euler(config.rotation[0], config.rotation[1], config.rotation[2])
                );
            };
            i++;

            // var id = cam.Key;
            // JToken camValue = cam.Value;
            // var world = camValue["world"];
            // //create camera+point cloud manager for each
            // var zed = Instantiate(pointCloudPrefabs[i]);
            // //align them according to the calibration
            // var rot = world["rotation"];
            // zed.transform.eulerAngles = new Vector3((float)rot[0], (float)rot[1], (float)rot[2]);
            // var trans = world["translation"];
            // zed.transform.position = new Vector3((float)trans[0], (float)trans[1], (float)trans[2]);
            // i++;
        }
    }
}