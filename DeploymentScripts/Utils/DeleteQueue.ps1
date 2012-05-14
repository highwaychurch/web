function Usage
{
    Write-Host "Usage:"
    Write-Host ""
    Write-Host "    DeleteQueue.ps1 <queuename>"
    Write-Host ""
    Write-Host "Where:"
    Write-Host "    <queuename> is the name of the queue to delete, required."
    Write-Host ""
    exit;
}

$queuename = $Args[0]

if($queuename -eq $null)
{
	Write-Host "Please specify a queue name"
	Usage;  
}
else
{
    $queuename = ".\private$\" + $Args[0]
}

Add-Type -AssemblyName System.Messaging

if ([System.Messaging.MessageQueue]::Exists($queuename) -eq 0)
{
	Write-Host "Queue called $queuename does not exist. Skipping delete..."
}
else
{
	Write-Host "Attempting to delete queue called $queuename"
    $qb =  new-object System.Messaging.MessageQueue($queuename)

    Write-Host "Granting FullControl permissions to Everyone just in case"
    $qb.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow) 

	Write-Host "Deleting queue"
	[System.Messaging.MessageQueue]::Delete($queuename) 
}

exit