<?php
set_include_path(get_include_path() . PATH_SEPARATOR . $_SERVER["DOCUMENT_ROOT"]);

require_once "test/tests/AuthorizationTester.php";

$testers = [
		new AuthorizationTester(),
];

$testerNames = array_map(function($tester) {return get_class($tester);}, $testers);

$testResults = array_combine($testerNames, array_map(function ($tester) {
	$results = $tester->test();
	uasort($results, function($r1, $r2) {return $r1->success - $r2->success;});
	return $results;
}, $testers));

$testCounts = array_map(function($results) {return count($results);}, $testResults);
$passedTestCounts = array_map(function($results) {
	return count(array_filter($results, function($result) {return $result->success;}));
}, $testResults);

$testerCount = count($testers);
$testCount = array_sum($testCounts);
$passedTestCount = array_sum($passedTestCounts);

$resultDisplay = join(PHP_EOL, array_map(function($results, $class) use ($testCounts, $passedTestCounts) {
	return "<h3>" . $class . "</h3>" . PHP_EOL
			. "<p>" . $passedTestCounts[$class] . "/" . $testCounts[$class] . " tests passed.</p>" . PHP_EOL
			. "<table><thead><tr><th></th><th>Name</th><th>Reason</th></tr></thead>" . PHP_EOL
			. join(PHP_EOL, array_map(function($result, $name) {
				return "<tr>" .
						"<td class=\"" . ($result->success ? "succeeded" : "failed") . "-test\">" . ($result->success ? "✔️" : "✘") . "</td>" .
						"<td>" . $name . "</td>" .
						"<td>" . $result->reason . "</td>" .
				"</tr>";
			}, $results, array_keys($results))) . PHP_EOL
			. "</table>";
}, $testResults, $testerNames)) . PHP_EOL;

?>
<html>
	<head>
		<title>Talos Unit Tests</title>
		<link rel="stylesheet" href="frontend/fonts.css" />
		<link rel="stylesheet" href="frontend/style.css" />
	</head>
	<body>
		<main>
			<h1>Test results</h1>
			<h2>Summary</h2>
			<p>
				<?php echo $testCount; ?> test<?php if($testCount != 1) {echo "s";} ?>
				across <?php echo $testerCount; ?> class<?php if($testerCount != 1) {echo "es";} ?> were executed,
				of which <?php echo $passedTestCount; ?> passed
				and <?php echo $testCount - $passedTestCount; ?> failed.
			</p>
			<h2>Individual tests</h2>
			<?php
				echo $resultDisplay;
			?>
		</main>
	</body>
</html>
