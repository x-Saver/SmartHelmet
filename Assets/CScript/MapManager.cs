using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Android;
using TMPro;
using static System.Net.WebRequestMethods;


public class MapManager : MonoBehaviour
{
    public RawImage mapRawImage;

    [Header("Map parameter")]
    private string mapAPIbaseURL = "https://naveropenapi.apigw.ntruss.com/map-static/v2/raster";
    private string navyAPIbaseURL = "https://naveropenapi.apigw.ntruss.com/map-direction/v1/driving";
    public string APIKey = "";
    public string secretKey = "";
    private string latitude;
    private string longitude;
    public int zoomLevel = 13;
    public int mapWidth;
    public int mapHeight;
    private int scale;
    public string startPoint;
    public string destPoint;
    public string navyOption = "traoptimal";
    public string Navy;

    public TextMeshProUGUI[] InfoText = new TextMeshProUGUI[4];
    public float GPS_delay;
    public float max_delay_time;
    void Start()
    {
        mapRawImage = GetComponent<RawImage>();
        //StartCoroutine(GPSManager());
        StartCoroutine(MapLoader());
        StartNavigate();
    }
    IEnumerator GPSManager()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            while(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }
        if(Input.location.isEnabledByUser)
        {
            InfoText[0].text = "GPS OFF";
        }
        Input.location.Start();
        while(Input.location.status == LocationServiceStatus.Initializing && GPS_delay <max_delay_time)
        {
            yield return new WaitForSeconds(1.0f);
            GPS_delay++;
        }
        if(Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
        {
            InfoText[0].text = "Delay Time Over";
        }
        latitude = Input.location.lastData.latitude.ToString();
        longitude = Input.location.lastData.longitude.ToString();
        InfoText[0].text = "위도 : " + latitude + ", 경도 : " + longitude;
        yield return new WaitForSeconds (5.0f);
    }
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
    private void StartNavigate()
    {
        string sendUrl = navyAPIbaseURL + "?start=" + startPoint + "&goal=" + destPoint + "&option=" + navyOption;

        Debug.Log(sendUrl);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(sendUrl);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", APIKey);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", secretKey);

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            DownloadHandlerTexture.GetContent(request);
        }
    }

}
