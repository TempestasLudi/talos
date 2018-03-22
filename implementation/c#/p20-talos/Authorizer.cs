using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            data.Split('\n').Select(r => regex.Replace(r, " ").Split(new[] {' '}, 3).Select(p => p.Trim()).ToArray())
                .ToList().ForEach(r =>
                {
                    if (r.Length < 3)
                    {
                        return;
                    }

                    if (r[1] == ">")
                    {
                        _inheritance[r[2]] = r[0];
                    }

                    if (r[0] != "allow" && r[0] != "deny")
                    {
                        return;
                    }

                    if (!_permissions.ContainsKey(r[1]))
                    {
                        _permissions[r[1]] = new RootNode();
                    }

                    if (r[2][0] == '/')
                    {
                        r[2] = r[2].Substring(1);
                    }

                    _permissions[r[1]].AddRule(r[0] == "allow", r[2]);
                });
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
    }
}