using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using LitJson;


public class Api
{
  private static readonly string _URL = "https://hf0xcousg6.execute-api.ap-northeast-1.amazonaws.com/BoidMainStage/";
  private static readonly string _FISH = "fish";
  private static readonly string _CHILD = "createchild";

  /// <summary>
  ///
  /// </summary>
  /// <returns></returns>
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
      JsonData data = JsonMapper.ToObject<JsonData>(www.downloadHandler.text.Trim());
      Debug.Log($"GetList Response {www.downloadHandler.text}");
      return data;
    }
  }

  /// <summary>
  ///
  /// </summary>
  /// <returns></returns>
  public static async UniTask<JsonData> GetChild(FishData daddy, FishData mammy)
  {
    string daddyData = daddy.ToJson();
    string mammyData = mammy.ToJson();
    byte[] postData = System.Text.Encoding.UTF8.GetBytes("{parents: [" + daddyData + "," + mammyData + "], generation: " + daddy.generation + "}");
    UnityWebRequest www = new UnityWebRequest(Api._URL + Api._CHILD, "GET");
    www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
    www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");
    await www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.error);
      return null;
    }
    else
    {
      JsonData data = JsonMapper.ToObject<JsonData>(www.downloadHandler.text.Trim());
      Debug.Log($"GetChild Response {www.downloadHandler.text}");
      return data;
    }
  }

}