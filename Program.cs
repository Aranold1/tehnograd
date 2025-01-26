using System.Globalization;
using System.Text.Json;

var DgisApiKey = Environment.GetEnvironmentVariable("DGIS_API_KEY");
var YandexApiKey = Environment.GetEnvironmentVariable("YANDEX_API_KEY");

while (true)
{
	Console.WriteLine("Введите адрес: ");
	var address = Console.ReadLine();
	// Проверяем адрес
	if (string.IsNullOrWhiteSpace(address))
	{
		Console.WriteLine();
		Console.WriteLine("Адрес не может быть пустым");
		Thread.Sleep(800);
		Console.WriteLine();
		continue;
	}

	Console.WriteLine();
	try
	{
		Task.WaitAll(GetCoordinatesFromDGisAsync(address), GetCoordinatesFromYandexAsync(address));
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}

async Task GetCoordinatesFromYandexAsync(string address)
{
	using (var client = new HttpClient())
	{
		string url = $"https://geocode-maps.yandex.ru/1.x/?format=json&apikey={YandexApiKey}&geocode={Uri.EscapeDataString(address)}";
		var response = await client.GetAsync(url);
		// Убеждаемся, что Response успешный
		response.EnsureSuccessStatusCode();
		// Преобразуем в JSON
		var jsonResponse = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
		// Преобразуем строку координат в массив
		var coordinatesAsStringArray = jsonResponse
			.GetProperty("response")
			.GetProperty("GeoObjectCollection")
			.GetProperty("featureMember")
			.EnumerateArray().First()
			.GetProperty("GeoObject")
			.GetProperty("Point")
			.GetProperty("pos")
			.GetString()
			.Split(' ');

		// В отличие от API 2GIS, API Yandex возвращает double с точкой, а не с запятой 
		var latitude = double.Parse(coordinatesAsStringArray[1], CultureInfo.GetCultureInfo("en-US"));
		var longitude = double.Parse(coordinatesAsStringArray[0], CultureInfo.GetCultureInfo("en-US"));

		// Выводим результат
		Console.WriteLine($"Yandex: \n Широта: {latitude}\n Долгота: {longitude}");
		Console.WriteLine();
	}
}

async Task GetCoordinatesFromDGisAsync(string address)
{
	using (var client = new HttpClient())
	{
		string url = $"https://catalog.api.2gis.com/3.0/items/geocode?q={Uri.EscapeDataString(address)}&fields=items.point&key={DgisApiKey}";
		var response = await client.GetAsync(url);
		// Убеждаемся, что Response успешный
		response.EnsureSuccessStatusCode();
		// Преобразуем в JSON
		var jsonResponse = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
		// Проверяем еще раз, так как 2GIS даже неуспешные запросы возвращает с кодом 200, а настоящий код хранится в Json
		if (jsonResponse.GetProperty("meta").GetProperty("code").GetInt32() == 404)
		{
			throw new Exception("Адрес не найден.");
		}

		var coordinates = jsonResponse
				.GetProperty("result")
				.GetProperty("items")
				.EnumerateArray()
				.First()
				.GetProperty("point");
		// Получаем значения долготы и широты
		double latitude = coordinates.GetProperty("lat").GetDouble();
		double longitude = coordinates.GetProperty("lon").GetDouble();

		// Выводим результат в нужном формате
		Console.WriteLine($"2gis: \n Широта: {latitude}\n Долгота: {longitude}");
		Console.WriteLine();
	}
}