:: Copyright (c) Microsoft. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

@echo off

:: Find vs2017path in registry
set REGEXE="%windir%\System32\reg.exe"
set KEY_NAME_32="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\SxS\VS7"
set KEY_NAME_64="HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7"
set VALUE_NAME=15.0
set REG_QUERY_VS2017_PATH_32BIT=%REGEXE% QUERY %KEY_NAME_32% /v %VALUE_NAME%
set REG_QUERY_VS2017_PATH_64BIT=%REGEXE% QUERY %KEY_NAME_64% /v %VALUE_NAME%

set vs2017path=

%REG_QUERY_VS2017_PATH_64BIT%
if ErrorLevel 1 (
    :: Cannot find 64-bit path, try to find 32-bit path
    %REG_QUERY_VS2017_PATH_32BIT%
    if ErrorLevel 1 (
        echo Visual Studio 2017 is not installed.
        exit /b 0
    ) else (
        FOR /F "usebackq tokens=1-2*" %%A IN (`"%REG_QUERY_VS2017_PATH_32BIT%"`) DO (
            set name=%%A
            set type=%%B
            set vs2017path=%%C
        )
    )
) else (
    FOR /F "usebackq tokens=1-2*" %%A IN (`"%REG_QUERY_VS2017_PATH_64BIT%"`) DO (
        set name=%%A
        set type=%%B
        set vs2017path=%%C
    )
)
