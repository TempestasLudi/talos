<?php

include "nodes.php";

class Authorizer {

	public $preconstruct;
	private $permissionRules = "";

	private $inheritance = [];
	private $permissions = [];

	function __construct($preconstruct = true) {
		$this->preconstruct = $preconstruct;
	}

	function hasAccess($role, $path, $variables, $sets) {
		if (!isset($this->permissions[$role])) {
			return false;
		}

		if (substr($path, 0, 1) === '/') {
			$path = substr($path, 1);
		}

		$hasAccess = $this->permissions[$role]->hasAccess($path, $variables, $sets);

		if ($hasAccess !== null) {
			return $hasAccess;
		}

		if (isset($this->inheritance[$role])) {
			return $this->hasAccess($this->inheritance[$role], $path, $variables, $sets);
		} else {
			return false;
		}
	}


	function loadPermissionsFile($filename) {
		$this->loadPermissions(file_get_contents($filename));
	}

	function loadPermissions($permissions) {
		$this->permissionRules .= PHP_EOL . $permissions;

		if ($this->preconstruct) {
			$this->parsePermissions();
		}
	}

	private function parsePermissions() {
		foreach (array_map(
								 function ($r) {
									 return array_map(
											 function ($word) {
												 return trim($word);
											 },
											 explode(" ", preg_replace("/[ ]{2,}/", " ", $r))
									 );
								 },
								 array_filter(
										 array_map(
												 function ($r) {
													 return trim($r);
												 },
												 explode("\n", $this->permissionRules)
										 ),
										 function ($r) {
											 return strlen($r) > 0 && substr($r, 0, 1) !== "#";
										 }
								 )
						 ) as $rule) {
			$this->checkRuleSyntax($rule);

			$ruleType = $this->getRuleType($rule);

			if ($ruleType === "Inheritance") {
				$this->setInheritance($rule);
			} else {
				$this->setPermission($rule);
			}
		}

		$this->permissionRules = "";
	}

	private function checkRuleSyntax($rule) {
		if (count($rule) != 3) {
			throw new MalformedRuleException("The rule \"" . join(", ", $rule) . "\" does not have three pieces.");
		}
	}

	private function getRuleType($rule) {
		$isInheritance = $rule[1] == ">";
		$isPermission = ($rule[0] === "allow" || $rule[0] === "deny");

		if ($isInheritance) {
			if ($isPermission) {
				throw new MalformedRuleException("The type of the rule \"" . join(", ", $rule) . "\" is ambiguous.");
			} else {
				return "Inheritance";
			}
		} else {
			if ($isPermission) {
				return "Permission";
			} else {
				throw new MalformedRuleException("The rule \"" . join(", ", $rule) . "\" is neither a inheritance rule nor a permission rule.");
			}
		}
	}

	private function setInheritance($rule) {
		$this->inheritance[$rule[2]] = $rule[0];
	}

	private function setPermission($rule) {
		if (!isset($this->permissions[$rule[1]])) {
			$this->permissions[$rule[1]] = new RootNode();
		}

		if (substr($rule[2], 0, 1) === "/") {
			$rule[2] = substr($rule[2], 1);
		}

		$this->permissions[$rule[1]]->addRule($rule[0] === "allow", $rule[2]);
	}

	function clearPermissions() {
		$this->permissionRules = "";
		$this->permissions = [];
		$this->inheritance = [];
	}
}

class MalformedRuleException extends Exception {}
