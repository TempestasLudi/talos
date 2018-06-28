<?php

abstract class Node {

	private $children = [];

	private $permission = null;

	protected abstract function matches($word, $variables, $sets);

	private static function getNode($expression) {
		if ($expression === "*") {
			return new UniversalNode();
		}

		if (strlen($expression) < 2) {
			return new LiteralNode($expression);
		}

		if (substr($expression, 0, 1) === '[' && substr($expression, -1) === ']') {
			return new VariableNode(substr($expression, 1, strlen($expression) - 2));
		}

		if (substr($expression, 0, 1) === '{' && substr($expression, -1) === '}') {
			return new SetNode(substr($expression, 1, strlen($expression) - 2));
		}

		return new LiteralNode($expression);
	}

	public function hasAccess($path, $variables, $sets) {
		if ($path === "") {
			return $this->permission;
		}

		$typeValues = [
				"LiteralNode" => 0,
				"VariableNode" => 1,
				"SetNode" => 2,
				"UniversalNode" => 3,
		];

		$parts = explode('/', $path, 2);
		$newPath = count($parts) > 1 ? $parts[1] : "";

		$matchingChildren = array_filter($this->children, function ($child) use ($parts, $variables, $sets) {
			return $child->matches($parts[0], $variables, $sets);
		});

		usort($matchingChildren, function ($child1, $child2) use ($typeValues) {
			return $typeValues[get_class($child1)] - $typeValues[get_class($child2)];
		});

		foreach ($matchingChildren as $child) {
			$access = $child->hasAccess($newPath, $variables, $sets);
			if ($access !== null) {
				return $access;
			}
		}

		return $this->permission;
	}

	public function addRule($allow, $path) {
		if ($path === "") {
			$this->permission = $allow;
			return;
		}

		$parts = explode('/', $path, 2);
		$node = self::getNode($parts[0]);

		if (($existingNode = $this->getExistingNode($node)) !== null) {
			$node = $existingNode;
		} else {
			$this->children[] = $node;
		}

		$node->addRule($allow, count($parts) > 1 ? $parts[1] : "");
	}

	public function getExistingNode($node) {
		$candidates = array_filter($this->children, function ($child) use ($node) {
			return get_class($child) === get_class($node) && (!property_exists($node, "name") || $child->name === $node->name);
		});
		return count($candidates) > 0 ? $candidates[0] : null;
	}

}

class MissingValueException extends Exception {
}

class RootNode extends Node {

	protected function matches($word, $variables, $sets) {
		return true;
	}

}

class LiteralNode extends Node {

	public $name;

	public function __construct($name) {
		$this->name = $name;
	}

	protected function matches($word, $variables, $sets) {
		return $this->name === $word;
	}

}

class VariableNode extends Node {

	public $name;

	public function __construct($name) {
		$this->name = $name;
	}

	protected function matches($word, $variables, $sets) {
		if (!isset($variables[$this->name])) {
			throw new MissingValueException("No variable named " . $this->name . " exists.");
		}
		return $variables[$this->name] === $word;
	}

}

class SetNode extends Node {

	public $name;

	public function __construct($name) {
		$this->name = $name;
	}

	protected function matches($word, $variables, $sets) {
		if (!isset($sets[$this->name])) {
			throw new MissingValueException("No set named " . $this->name . " exists.");
		}
		return !!array_search($word, $sets[$this->name]);
	}

}

class UniversalNode extends Node {

	protected function matches($word, $variables, $sets) {
		return true;
	}

}
