﻿$openCoverPath = Join-Path $PSScriptRoot "src\packages\OpenCover.4.6.166\tools\OpenCover.Console.exe"
$coverallsPath = Join-Path $PSScriptRoot "src\packages\coveralls.io.1.3.4\tools\coveralls.net.exe"
$reportGeneratorPath = Join-Path $PSScriptRoot "src\packages\ReportGenerator.2.4.0.0\tools\ReportGenerator.exe"
$testDllPath = Join-Path $PSScriptRoot "src\stashbox.decoratorextension.tests\bin\release\Stashbox.DecoratorExtension.Tests.dll"
$vsTestPath = "c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$arguments = "-register:user", "`"-filter:+[*]* -[Stashbox.DecoratorExtension.Tests]*`"", "-target:$vsTestPath", "`"-targetargs:$testDllPath`"", "-output:coverage.xml"
. $openCoverPath $arguments
. $coverallsPath --opencover coverage.xml
. $reportGeneratorPath -verbosity:Info -reports:coverage.xml -targetdir:$coverageReportDir "-assemblyfilters:-Stashbox.DecoratorExtension.Tests*"