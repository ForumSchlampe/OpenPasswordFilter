# Install Service
$GMSAAccount = Read-Host -Prompt "Input GMSA Accountname with Domain"
New-Service -BinaryPathName "C:\Program Files\OpenPasswordFilter\opfservice.exe" -Name OpenPasswordFilter -DependsOn Kdc -Description "Dieser Dienst dient dem Prüfen beim Passwortänderungen gegen bekannte Leaks oder ungewollte Zeichenkombinationen" -StartupType AutomaticDelayedStart 
# Yes it is necessary to start the Service once as "System"
Start-Service -Name "OpenPasswordFilter"
Stop-Service -Name "OpenPasswordFilter"
$service = gwmi win32_service -filter "name='OpenPasswordFilter'"
$service.change($null,$null,$null,$null,$null,$null,$GMSAAccount,$null)
restart-Service -Name "OpenPasswordFilter"


# Install DLL Service
$registryPath = "HKLM:\Software\OpenPasswordFilter"
New-Item -Path $registryPath -Force | Out-Null
New-ItemProperty -Path $registryPath -Name ServiceIP -Value "127.0.0.1" -PropertyType STRING -Force | Out-Null
New-ItemProperty -Path $registryPath -Name ServicePort -Value "5999" -PropertyType STRING -Force | Out-Null
$key = "HKLM:\System\CurrentControlSet\Control\LSA"
$name = 'Notification Packages'  
[string[]]$add = 'OpenPasswordFilter'  
[string[]]$values = (Get-ItemProperty $key -Name $name).$name + $add
New-ItemProperty $key -Name $name -Value $values -PropertyType MultiString -Force