using System.Collections;
using UnityEngine.Networking;
using LitJson;
using UnityEngine;


public class Api
{
  private static readonly string _URL = "https://hf0xcousg6.execute-api.ap-northeast-1.amazonaws.com/BoidMainStage/";
  private static readonly string _FISH = "fish";
  private static readonly string _CHILD = "child";

  /// <summary>
  /// 演奏一覧を取得
  /// </summary>
  /// <returns></returns>
  public static IEnumerator GetList()
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