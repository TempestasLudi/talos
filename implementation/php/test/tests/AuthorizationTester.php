<?php

require_once "test/lib/UnitTester.php";
require_once "src/Authorizer.php";

class AuthorizationTester extends UnitTester {

	function setup() {
		chdir($_SERVER["DOCUMENT_ROOT"] . "/test");
	}

	function cleanup() {
		chdir($_SERVER["DOCUMENT_ROOT"]);
	}

	function emptyTest() {
		$authorizer = new Authorizer(true);

		$this->assertFalse($authorizer->hasAccess("Junior", "/hello", [], []));
	}

	function commentTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/comment.talos");

		$this->assertFalse($authorizer->hasAccess("Junior", "/hello", [], []));
	}

	function commonTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/common.talos");

		$this->assertTrue($authorizer->hasAccess("Admin", "/", [], []));
		$this->assertTrue($authorizer->hasAccess("Admin", "/home/5", [], []));
		$this->assertTrue($authorizer->hasAccess("Admin", "/tmp/srv/thing", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets/things", [], []));
	}

	function clear1Test() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/common.talos");
		$authorizer->clearPermissions();

		$this->assertFalse($authorizer->hasAccess("Admin", "/", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/tmp/srv/thing", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets/things", [], []));
	}

	function clear2Test() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/inheritance1.talos");
		$authorizer->clearPermissions();
		$authorizer->loadPermissionsFile("test-files/normal/common.talos");

		$this->assertTrue($authorizer->hasAccess("Admin", "/", [], []));
		$this->assertTrue($authorizer->hasAccess("Admin", "/home/5", [], []));
		$this->assertTrue($authorizer->hasAccess("Admin", "/tmp/srv/thing", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets", [], []));
		$this->assertFalse($authorizer->hasAccess("Admin", "/home/5/personalsecrets/things", [], []));
	}

	function inheritance1Test() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/inheritance1.talos");

		$this->assertTrue($authorizer->hasAccess("A", "x", [], []));
		$this->assertTrue($authorizer->hasAccess("A", "x/y", [], []));
		$this->assertTrue($authorizer->hasAccess("A", "x/z/g", [], []));
		$this->assertTrue($authorizer->hasAccess("B", "x", [], []));
		$this->assertFalse($authorizer->hasAccess("B", "x/y", [], []));
		$this->assertTrue($authorizer->hasAccess("B", "x/z/g", [], []));
	}

	function inheritance2Test() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/inheritance2.talos");

		$this->assertTrue($authorizer->hasAccess("A", "x", [], []));
		$this->assertFalse($authorizer->hasAccess("A", "x/y", [], []));
		$this->assertFalse($authorizer->hasAccess("A", "x/z", [], []));
		$this->assertFalse($authorizer->hasAccess("B", "x/z", [], []));
		$this->assertTrue($authorizer->hasAccess("C", "x/y", [], []));
		$this->assertTrue($authorizer->hasAccess("C", "x/z", [], []));
		$this->assertFalse($authorizer->hasAccess("C", "x/u", [], []));
	}

	function variablesTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/variables.talos");
		$variables = ["sesid" => "15"];

		$this->assertFalse($authorizer->hasAccess("User", "session", $variables, []));
		$this->assertFalse($authorizer->hasAccess("User", "session/5517", $variables, []));
		$this->assertTrue($authorizer->hasAccess("User", "session/15", $variables, []));

		$variables = ["sesid" => "20"];

		$this->assertTrue($authorizer->hasAccess("User", "session/20", $variables, []));
	}

	function setsTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/sets.talos");
		$sets = [
				"ownedDevices" => ["1", "2", "3"],
				"public" => ["4", "5"],
				"allowedDevices" => ["6", "17"],
		];

		$this->assertFalse($authorizer->hasAccess("User", "devices", [], $sets));
		$this->assertTrue($authorizer->hasAccess("User", "devices/3", [], $sets));
		$this->assertFalse($authorizer->hasAccess("User", "devices/5", [], $sets));
		$this->assertTrue($authorizer->hasAccess("User", "devices/5/control", [], $sets));
		$this->assertFalse($authorizer->hasAccess("User", "devices/17/", [], $sets));
		$this->assertTrue($authorizer->hasAccess("User", "devices/17/control", [], $sets));
		$this->assertFalse($authorizer->hasAccess("User", "devices/7/control", [], $sets));
		$this->assertTrue($authorizer->hasAccess("Admin", "devices/7", [], $sets));
		$this->assertTrue($authorizer->hasAccess("Admin", "devices/7/control", [], $sets));
	}

	function permissionConflictTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/sets.talos");

		$this->assertFalse($authorizer->hasAccess("A", "x/y/z", [], []));
	}

	function missingVariableTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/missing-variable.talos");

		$this->assertException(function () use ($authorizer) {$authorizer->hasAccess("A", "/a", [], []);}, "MissingValueException");
	}

	function missingSetTest() {
		$authorizer = new Authorizer(true);
		$authorizer->loadPermissionsFile("test-files/normal/missing-set.talos");

		$this->assertException(function () use ($authorizer) {$authorizer->hasAccess("A", "/a", [], []);}, "MissingValueException");
	}

	function malformedInheritance1Test() {
		$authorizer = new Authorizer(true);

		$this->assertException(function () use ($authorizer) {
			$authorizer->loadPermissionsFile("test-files/malformed/inheritance1.talos");
		}, "MalformedRuleException");
	}

	function malformedInheritance2Test() {
		$authorizer = new Authorizer(true);

		$this->assertException(function () use ($authorizer) {
			$authorizer->loadPermissionsFile("test-files/malformed/inheritance2.talos");
		}, "MalformedRuleException");
	}

	function ambiguousRuleTest() {
		$authorizer = new Authorizer(true);

		$this->assertException(function () use ($authorizer) {
			$authorizer->loadPermissionsFile("test-files/malformed/ambiguous.talos");
		}, "MalformedRuleException");
	}

	function malformedPermissionTest() {
		$authorizer = new Authorizer(true);

		$this->assertException(function () use ($authorizer) {
			$authorizer->loadPermissionsFile("test-files/malformed/permission.talos");
		}, "MalformedRuleException");
	}

}