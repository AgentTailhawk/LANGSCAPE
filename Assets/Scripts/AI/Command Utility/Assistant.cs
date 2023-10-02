using UnityEngine;
using UnityEngine.UI;
using OpenAI; // Import the OpenAI API client library

public class Assistant : MonoBehaviour
{
    public Text userText;
    public Text assistantText;
    public AudioSource audioSource;

    private string apiKey = "YOUR_OPENAI_API_KEY_HERE"; // Replace with your OpenAI API key
    private ChatGPT chatGPT;

    void Start()
    {
        chatGPT = new ChatGPT(apiKey);
    }

    async void GenerateAssistantResponse(string userInput)
    {
        try
        {
            // Define a conversation with user input and context
            var conversation = new Conversation
            {
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "system",
                        Content = "You are a helpful assistant that provides information in a VR application where structures are built with voice commands from a user"
                    },
                    new Message
                    {
                        Role = "user",
                        Content = userInput
                    }
                }
            };

            // Generate a response from ChatGPT
            var response = await chatGPT.CreateCompletion(conversation);

            string assistantResponse = response.Choices[0].Message.Content;

            assistantText.text = assistantResponse;

            // Use text-to-speech to speak the assistant's response
            SpeakText(assistantResponse);
        }
        catch (System.Exception e)
        {
            Debug.LogError("ChatGPT API Error: " + e.Message);
        }
    }

    void SpeakText(string textToSpeak)
    {
        // Use text-to-speech to speak the provided text
        var input = new SynthesisInput
        {
            Text = textToSpeak
        };

        var voiceSelection = new VoiceSelectionParams
        {
            LanguageCode = "en-US",
            Name = "en-US-Wavenet-D" // Choose a voice
        };

        var audioConfig = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Linear16
        };

        var response = chatGPT.SynthesizeSpeech(input, voiceSelection, audioConfig);

        audioSource.clip = WavUtility.ToAudioClip(response.AudioContent.ToByteArray(), 0, response.AudioContent.ToByteArray().Length, 0, 0);
        audioSource.Play();
    }

    void Update()
    {
        string userInput = userText.text;
        if (!string.IsNullOrEmpty(userInput))
        {
            GenerateAssistantResponse(userInput);
            userText.text = ""; // Clear user input text box
        }
    }
}