param location string = resourceGroup().location
param environmentId string
param containerAppName string
param acrLoginServer string
param acrName string
param imageTag string = 'latest'
param tags object = {}
param storageAccountName string
@secure()
param storageAccountKey string

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
        {
          name: 'storage-account-key'
          value: storageAccountKey
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
            cpu: json('2.0')
            memory: '4.0Gi'
          }
          volumeMounts: [
            {
              volumeName: 'ollama-models'
              mountPath: '/root/.ollama'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
      volumes: [
        {
          name: 'ollama-models'
          storageType: 'AzureFile'
          storage: {
            storageAccountName: storageAccountName
            shareName: 'ollama-models'
            accountKeySecretRef: 'storage-account-key'
          }
        }
      ]
    }
  }
  tags: tags
}

output containerAppName string = ollamaContainer.name
output fqdn string = ollamaContainer.properties.configuration.ingress.fqdn
output serviceId string = ollamaContainer.id
