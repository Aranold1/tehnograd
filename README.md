# GeoCoordinates Finder

Консольное приложение для получения географических координат по адресу с использованием API 2GIS и Яндекс.Карт.

## Требования

- .NET 8.0 SDK
- API-ключи от [2GIS](https://dev.2gis.com/) и [Яндекс.Геокодер](https://developer.tech.yandex.ru/services/)

## Установка

1. Клонируйте репозиторий

```bash
git clone https://github.com/Aranold1/tehnograd
```

2. Замените API-ключи в файле Program.cs

```csharp
var DgisApiKey = "ваш_ключ_2gis";
var YandexApiKey = "ваш_ключ_yandex";
```

3. Запустите программу

```shell
dotnet run
```
