<?php

class TestResult {
	public $success;
	public $reason;

	function __construct($success = true, $reason = null)
	{
		$this->success = $success;
		$this->reason = $reason;
	}
}

