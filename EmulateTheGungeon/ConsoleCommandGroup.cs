using System;
using System.Collections.Generic;
using System.Linq;

public class ConsoleCommandGroup : ConsoleCommandUnit
{

    public ConsoleCommandGroup(object obj) : base(obj)
    {
    }

    public ConsoleCommandGroup AddUnit(string name, ConsoleCommand command)
    {
        representedObject.InvokeMethod("AddUnit", name, command);
        return this;
    }

    public ConsoleCommandGroup AddUnit(string name, Action<string[]> action)
    {
        representedObject.InvokeMethod("AddUnit", name, action);
        return this;
    }

    public ConsoleCommandGroup AddUnit(string name, Action<string[]> action, AutocompletionSettings autocompletion)
    {
        representedObject.InvokeMethod("AddUnit", name, action, autocompletion.represented);
        return this;
    }

    public ConsoleCommandGroup AddUnit(string name, ConsoleCommandGroup group)
    {
        representedObject.InvokeMethod("AddUnit", name, group);
        return this;
    }

    public ConsoleCommandGroup AddGroup(string name)
    {
        representedObject.InvokeMethod("AddGroup", name);
        return this;
    }

    public ConsoleCommandGroup AddGroup(string name, Action<string[]> action)
    {
        representedObject.InvokeMethod("AddGroup", name, action);
        return this;
    }

    public ConsoleCommandGroup AddGroup(string name, Action<string[]> action, AutocompletionSettings autocompletion)
    {
        representedObject.InvokeMethod("AddGroup", name, action, autocompletion.represented);
        return this;
    }

    public UnitSearchResult SearchUnit(string[] path)
    {
        ConsoleCommandGroup currentgroup = this;
        UnitSearchResult result = new UnitSearchResult();
        for (int i = 0; i < path.Length; i++)
        {
            ConsoleCommandGroup group = currentgroup.GetGroup(path[i]);
            ConsoleCommand command = currentgroup.GetCommand(path[i]);
            if (group != null)
            {
                currentgroup = group;
                result.index++;
            }
            else if (command != null)
            {
                result.index++;
                result.unit = command;
                return result;
            }
        }
        result.unit = currentgroup;
        return result;
    }

    /*public List<List<string>> ConstructPaths()
    {
        List<List<string>> ret = new List<List<string>>();
        foreach (string key in _Commands.Keys)
        {
            List<string> tmp = new List<string>();
            tmp.Add(key);
            ret.Add(tmp);
        }

        foreach (string key in _Groups.Keys)
        {
            List<string> groupkeytmp = new List<string>();
            groupkeytmp.Add(key);
            ret.Add(groupkeytmp);

            List<List<string>> tmp = _Groups[key].ConstructPaths();
            for (int i = 0; i < tmp.Count; i++)
            {
                List<string> prefixtmp = new List<string>();
                prefixtmp.Add(key);
                for (int j = 0; j < tmp[i].Count; j++)
                {
                    prefixtmp.Add(tmp[i][j]);
                }
                ret.Add(prefixtmp);
            }
        }

        return ret;
    }*/

    public string[] GetAllUnitNames()
    {
        return (string[])representedObject.InvokeMethod("GetAllUnitNames");
    }

    public ConsoleCommandUnit GetUnit(string[] unit)
    {
        var u = SearchUnit(unit).unit;
        if(u == null)
        {
            return null;
        }
        return new ConsoleCommandUnit();
    }

    public ConsoleCommandGroup GetGroup(string[] unit)
    {
        var u = SearchUnit(unit).unit;
        if(u == null)
        {
            return null;
        }
        return new ConsoleCommandGroup(u);
    }

    public ConsoleCommandGroup GetGroup(string unit)
    {
        var u = representedObject.InvokeMethod("GetGroup", unit);
        if (u == null)
        {
            return null;
        }
        return new(u);
    }

    public ConsoleCommand GetCommand(string[] unit)
    {
        var u = representedObject.InvokeMethod("GetCommand", unit);
        if(u == null)
        {
            return null;
        }
        return new(u);
    }

    public ConsoleCommand GetCommand(string name)
    {
        var u = representedObject.InvokeMethod("GetCommand", name);
        if(u == null)
        {
            return null;
        }
        return new(u);
    }

    public int GetFirstNonUnitIndexInPath(string[] path)
    {
        return SearchUnit(path).index + 1;
    }

    public class UnitSearchResult
    {
        public int index = -1;
        public ConsoleCommandUnit unit;
        public UnitSearchResult(int index, ConsoleCommandUnit unit)
        {
            this.index = index;
            this.unit = unit;
        }
        public UnitSearchResult() { }
    }
}