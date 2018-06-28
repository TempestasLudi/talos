<?php
require_once "test/lib/TestResult.php";

class UnitTester {

	function test() {
		$this->setup();

		$baseClass = new ReflectionClass('UnitTester');
		$testClass = new ReflectionClass($this);

		$methods = array_filter($testClass->getMethods(), function ($method) use ($baseClass) {
			return !$baseClass->hasMethod($method->name);
		});

		$methodNames = array_map(function ($method) {
			return $method->name;
		}, $methods);

		$results = array_combine($methodNames, array_map(function ($method) {
			try {
				$this->{$method->name}();
			} catch (TestException $exception) {
				return new TestResult(false, $exception->getMessage());
			} catch (Exception $exception) {
				$reason = "An " . get_class($exception) . " was thrown, saying: \"" . $exception->getMessage() . "\".";
				return new TestResult(false, $reason);
			}
			return new TestResult();
		}, $methods));

		$this->cleanup();

		return $results;
	}

	function setup() {}

	function cleanup() {}

	function assertTrue($value) {
		$this->assertEquals(true, $value);
	}

	function assertFalse($value) {
		$this->assertEquals(false, $value);
	}

	function assertEquals($expected, $value, $delta = 0) {
		if (gettype($expected) !== gettype($value)) {
			throw new TestException("Expected " . $this->addIndefiniteArticle(gettype($expected)) . ", but got " . $this->addIndefiniteArticle(gettype($value)) . ". At " . $this->getCallingCode() . ".");
		}
		if ($expected !== $value) {
			if (is_numeric($expected)) {
				if (abs($expected - $value) <= $delta) {
					return;
				}
			}
			throw new TestException("Expected " . $expected . " but got " . $value . ". At " . $this->getCallingCode() . ".");
		}
	}

	function addIndefiniteArticle($word) {
		if (!array_search(substr($word, 0, 1), ['a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U'])) {
			return "a " . $word;
		}
		return "an " . $word;
	}

	function assertException($function, $type) {
		try {
			$function();
		} catch (Exception $e) {
			if (get_class($e) == $type || is_subclass_of($e, $type)) {
				return;
			}
			throw new TestException("Expected " . $this->addIndefiniteArticle($type) . ", but got " . $this->addIndefiniteArticle(get_class($e)) . ", saying: \"" . $e->getMessage() . "\". At " . $this->getCallingCode() . ".");
		}
		throw new TestException("Expected " . $this->addIndefiniteArticle($type) . ", but no exception was thrown. At " . $this->getCallingCode() . ".");
	}

	function getCallingCode() {
		$trace = debug_backtrace();

		$caller = null;

		do {
			$caller = array_shift($trace);
			echo PHP_EOL;
		} while ($trace[0]["class"] === "UnitTester");

		return "line " . $caller["line"];
	}

}

class TestException extends Exception {
}
