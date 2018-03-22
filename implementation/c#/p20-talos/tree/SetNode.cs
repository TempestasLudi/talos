using System.Collections.Generic;

namespace p20_talos.tree
{
    public class SetNode : Node
    {
        private readonly string _name;

        public SetNode(string name)
        {
            _name = name;
        }

        protected override bool Matches(string word, Dictionary<string, string> variables, Dictionary<string, HashSet<string>> sets)
        {
            if (!sets.ContainsKey(_name))
            {
                throw new AuthorizationException("No set named " + _name + " exists.");
            }
            return sets[_name].Contains(word);
        }

        private bool Equals(SetNode other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SetNode) obj);
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