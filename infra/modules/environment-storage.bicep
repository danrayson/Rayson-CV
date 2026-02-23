param environmentName string
param storageAccountName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

var storageAccountKey = storageAccount.listKeys().keys[0].value

resource postgresEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${environmentName}/postgres-storage'
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'postgres-data'
      accessMode: 'ReadWrite'
      accountKey: storageAccountKey
    }
  }
}

resource seqEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${environmentName}/seq-storage'
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'seq-data'
      accessMode: 'ReadWrite'
      accountKey: storageAccountKey
    }
  }
}

resource pgAdminEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${environmentName}/pgadmin-storage'
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'pgadmin-data'
      accessMode: 'ReadWrite'
      accountKey: storageAccountKey
    }
  }
}
