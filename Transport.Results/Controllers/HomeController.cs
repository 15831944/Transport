using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Transport.DataAccessLayer;
using System.Windows.Media;
using Transport.Results.Extensions;

namespace Transport.Results.Controllers
{
    public class HomeController : Controller
    {
        private readonly TransportContext _db = new TransportContext();

        public ActionResult Index()
        {
            var areas = _db.Areas.Select(a => new {Id = a.AreaId, Name = a.AreaId + ". " + a.Name});
            ViewBag.Centres = new SelectList(areas, "Id", "Name");

            return View();
        }

        public JsonResult GetAreas()
        {
            var areas = _db.Areas.Include(a => a.Buildings).Include(a => a.Busstops).ToList();
            var minWeight = int.MaxValue;
            var maxWeight = int.MinValue;
            var avgWeight = (int)areas.Average(a => a.People);

            foreach (var area in areas)
            {
                if (area.People < minWeight) minWeight = area.People;
                if (area.People > maxWeight) maxWeight = area.People;
            }


            var data = new
            {
                type = "FeatureCollection",
                features = areas.Select(a => new
                {
                    type = "Feature",
                    id = a.AreaId,
                    geometry = new
                    {
                        type = "Point",
                        coordinates = new[] { a.Location.Latitude, a.Location.Longitude }
                    },
                    properties = new
                    {
                        balloonContentBody = "Название: " + a.Name,
                        name = "" + a.Name.ToString(),
                        location = "" + a.Location,
                        weight = a.People,
                        station =  a.Busstops.Where(b => b.AreaId == a.AreaId).Select(b => new {
                            id = b.BusstopId,
                            name = b.Name
                        }),
                        buildings=a.Buildings.Count,
                        iconContent = a.AreaId
                    },                    
                    options = new
                    {
                        preset = "islands#circleDotIcon",
                        iconColor = WeightToColor(a.People, minWeight, avgWeight, maxWeight, Colors.Green, Colors.Yellow, Colors.Red).ToHtml()
                    }
                }).ToList()
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Перевод значения в диапазоне в цвет
        /// </summary>
        /// <param name="weight">значение</param>
        /// <param name="minWeight">минимальное значение</param>
        /// <param name="avgWeight"></param>
        /// <param name="maxWeight">максимальное значение</param>
        /// <param name="minColor">цвет минимального значения</param>
        /// <param name="avgColor">цвет среднего значения</param>
        /// <param name="maxColor">цвет максимального значения</param>
        /// <returns></returns>
        private Color WeightToColor(
            int weight,
            int minWeight,
            int avgWeight,
            int maxWeight,
            Color minColor,
            Color avgColor,
            Color maxColor)
        {
            var midWeight = avgWeight; // возможно требуеся пересчитать через avg

            Color lColor, rColor;
            float delta;
            if (weight < midWeight)
            {
                lColor = minColor;
                rColor = avgColor;
                delta = (float) weight / (midWeight - minWeight);
            }
            else
            {
                lColor = avgColor;
                rColor = maxColor;
                delta = (float) weight / (maxWeight - midWeight);
            }

            var dClr = Color.Subtract(rColor, lColor);

            return Color.Add(lColor, Color.FromScRgb(dClr.ScA, dClr.ScR * delta, dClr.ScG * delta,
                dClr.ScB * delta));
        }
    }
}