Remove-Item ..\QuickLook.Plugin.HelloWorld.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\bin\Debug\ -Exclude *.xml
Compress-Archive $files ..\QuickLook.Plugin.Slideshow.zip
Move-Item ..\QuickLook.Plugin.Slideshow.zip ..\QuickLook.Plugin.Slideshow.qlplugin