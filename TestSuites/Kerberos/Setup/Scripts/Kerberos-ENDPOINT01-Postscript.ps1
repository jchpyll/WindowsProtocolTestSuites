###########################################################################################
## Copyright (c) Microsoft Corporation. All rights reserved.
## Licensed under the MIT license. See LICENSE file in the project root for full license information.
###########################################################################################

#------------------------------------------------------------------------------------------
# Parameters:
# Help: whether to display the help information
# Step: Current step for configuration
#------------------------------------------------------------------------------------------
Param
(
	$WorkingPath      	 = "C:\Temp"
)

Push-Location $WorkingPath

$Parameters              = @{}
$CurrentScriptPath 		 = $MyInvocation.MyCommand.Definition
$ScriptsSignalFile = "$WorkingPath\post.finished.signal" # Config signal file


#-----------------------------------------------------------------------------
# Function: Prepare
# Usage   : Start executing the script; Push directory to working directory
# Params  : 
# Remark  : 
#-----------------------------------------------------------------------------
Function Prepare()
{
    Write-Host "Executing [PostScript-PDC.ps1] ..." -ForegroundColor Cyan
	
	# Check signal file
    if(Test-Path -Path $ScriptsSignalFile)
    {
        Write-Host "The script execution is complete." -ForegroundColor Red
        exit 0
    }

    # Change to absolute path
    Write-Host "Current path is $CurrentScriptPath" -ForegroundColor Cyan
    $WorkingPath = (Get-Item $WorkingPath).FullName

    Write-Host "Put current dir as $WorkingPath" -ForegroundColor Yellow
    Push-Location $WorkingPath
}

#------------------------------------------------------------------------------------------
# Function: Write-ConfigLog
# Write information to log file
#------------------------------------------------------------------------------------------
Function Write-ConfigLog
{
    Param (
        [Parameter(ValueFromPipeline=$true)] $text,
        $ForegroundColor = "Green"
    )

    $date = Get-Date -f MM-dd-yyyy_HH_mm_ss
    Write-Output "[$date] $text"
}

#------------------------------------------------------------------------------------------
# Function: Read-ConfigParameters
# Read Config Parameters
#------------------------------------------------------------------------------------------
Function Read-ConfigParameters()
{
    Write-ConfigLog "Getting the parameters from environment config file..." -ForegroundColor Yellow
    $VMName = .\GetVMNameByComputerName.ps1
    .\GetVmParameters.ps1 -VMName $VMName -RefParamArray ([ref]$Parameters)
    $Parameters
}

#------------------------------------------------------------------------------------------
# Function: Config-Phase1
# Configure the environment phase 1:
#  * Set execution policy as unrestricted
#  * Set network configurations
#  * Set autologon
#  * Join this computer to domain
#  * Register Windbg dbgsrv for debugging purpose
#------------------------------------------------------------------------------------------
Function Config-Phase1()
{
    Write-ConfigLog "Entering Phase 1..."
	
    # Set execution policy as unrestricted
    Write-ConfigLog "Setting execution policy..." -ForegroundColor Yellow
    .\Set-ExecutionPolicy-Unrestricted.ps1

    # Set network configurations
    Write-ConfigLog "Setting network configurations..." -ForegroundColor Yellow
    .\Set-NetworkConfiguration.ps1 -IPAddress $Parameters["ip"] -SubnetMask $Parameters["subnet"] -Gateway $Parameters["gateway"] -DNS ($Parameters["dns"].Split(';'))

    # Get domain account
    Write-ConfigLog "Trying to get the domain account" -ForegroundColor Yellow
    $DomainParameters = @{}
    .\Get-DomainControllerParameters.ps1 -DomainName $Parameters["domain"] -RefParamArray ([ref]$DomainParameters)

    # Set autologon, use domain administrator to logon
    Write-ConfigLog "Setting autologon..." -ForegroundColor Yellow
    .\Set-AutoLogon.ps1 -Domain $DomainParameters["domain"] -Username $DomainParameters["username"] -Password $DomainParameters["password"]

    # Join Domain
    Write-ConfigLog "Joining the computer to domain" -ForegroundColor Yellow
    Sleep 60 # Kept receiving 53, if not stabilize during domain join
    .\Join-Domain.ps1 -domainWorkgroup "Domain" -domainName $DomainParameters["domain"] -userName $DomainParameters["username"] -userPassword $DomainParameters["password"] -testResultsPath $ScriptPath 2>&1 | Write-Output
	
	# Turn off firewall
    Write-ConfigLog "Turn off firewall..." -ForegroundColor Yellow
    .\Disable_Firewall.ps1
	
}

#------------------------------------------------------------------------------------------
# Main Function
#------------------------------------------------------------------------------------------
Function Main
{
	Prepare

	Read-ConfigParameters

	Config-Phase1

    # Write signal file
	Write-ConfigLog "Write signal file`: post.finished.signal to hard drive."
	cmd /C ECHO CONFIG FINISHED > $ScriptsSignalFile

	Sleep 5
	Restart-Computer
}

Main
