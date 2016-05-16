using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
///TianQi 的摘要说明
/// </summary>
[Serializable]
public class CityWeatherResponse
{
    public string error { get; set; }
    public string status { get; set; }
    public string date { get; set; }
    public Result[] results;
}

[Serializable]
public class Result
{
    public string currentCity;
    public string pm25;
    public Index[] index;
    public WeatherData[] weather_data;
}

[Serializable]
public class Index
{
    public string title;
    public string zs;
    public string tipt;
    public string des;
}

[Serializable]
public class WeatherData
{
    public string date;
    public string dayPictureUrl;
    public string nightPictureUrl;
    public string weather;
    public string wind;
    public string temperature;
}