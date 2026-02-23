param location string = resourceGroup().location
param containerAppName string
param environmentId string
param tags object = {}

resource postgresService 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  tags: tags
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      service: {
        type: 'postgres'
      }
    }
  }
}

output serviceId string = postgresService.id
output containerAppName string = postgresService.name
