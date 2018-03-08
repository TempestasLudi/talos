using System;
using System.Collections.Generic;
using System.Linq;

namespace p20_talos.tree
{
    public abstract class Node
    {
        private readonly List<Node> _children = new List<Node>();

        private bool? _permission;

        protected abstract bool Matches(string word, Dictionary<string, string> variables,
            Dictionary<string, HashSet<string>> sets);

        private static Node GetNode(string expression)
        {
            if (expression == "*")
            {
                return new UniversalNode();
            }

            if (expression.Length < 2)
            {
                return new LiteralNode(expression);
            }
            if (expression[0] == '{' && expression[expression.Length - 1] == '}')
            {
                return new SetNode(expression.Substring(1, expression.Length - 2));
            }
            if (expression[0] == '[' && expression[expression.Length - 1] == ']')
            {
                return new VariableNode(expression.Substring(1, expression.Length - 2));
            }
            return new LiteralNode(expression);
        }

        public bool? HasAccess(string path, Dictionary<string, string> variables, Dictionary<string, HashSet<string>> sets)
        {
            if (path == "")
            {
                return _permission;
            }
            
            var typeValues = new Dictionary<Type, int>
            {
                {typeof(LiteralNode), 0},
                {typeof(VariableNode), 1},
                {typeof(SetNode), 2},
                {typeof(UniversalNode), 3}
            };
            var parts = path.Split(new[] {'/'}, 2);
            var newPath = parts.Length > 1 ? parts[1] : "";
            var matchingChildren = _children.OrderBy(child =>
                    typeValues.ContainsKey(child.GetType()) ? typeValues[child.GetType()] : 4)
                       .Where(child => child.Matches(parts[0], variables, sets));
            
            foreach (var child in matchingChildren)
            {
                var access = child.HasAccess(newPath, variables, sets);
                if (access.HasValue)
                {
                    return access;
                }
            }
            
            return _permission;
        }

        public void AddRule(bool allow, string path)
        {
            if (path == "")
            {
                _permission = allow;
                return;
            }

            var parts = path.Split(new[]{'/'}, 2);
            var node = GetNode(parts[0]);
            if (_children.Contains(node))
            {
                node = _children[_children.IndexOf(node)];
            }
            else
            {
                _children.Add(node);
            }
            node.AddRule(allow, parts.Length > 1 ? parts[1] : "");
        }
    }
}