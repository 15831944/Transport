using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Transport.Common;
using Transport.DataAccessLayer;
using Transport.Domain.Entities;

namespace Transport.OsmReader
{
    class Program
    {
        private const int MaxFoundedStreetsInQuery = 5;

        private static string GetTagNodeAttributeValue(XPathNavigator way, string key)
        {
            var attr = way.Select($"tag[@k='{key}']/@v");
            return attr.MoveNext() ? attr.Current.ToString() : string.Empty;
        }

        private static List<long> GetNodesIds(XPathNodeIterator nodes)
        {
            var ids = new List<long>();

            while (nodes.MoveNext())
            {
                ids.Add(nodes.Current.ValueAsLong);
            }

            return ids;
        }

        /// <summary>
        /// Функция выполняет формирование полигона по идентификаторам узлов
        /// </summary>
        /// <param name="nodes">Итератор над набором узлов</param>
        /// <param name="ids">Идентификаторы узлов для выборки</param>
        /// <returns></returns>
        private static DbGeography GetPolygonFromNodesIds(XPathNodeIterator nodes, IEnumerable<long> ids)
        {
            if (!nodes.MoveNext()) return null;
            var latLonPoints = new StringBuilder();
            var selectExpr = new StringBuilder("//node[");

            var idsarr = ids as long[] ?? ids.ToArray();

            for (var i = 0; i < idsarr.Length; i++)
            {
                selectExpr.Append($"@id={idsarr[i]}");
                selectExpr.Append(i != idsarr.Length - 1 ? " or " : "]");
            }

            var selectedNodes = nodes.Current.Select(selectExpr.ToString());

            for (var i = 0; i < idsarr.Length; i++)
            {
                var node = selectedNodes.Current.SelectSingleNode($"//node[@id={idsarr[i]}]");

                if (node == null) continue;
                if (node.HasChildren && node.InnerXml.Contains("entrance")) continue;

                var lat = node.GetAttribute("lat", "");
                latLonPoints.Append(lat);
                latLonPoints.Append(" ");
                var lon = node.GetAttribute("lon", "");
                latLonPoints.Append(lon);
                latLonPoints.Append(" ");
            }

            return GeoUtils.CreatePolygon(latLonPoints.ToString());
        }

        private static TransportContext _db;
        // словарь соответствий названий улиц в Osm и 2gis
        private static Dictionary<string, string> _correspondenceStreetNames;
        private static int _correspStrCount;
        // словарь соответствий полных адресов в Osm и 2gis
        private static Dictionary<string, string> _correspondenceAddresses;
        private static int _correspAddrCount;

        static void Main(string[] args)
        {
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var filePath = Path.Combine(dir, @"Maps\Tomsk.osm");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {filePath} не найден");
                Console.ReadKey();
                return;
            }


