using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using LitJson;
using UnityEngine;


public class Api
{
  private static readonly string _URL = "https://hf0xcousg6.execute-api.ap-northeast-1.amazonaws.com/BoidMainStage/";
  private static readonly string _FISH = "fish";
  private static readonly string _CHILD = "child";

  public static async UniTask<JsonData> GetList()
  {
    UnityWebRequest www = new UnityWebRequest(Api._URL + Api._FISH, "GET");
    www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    await www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.error);
      return null;
    }
    else
    {
      // NOTE: 長すぎて切り捨てられている可能性
      Debug.Log(www.downloadHandler.text);
      JsonData data = JsonMapper.ToObject<JsonData>(www.downloadHandler.text.Trim());
      // JsonData data = JsonMapper.ToObject<JsonData>("{'hoge': 1}");
      return data;
    }
  }

  /// <summary>
  /// 演奏一覧を取得
  /// </summary>
  /// <returns></returns>
  public static IEnumerator _GetList()
  {
    UnityWebRequest www = new UnityWebRequest(Api._URL + Api._FISH, "GET");
    www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    yield return www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.error);
    }
    else
    {
      Debug.Log(www.downloadHandler.text);
      JsonData data = JsonMapper.ToObject<JsonData>(www.downloadHandler.text);
    }
  }

}