function Usage
{
    Write-Host "Usage:"
    Write-Host ""
    Write-Host "    CreateQueue.ps1 <queuename> [public] [username] [all:restrictedPermission] [nonTransactional]"
    Write-Host ""
    Write-Host "Where:"
    Write-Host "    <queuename> is the name of the queue to create, required."
    Write-Host "    [public] specify the optional parameter public to create a public queue, default is private"
    Write-Host "    [username] optionally specify the username to grant permissions to, default is everyone"
    Write-Host "    [all:restricted] optionally specify the permission type to apply to the user, default is restricted"
    Write-Host "    [nonTransactional] optional specify the nonTransactional paramter to create a non-transactional queue, default is transactional"
    Write-Host ""
    exit;
}

$queuename = $Args[0]
$private = $Args[1]
$user = $Args[2]
$permission = $Args[3]
$pTransactional = $Args[4]

if($queuename -eq $null)
{
	Write-Host "Please specify a queue name"
	Usage;  
}

if ($private -ieq "public")
{
}
else
{
	$private = "private"
	$queuename = ".\private$\" + $Args[0]
}

if ($pTransactional -ieq "nonTransactional")
{
    $transactional = 0
}
else
{
	$pTransactional = "transactional"
    $transactional = 1
}

if ($user -eq $null)
{
	$user = "Everyone"
}

Add-Type -AssemblyName System.Messaging

if ([System.Messaging.MessageQueue]::Exists($queuename) -eq 1)
{
	Write-Host "Queue called $queuename already exists. Skipping create..."
}
else
{
	Write-Host "Attempting to create $private $pTransactional queue called $queuename"
	$qb = [System.Messaging.MessageQueue]::Create($queuename, $transactional) 

	if($qb -eq $null)
	{
		Write-Host "ERROR: The MessageQueue.Create() method did not return a MessageQueue instance."
		exit
	}
}

$qb =  new-object System.Messaging.MessageQueue($queuename)

Write-Host "Granting FullControl permissions to Local Administrators"
$qb.SetPermissions("Administrators", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow) 

Write-Host "Granting FullControl permissions to user $user"
$qb.SetPermissions($user, [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow) 
$qb.label = $queuename

exit