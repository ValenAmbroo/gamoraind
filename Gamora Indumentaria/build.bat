@echo off
echo Compilando Gamora Indumentaria...

REM Cambiar al directorio del proyecto
cd /d "c:\Users\ivanv\Desktop\gamoraind\Gamora Indumentaria"

REM Buscar el compilador de C# en diferentes ubicaciones
set CSC_PATH=""

REM Intentar encontrar el compilador en .NET Framework
if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" (
    set CSC_PATH="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
) else if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" (
    set CSC_PATH="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

if %CSC_PATH%=="" (
    echo No se encontro el compilador de C#
    echo Intentando usar MSBuild...
    msbuild "Gamora Indumentaria.csproj" /p:Configuration=Debug
) else (
    echo Usando compilador en %CSC_PATH%
    %CSC_PATH% /target:winexe /out:"bin\Debug\Gamora Indumentaria.exe" ^
        /reference:"System.Windows.Forms.dll" ^
        /reference:"System.Drawing.dll" ^
        /reference:"System.Data.dll" ^
        /reference:"System.dll" ^
        *.cs Data\*.cs
)

echo Compilacion completada.
pause
