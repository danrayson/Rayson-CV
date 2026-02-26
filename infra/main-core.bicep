targetScope = 'subscription'

param location string
param resourceGroupName string
param environmentName string
param acrName string

param tags object = {
  Environment: environmentName
  Project: 'RaysonCV'
}

var containerAppsEnvName = 'cae-raysoncv-${environmentName}'

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module acr 'modules/container-registry.bicep' = {
  name: 'container-registry'
  scope: rg
  params: {
    location: location
    acrName: acrName
    tags: tags
  }
}

module storage 'modules/storage.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    location: location
    environmentName: environmentName
    tags: tags
  }
}

module containerAppsEnv 'modules/container-apps-environment.bicep' = {
  name: 'container-apps-environment'
  scope: rg
  params: {
    location: location
    environmentName: containerAppsEnvName
    tags: tags
  }
}

output acrLoginServer string = acr.outputs.acrLoginServer
output acrName string = acr.outputs.acrName
output environmentId string = containerAppsEnv.outputs.environmentId
output defaultDomain string = containerAppsEnv.outputs.defaultDomain
output storageAccountName string = storage.outputs.storageAccountName
output blobBaseUrl string = storage.outputs.blobBaseUrl
output storageAccountKey string = storage.outputs.storageAccountKey
