param location string = resourceGroup().location
param environmentId string
param containerAppName string
param tags object = {}

resource lokiContainerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 3100
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'loki'
          image: 'grafana/loki:3.2.0'
          args: [
            '-config.file=/etc/loki/local-config.yaml'
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

output containerAppName string = lokiContainerApp.name
output fqdn string = lokiContainerApp.properties.configuration.ingress.fqdn
output url string = 'http://${lokiContainerApp.properties.configuration.ingress.fqdn}:3100'
