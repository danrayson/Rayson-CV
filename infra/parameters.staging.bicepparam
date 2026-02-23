using '../main.bicep'

param location = 'uksouth'
param resourceGroupName = 'rg-raysondev-staging'
param environmentName = 'staging'
param acrName = 'acrraysondevstaging'
param postgresDb = 'raysondev'
param postgresUser = 'raysondev_user'
param postgresPassword = ''
param jwtIssuer = 'danrayson'
param jwtAudience = 'RaysonDevUsers'
param jwtSigningKey = ''
param corsOrigins = [
  'https://ca-ui-staging.uksouth.azurecontainerapps.io'
]
param seqAdminPassword = ''
param pgAdminEmail = ''
param pgAdminPassword = ''
param imageTag = 'latest'
