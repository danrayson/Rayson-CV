param environmentName string
param storageAccountName string

resource postgresEnvStorage 'Microsoft.App/managedEnvironments/storages@2023-05-01' = {
  name: '${environmentName}/postgres-storage'
  properties: {
    azureFile: {
      accountName: storageAccountName
      shareName: 'postgres-data'
      accessMode: 'ReadWrite'
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
    }
  }
}
