using NUnit.Framework.Constraints;
using System.Collections;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenLabsTTS : MonoBehaviour
{
    public string API_Key = "sk_3f2daca8220418cd6901317f0ebfc1f9194f1fd8c90d09e9";
    public string VoiceID = "";
    private string API_URL = "https://api.elevenlabs.io";

    public bool Streaming = false;

    public AudioSource audio_source;

    private void Start()
    {
        StartCoroutine(SendRequest("HELLO WORLD!"));
    }

    private IEnumerator SendRequest(string message)
    { 
        string stream = Streaming ? "/stream" : "";
        string url = $"{API_URL}/v1/text-to-speech/{VoiceID}{stream}";
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "application/json"))
        {
            // Create JSON payload
            string jsonBody = "{\"text\": \"" + message + "\"}";
            UploadHandler uh = new UploadHandlerRaw(Encoding.ASCII.GetBytes(jsonBody));
            DownloadHandlerAudioClip duh = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            if (Streaming)
            {
                duh.streamAudio = true;
            }

            request.uploadHandler = uh;
            request.downloadHandler = duh;

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", API_Key);
            request.SetRequestHeader("Accept", "audio/mpeg");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading audio: " + request.error);
                yield break;
            }

            AudioClip ac = duh.audioClip;
            if (audio_source != null && ac != null)
            {
                audio_source.clip = ac;
                audio_source.Play();
            }
        }
    }
}
