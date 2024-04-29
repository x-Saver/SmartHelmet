using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Android;
using TMPro;
using Newtonsoft;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using ICSharpCode.SharpZipLib.Core;


public class MapManager_TmapAPI : MonoBehaviour
{
   

    public RawImage mapRawImage;
    private string APIKey = "6x8EhJL8Em30JTP1gTayM7xBiavfbKhz7MK0GdfL";

    [Header("Map parameter")]
    private string mapAPIbaseURL = "https://apis.openapi.sk.com/tmap/staticMap";
    private string coordType = "WGS84GEO";
    private string format = "PNG";
    public string latitude = "37.5446283608815"; // 소수점 11자리
    public string longitude = "126.83529138565";
    public int zoomLevel = 15;
    public int mapWidth = 512;
    public int mapHeight = 512;
    public string marker = "126.978155,37.566371";

    [Header("navigate parameter")]
    private string navyAPIbaseURL = "https://apis.openapi.sk.com/tmap/routes/pedestrian";
    

    public TextMeshProUGUI[] InfoText = new TextMeshProUGUI[4];
    public float GPS_delay;
    public float max_delay_time;

    // json data 정의
    [System.Serializable]
    public class NavyJson
    {
        public string type;
        public List<Feature> features;
    }

    [System.Serializable]
    public class Feature
    {
        public string type;
        public Geometry geometry;
        public Properties properties;
    }

    [System.Serializable]
    public class Geometry
    {
        public string type;
        public object coordinates;
    }

    [System.Serializable]
    public class Properties
    {
        public int index;
        public int lineIndex;
        public string name;
        public string description;
        public float distance;
        public float time;
        public int? roadType;
        public int? categoryRoadType;
        public int? facilityType;
        public string facilityName;
        public string direction;
        public string nearPoiName;
        public string nearPoiX;
        public string nearPoiY;
        public string intersectionName;
        public int? turnType;
        public string pointType;
    }

    void Start()
    {
        mapRawImage = GetComponent<RawImage>();
        //StartCoroutine(GPSManager());
        StartCoroutine(MapLoader());
        StartCoroutine(StartNavigate());
    }
    //IEnumerator GPSManager()
    //{
    //    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
    //    {
    //        Permission.RequestUserPermission(Permission.FineLocation);
    //        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
    //        {
    //            yield return null;
    //        }
    //    }
    //    if (Input.location.isEnabledByUser)
    //    {
    //        InfoText[0].text = "GPS OFF";
    //    }
    //    Input.location.Start();
    //    while (Input.location.status == LocationServiceStatus.Initializing && GPS_delay < max_delay_time)
    //    {
    //        yield return new WaitForSeconds(1.0f);
    //        GPS_delay++;
    //    }
    //    if (Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
    //    {
    //        InfoText[0].text = "Delay Time Over";
    //    }
    //    latitude = Input.location.lastData.latitude.ToString();
    //    longitude = Input.location.lastData.longitude.ToString();
    //    InfoText[0].text = "위도 : " + latitude + ", 경도 : " + longitude;
    //    yield return new WaitForSeconds(5.0f);
    //}
    IEnumerator MapLoader()
    {
        string sendUrl = mapAPIbaseURL +
            "?version=1" +
            "&coordType=" + coordType +
            "&width=" + mapWidth + 
            "&height=" + mapHeight +
            "&zoom=" + zoomLevel +
            "&format=" + format +
            "&longitude=" + longitude +
            "&latitude=" + latitude +
            "&markers=" + marker;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(sendUrl))
        {
            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("appKey", APIKey);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (mapRawImage != null) mapRawImage.texture = texture;
                else Destroy(texture);
            }
        }
    }
    IEnumerator StartNavigate()
    {
        string sendUrl = navyAPIbaseURL + "?version=1";
        float startPoint_X = 126.92365493654832F;
        float startPoint_Y = 37.556770374096615F;
        float endPoint_X = 126.92432158129688F;
        float endPoint_Y = 37.55279861528311F;
        string startName = "%EA%B4%91%EC%B9%98%EA%B8%B0%ED%95%B4%EB%B3%80"; // url 인코딩 해야됨
        string endName = "%EC%98%A8%ED%8F%89%ED%8F%AC%EA%B5%AC";
        var request_json = new
            {
                startName = startName,
                startX = startPoint_X.ToString(),
                startY = startPoint_Y.ToString(),
                endName = endName,
                endX = endPoint_X.ToString(),
                endY = endPoint_Y,
                searchOption = "0",
                sort = "custom"
            };
        
        string str_navyjson = JsonConvert.SerializeObject(request_json);

        using (UnityWebRequest request = UnityWebRequest.Post(sendUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(str_navyjson);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("appKey", APIKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
               
                Debug.Log($"Error: {request.error}");
                Debug.Log($"Response: {request.downloadHandler.text}");
                Debug.Log($"Response Code: {request.responseCode}");
                
            }
            else
            {
                try
                {
                    NavyJson navyjson = JsonConvert.DeserializeObject<NavyJson>(request.downloadHandler.text);
                    ProcessFeatures(navyjson);
                }
                catch(JsonSerializationException ex)
                {
                    Debug.LogError("Json Serialized Error :" + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.LogError("General Error : " + ex.Message);
                }
                
            }
        }   
    }
    void ProcessFeatures(NavyJson navyjson)
    {
        foreach(var feature in navyjson.features) Debug.Log($"Feature Name: {feature.properties.name}, Description: {feature.properties.description}");
    }
   
}
