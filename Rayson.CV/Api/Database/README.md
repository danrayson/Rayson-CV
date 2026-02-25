# Database Docker Images

## Sql Server

docker run --name dockersql -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=M45S1VE316!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest -n SqlServer

to connect, connect to localhost,1433  Not localhost:1433 lawl

## Postgres

```bash
docker run --name market-postgres -e POSTGRES_USER=marketuser -e POSTGRES_PASSWORD=0me3eg4lulz -v postgresql-data:/var/lib/postgresql/data -d postgres
```
`--name`: What to name the image
`-e`: Environment variables to set in the image
`-d`: Run the image in detached mode (in the background)
`-v`: Name the mount so it's easy to see where it is (unsure if this overrides yet)
`postgres`: The image to run
`POSTGRES_USER`: The username to create in the database
`POSTGRES_PASSWORD`: The password to use for the user

To connect use findtheip:5432, whatever username and password you used, and default database (blank)


## Migrations
to make a migration, run this from the solution folder:

dotnet ef migrations add Init -p Database -s Presentation

## Find current ip of container
```
docker inspect --format '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' market-postgres
```


## To re-run existing container
```
docker start --name market-postgres
```

## Connection string

for sql:
Server={find your ip},5432;Database=marketuser;User Id=marketuser;Password=0me3eg4lulz