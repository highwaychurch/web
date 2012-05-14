function Usage
{
    Write-Host "Usage:"
    Write-Host ""
    Write-Host "    WarmUpWebApplication.ps1 <url>"
    Write-Host ""
    Write-Host "Where:"
    Write-Host "    <url> is the url to request in order to warm up the web site."
    Write-Host ""
    exit;
}

$url = $Args[0]

if($url -eq $null)
{
	Write-Host "Please specify a url"
	Usage;  
}

write-host $url;
$r = [System.Net.WebRequest]::Create($url);
[Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
$resp = $r.GetResponse()
$reqstream = $resp.GetResponseStream()
$sr = new-object System.IO.StreamReader $reqstream
$result = $sr.ReadToEnd()
write-host $result

exit;