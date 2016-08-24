using System.Collections.Generic;
using System.Linq;
using Transport.Aca3.Models;
using Transport.Aca3.ViewModels;
using Transport.DataAccessLayer;

namespace Transport.Aca3.Services
{
    public class DataAccessService : IDataAccessService
    {
        public List<NodeViewModel> GetGraphNodes()
        {
            var result = new List<NodeViewModel>();

            using (var db = new TransportContext())
            {
                result.AddRange(db.Areas.Select(area => new NodeViewModel
                {
                    Id = area.AreaId,
                    Name = area.Name,
                    Lat = area.Location.Latitude ?? 0.0,
                    Lon = area.Location.Longitude ?? 0.0
                }));

                foreach (var areaRoute in db.AreaRoutes)
                {
                    var origin = result.Find(n => n.Id == areaRoute.OriginId);
                    var dest = result.Find(n => n.Id == areaRoute.DestinationId);

                    origin.Edges.Add(new EdgeViewModel {Source = origin, Dest = dest, Distance = areaRoute.Distance});
                }
            }

            return result;
        }
    }
}
