using System.Collections.Generic;

namespace p20_talos.tree
{
    public class UniversalNode : Node
    {
        protected override bool Matches(string word, Dictionary<string, string> variables, Dictionary<string, HashSet<string>> sets)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override string ToString()
        {
            return "UniversalNode";
        }
    }
}