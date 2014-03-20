 param (
    [string]$build
 )

$launcherExe = "../RXL.WPFClient/bin/Release/RXL.exe"
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($launcherExe).FileVersion
$version = $version.Remove($version.LastIndexOf('.'));
$version = $version.Remove($version.LastIndexOf('.'));

if($build) {
  $version = $version + "." + $build	
}

"Building installer, setting version to " + $version + "."

Remove-Item -Force -Recurse "AnyCPU_ReleaseSetupFiles" -ea SilentlyContinue
Remove-Item -Force -Recurse "out" -ea SilentlyContinue
Remove-Item -Force -Recurse "setup-cache" -ea SilentlyContinue

$advInstallerExe = "C:/Program Files (x86)/Caphyon/Advanced Installer 11.0/bin/x86/advinst.exe"
Start-Process -NoNewWindow -Wait $advInstallerExe ("/edit setup.aip /SetVersion " + $version)
Start-Process -NoNewWindow -Wait $advInstallerExe ("/edit setup.aip /SetProductCode -langid 1033")
Start-Process -NoNewWindow -Wait $advInstallerExe "/rebuild setup.aip"