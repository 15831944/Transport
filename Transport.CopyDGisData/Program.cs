using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Transport.Common;
using Transport.DataAccessLayer;
using Transport.DGis.DataAccessLayer;
using Transport.Domain.Entities;

namespace Transport.CopyDGisData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выполнение завершено. Нажмите любую кнопку...");
            Console.ReadKey();
        }

        static void CopyBusstops()
        {
            using (var dgis = new DGisContext("Томск"))
            {
                using (var transportCtx = new TransportContext())
                {
                    Console.Write("Копирование остановок... ");

                    var dgisBusstops = dgis.Busstops.GetAll();

                    foreach (var dgisBusstop in dgisBusstops)
                    {
                        if (dgisBusstop.Name.Contains("(трамвай)")) continue;

                        transportCtx.Busstops.Add(new Busstop
                        {
                            Name = dgisBusstop.Name,
                            Location = GeoUtils.CreatePoint(dgisBusstop.PosY, dgisBusstop.PosX)
                        });
                    }

                    transportCtx.SaveChanges();

                    Console.WriteLine("завершено");
                }
            }
        }

        static void CopyBuildings()
        {
            using (var dgis = new DGisContext("Томск"))
            {
                using (var transportCtx = new TransportContext())
                {
                    Console.Write("Копирование зданий... ");

                    var dgisBuildings = dgis.Buildings.GetAll();

                    foreach (var dgisBuilding in dgisBuildings)
                    {
                        var building = new Building
                        {
                            Location = GeoUtils.CreatePoint(dgisBuilding.PosY, dgisBuilding.PosX),
                            Levels = dgisBuilding.Levels,
                            PostIndex = dgisBuilding.PostIndex,
                            Purpose = dgisBuilding.Purpose,
                        };

                        foreach (var addressesId in dgisBuilding.AddressesIds)
                        {
                            var dgisAddress = dgis.Addresses.GetById(addressesId);
                            building.Addresses.Add(new Address
                            {
                                Number = dgisAddress.Number,
                                Street = dgisAddress.Street,
                                Building = building
                            });
                        }
                        transportCtx.Buildings.Add(building);
                    }

                    transportCtx.SaveChanges();

                    Console.WriteLine("завершено");
                }
            }
        }

        static void CopyOrgs()
        {
            using (var dgis = new DGisContext("Томск"))
            {
                using (var transportCtx = new TransportContext())
                {
                    Console.Write("Копирование категорий предприятий... ");

                    var dgisOrgRubs1 = dgis.OrgRubs1.GetAll();
                    var orgRubs = new List<OrgRub>();
                    foreach (var orgRub1 in dgisOrgRubs1)
                    {
                        orgRubs.Add(new OrgRub
                        {
                            Name = orgRub1.Name,
                            ParentOrgRub = null
                        });
                    }

                    var dgisOrgRubs2 = dgis.OrgRubs2.GetAll();
                    foreach (var orgRub2 in dgisOrgRubs2)
                    {
                        if (orgRubs.Any(rub => rub.Name == orgRub2.Name)) continue;

                        var parentRubName = dgis.OrgRubs1.GetById(orgRub2.OrgRub1Id).Name;

                        orgRubs.Add(new OrgRub
                        {
                            Name = orgRub2.Name,
                            ParentOrgRub = orgRubs.FirstOrDefault(rub => rub.Name == parentRubName)
                        });
                    }

                    var dgisOrgRubs3 = dgis.OrgRubs3.GetAll();
                    foreach (var orgRub3 in dgisOrgRubs3)
                    {
                        if (orgRubs.Any(rub => rub.Name == orgRub3.Name)) continue;

                        var parentRubName = dgis.OrgRubs2.GetById(orgRub3.OrgRub2Id).Name;

                        orgRubs.Add(new OrgRub
                        {
                            Name = orgRub3.Name,
                            ParentOrgRub = orgRubs.FirstOrDefault(rub => rub.Name == parentRubName)
                        });
                    }

                    transportCtx.OrgRubs.AddRange(orgRubs);
                    transportCtx.SaveChanges();
                    Console.WriteLine("завершено");

                    Console.Write("Копирование предприятий... ");

                    var dgisOrgs = dgis.Orgs.GetAll();
                    foreach (var dgisOrg in dgisOrgs)
                    {
                        var org = new Org
                        {
                            Name = dgisOrg.Name,
                        };

                        foreach (var orgRubs3Id in dgisOrg.OrgRubs3Ids)
                        {
                            var dgisOrgRub3 = dgis.OrgRubs3.GetById(orgRubs3Id);
                            var orgRub = transportCtx.OrgRubs.FirstOrDefault(rub => rub.Name == dgisOrgRub3.Name);
                            if (orgRub == null) continue;
                            org.OrgRubs.Add(orgRub);
                        }

                        foreach (var dgisOrgFil in dgisOrg.OrgFils)
                        {
                            var orgFil = new OrgFil
                            {
                                Org = org
                            };

                            foreach (var addressesId in dgisOrgFil.AddressesIds)
                            {
                                var dgisAddress = dgis.Addresses.GetById(addressesId);
                                if (dgisAddress.BuildingId == null) continue;

                                var address =
                                    transportCtx.Addresses.FirstOrDefault(addr => addr.Number == dgisAddress.Number
                                    && addr.Street == dgisAddress.Street);
                                if (address == null) continue;
                                orgFil.BuildingId = address.BuildingId;
                                break;
                            }

                            if (orgFil.BuildingId == 0) continue;

                            org.OrgFils.Add(orgFil);
                        }

                        transportCtx.Orgs.Add(org);
                    }

                    transportCtx.SaveChanges();
                    Console.WriteLine("завершено");
                }
            }
        }

        static void ClearDatabase()
        {
            using (var transportCtx = new TransportContext())
            {
                if (!transportCtx.Addresses.Any()
                    && !transportCtx.Buildings.Any()
                    && !transportCtx.Busstops.Any()
                    && !transportCtx.OrgFils.Any()
                    && !transportCtx.OrgRubs.Any()
                    && !transportCtx.Orgs.Any())
                    return;

                Console.Write("Очистка данных БД... ");

                transportCtx.Addresses.RemoveRange(transportCtx.Addresses);
                transportCtx.Buildings.RemoveRange(transportCtx.Buildings);
                transportCtx.Busstops.RemoveRange(transportCtx.Busstops);
                transportCtx.Orgs.RemoveRange(transportCtx.Orgs);
                transportCtx.OrgFils.RemoveRange(transportCtx.OrgFils);
                transportCtx.OrgRubs.RemoveRange(transportCtx.OrgRubs);
                transportCtx.SaveChanges();

                Console.WriteLine("завершено");
            }
        }

        static void ReturnAddresses()
        {
            using (var db = new TransportContext())
            {
                using (var dgis = new DGisContext("Томск"))
                {
                    var query = db.Buildings.Include(b => b.Addresses).Where(b => !b.Addresses.Any());
                    var progress = new Common.ProgressBar(query.Count(), "Восстанавливаем адреса... ");
                    var current = 0;
                    foreach (var buildingNoAddr in query)
                    {
                        progress.Report(current++);
                        DGis.Domain.Entities.Building dgisBuilding;
                        try
                        {
                            dgisBuilding = dgis.Buildings.GetBuilingByCoords(buildingNoAddr.Location.Latitude.Value,
                               buildingNoAddr.Location.Longitude.Value);
                        }
                        catch (InvalidOperationException e)
                        {
                            continue;
                        }

                        if (dgisBuilding == null) continue;

                        foreach (var addressesId in dgisBuilding.AddressesIds)
                        {
                            var dgisAddress = dgis.Addresses.GetById(addressesId);
                            buildingNoAddr.Addresses.Add(new Address
                            {
                                Number = dgisAddress.Number,
                                Street = dgisAddress.Street,
                                BuildingId = buildingNoAddr.BuildingId
                            });
                        }
                    }

                    progress.Pause();
                }

                Console.WriteLine("\r\nСохраняем в БД");
                db.SaveChanges();
            }
        }
    }
}
