param location string = resourceGroup().location
param environmentId string
param containerAppName string
param acrLoginServer string
param acrName string
param imageTag string = 'latest'
param tags object = {}

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource ollamaContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'acr-password'
          value: acr.listCredentials().passwords[0].value
        }
      ]
      activeRevisionsMode: 'Single'
      registries: [
        {
          server: acrLoginServer
          username: acrName
          passwordSecretRef: 'acr-password'
        }
      ]
      ingress: {
        external: false
        targetPort: 11434
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'ollama'
          image: '${acrLoginServer}/raysoncv-ollama:${imageTag}'
          env: [
            {
              name: 'OLLAMA_HOST'
              value: '0.0.0.0'
            }
          ]
          resources: {
            cpu: json('1.0')
            memory: '2.0Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
  tags: tags
}

output containerAppName string = ollamaContainer.name
output fqdn string = ollamaContainer.properties.configuration.ingress.fqdn
output serviceId string = ollamaContainer.id
