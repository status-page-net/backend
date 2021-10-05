$DownloadsPath = (New-Object -ComObject Shell.Application).NameSpace('shell:Downloads').Self.Path

# MongoDB Server

$MongoVersion = '5.0.3'
$MongoMsiFile = 'mongodb-windows-x86_64-' + $MongoVersion + '-signed.msi'
$MongoMsiPath = $DownloadsPath + '\' + $MongoMsiFile
$MongoMsiUrl = 'https://fastdl.mongodb.org/windows/' + $MongoMsiFile

if (-not(Test-Path -Path $MongoMsiPath -PathType Leaf)) {
	Invoke-WebRequest $MongoMsiUrl -OutFile $MongoMsiPath
}

# https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows-unattended/
Start-Process `
	-FilePath 'msiexec.exe' `
	-ArgumentList '/qb','/i',$MongoMsiPath,'ADDLOCAL="ServerService"','SHOULD_INSTALL_COMPASS="0"' `
	-Wait