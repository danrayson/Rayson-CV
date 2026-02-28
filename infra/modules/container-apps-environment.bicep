param location string = resourceGroup().location
param environmentName string
param tags object = {}
param storageAccountName string = ''
@secure()
param storageAccountKey string = ''
param storageShareName string = ''

resource containerAppsEnv 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: 'cae-raysoncv-${environmentName}'
  location: location
  properties: {}
  tags: tags
}

resource ollamaStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = if (storageAccountName != '') {
  name: 'ollama-models'
  parent: containerAppsEnv
  properties: {
    azureFile: {
      accessMode: 'ReadWrite'
      accountName: storageAccountName
      shareName: storageShareName
      accountKey: storageAccountKey
    }
  }
}

output containerAppEnvId string = containerAppsEnv.id
output containerAppEnvName string = containerAppsEnv.name
output defaultDomain string = containerAppsEnv.properties.defaultDomain
