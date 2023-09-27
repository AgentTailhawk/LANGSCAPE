using OpenAI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Rendering.DebugUI;
#if !UNITY_STANDALONE_WIN
using Oculus.Interaction;
using Wit; // Wit.ai Unity SDK
//using UnityEngine.Windows.Speech;
#endif

public class CommandInterpreter : MonoBehaviour {
    // Display
    [SerializeField] private TMP_Text inputBox;
    [SerializeField] private TMP_Text outputBox;
    [SerializeField] private UnityEngine.UI.Image symbol;
    [SerializeField] private TMP_Dropdown dropdown;

    // OpenAI Settings
    private OpenAIApi openai;
    private string prompt;


    //Gesture Detect
#if !UNITY_STANDALONE_WIN
    public GestureTest gesture;
#endif

    // ChatGPT
    private List<ChatMessage> messages = new List<ChatMessage>();

    // Loads prompt from file in Assets/Resources/prompt
    void Awake() {
        openai = new OpenAIApi(apiKey: "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");
        TextAsset filedata = Resources.Load<TextAsset>("OpenAI/PROMPT");
        if (filedata == null)
            throw new System.Exception("No file found called prompt in 'Assets/Resources/OpenAI/PROMPT");
        prompt = filedata.text;
        Debug.Log(prompt);
    }

    // Wit

    private readonly string fileName = "output.wav";
    private readonly int MAX_DURATION = 30;
    private AudioClip clip;
    private bool isRecording;
    private float time = 0;

    private Wit.Wit witClient;

    // Start is called before the first frame update
    void Start()
    {
        witClient = new Wit.Wit("4BQIS2VAGGAEJB4YQ5OSYWGV7HSX4CJO");
        // Initialize microphone dropdown with available devices
        PopulateMicrophoneDropdown();
    }

    private void PopulateMicrophoneDropdown()
    {
        string[] microphoneDevices = Microphone.devices;
        microphoneDropdown.ClearOptions();
        microphoneDropdown.AddOptions(new List<string>(microphoneDevices));
    }

    public void StartRecording()
    {
        isRecording = true;
        symbol.enabled = true;
        inputBox.text = "Listening...";

        int selectedMicrophoneIndex = microphoneDropdown.value;
        string selectedMicrophone = Microphone.devices[selectedMicrophoneIndex];

        clip = Microphone.Start(selectedMicrophone, false, 30, 44100);
    }

public async void EndRecording()
{
    isRecording = false;
    symbol.enabled = false;
    time = 0;
    inputBox.text = "Transcribing...";
    Microphone.End(null);

    byte[] audioData = WavUtility.GetWavData(clip);

    // Send audio data to Wit.ai for speech recognition
    WitResponse response = await witClient.AudioMessageAsync(audioData);

    // Extract text from Wit.ai response
    if (response != null && response.Entities.ContainsKey("message_body"))
    {
        string messageBody = response.Entities["message_body"][0].Value;
        inputBox.text = messageBody;

        // Now you have the recognized text from Wit.ai (messageBody).
        // You can use this text for further processing.

        // For example, if you want to send it to an AI chatbot, you can do so here.
        
    }
    else
    {
        inputBox.text = "Failed to transcribe audio.";
    }
}


    // Update is called once per frame
    void Update() {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.V))
            StartRecording();
        if (Input.GetKeyUp(KeyCode.V))
            EndRecording();
#else
        if (!isRecording && gesture.selected)
            StartRecording();
        if (isRecording && !gesture.selected)
            EndRecording();
#endif

        if (isRecording) {
            time += Time.deltaTime;
            if (time >= MAX_DURATION)
                EndRecording();
        }
    }
    private async void CreateJSON(string request) {
        ChatMessage userRequest = new ChatMessage() {
            Role = "user",
            Content = request
        };

        messages.Add(userRequest);

        outputBox.text = "Loading response...";

        // Complete the instruction
        try {
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
                Model = "gpt-3.5-turbo-16k",
                Messages = messages,
                Temperature = 0f,
                MaxTokens = 256,
                PresencePenalty = 0,
                FrequencyPenalty = 0
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) {
                var aiResponse = completionResponse.Choices[0].Message;
                aiResponse.Content = aiResponse.Content.Trim();

                messages.Add(aiResponse);
                outputBox.text = aiResponse.Content;

#if TEST_CUBEPLACER
            WorldCommand commands = JSONParser.ParseCommand(message.Content);
            if (commands != null && commands.success && commands.modified != null) {
                Debug.Log("Placing: " + commands.modified.Length);
                GridMesh.Instance.Multiplace(commands.modified);
            }
#else
                //Debug.Log("CommandInterpreter message: " + message.Content.ToString()); //[[[CONSIDER MOVING THIS TO DEBUG SUITE]]]
                WorldStateManager.Instance.BuildCommandBatch(aiResponse.Content, userRequest.Content);
#endif
            } else {
                outputBox.text = "No text was generated from this prompt.";
            }
        } catch (System.Exception e) {
            outputBox.text = e.Message;
        }
    }
}