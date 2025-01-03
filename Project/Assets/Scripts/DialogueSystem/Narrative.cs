using System;
using System.Collections.Generic;


[Serializable]
public class Dialogue
{
    public string text;
    public string actor;
}

[Serializable]
public class Choice
{
    public string text;
    public string nextNarrativeId; 
}

[Serializable]
public class Conversation
{
    public string id;
    public List<Dialogue> dialogues;
    public List<Choice> choices; 
}

[Serializable]
public class Narrative
{
    public List<Conversation> conversations;
}





