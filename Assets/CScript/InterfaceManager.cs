using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

// ��⺰�� �ٸ� �� �ִ� �Է� �� ��Ʈ���� project settings -> player -> Script Compliation�� scripting define Symbols�� �����ϴ� ������ ���� ����.
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
            yield return webRequest.SendWebRequest(); // ��û ������ ������ ��ٸ��� ��

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
