<#
.Synopsis
	Short description
.DESCRIPTION
	Long description
.EXAMPLE
	Example of how to use this cmdlet
.EXAMPLE
	Another example of how to use this cmdlet
#>

[CmdletBinding()]
param (
    [string]
    $RootCommonName = "tsa.local",

    [string]
    $CertificateStorePath = "Cert:\LocalMachine\My",

    [string]
    $OutputPath = ".\src\docker\certs"
)
    
begin {
    $ErrorActionPreference = "Stop"

    $validCertificateStorePath = Test-Path $CertificateStorePath

    $rootCaCertificateOutputPath = $OutputPath
    $apiCertificateOutputPath = $OutputPath
    $identityServerCertificateOutputPath = $OutputPath
    $submissionsCertificateOutputPath = $OutputPath

    $trustedRootCaPath = "Cert:\LocalMachine\Root"

    if (!$validCertificateStorePath) {
        Write-Error "The specified certificate store path '$CertificateStorePath' is not valid on this machine."
    }

    $createRootCaCertificate = (
        Get-ChildItem -Path $CertificateStorePath -Recurse |
        Where-Object { $_.Subject -eq "CN=$RootCommonName" } |
        Measure-Object
    ).Count -eq 0

    $importRootCaCertificate = (
        Get-ChildItem -Path $trustedRootCaPath -Recurse |
        Where-Object { $_.Subject -eq "CN=$RootCommonName" } |
        Measure-Object
    ).Count -eq 0

    $apiCommonNames = "api.tsa.local", "localhost"
    $createApiCertificate = (
        Get-ChildItem -Path $CertificateStorePath -Recurse |
        Where-Object { $_.Subject -eq "CN=$($apiCommonNames[0])" } |
        Measure-Object
    ).Count -eq 0

    $identityServerCommonNames = "identity.tsa.local", "localhost"
    $createIdentityServerCertificate = (
        Get-ChildItem -Path $CertificateStorePath -Recurse |
        Where-Object { $_.Subject -eq "CN=$($identityServerCommonNames[0])" } |
        Measure-Object
    ).Count -eq 0

    $submissionsCommonNames = "submissions.tsa.local", "localhost"
    $createSubmissionsCertificate = (
        Get-ChildItem -Path $CertificateStorePath -Recurse |
        Where-Object { $_.Subject -eq "CN=$($submissionsCommonNames[0])" } |
        Measure-Object
    ).Count -eq 0
}
    
process {
    if ($createRootCaCertificate) {
        Write-Output "Creating Root CA Certificate"
        $rootCaCertificate = New-SelfSignedCertificate -Subject $RootCommonName -KeyUsageProperty Sign -KeyUsage CertSign -CertStoreLocation $CertificateStorePath
    }
    else {
        Write-Output "Root CA Certificate Exists - Retrieving Certificate"
        $rootCaCertificate = (Get-ChildItem -Path $CertificateStorePath -Recurse | Where-Object { $_.Subject -eq "CN=$RootCommonName" })[0]
    }

    if ($createApiCertificate) {
        Write-Output "Creating API Certificate"
        $apiCertificate = New-SelfSignedCertificate -DnsName $apiCommonNames -Signer $rootCaCertificate -CertStoreLocation $CertificateStorePath
    }
    else {
        Write-Output "API Certificate Exists - Retrieving Certificate"
        $apiCertificate = (Get-ChildItem -Path $CertificateStorePath -Recurse | Where-Object { $_.Subject -eq "CN=$($apiCommonNames[0])" })[0]
    }

    if ($createIdentityServerCertificate) {
        Write-Output "Creating IdentityServer Certificate"
        $identityServerCertificate = New-SelfSignedCertificate -DnsName $identityServerCommonNames -Signer $rootCaCertificate -CertStoreLocation $CertificateStorePath
    }
    else {
        Write-Output "IdentityServer Certificate Exists - Retrieving Certificate"
        $identityServerCertificate = (Get-ChildItem -Path $CertificateStorePath -Recurse | Where-Object { $_.Subject -eq "CN=$($identityServerCommonNames[0])" })[0]
    }

    if ($createSubmissionsCertificate) {
        Write-Output "Creating Submissions Certificate"
        $submissionsCertificate = New-SelfSignedCertificate -DnsName $submissionsCommonNames -Signer $rootCaCertificate -CertStoreLocation $CertificateStorePath
    }
    else {
        Write-Output "Submissions Certificate Exists - Retrieving Certificate"
        $submissionsCertificate = (Get-ChildItem -Path $CertificateStorePath -Recurse | Where-Object { $_.Subject -eq "CN=$($submissionsCommonNames[0])" })[0]
    }

    #TODO: Figure out a better way to handle this
    $certificatePassword = ConvertTo-SecureString -String "b05e4983-84e0-447d-97c0-3fdf5b91ddb4" -Force -AsPlainText

    Write-Output "Exporting Root CA PFX Certificate"
    if (!(Test-Path -Path $rootCaCertificateOutputPath)) {
        New-Item -ItemType Directory -Path $rootCaCertificateOutputPath | Out-Null
    }
    Export-PfxCertificate -Cert $rootCaCertificate -FilePath "$rootCaCertificateOutputPath\rootCaCertificate.pfx" -Password $certificatePassword | Out-Null

    Write-Output "Exporting API PFX Certificate"
    if (!(Test-Path -Path $apiCertificateOutputPath)) {
        New-Item -ItemType Directory -Path $apiCertificateOutputPath | Out-Null
    }
    Export-PfxCertificate -Cert $apiCertificate -FilePath "$apiCertificateOutputPath\apiCertificate.pfx" -Password $certificatePassword | Out-Null

    Write-Output "Exporting IdentityServer PFX Certificate"
    if (!(Test-Path -Path $identityServerCertificateOutputPath)) {
        New-Item -ItemType Directory -Path $identityServerCertificateOutputPath | Out-Null
    }
    Export-PfxCertificate -Cert $identityServerCertificate -FilePath "$identityServerCertificateOutputPath\identityServerCertificate.pfx" -Password $certificatePassword | Out-Null

    Write-Output "Exporting Submissions PFX Certificate"
    if (!(Test-Path -Path $submissionsCertificateOutputPath)) {
        New-Item -ItemType Directory -Path $submissionsCertificateOutputPath | Out-Null
    }
    Export-PfxCertificate -Cert $submissionsCertificate -FilePath "$submissionsCertificateOutputPath\submissionsCertificate.pfx" -Password $certificatePassword | Out-Null

    Write-Output "Exporting Root CA Certificate in Binary DER Format"
    $publicRootCaCertificateFileInfo = Export-Certificate -Cert $rootCaCertificate -FilePath "$rootCaCertificateOutputPath\publicRootCaCertificate.cer" -Type CERT
    
    if ($importRootCaCertificate) {
        Write-Output "Importing Root CA Certificate into Trusted Root Certification Authorities"
        Import-Certificate -CertStoreLocation $trustedRootCaPath -FilePath $publicRootCaCertificateFileInfo.FullName | Out-Null
    }

    Write-Output "Converting Root CA Binary DER Format to Base64-Encoded"
    openssl x509 -inform DER -in "$rootCaCertificateOutputPath\publicRootCaCertificate.cer" -out "$rootCaCertificateOutputPath\publicRootCaCertificate.crt"
}
    
end {
    Write-Host "Done"
}