if ($args[0] -eq $null -or $args[1] -eq $null){

      Write-Host "Insufficient parameters"
      Write-Host "Usage:certaccess.ps1 thumbprint username"

      exit}

else
{
    $TP=$args[0]
    $uname=$args[1]
    Write-Host "Attempting to grant Read+Execute rights to $uname..."
    $keyname=(((gci cert:\LocalMachine\my | ? {$_.thumbprint -like $tp}).PrivateKey).CspKeyContainerInfo).UniqueKeyContainerName
    if ($keyname -eq $null)
    {
        Write-Host "The private key could not be found in the LocalMachine\My certificate store."
        exit 1;
    }
    else
    {
        $keypath = $env:ProgramData + "\Microsoft\Crypto\RSA\MachineKeys\"
        $fullpath=$keypath+$keyname
        icacls $fullpath /grant $uname`:RX
    }
}