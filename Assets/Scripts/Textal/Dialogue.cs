public class Dialogue
{
    public string content;
    public string speaker;
    public string dialogueLocation;
    public int dialogueNumber;
    public Dialogue(string content, string speaker)
    {
        this.content = content;
        this.speaker = speaker;
    }
    public void assignIDLoc(string dialogueLocation, int dialogueNumber)
    {
        this.dialogueLocation = dialogueLocation;
        this.dialogueNumber = dialogueNumber;
    }
}
