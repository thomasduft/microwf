# --------------------------------------------------------------------------------------------------
# Creates a self signed certificate
#
# Call ps script with admin rights
# Samples:
# - .\create_cert.ps1
# - .\create_cert.ps1 -issuer $env:computername
# - .\create_cert.ps1 -issuer localhost -validityInYears 5
# --------------------------------------------------------------------------------------------------
param(
  [string]$issuer = "localhost",
  [int]$validityInYears = 21
)

$secret = "MicroWFApi"

# setup certificate properties including the commonName (DNSName) property for Chrome 58+
$certificate = New-SelfSignedCertificate `
  -Subject $issuer `
  -DnsName $issuer `
  -KeyAlgorithm RSA `
  -KeyLength 2048 `
  -NotBefore (Get-Date) `
  -NotAfter (Get-Date).AddYears($validityInYears) `
  -CertStoreLocation "cert:CurrentUser\My" `
  -FriendlyName "MicroWF Api" `
  -HashAlgorithm SHA256 `
  -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment `
  -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1")
$certificatePath = 'Cert:\CurrentUser\My\' + ($certificate.ThumbPrint)

# create temporary certificate path
$tmpPath = "C:\temp"
If (!(test-path $tmpPath)) {
  New-Item -ItemType Directory -Force -Path $tmpPath
}

# set certificate password here
$pfxPassword = ConvertTo-SecureString -String $secret -Force -AsPlainText
$pfxFilePath = 'c:\temp\' + ($issuer) + '.pfx'
$cerFilePath = 'c:\temp\' + ($issuer) + '.cer'

# create pfx certificate
Export-PfxCertificate -Cert $certificatePath -FilePath $pfxFilePath -Password $pfxPassword
Export-Certificate -Cert $certificatePath -FilePath $cerFilePath

# import the pfx certificate
Import-PfxCertificate -FilePath $pfxFilePath Cert:\LocalMachine\My -Password $pfxPassword -Exportable

# trust the certificate by importing the pfx certificate into your trusted root
Import-Certificate -FilePath $cerFilePath -CertStoreLocation Cert:\CurrentUser\Root

# optionally delete the physical certificates (don’t delete the pfx file as you need to copy this to your app directory)
# Remove-Item $pfxFilePath
Remove-Item $cerFilePath
