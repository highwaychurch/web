Param(
	$server = 'MEDIACENTER',
    $serverAndAuth = ',wmsvc=$server,authType=basic,username=IISWebDeploy,password=ten4c1ty!',
	$buildConfig = "Demo",
	$baseDirectory = "C:\Angel\trunk"
)


$remoteBatFileName = "Deploy$($buildConfig).bat"
$remoteDestination = "C:\Highway"
$remoteDeployScript = "$($remoteDestination)\DeploymentScripts\Remote\$remoteBatFileName"
$msDeployCmd = 'C:\Program Files\IIS\Microsoft Web Deploy V2\msdeploy.exe'

Write-Output "Copying the DeploymentPackages to $server `n"
$outputString1 = & $msDeployCmd -verb:sync -source:dirPath=$baseDirectory\DeploymentPackages "-dest:dirPath=$remoteDestination\DeploymentPackages$serverAndAuth"
Write-Output $outputString1

Write-Output "Copying the DeploymentScripts to $server `n"
$outputString1 = & $msDeployCmd -verb:sync -source:dirPath=$baseDirectory\DeploymentScripts "-dest:dirPath=$remoteDestination\DeploymentScripts$serverAndAuth"
Write-Output $outputString1


Write-Output "Execute a bat file remotely on $server `n"
$msDeployDestCmd = '-dest:runCommand="' + $remoteDeployScript + '"' + $serverAndAuth + ',waitinterval=60000,waitAttempts=30' # that's a 30 attemps with 1 minute intervals
$outputString2 = & $msDeployCmd -verb:sync -source:runCommand $msDeployDestCmd 
Write-Output $outputString2 