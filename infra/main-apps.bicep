param location string = resourceGroup().location
param environmentId string
param defaultDomain string
param acrLoginServer string
param acrName string
param imageTag string
param blobBaseUrl string
param environmentName string
param postgresHost string
param postgresAdminUsername string
@secure()
param postgresAdminPassword string
param tags object = {
  Environment: environmentName
  Project: 'RaysonCV'
}

var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var uiFqdn = '${uiAppName}.${defaultDomain}'
var ollamaFqdn = 'http://localhost:11434'

module api 'modules/api-container.bicep' = {
  name: 'api-container'
  params: {
    location: location
    environmentId: environmentId
    environmentName: environmentName
    containerAppName: apiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    uiFqdn: uiFqdn
    ollamaFqdn: ollamaFqdn
    postgresHost: postgresHost
    postgresUsername: postgresAdminUsername
    postgresPassword: postgresAdminPassword
    postgresDatabase: 'raysoncv'
    tags: tags
  }
}

module ui 'modules/ui-container.bicep' = {
  name: 'ui-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: uiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    apiHealthUrl: 'http://${apiAppName}.internal.${defaultDomain}:8080/health'
    appDownloadUrl: blobBaseUrl
    tags: tags
  }
  dependsOn: [
    api
  ]
}

output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