            using (_db = new TransportContext())
            {
                // очистка площадей и полигонов перед пересчетом
                //_db.Buildings.ToList().ForEach(b =>
                //{
                //    b.Square = 0.0;
                //    b.Polygon = null;
                //});
                //_db.SaveChanges();

                var correpStreetNamesFile = new FileStream(@"Dic\correspStreet.txt", FileMode.OpenOrCreate);
                using (var reader = new StreamReader(correpStreetNamesFile))
                {
                    _correspondenceStreetNames = GetCorrespondenceDictionary(reader.ReadToEnd());
                }
                _correspStrCount = _correspondenceStreetNames.Count;

                var correspAddrFile = new FileStream(@"Dic\correspAddr.txt", FileMode.OpenOrCreate);
                using (var reader = new StreamReader(correspAddrFile))
                {
                    _correspondenceAddresses = GetCorrespondenceDictionary(reader.ReadToEnd());
                }
                _correspAddrCount = _correspondenceAddresses.Count;

                try
                {
                    using (var xmlReader = XmlReader.Create(filePath))
                    {
                        var doc = new XPathDocument(xmlReader);
                        var xpath = doc.CreateNavigator();
                        var nodes = xpath.Select("//node");
                        var ways = xpath.Select("//way[tag/@k='building']");

                        var progress = new ProgressBar(ways.Count, "Получение площадей зданий...");
                        while (ways.MoveNext())
                        {
                            // изменение ProgressBar'a
                            progress.Report(ways.CurrentPosition);

                            // Получение названия улицы из osm-файла
                            var osmStreetName = GetTagNodeAttributeValue(ways.Current, "addr:street");
                            if (osmStreetName == string.Empty) continue;
                            // Очистка навания улицы в соответствии с правилами
                            osmStreetName = CleanStreetName(osmStreetName);

                            // Получение номера дома из osm-файла
                            var osmHouseNumber = GetTagNodeAttributeValue(ways.Current, "addr:housenumber");
                            if (osmHouseNumber == string.Empty) continue;
                            // Очистка номера дома в соответствии с правилами
                            osmHouseNumber = CleanHouseNumber(osmHouseNumber);

                            // Получение идентификаторов узлов, входящих в текущее здание
                            var ids = GetNodesIds(ways.Current.Select("nd/@ref"));
                            // Получение полигона по найденнымидентификаторам узлов
                            DbGeography polygon;
                            try
                            {
                                polygon = GetPolygonFromNodesIds(nodes, ids);
                            }
                            catch (Exception)
                            {
                                continue;
                            }


                            var repeatSearch = true; // повтор поиска, если не найдено
                            var skipAddress = false; // пропуск адреса по нажатию на Esc
                            var exactAddress = new Address();
                            var needSaveStreetName = false;
                            var needSaveAddress = false;

                            // Адрес уже был найден и хранится в словаре
                            if (!_correspondenceAddresses.ContainsKey($"{osmStreetName},{osmHouseNumber}"))
                            {
                                var gisStreetNames = new List<string>(); // похожие названия улицы в 2gis
                                // Название улицы уже было найдено и хранится в словаре
                                if (!_correspondenceStreetNames.ContainsKey(osmStreetName))
                                {
                                    var gisStreetName = osmStreetName;
                                    // поиск названия улицы
                                    do
                                    {
                                        var addressesByStreet =
                                            _db.Addresses.Where(a => a.Street.Contains(gisStreetName));
                                        if (addressesByStreet.Any())
                                        {
                                            // Есть улицы с таким названием
                                            // Выбираем все названия
                                            var searchResults = addressesByStreet.Select(a => a.Street).Distinct();
                                            if (searchResults.Count() < MaxFoundedStreetsInQuery)
                                            {
                                                gisStreetNames = searchResults.ToList();
                                                repeatSearch = false;
                                            }
                                            else
                                            {
                                                progress.Pause();
                                                Console.Write(
                                                    $"\r\n***\r\nВыполняется поиск адреса {osmStreetName}, {osmHouseNumber}" +
                                                    $"По названию улицы \"{gisStreetName}\" найдено слишком большее количество совпадений." +
                                                    "Уточните запрос, либо пропустите адрес (Esc): ");
                                                gisStreetName = ConsoleReadLineWithSkip();
                                                needSaveStreetName = true;
                                                if (gisStreetName == null)
                                                {
                                                    skipAddress = true;
                                                    break;
                                                }
                                                if (gisStreetName == string.Empty)
                                                {
                                                    gisStreetName = osmStreetName;
                                                    needSaveStreetName = false;
                                                }

                                                progress.Start();
                                            }
                                        }
                                        else
                                        {
                                            // Не найдено улицы с таким названием
                                            progress.Pause();
                                            Console.WriteLine("\r\n****");
                                            Console.WriteLine(
                                                $"Выполняется поиск адреса {osmStreetName}, {osmHouseNumber}");
                                            Console.Write(
                                                $"Улица \"{gisStreetName}\" не найдена. (Esc - пропуск) \r\nНовое название: ");

                                            gisStreetName = ConsoleReadLineWithSkip();
                                            needSaveStreetName = true;
                                            if (gisStreetName == null)
                                            {
                                                skipAddress = true;
                                                break;
                                            }
                                            if (gisStreetName == string.Empty)
                                            {
                                                gisStreetName = osmStreetName;
                                                needSaveStreetName = false;
                                            }
                                            progress.Start();
                                        }
                                    } while (repeatSearch);
                                }
                                else
                                {
                                    // Добавляем в результирующий список один элемент из словаря
                                    gisStreetNames.Add(_correspondenceStreetNames[osmStreetName]);
                                }

                                repeatSearch = true;
                                if (skipAddress) continue;

                                var gisHouseNumber = osmHouseNumber;
                                // поиск полного адреса
                                do
                                {
                                    var addressByStreetAndHouse =
                                        _db.Addresses.Where(
                                            a => gisStreetNames.Contains(a.Street) && a.Number == gisHouseNumber);

                                    if (addressByStreetAndHouse.Count() == 1)
                                    {
                                        // Если найден точный адрес
                                        exactAddress = addressByStreetAndHouse.First();
                                        repeatSearch = false;
                                    }
                                    else
                                    {
                                        // Если точный адрес не найден - требуется уточнить запрос
                                        progress.Pause();
                                        Console.Write(
                                            $"\r\n****\r\nВыполняется поиск адреса {osmStreetName}, {osmHouseNumber}" +
                                            "\r\nТочный результат не найден.");
                                        // получаем базовый номер дома для поиска вариантов
                                        var baseHouseNumber = GetBaseHouseNumber(osmHouseNumber);

                                        // выбираем адреса с базовым номером дома
                                        var searchResults =
                                            _db.Addresses.Where(
                                                a => gisStreetNames.Contains(a.Street)
                                                     && a.Number.StartsWith(baseHouseNumber));

                                        var matchesSearchResult =
                                            searchResults.AsEnumerable()
                                                .Where(a => Regex.IsMatch(a.Number, $@"^{baseHouseNumber}($|\D)"))
                                                .ToList();

                                        Console.Write(matchesSearchResult.Any() ? " Варианты:\r\n" : " Введите адрес: ");

                                        var resultsArray = matchesSearchResult.ToArray();
                                        for (var i = 0; i < resultsArray.Length; i++)
                                        {
                                            Console.WriteLine(
                                                $"{i + 1}. {resultsArray[i].Street}, {resultsArray[i].Number}");
                                        }

                                        // обработка пропуска адреса (нажатия Esc)
                                        var gisHouseNumberOrIndex = ConsoleReadLineWithSkip();
                                        needSaveAddress = true;
                                        if (gisHouseNumberOrIndex == null)
                                        {
                                            skipAddress = true;
                                            break;
                                        }
                                        if (gisHouseNumberOrIndex == string.Empty)
                                        {
                                            gisHouseNumber = osmHouseNumber;
                                            needSaveAddress = false;
                                        }
                                        else
                                        {
                                            // обработка выбранного варианта из предложеных
                                            int selectedIndx;
                                            if (int.TryParse(gisHouseNumberOrIndex, NumberStyles.Integer,
                                                CultureInfo.InvariantCulture, out selectedIndx)
                                                && selectedIndx - 1 >= 0 && resultsArray.Length > selectedIndx - 1)
                                            {
                                                exactAddress = resultsArray[selectedIndx - 1];
                                                repeatSearch = false;
                                            }
                                            else
                                            {
                                                var sn = gisHouseNumberOrIndex.Split(new[] { "," },
                                                    StringSplitOptions.RemoveEmptyEntries);
                                                if (sn.Length == 2)
                                                {
                                                    gisStreetNames.Clear();
                                                    gisStreetNames.Add(sn[0].Trim());
                                                    gisHouseNumber = sn[1].Trim();
                                                }
                                                else
                                                {
                                                    gisHouseNumber = osmHouseNumber;
                                                    needSaveAddress = false;
                                                }
                                            }
                                        }
                                        progress.Start();
                                    }
                                } while (repeatSearch);
                            }
                            else
                            {
                                var sn = _correspondenceAddresses[$"{osmStreetName},{osmHouseNumber}"]
                                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                exactAddress.Street = sn[0];
                                exactAddress.Number = sn[1];
                            }

                            if (skipAddress) continue;

                            // если был найден объект точного адреса, то поиск посуществлять не требуется
                            var address = exactAddress.AddressId != 0
                                ? exactAddress
                                : _db.Addresses.FirstOrDefault(
                                    a => a.Street == exactAddress.Street && a.Number == exactAddress.Number);

                            // если найденный адрес отличается от исходного (были переименования)
                            // то предлагается сохранить соответствие
                            if (needSaveStreetName)
                            {
                                progress.Pause();
                                Console.WriteLine(
                                    $"\r\n****\r\nСохраниь соответствие названий улицы: \"{osmStreetName}\" - \"{exactAddress.Street}\" (Y/N)?");
                                if (Console.ReadKey().Key == ConsoleKey.Y)
                                {
                                    _correspondenceStreetNames.Add(osmStreetName, exactAddress.Street);
                                }
                            }
                            if (needSaveAddress)
                            {
                                progress.Pause();
                                Console.WriteLine(
                                    $"\r\n****\r\nСохраниь соответствие адресов: \"{osmStreetName}, {osmHouseNumber}\" - \"{exactAddress.Street}, {exactAddress.Number}\" (Y/N)?");
                                if (Console.ReadKey().Key == ConsoleKey.Y)
                                {
                                    _correspondenceAddresses.Add($"{osmStreetName},{osmHouseNumber}",
                                        $"{exactAddress.Street},{exactAddress.Number}");
                                }
                            }

                            progress.Start();
                            var building = _db.Buildings.Find(address?.BuildingId);

                            if (building != null)
                            {
                                var square = polygon.Area ?? 0;

                                if (building.Polygon != null)
                                {
                                    square += building.Square;
                                    polygon = polygon.Union(building.Polygon);
                                }

                                building.Polygon = polygon;
                                building.Square = square;
                            }
                        }
                        progress.Pause();
                    }
                    Console.WriteLine("Выполнено");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Произошла ошибка: {e.Message}");
                }
                finally
                {
                    SaveCurrentResults();
                }
            }

