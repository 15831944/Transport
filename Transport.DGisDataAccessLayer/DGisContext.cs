using System;
using System.Runtime.InteropServices;
using GrymCore;
using Transport.DGis.DataAccessLayer.Repository;

namespace Transport.DGis.DataAccessLayer
{
    public class DGisContext : IDisposable
    {
        private readonly IGrym _grymApp;
        private readonly IBaseReference _baseRef;
        private readonly IBaseViewThread _baseViewTread;
        private readonly IDataRow _cityDataRow;
        
        public DGisContext(string city)
        {
            // Создаем объект приложения Grym.
            // Если приложение не было запущено, то при
            // первом же обращении к объекту оно запустится.
            _grymApp = new GrymClass();

            // Получаем описание файла данных для заданного города
            // из коллекции описаний.
            _baseRef = _grymApp.BaseCollection.FindBase(city);
            if (_baseRef == null)
            {
                throw new ArgumentException($"Файл данных указанного города \"{city}\" не найден");
            }

            // Получаем оболочку просмотра данных по описанию файла данных
            _baseViewTread = _grymApp.GetBaseView(_baseRef, true, false);
            if (_baseViewTread == null)
            {
                throw new Exception("Не удалось запустить оболочку просмотра данных");
            }

            // Получение объекта Города
            var cityTable = _baseViewTread.Database.Table["grym_map_city"];
            for (var i = 1; i <= cityTable.RecordCount; i++)
            {
                _cityDataRow = cityTable.GetRecord(i);
                var cityValue = (IDataRow)_cityDataRow.Value["city"];
                if ((string) cityValue.Value["name"] == city) break;
            }

            if (_cityDataRow == null)
            {
                throw new Exception("Не удалось найти объект города для выполнения запросов");
            }

        }


        public AddressRepository Addresses => new AddressRepository(_baseViewTread.Database.Table["grym_address"], _baseViewTread);
        public BuildingsRepository Buildings => new BuildingsRepository(_cityDataRow,
            _baseViewTread.Database.Table["grym_map_building"], _baseViewTread);
        public BusstopsRepository Busstops => new BusstopsRepository(_cityDataRow, 
            _baseViewTread.Database.Table["grym_map_stationbay"], _baseViewTread);
        public OrgRepository Orgs => new OrgRepository(_baseViewTread.Database.Table["grym_org"], _baseViewTread);
        public OrgRub1Repository OrgRubs1 => new OrgRub1Repository(_baseViewTread.Database.Table["grym_rub1"], _baseViewTread);
        public OrgRub2Repository OrgRubs2 => new OrgRub2Repository(_baseViewTread.Database.Table["grym_rub2"], _baseViewTread);
        public OrgRub3Repository OrgRubs3 => new OrgRub3Repository(_baseViewTread.Database.Table["grym_rub3"], _baseViewTread);
        

        public void Dispose()
        {
            if (_grymApp != null)
                Marshal.FinalReleaseComObject(_grymApp);
            if (_baseRef != null)
                Marshal.FinalReleaseComObject(_baseRef);
            if (_baseViewTread != null)
                Marshal.FinalReleaseComObject(_baseViewTread);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
