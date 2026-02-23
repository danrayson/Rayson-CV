targetScope = 'subscription'

param location string
param resourceGroupName string
param environmentName string
param acrName string
param postgresDb string
param postgresUser string
@secure()
param postgresPassword string
param jwtIssuer string
param jwtAudience string
@secure()
param jwtSigningKey string
param corsOrigins array
@secure()
param seqAdminPassword string
param imageTag string = 'latest'
param tags object = {}

var containerAppsEnvName = 'cae-raysondev-${environmentName}'
var postgresAppName = 'ca-postgres-${environmentName}'
var seqContainerName = 'ci-seq-${environmentName}'
var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var storageAccountName = 'straysondev${environmentName}'

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
    storageAccountName: storageAccountName
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

module postgresService 'modules/postgres-service.bicep' = {
  name: 'postgres-service'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: postgresAppName
    tags: tags
  }
}

module seq 'modules/seq-container.bicep' = {
  name: 'seq-container'
  scope: rg
  params: {
    location: location
    containerGroupName: seqContainerName
    storageAccountName: storage.outputs.storageAccountName
    storageAccountKey: storage.outputs.storageAccountKey
    seqAdminPassword: seqAdminPassword
    tags: tags
  }
}

module api 'modules/api-container.bicep' = {
  name: 'api-container'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: apiAppName
    acrLoginServer: acr.outputs.acrLoginServer
    acrName: acrName
    imageTag: imageTag
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience
    jwtSigningKey: jwtSigningKey
    corsOrigins: corsOrigins
    seqUrl: 'http://${seq.outputs.fqdn}:5341'
    postgresServiceId: postgresService.outputs.serviceId
    tags: tags
  }
}

module ui 'modules/ui-container.bicep' = {
  name: 'ui-container'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: uiAppName
    acrLoginServer: acr.outputs.acrLoginServer
    acrName: acrName
    imageTag: imageTag
    apiHealthUrl: 'http://${apiAppName}.internal.${containerAppsEnv.outputs.defaultDomain}:8080/health'
    tags: tags
  }
  dependsOn: [
    api
  ]
}

output acrLoginServer string = acr.outputs.acrLoginServer
output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
