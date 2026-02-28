param location string = resourceGroup().location
param environmentId string
param defaultDomain string
param acrLoginServer string
param acrName string
param imageTag string
param blobBaseUrl string
param environmentName string
param postgresFqdn string
param postgresAdminUsername string
@secure()
param postgresAdminPassword string
param logLevel string = 'Debug'
param customDomainName string = ''
param tags object = {
  Environment: environmentName
  Project: 'RaysonCV'
}

var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var uiFqdn = '${uiAppName}.${defaultDomain}'

module api 'modules/api-container.bicep' = {
  name: 'api-container-${environmentName}'
  params: {
    location: location
    environmentId: environmentId
    environmentName: environmentName
    containerAppName: apiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    uiFqdn: uiFqdn
    ollamaFqdn: 'http://localhost:11434'
    postgresFqdn: postgresFqdn
    postgresUsername: postgresAdminUsername
    postgresPassword: postgresAdminPassword
    postgresDatabase: 'raysoncv'
    logLevel: logLevel
    tags: tags
  }
}

module ui 'modules/ui-container.bicep' = {
  name: 'ui-container-${environmentName}'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: uiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    apiHealthUrl: 'http://${apiAppName}.internal.${defaultDomain}:8080/health'
    appDownloadUrl: blobBaseUrl
    customDomainName: customDomainName
    tags: tags
    environmentName: environmentName
  }
  dependsOn: [
    api
  ]
}

output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
