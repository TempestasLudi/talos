using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using p20_talos.tree;

namespace p20_talos
{
    public class Authorizer
    {
        private readonly Dictionary<string, Node> _permissions = new Dictionary<string, Node>();

        private readonly Dictionary<string, string> _inheritance = new Dictionary<string, string>();

        public bool HasAccess(string role, string resource, Dictionary<string, string> variables,
            Dictionary<string, HashSet<string>> sets)
        {
            if (!_permissions.ContainsKey(role))
            {
                return false;
            }
            
            if (resource[0] == '/')
            {
                resource = resource.Substring(1);
            }

            bool? hasAccess = _permissions[role].HasAccess(resource, variables, sets) ??
                              _inheritance.ContainsKey(role) &&
                              HasAccess(_inheritance[role], resource, variables, sets);

            return hasAccess.Value;
        }

        public void LoadPermissions(string data)
        {
            var doubleSpacesRegex = new Regex("[ ]{2,}", RegexOptions.None);
            data.Split('\n')
                .Select(r => r.Trim())
                .Where(r => r.Length > 0 && r[0] != '#')
                .Select(r => doubleSpacesRegex.Replace(r, " ").Split(' ').Select(p => p.Trim()).ToArray())
                .ToList().ForEach(r =>
                {
                    CheckRuleSyntaxis(r);
                    
                    var ruleType = GetRuleType(r);

                    if (ruleType == RuleType.Inheritance)
                    {
                        SetInheritance(r);
                    }
                    else
                    {
                        SetPermission(r);
                    }
                });
        }

        private static void CheckRuleSyntaxis(string[] rule)
        {
            if (rule.Length != 3)
            {
                throw new ParseException("Malformed rule '" + string.Join(" ", rule) + "': a rule should have three parts.");
            }
        }

        private static RuleType GetRuleType(IReadOnlyList<string> rule)
        {
            var isInheritance = rule[1] == ">";
            var isPermission = (rule[0] == "allow" || rule[0] == "deny");

            if (isInheritance)
            {
                if (isPermission)
                {
                    throw new ParseException(
                        "Ambiguous rule: a rule cannot be both a permission rule and an inheritance rule.");
                }

                return RuleType.Inheritance;
            }

            if (isPermission)
            {
                return RuleType.Permission;
            }

            throw new ParseException(
                "Malformed rule: a rule should either be a permission rule or a inheritance rule.");
        }

        private void SetInheritance(IReadOnlyList<string> rule)
        {
            _inheritance[rule[2]] = rule[0];
        }

        private void SetPermission(IReadOnlyList<string> rule)
        {
            if (!_permissions.ContainsKey(rule[1]))
            {
                _permissions[rule[1]] = new RootNode();
            }

            var path = rule[2].Substring(rule[2][0] == '/' ? 1 : 0);

            _permissions[rule[1]].AddRule(rule[0] == "allow", path);
        }

        public void LoadPermissionsFile(string filename)
        {
            LoadPermissions(File.ReadAllText(filename));
        }

        public void ClearPermissions()
        {
            _inheritance.Clear();
            _permissions.Clear();
        }

        private enum RuleType
        {
            Inheritance, Permission
        }
    }
}