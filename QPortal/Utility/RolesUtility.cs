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

        public static List<Farms> AssignFarmRoles(List<FarmRoles> FarmRoles)
        {
            List<Farms> FarmList = new List<Farms>();

            List<string> farmsId = new List<string>();
            farmsId = (from f in FarmRoles
                       select f.FarmId).ToList();

            FarmList = FarmsUtility.GetFarmsById(farmsId);

            foreach (var role in FarmRoles)
            {
                FarmList = AssignRoles(role.FarmId, role.Role, FarmList);
            }

            return FarmList;
        }

        public static List<Farms> AssignRoles(string farmId, string farmRole, List<Farms> FarmList)
        {
            List<Farms> Farms = new List<Farms>(FarmList);

            var farmNodes = (from r in FarmList
                             where r.Id.Equals(farmId)
                             select r)
                            .SelectMany(r => r.Nodes)
                            .ToList();

            foreach (var node in farmNodes)
            {
                switch (farmRole)
                {
                    case RoleName.User:
                        if (node.Id == 0)
                        {
                            node.btnEnterClass = "";
                            node.btnArchiveClass = "disabled";
                            node.btnDistributeClass = "disabled";
                        }
                        else
                        {
                            node.btnEnterClass = "disabled";
                            node.btnArchiveClass = "disabled";
                            node.btnDistributeClass = "disabled";
                        }
                        break;
                    case RoleName.Developer:
                        if (node.Id == 0)
                        {
                            node.btnEnterClass = "";
                            node.btnArchiveClass = "";
                            node.btnDistributeClass = "disabled";
                        }
                        else
                        {
                            node.btnEnterClass = "";
                            node.btnArchiveClass = "";
                            node.btnDistributeClass = "disabled";
                        }
                        break;
                    case RoleName.Supervisor:
                        if (node.Id == 0)
                        {
                            node.btnEnterClass = "";
                            node.btnArchiveClass = "";
                            node.btnDistributeClass = "";
                        }
                        else
                        {
                            node.btnEnterClass = "";
                            node.btnArchiveClass = "";
                            node.btnDistributeClass = "";
                        }
                        break;
                }
            }

            return Farms;
        }
    }
}