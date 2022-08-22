using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ETGModConsole : ETGModMenu
{
    public static ConsoleCommandGroup Commands;

    public static void Log(string message, bool debuglog = false)
    {
        InvokeStaticMTGMethod("ETGModConsole", "Log", message, debuglog);
    }

    public override void Start()
    {
    }
}