targetScope = 'subscription'

param location string
param resourceGroupName string
param environmentName string
param acrName string
param postgresDb string
param postgresUser string
param postgresPassword string
param jwtIssuer string
param jwtAudience string
param jwtSigningKey string
param corsOrigins array
param seqAdminPassword string
param pgAdminEmail string
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

module storageAccount 'Microsoft.Resources/deployments@2022-09-01' = {
  name: 'storage-account'
  scope: rg
  params: {
    location: location
    storageAccountName: storageAccountName
    tags: tags
  }
  properties: {
    mode: 'Incremental'
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      parameters: {
        location: { type: 'string' }
        storageAccountName: { type: 'string' }
        tags: { type: 'object' }
      }
      resources: [
        {
          type: 'Microsoft.Storage/storageAccounts'
          apiVersion: '2023-01-01'
          name: '[parameters(\'storageAccountName\')]'
          location: '[parameters(\'location\')]'
          sku: {
            name: 'Standard_LRS'
          }
          kind: 'StorageV2'
          properties: {
            accessTier: 'Hot'
          }
          tags: '[parameters(\'tags\')]'
        }
      ]
    }
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
  dependsOn: [
    storageAccount
  ]
}

module fileShares 'Microsoft.Resources/deployments@2022-09-01' = {
  name: 'file-shares'
  scope: rg
  params: {
    storageAccountName: storageAccountName
  }
  properties: {
    mode: 'Incremental'
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      parameters: {
        storageAccountName: { type: 'string' }
      }
      resources: [
        {
          type: 'Microsoft.Storage/storageAccounts/fileServices/shares'
          apiVersion: '2023-01-01'
          name: '[concat(parameters(\'storageAccountName\'), \'/default/postgres-data\')]'
          properties: {
            shareQuota: 5
          }
        }
        {
          type: 'Microsoft.Storage/storageAccounts/fileServices/shares'
          apiVersion: '2023-01-01'
          name: '[concat(parameters(\'storageAccountName\'), \'/default/seq-data\')]'
          properties: {
            shareQuota: 2
          }
        }
        {
          type: 'Microsoft.Storage/storageAccounts/fileServices/shares'
          apiVersion: '2023-01-01'
          name: '[concat(parameters(\'storageAccountName\'), \'/default/pgadmin-data\')]'
          properties: {
            shareQuota: 1
          }
        }
      ]
    }
  }
  dependsOn: [
    storageAccount
  ]
}

module environmentStorage 'Microsoft.Resources/deployments@2022-09-01' = {
  name: 'environment-storage'
  scope: rg
  params: {
    environmentName: containerAppsEnvName
    storageAccountName: storageAccountName
  }
  properties: {
    mode: 'Incremental'
    template: {
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      parameters: {
        environmentName: { type: 'string' }
        storageAccountName: { type: 'string' }
      }
      resources: [
        {
          type: 'Microsoft.App/managedEnvironments/storages@2023-05-01'
          name: '[concat(parameters(\'environmentName\'), \'/postgres-storage\')]'
          properties: {
            azureFile: {
              accountName: '[parameters(\'storageAccountName\')]'
              shareName: 'postgres-data'
              accessMode: 'ReadWrite'
            }
          }
        }
        {
          type: 'Microsoft.App/managedEnvironments/storages@2023-05-01'
          name: '[concat(parameters(\'environmentName\'), \'/seq-storage\')]'
          properties: {
            azureFile: {
              accountName: '[parameters(\'storageAccountName\')]'
              shareName: 'seq-data'
              accessMode: 'ReadWrite'
            }
          }
        }
        {
          type: 'Microsoft.App/managedEnvironments/storages@2023-05-01'
          name: '[concat(parameters(\'environmentName\'), \'/pgadmin-storage\')]'
          properties: {
            azureFile: {
              accountName: '[parameters(\'storageAccountName\')]'
              shareName: 'pgadmin-data'
              accessMode: 'ReadWrite'
            }
          }
        }
      ]
    }
  }
  dependsOn: [
    containerAppsEnv
    fileShares
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
    storageAccountName: storageAccountName
    tags: tags
  }
  dependsOn: [
    containerAppsEnv
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
    containerAppsEnv
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
    acrUsername: acr.outputs.acrName
    acrPassword: listKeys(acr.outputs.acrId, '2023-07-01').passwords[0].value
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
    containerAppsEnv
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
    acrUsername: acr.outputs.acrName
    acrPassword: listKeys(acr.outputs.acrId, '2023-07-01').passwords[0].value
    imageTag: imageTag
    apiHealthUrl: 'http://${apiAppName}.internal.${containerAppsEnv.outputs.defaultDomain}:8080/health'
    tags: tags
  }
  dependsOn: [
    containerAppsEnv
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
    environmentStorage
    postgres
  ]
}

output acrLoginServer string = acr.outputs.acrLoginServer
output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
output pgAdminFqdn string = pgAdmin.outputs.fqdn
