param location string = resourceGroup().location
param environmentId string
param containerAppName string
param acrLoginServer string
param acrUsername string
param acrPassword string
param imageTag string = 'latest'
param apiHealthUrl string
param tags object = {}

resource uiContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'acr-password'
          value: acrPassword
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 3000
        transport: 'auto'
      }
      registries: [
        {
          server: acrLoginServer
          username: acrUsername
          passwordSecretRef: 'acr-password'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'ui'
          image: '${acrLoginServer}/raysondev-ui:${imageTag}'
          env: [
            {
              name: 'API_HEALTH_URL'
              value: apiHealthUrl
            }
            {
              name: 'LOG_LEVEL'
              value: 'info'
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
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

output containerAppName string = uiContainer.name
output fqdn string = uiContainer.properties.configuration.ingress.fqdn
