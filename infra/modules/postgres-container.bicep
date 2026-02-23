param location string = resourceGroup().location
param environmentId string
param containerAppName string
param postgresDb string
param postgresUser string
@secure()
param postgresPassword string
param tags object = {}

resource postgresContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'postgres-password'
          value: postgresPassword
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: 5432
      }
    }
    template: {
      containers: [
        {
          name: 'postgres'
          image: 'postgres:16-alpine'
          env: [
            {
              name: 'POSTGRES_DB'
              value: postgresDb
            }
            {
              name: 'POSTGRES_USER'
              value: postgresUser
            }
            {
              name: 'POSTGRES_PASSWORD'
              secretRef: 'postgres-password'
            }
            {
              name: 'PGDATA'
              value: '/var/lib/postgresql/data/pgdata'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'postgres-data'
              mountPath: '/var/lib/postgresql/data'
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
          name: 'postgres-data'
          storageType: 'AzureFile'
          storageName: 'postgres-storage'
        }
      ]
    }
  }
  tags: tags
}

output containerAppName string = postgresContainer.name
