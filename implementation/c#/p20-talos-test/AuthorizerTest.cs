using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using p20_talos;

namespace p20_talos_test
{
    [TestFixture]
    public class AuthorizerTest
    {
        [Test]
        public void TestEmpty()
        {
            var authorizer = new Authorizer();
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();
            Assert.IsFalse(authorizer.HasAccess("Junior", "/hello", variables, sets));
        }

        [Test]
        public void TestComment()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/comment.talos");
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();
            Assert.IsFalse(authorizer.HasAccess("Junior", "/hello", variables, sets));
        }
        
        [Test]
        public void TestCommon1()
        {
            var authorizer = new Authorizer();
            Console.WriteLine(Directory.GetCurrentDirectory());
            authorizer.LoadPermissionsFile("test-files/normal/common.talos");
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsTrue(authorizer.HasAccess("Admin", "/", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "/home/5", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "/tmp/srv/thing", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("Admin", "/home/5/personalsecrets", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("Admin", "/home/5/personalsecrets/things", variables, sets));
        }

        [Test]
        public void TestInheritance1()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/inheritance1.talos");
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsTrue(authorizer.HasAccess("A", "x", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("A", "x/y", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("A", "x/z/g/", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("B", "x", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("B", "x/y", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("B", "x/z/g/", variables, sets));
        }
        
        [Test]
        public void TestInheritance2()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/inheritance2.talos");
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsTrue(authorizer.HasAccess("A", "x", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("A", "x/y", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("A", "x/z", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("B", "x/z", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("C", "x/y", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("C", "x/z", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("C", "x/u", variables, sets));
        }

        [Test]
        public void TestVariables()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/variables.talos");

            var variables = new Dictionary<string, string> {{"sesid", "15"}};
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsFalse(authorizer.HasAccess("User", "session", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("User", "session/5517", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "session/15", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "session/20", new Dictionary<string, string> {{"sesid", "20"}},
                sets));
        }

        [Test]
        public void TestSets()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/sets.talos");

            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>
            {
                {"ownedDevices", new HashSet<string> {"1", "2", "3"}},
                {"public", new HashSet<string> {"4", "5"}},
                {"allowedDevices", new HashSet<string> {"6", "17"}}
            };

            Assert.IsFalse(authorizer.HasAccess("User", "devices", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "devices/3", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("User", "devices/5", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "devices/5/control", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("User", "devices/17/", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "devices/17/control", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("User", "devices/7/control", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "devices/7", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "devices/7/control", variables, sets));
        }

        [Test]
        public void TestPermissionConflict()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/permission-conflict.talos");
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsFalse(authorizer.HasAccess("A", "x/y/z", variables, sets));
        }

        [Test]
        public void TestMissingVariable()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/missing-variable.talos");
            Assert.Catch<AuthorizationException>(() => authorizer.HasAccess("A", "/a", new Dictionary<string, string>(), new Dictionary<string, HashSet<string>>()));
        }

        [Test]
        public void TestMissingSet()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissionsFile("test-files/normal/missing-set.talos");
            Assert.Catch<AuthorizationException>(() => authorizer.HasAccess("A", "/a", new Dictionary<string, string>(), new Dictionary<string, HashSet<string>>()));
        }

        [Test]
        public void TestMalformedInheritance1()
        {
            var authorizer = new Authorizer();
            Assert.Catch<ParseException>(() => authorizer.LoadPermissionsFile("test-files/malformed/inheritance1.talos"));
        }

        [Test]
        public void TestMalformedInheritance2()
        {
            var authorizer = new Authorizer();
            Assert.Catch<ParseException>(() => authorizer.LoadPermissionsFile("test-files/malformed/inheritance1.talos"));
        }

        [Test]
        public void TestAmbiguousRule()
        {
            var authorizer = new Authorizer();
            Assert.Catch<ParseException>(() => authorizer.LoadPermissionsFile("test-files/malformed/ambiguous.talos"));
        }

        [Test]
        public void TestMalformedPermission()
        {
            var authorizer = new Authorizer();
            Assert.Catch<ParseException>(() => authorizer.LoadPermissionsFile("test-files/malformed/permission.talos"));
        }
    }
}