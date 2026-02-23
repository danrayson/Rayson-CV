param location string = resourceGroup().location
param environmentId string
param containerAppName string
@secure()
param seqAdminPassword string
param tags object = {}

resource seqContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'seq-admin-password'
          value: seqAdminPassword
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 80
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'seq'
          image: 'datalust/seq:latest'
          env: [
            {
              name: 'ACCEPT_EULA'
              value: 'Y'
            }
            {
              name: 'SEQ_FIRSTRUN_ADMINPASSWORD'
              secretRef: 'seq-admin-password'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'seq-data'
              mountPath: '/data'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
        }
      ]
      volumes: [
        {
          name: 'seq-data'
          storageType: 'AzureFile'
          storageName: 'seq-storage'
        }
      ]
    }
  }
  tags: tags
}

output containerAppName string = seqContainer.name
output fqdn string = seqContainer.properties.configuration.ingress.fqdn
