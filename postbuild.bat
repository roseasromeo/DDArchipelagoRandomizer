@echo off
setlocal enabledelayedexpansion

if not exist "%PROJECT_DIR%config.txt" (
    echo WARNING: 'config.txt' not found. Consider adding one with a 'PLUGIN_PATH' variable to the plugin directory to have build files be automatically copied to there.
) else (
    for /f "tokens=2 delims==" %%A in ('findstr /b "PLUGIN_PATH=" "%PROJECT_DIR%config.txt"') do (
        set PLUGIN_PATH=%%A
    )

    if not defined PLUGIN_PATH (
        echo WARNING: No 'PLUGIN_PATH' defined in 'config.txt' Files will not be copied.
    ) else (
        if not exist "!PLUGIN_PATH!" (
            echo WARNING: The specified 'PLUGIN_PATH' does not exist: '!PLUGIN_PATH!'. Create it to allow for automatic copying of build files.
        ) else (
            set OUTPUT_DIR=%1

            if not exist "!OUTPUT_DIR!" (
                echo WARNING: The output directory does not exist: '!OUTPUT_DIR!'
            ) else (
                xcopy /Y /E /H "!OUTPUT_DIR!*.*" "!PLUGIN_PATH!\"
            )
        )
    )
)

exit /b 0