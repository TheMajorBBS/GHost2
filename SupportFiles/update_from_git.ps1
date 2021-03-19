function DownloadFile($filename) {
    Write-Host "Downloading $($filename)"
	Invoke-WebRequest "https://github.com/rickparrish/GHost/raw/master/bin/Release/$($filename)" -OutFile $filename
}

if ($args.count -eq 0) {
    Write-Host "Downloading latest update_from_git.ps1 file"
    DownloadFile("update_from_git.ps1")
    Invoke-Expression -Command ".\update_from_git.ps1 DOWNLOAD"
} else {
    DownloadFile("GHost.dll")
    DownloadFile("GHostConfig.exe")
    DownloadFile("GHostConsole.exe")
    DownloadFile("GHostGUI.exe")
    DownloadFile("GHostService.exe")
    DownloadFile("RMLib.dll")
    DownloadFile("RMLibUI.dll")
    DownloadFile("Upgrade.exe")
    DownloadFile("W32Door.exe")
}