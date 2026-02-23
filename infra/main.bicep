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

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
  scope: rg
}

resource newStorageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  scope: rg
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
  tags: tags
}

resource postgresFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  name: '${storageAccountName}/default/postgres-data'
  scope: rg
  properties: {
    shareQuota: 5
  }
  dependsOn: [
    newStorageAccount
  ]
}

resource seqFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  name: '${storageAccountName}/default/seq-data'
  scope: rg
  properties: {
    shareQuota: 2
  }
  dependsOn: [
    newStorageAccount
  ]
}

resource pgAdminFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  name: '${storageAccountName}/default/pgadmin-data'
  scope: rg
  properties: {
    shareQuota: 1
  }
  dependsOn: [
    newStorageAccount
  ]
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

resource postgresEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${containerAppsEnvName}/postgres-storage'
  scope: rg
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'postgres-data'
      accessMode: 'ReadWrite'
    }
  }
  dependsOn: [
    containerAppsEnv
    postgresFileShare
  ]
}

resource seqEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${containerAppsEnvName}/seq-storage'
  scope: rg
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'seq-data'
      accessMode: 'ReadWrite'
    }
  }
  dependsOn: [
    containerAppsEnv
    seqFileShare
  ]
}

resource pgAdminEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${containerAppsEnvName}/pgadmin-storage'
  scope: rg
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'pgadmin-data'
      accessMode: 'ReadWrite'
    }
  }
  dependsOn: [
    containerAppsEnv
    pgAdminFileShare
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
    containerAppsEnv
    postgresEnvStorage
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
    containerAppsEnv
    seqEnvStorage
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
    acr
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
    acr
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
    postgresHost: '${postgresAppName}.internal.${containerAppsEnv.outputs.defaultDomain}'
    postgresPort: 5432
    postgresDb: postgresDb
    postgresUser: postgresUser
    tags: tags
  }
  dependsOn: [
    containerAppsEnv
    pgAdminEnvStorage
    postgres
  ]
}

output acrLoginServer string = acr.outputs.acrLoginServer
output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
output pgAdminFqdn string = pgAdmin.outputs.fqdn
