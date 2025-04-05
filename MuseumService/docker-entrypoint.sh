#!/bin/bash
set -e

# Ожидание доступности базы данных
until pg_isready -h postgres -U postgres; do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 1
done

>&2 echo "Postgres is up - executing command"

# Запуск приложения
exec dotnet MuseumService.dll