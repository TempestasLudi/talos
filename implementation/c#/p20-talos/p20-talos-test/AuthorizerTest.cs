using System.Collections.Generic;
using NUnit.Framework;
using p20_talos;

namespace p20_talos_test
{
    [TestFixture]
    public class AuthorizerTest
    {
        [Test]
        public void Test1()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"
allow Admin   /
deny  Admin   /home/*/personalsecrets"
            );
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsTrue(authorizer.HasAccess("Admin", "/", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "/home/5", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("Admin", "/tmp/srv/thing", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("Admin", "/home/5/personalsecrets", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("Admin", "/home/5/personalsecrets/things", variables, sets));
        }

        [Test]
        public void Test2()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"A > B
B > C

allow A x
deny  A x/*
allow B x/y
allow C x/z");
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
        public void Test3()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"A > B

allow A x
deny  B x/y");
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
        public void Test4()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"deny User session
allow User session/[sesid]");

            var variables = new Dictionary<string, string> {{"sesid", "15"}};
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsFalse(authorizer.HasAccess("User", "session", variables, sets));
            Assert.IsFalse(authorizer.HasAccess("User", "session/5517", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "session/15", variables, sets));
            Assert.IsTrue(authorizer.HasAccess("User", "session/20", new Dictionary<string, string> {{"sesid", "20"}},
                sets));
        }

        [Test]
        public void Test5()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"User > Admin

deny  User  devices/*
allow User  devices/{ownedDevices}
allow User  devices/{public}/control
allow User  devices/{allowedDevices}/control

allow Admin devices");

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
        public void Test6()
        {
            var authorizer = new Authorizer();
            authorizer.LoadPermissions(
                @"allow A x/*/z
deny  A x/y/*"
            );
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();

            Assert.IsFalse(authorizer.HasAccess("A", "x/y/z", variables, sets));
        }

        [Test]
        public void TestEmpty()
        {
            var authorizer = new Authorizer();
            var variables = new Dictionary<string, string>();
            var sets = new Dictionary<string, HashSet<string>>();
            Assert.IsFalse(authorizer.HasAccess("Junior", "/hello", variables, sets));
        }
    }
}