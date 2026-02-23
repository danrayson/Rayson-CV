param location string = resourceGroup().location
param environmentId string
param containerAppName string
param pgAdminEmail string
param pgAdminPassword string
param postgresHost string
param postgresPort int = 5432
param postgresDb string
param postgresUser string
param tags object = {}

var serversJson = {
  Servers: {
    '1': {
      Name: 'RaysonDev Staging'
      Group: 'Servers'
      Host: postgresHost
      Port: postgresPort
      MaintenanceDB: postgresDb
      Username: postgresUser
      SSLMode: 'prefer'
    }
  }
}

resource pgAdminContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'pgadmin-password'
          value: pgAdminPassword
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 5050
        transport: 'auto'
      }
    }
    template: {
      containers: [
        {
          name: 'pgadmin'
          image: 'dpage/pgadmin4:latest'
          env: [
            {
              name: 'PGADMIN_DEFAULT_EMAIL'
              value: pgAdminEmail
            }
            {
              name: 'PGADMIN_DEFAULT_PASSWORD'
              secretRef: 'pgadmin-password'
            }
            {
              name: 'PGADMIN_LISTEN_PORT'
              value: '5050'
            }
            {
              name: 'PGADMIN_CONFIG_SERVER_MODE'
              value: 'False'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'pgadmin-data'
              mountPath: '/var/lib/pgadmin'
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]
      volumes: [
        {
          name: 'pgadmin-data'
          storageType: 'AzureFile'
          storageName: 'pgadmin-storage'
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

output containerAppName string = pgAdminContainer.name
output fqdn string = pgAdminContainer.properties.configuration.ingress.fqdn
