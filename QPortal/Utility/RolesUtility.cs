using QPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QPortal.Utility
{
    public class FarmRoles
    {
        public string FarmId { get; set; }
        public string Role { get; set; }
    }

    public class RolesUtility
    {

        public static List<FarmRoles> GetRoles(XDocument root, List<string> roles)
        {
            List<FarmRoles> FarmRoles = new List<FarmRoles>();

            if (root != null)
            {
                //get the roles with the highest priority
                var result = (from r in root.Elements("farms") select r)
                                .SelectMany(r => r.Elements("role"))
                                .Where(r => r.Attributes("id").Any(x => roles.Contains(x.Value)))
                                .OrderByDescending(el => el.Attribute("priority").Value)
                                .GroupBy(p => p.Attribute("farm").Value)
                                .Select(g => g.First())
                                .ToList();

                foreach (var item in result)
                {
                    FarmRoles selectedRole = (from r in result
                                              where r.Equals(item)
                                              select new FarmRoles
                                              {
                                                  FarmId = r.Attribute("farm").Value,
                                                  Role = r.Value
                                              }).FirstOrDefault();

                    if (selectedRole != null)
                        FarmRoles.Add(selectedRole);
                }
                //order by FarmId
                FarmRoles = FarmRoles.OrderBy(f => f.FarmId).ToList();
            }
            return FarmRoles;
        }

        public static string GetFarmRoleById(int farmId, List<string> roles)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.RolesXML);
            XDocument root = FarmsUtility.GetXmlDocument(path);

            string role = null;
            if(root != null)
            {
                 role = root.Elements("farms").Elements("role")
                            .Where(r => r.Attributes("id").Any(x => roles.Contains(x.Value)))
                            .Where(r => r.Attribute("farm").Value.Equals(farmId.ToString()))
                            .OrderByDescending(p => p.Attribute("priority").Value)
                            .Select(r => r.Value).FirstOrDefault();
            }

            return role;
        }

        public static List<Farms> AssignFarmRoles(List<FarmRoles> FarmRoles)
        {
            List<Farms> FarmList = new List<Farms>();

            List<string> farmsId = new List<string>();
            farmsId = (from f in FarmRoles
                       select f.FarmId).ToList();

            FarmList = FarmsUtility.GetFarmsById(farmsId);

            foreach (var farm in FarmList)
            {
                string role = FarmRoles.Where(r => r.FarmId.Equals(farm.Id)).Select(x => x.Role).SingleOrDefault().ToString();

                farm.role = role;
            }

            return FarmList;
        }

    }
}