            Console.ReadKey(true);
        }

        private static void SaveCurrentResults()
        {
            Console.Write("Выполняется сохранение данных:\r\n" +
                                      "Словарь соответствий названий улиц... ");
            using (var writer = new StreamWriter(@"correspStreet.txt", true))
            {
                foreach (var correspStr in _correspondenceStreetNames.Skip(_correspStrCount))
                {
                    writer.WriteLine($"{correspStr.Key};{correspStr.Value}");
                }
                _correspStrCount = _correspondenceStreetNames.Count;
            }
            Console.Write("Выполнено\r\nСловарь соответствий адресов...");
            using (var writer = new StreamWriter(@"correspAddr.txt", true))
            {
                foreach (var correspAddr in _correspondenceAddresses.Skip(_correspAddrCount))
                {
                    writer.WriteLine($"{correspAddr.Key};{correspAddr.Value}");
                }
                _correspAddrCount = _correspondenceAddresses.Count;
            }
            Console.Write("Выполнено\r\nДанные в БД... ");
            _db.SaveChanges();
            Console.Write("Выполнено");
        }

        // Чтение строки из консоли с проверкой на отмену (нажатие Esc)
        private static string ConsoleReadLineWithSkip()
        {
            var firstInputKey = Console.ReadKey(true);
            if (firstInputKey.Key == ConsoleKey.Escape)
            {
                return null;
            }
            else if (firstInputKey.Key == ConsoleKey.W && firstInputKey.Modifiers == ConsoleModifiers.Control)
            {
                SaveCurrentResults();
                return string.Empty;
            }
            else if (firstInputKey.Key == ConsoleKey.Q && firstInputKey.Modifiers == ConsoleModifiers.Control)
            {
                Environment.Exit(0);
            }
            else if (char.IsLetterOrDigit(firstInputKey.KeyChar))
            {
                Console.Write(firstInputKey.KeyChar);
                return firstInputKey.KeyChar + Console.ReadLine();
            }
            else
            {
                return Console.ReadLine();
            }

            return null;
        }

