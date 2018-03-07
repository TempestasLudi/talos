using System.Collections.Generic;

namespace p20_talos.tree
{
    public class VariableNode : Node
    {
        private readonly string _name;

        public VariableNode(string name)
        {
            _name = name;
        }

        protected override bool Matches(string word, Dictionary<string, string> variables, Dictionary<string, HashSet<string>> sets)
        {
            return variables[_name] == word;
        }

        private bool Equals(VariableNode other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VariableNode) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(_name)}: {_name}";
        }
    }
}