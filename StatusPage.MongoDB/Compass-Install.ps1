$DownloadsPath = (New-Object -ComObject Shell.Application).NameSpace('shell:Downloads').Self.Path

# Compass

$CompassVersion = '1.28.4'
$CompassMsiFile = 'mongodb-compass-' + $CompassVersion + '-win32-x64.msi'
$CompassMsiPath = $DownloadsPath + '\' + $CompassMsiFile
$CompassMsiUrl = 'https://downloads.mongodb.com/compass/' + $CompassMsiFile

if (-not(Test-Path -Path $CompassMsiPath -PathType Leaf)) {
	Invoke-WebRequest $CompassMsiUrl -OutFile $CompassMsiPath
}

Start-Process `
	-FilePath 'msiexec.exe' `
	-ArgumentList '/qb','/i',$CompassMsiPath `
	-Wait