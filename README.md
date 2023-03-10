# Poolit


## Команды для запуска программы

Запуск контейнера с бд:
```
docker run --detach --name poolit-db  --publish 5432:5432 --env "POSTGRES_DB=poolit" --env "POSTGRES_USER=postgres_user" --env "POSTGRES_PASSWORD=postgres_password" postgres
```

Запуск миграций для *development*-a:
```
dotnet run --project DBMigrations Development
```
