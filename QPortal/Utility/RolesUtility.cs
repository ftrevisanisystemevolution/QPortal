using QPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QPortal.Utility
{
    public class AmbitoRoles
    {
        public string AmbitoId { get; set; }
        public string Role { get; set; }
    }

    public class RolesUtility
    {

        public static List<AmbitoRoles> GetRoles(XDocument root, List<string> roles)
        {
            List<AmbitoRoles> AmbitoRoles = new List<AmbitoRoles>();

            if (root != null)
            {
                //get the roles with the highest priority
                var result = (from r in root.Elements("ambiti") select r)
                                .SelectMany(r => r.Elements("role"))
                                .Where(r => r.Attributes("id").Any(x => roles.Contains(x.Value)))
                                .OrderByDescending(el => el.Attribute("priority").Value)
                                .GroupBy(p => p.Attribute("ambito").Value)
                                .Select(g => g.First())
                                .ToList();

                foreach (var item in result)
                {
                    AmbitoRoles selectedRole = (from r in result
                                              where r.Equals(item)
                                              select new AmbitoRoles
                                              {
                                                  AmbitoId = r.Attribute("ambito").Value,
                                                  Role = r.Value
                                              }).FirstOrDefault();

                    if (selectedRole != null)
                        AmbitoRoles.Add(selectedRole);
                }
                AmbitoRoles = AmbitoRoles.OrderBy(f => f.AmbitoId).ToList();
            }
            return AmbitoRoles;
        }

        public static string GetAmbitoRoleById(int ambitoId, List<string> roles)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.RolesXML);
            XDocument root = AmbitiUtility.GetXmlDocument(path);

            string role = null;
            if(root != null)
            {
                 role = root.Elements("ambiti").Elements("role")
                            .Where(r => r.Attributes("id").Any(x => roles.Contains(x.Value)))
                            .Where(r => r.Attribute("ambito").Value.Equals(ambitoId.ToString()))
                            .OrderByDescending(p => p.Attribute("priority").Value)
                            .Select(r => r.Value).FirstOrDefault();
            }

            return role;
        }

        public static List<Ambiti> AssignAmbitoRoles(List<AmbitoRoles> AmbitoRoles)
        {
            List<Ambiti> AmbitoList = new List<Ambiti>();

            List<string> ambitiId = new List<string>();
            ambitiId = (from f in AmbitoRoles
                       select f.AmbitoId).ToList();

            AmbitoList = AmbitiUtility.GetAmbitoById(ambitiId);

            foreach (var ambito in AmbitoList)
            {
                string role = AmbitoRoles.Where(r => r.AmbitoId.Equals(ambito.Id)).Select(x => x.Role).SingleOrDefault().ToString();

                ambito.role = role;
            }

            return AmbitoList;
        }

    }
}