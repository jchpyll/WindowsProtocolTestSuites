:: Copyright (c) Microsoft. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

@echo off

echo ======================================
echo          Start to build MS-AZOD
echo ======================================

set CurrentPath=%~dp0
set TestSuiteRoot=%CurrentPath%..\..\..\

call "%CurrentPath%..\..\..\common\setBuildTool.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\setVsPath.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\checkWix.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\checkSpecExplorer.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\checkMMA.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\setPtfVer.cmd"
if ErrorLevel 1 (
	exit /b 1
)

call "%CurrentPath%..\..\..\common\setTestSuiteVer.cmd"
if ErrorLevel 1 (
	exit /b 1
)

set KeyFile=%1
if not defined KeyFile (
	%buildtool% "%TestSuiteRoot%TestSuites\MS-AZOD\src\MS-AZOD_OD.sln" /t:clean;rebuild /p:ProtocolName="MS-AZOD" /p:Configuration="Release"
) else (
	%buildtool% "%TestSuiteRoot%TestSuites\MS-AZOD\src\MS-AZOD_OD.sln" /t:clean;rebuild /p:AssemblyOriginatorKeyFile=%KeyFile% /p:DelaySign=true /p:SignAssembly=true /p:ProtocolName="MS-AZOD" /p:Configuration="Release"
)

if ErrorLevel 1 (
	echo Error: Failed to build MS-AZOD test suite
	exit /b 1
)

if exist "%TestSuiteRoot%drop\TestSuites\MS-AZOD" (
	rd /s /q "%TestSuiteRoot%drop\TestSuites\MS-AZOD"
)

%buildtool% "%TestSuiteRoot%TestSuites\MS-AZOD\src\deploy\deploy.wixproj" /t:Clean;Rebuild /p:ProtocolName="MS-AZOD" /p:Configuration="Release"

if ErrorLevel 1 (
	echo Error: Failed to generate the msi installer
	exit /b 1
)

echo ==========================================================
echo          Build MS-AZOD test suite successfully
echo ==========================================================
