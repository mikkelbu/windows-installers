$currentDir = Split-Path -parent $MyInvocation.MyCommand.Path
Set-Location $currentDir

# mapped sync folder for common scripts
. $currentDir\..\common\Utils.ps1
. $currentDir\..\common\CommonTests.ps1

$InstallDir = "C:\temp dir\Elasticsearch\"
$DataDir = "C:\foo\data"
$ConfigDir = "C:\bar\config"
$LogsDir = "C:\baz\logs"

Describe "Silent Install with different install locations" {

    $InstallLocations = "INSTALLDIR=$InstallDir","DATADIRECTORY=$DataDir","CONFIGDIRECTORY=$ConfigDir","LOGSDIRECTORY=$LogsDir"

    Invoke-SilentInstall -ExeArgs $InstallLocations

    Context-ElasticsearchService

    Context-PingNode -XPackSecurityInstalled $false

    Context-EsHomeEnvironmentVariable -Expected $InstallDir

    Context-EsConfigEnvironmentVariable -Expected $ConfigDir

    Context-PluginsInstalled

    Context-MsiRegistered

    Context-ServiceRunningUnderAccount -Expected "LocalSystem"

    Context-EmptyEventLog
  
    Context-ClusterNameAndNodeName

    Context-ElasticsearchConfiguration -Expected @{Data = $DataDir; Logs = $LogsDir }

    Context-JvmOptions
}

Describe "Silent Uninstall with different install locations" {

    Invoke-SilentUninstall

	Context-NodeNotRunning

	Context-EnvironmentVariableNull -Name "CONF_DIR"

	Context-EnvironmentVariableNull -Name "ES_HOME"

	Context-MsiNotRegistered

	Context-ElasticsearchServiceNotInstalled

	Context-EmptyInstallDirectory -Path $InstallDir
}