using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ETGModConsole
{
    public static ConsoleCommandGroup Commands;

    public static void Log(string message, bool debuglog = false)
    {
        InvokeStaticMTGMethod("ETGModConsole", "Log", message, debuglog);
    }
}