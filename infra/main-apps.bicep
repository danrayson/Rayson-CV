param location string = resourceGroup().location
param environmentId string
param defaultDomain string
param acrLoginServer string
param acrName string
param imageTag string
param storageAccountName string
param blobBaseUrl string
param environmentName string
param tags object = {
  Environment: environmentName
  Project: 'RaysonCV'
}

var postgresServiceName = 'ca-postgres-${environmentName}'
var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var ollamaAppName = 'ca-ollama-${environmentName}'
var uiFqdn = '${uiAppName}.${defaultDomain}'
var ollamaFqdn = '${ollamaAppName}.internal.${defaultDomain}:11434'

module postgresService 'modules/postgres-service.bicep' = {
  name: 'postgres-service'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: postgresServiceName
    tags: tags
  }
}

module ollama 'modules/ollama-container.bicep' = {
  name: 'ollama-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: ollamaAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    storageAccountName: storageAccountName
    tags: tags
  }
}

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
    postgresServiceId: postgresService.outputs.serviceId
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
