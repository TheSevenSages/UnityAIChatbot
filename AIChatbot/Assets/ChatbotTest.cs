using UnityEngine;
using LLMUnity;
using System.Threading.Tasks;

public class ChatbotTest : MonoBehaviour
{
    public LLMCharacter character;
    public TMPro.TMP_Text UI_1;

    void HandleReply(string reply)
    {
        Debug.Log(reply);
        UI_1.text = reply;
    }

    void ReplyCompleted()
    {
        Debug.Log("The AI Replied");
    }

    public void SendAIMessage(string message)
    {
        character.CancelRequests();
        _ = character.Chat(message, HandleReply, ReplyCompleted);
    }
}