        private static Dictionary<string, string> GetCorrespondenceDictionary(string text)
        {
            var dic = new Dictionary<string, string>();
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var keyVal = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyVal.Length == 2)
                {
                    dic.Add(keyVal[0], keyVal[1]);
                }
                else
                {
                    throw new Exception("Не удалось получить данные для словаря");
                }
            }

            return dic;
        }

        /// <summary>
        /// Выполняет изменение названия улицы в соответствиями с правилами 2Gis (проспект, улица и т.д.)
        /// </summary>
        /// <param name="osmStreet"></param>
        /// <returns></returns>
        private static string CleanStreetName(string osmStreet)
        {
            var lowerStreetName = osmStreet.ToLower();

            var prefixes = new[] { "улица", "проспект", "переулок", "площадь", "проезд" };

            foreach (var prefix in prefixes)
            {
                if (lowerStreetName.Contains(prefix))
                {
                    osmStreet = osmStreet.Remove(lowerStreetName.IndexOf(prefix, StringComparison.CurrentCulture), prefix.Length);
                    osmStreet = osmStreet.Trim();

                    if (prefix != "улица")
                    {
                        var sb = new StringBuilder(osmStreet);
                        sb.Append($" {prefix}");
                        osmStreet = sb.ToString();
                    }

                    break;
                }
            }

            return osmStreet;
        }

        private static string CleanHouseNumber(string osmHouseNumber)
        {
            osmHouseNumber = osmHouseNumber.ToLower();

            if (osmHouseNumber.Contains("строение "))
            {
                osmHouseNumber = osmHouseNumber.Replace("строение ", "ст");
            }

            return osmHouseNumber;
        }

        private static string GetBaseHouseNumber(string houseNumber)
        {
            var baseHouseNumber = new StringBuilder();
            var i = 0;
            var hnLen = houseNumber.Length;
            while (i < hnLen && char.IsDigit(houseNumber[i]))
            {
                baseHouseNumber.Append(houseNumber[i]);
                i++;
            }

            return baseHouseNumber.ToString();
        }
    }
}
