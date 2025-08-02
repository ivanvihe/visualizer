$fxc = "fxc.exe"
Get-ChildItem Shaders *.ps |
ForEach-Object {
    & $fxc /T ps_3_0 /E main /Fo $_.FullName $_.FullName
}
