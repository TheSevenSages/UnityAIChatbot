using UnityEngine;
using LLMUnity;
using System.Threading.Tasks;
using UnityEngine.Events;

public class ChatbotTest : MonoBehaviour
{
    public LLMCharacter character;
    public TMPro.TMP_Text UI_1;
    public UnityEvent<string> TTS;

    void HandleReply(string reply)
    {
        Debug.Log(reply);
        UI_1.text = reply;
    }

    void ReplyCompleted()
    {
        if (TTS != null)
        {
            Debug.Log("The AI Replied");
            TTS.Invoke(UI_1.text);
        }
        else
        {
            Debug.Log("ERROR");
        }
    }

    public void SendAIMessage(string message)
    {
        character.CancelRequests();
        _ = character.Chat(message, HandleReply, ReplyCompleted);
    }
}
