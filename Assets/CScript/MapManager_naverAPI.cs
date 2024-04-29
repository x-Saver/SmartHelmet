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

public  class NavyJson
{
    public string code;
    public string message;
    public string currentDateTime;
    public Dictionary<string, string> route;
};


public class MapManager_naverAPI : MonoBehaviour
{
    public RawImage mapRawImage;

    [Header("Map parameter")]
    private string mapAPIbaseURL = "https://naveropenapi.apigw.ntruss.com/map-static/v2/raster";
    private string navyAPIbaseURL = "https://naveropenapi.apigw.ntruss.com/map-direction/v1/driving";
    public string APIKey = "";
    public string secretKey = "";
    private string latitude = "37.5655193";
    private string longitude = "126.9734967";
    public int zoomLevel = 16;
    public int mapWidth;
    public int mapHeight;
    private int scale = 2;
    private string startPoint = "37.5700275,126.9719180";
    private string destPoint = "37.5701065,126.9875905"; //주안역
    public string navyOption = "traoptimal";

    public TextMeshProUGUI[] InfoText = new TextMeshProUGUI[4];
    public float GPS_delay;
    public float max_delay_time;

    public NavyJson navydata = new NavyJson();

    string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
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
        string sendUrl = mapAPIbaseURL + "?w=" + mapWidth.ToString() + "&h=" + mapHeight.ToString() +
            "&center=" + longitude + "," + latitude + "&level=" + zoomLevel.ToString() +"&scale=" + scale;

        Debug.Log(sendUrl);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(sendUrl);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", APIKey);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", secretKey);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            mapRawImage.texture = DownloadHandlerTexture.GetContent(request);
        }
    }
    IEnumerator StartNavigate()
    {
        string sendUrl = navyAPIbaseURL + "?start=" + startPoint + "&goal=" + destPoint + "&option=" + navyOption;

        Debug.Log(sendUrl);

        UnityWebRequest request = UnityWebRequest.Get(sendUrl);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", APIKey);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", secretKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var text = request.downloadHandler.text;
            NavyJson navydata = new NavyJson();
            string jsonData = ObjectToJson(text);
            Debug.Log(jsonData);
        }
    }

}
