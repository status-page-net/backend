$FuncExe = $env:LOCALAPPDATA + "\AzureFunctionsTools\Releases\3.23.5\cli_x64\func.exe"
$FuncRoot = (Get-Location).Path + "\out\Debug"

Start-Process `
    -FilePath $FuncExe `
    -ArgumentList "host","start","--csharp" `
    -WorkingDirectory $FuncRoot `
    -NoNewWindow `
    -Wait