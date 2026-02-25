param location string = resourceGroup().location
param environmentId string
param containerAppName string
param tags object = {}

resource grafanaContainerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 3000
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'grafana'
          image: 'grafana/grafana:11.4.0'
          env: [
            {
              name: 'GF_SECURITY_ADMIN_PASSWORD'
              value: 'admin'
            }
            {
              name: 'GF_AUTH_ANONYMOUS_ENABLED'
              value: 'true'
            }
            {
              name: 'GF_SERVER_ROOT_URL'
              value: 'https://${containerAppName}.${environmentId}.${location}.azurecontainerapps.io'
            }
            {
              name: 'GF_SERVER_SERVE_FROM_SUB_PATH'
              value: 'true'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
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

output containerAppName string = grafanaContainerApp.name
output fqdn string = grafanaContainerApp.properties.configuration.ingress.fqdn
output url string = 'https://${grafanaContainerApp.properties.configuration.ingress.fqdn}'
