using System;

public class ConsoleCommandUnit
{
    public object representedObject;

    public ConsoleCommandUnit(object represented)
    {
        representedObject = represented;
        Name = (string)represented.GetField("Name");
        CommandReference = (Action<string[]>)represented.GetField("CommandReference");
    }

    public string Name;

    public Action<string[]> CommandReference;

    public void RunCommand(string[] args)
    {
        representedObject.InvokeMethod("RunCommand", new object[] { args });
    }

    public AutocompletionSettings Autocompletion;

    public ConsoleCommandUnit()
    {
    }
}