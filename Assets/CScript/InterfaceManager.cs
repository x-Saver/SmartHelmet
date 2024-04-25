using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

// 기기별로 다를 수 있는 입력 및 컨트롤은 project settings -> player -> Script Compliation의 scripting define Symbols를 설정하는 것으로 대응 가능.
public class InterfaceManager : MonoBehaviour
{
    public Button BT_SendRequest;
    public TextMeshProUGUI simpleText;
    void Start()
    {
        BT_SendRequest.onClick.AddListener(OnClicked_BT_SendRequest);
    }

   void OnClicked_BT_SendRequest()
    {
        StartCoroutine(SendWebRequest("https://httpbin.org/get"));
    }
    IEnumerator SendWebRequest(string url)
    {
        using ( UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest(); // 요청 보내고 응답을 기다리는 중

            if(webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
                simpleText.text = webRequest.error;
            }
            else
            {
                Debug.Log("Text : " + webRequest.downloadHandler.text);
                simpleText.text = webRequest.downloadHandler.text;
            }

        }
    }
}
