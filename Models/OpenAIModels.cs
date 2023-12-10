// OpenAIModels.cs file
public class ChatMessage
{
    public string Content { get; set; }
}

public class OpenAIResponse
{
    public Choice[] choices { get; set; }
}

public class Choice
{
    public string text { get; set; }
}
