# Информация о механизме миграций.
## Для создания миграции выполнить:

```
add-migration -name "<ИМЯ МИГРАЦИИ>" -ProjectName "Transport.DataAccessLayer" -StartUpProjectName Transport.DataAccessLayer
```

## Для обновления БД:

```
Update-database -ProjectName Transport.DataAccessLayer -StartUpProjectName Transport.DataAccessLayer
```

## Примечание

Работает только для бд, на которую применены миграции ранее (или создана с их помощью)

Все команды в Package Manager Console (консоль от Nuget).
