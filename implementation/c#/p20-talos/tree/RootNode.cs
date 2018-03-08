using System.Collections.Generic;

namespace p20_talos.tree
{
    public class RootNode : Node
    {
        protected override bool Matches(string path, Dictionary<string, string> variables, Dictionary<string, HashSet<string>> sets)
        {
            return true;
        }
    }
}