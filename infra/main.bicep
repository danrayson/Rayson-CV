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
param pgAdminEmail string
@secure()
param pgAdminPassword string
param imageTag string = 'latest'

param tags object = {
  Environment: environmentName
  Project: 'RaysonDev'
}

var containerAppsEnvName = 'cae-raysondev-${environmentName}'
var postgresAppName = 'ca-postgres-${environmentName}'
var seqAppName = 'ca-seq-${environmentName}'
var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var pgAdminAppName = 'ca-pgadmin-${environmentName}'
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

module environmentStorage 'modules/environment-storage.bicep' = {
  name: 'environment-storage'
  scope: rg
  params: {
    environmentName: containerAppsEnvName
    storageAccountName: storageAccountName
    storageAccountKey: storage.outputs.storageAccountKey
  }
  dependsOn: [
    containerAppsEnv
    storage
  ]
}

module postgres 'modules/postgres-container.bicep' = {
  name: 'postgres-container'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: postgresAppName
    postgresDb: postgresDb
    postgresUser: postgresUser
    postgresPassword: postgresPassword
    tags: tags
  }
  dependsOn: [
    environmentStorage
  ]
}

module seq 'modules/seq-container.bicep' = {
  name: 'seq-container'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: seqAppName
    seqAdminPassword: seqAdminPassword
    tags: tags
  }
  dependsOn: [
    environmentStorage
  ]
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
    defaultConnection: 'Server=${postgresAppName}.internal.${containerAppsEnv.outputs.defaultDomain};Port=5432;Database=${postgresDb};User Id=${postgresUser};Password=${postgresPassword};TrustServerCertificate=True'
    corsOrigins: corsOrigins
    seqUrl: 'http://${seqAppName}.internal.${containerAppsEnv.outputs.defaultDomain}:5341'
    tags: tags
  }
  dependsOn: [
    postgres
    seq
  ]
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

module pgAdmin 'modules/pgadmin-container.bicep' = {
  name: 'pgadmin-container'
  scope: rg
  params: {
    location: location
    environmentId: containerAppsEnv.outputs.environmentId
    containerAppName: pgAdminAppName
    pgAdminEmail: pgAdminEmail
    pgAdminPassword: pgAdminPassword
    tags: tags
  }
  dependsOn: [
    environmentStorage
    postgres
  ]
}

output acrLoginServer string = acr.outputs.acrLoginServer
output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
output pgAdminFqdn string = pgAdmin.outputs.fqdn
