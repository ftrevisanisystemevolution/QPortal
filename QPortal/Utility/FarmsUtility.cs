using QPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace QPortal.Utility
{
    public class FarmsUtility
    {
        public static XDocument GetXmlDocument(string path)
        {
            XDocument root = new XDocument();
            try
            {
                root = XDocument.Load(path);
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
            return root;
        }

        public static List<Farms> GetFarmsById(List<string> farmsId)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.FarmsXML);
            XDocument root = GetXmlDocument(path);

            List<Farms> Farms = new List<Farms>();

            if (root != null)
            {

                var farms = (from r in root.Elements("farms") select r)
                                .SelectMany(r => r.Elements("farm"))
                                .Where(r => r.Attributes("id").Any(x => farmsId.Contains(x.Value)))
                                .ToList();

                foreach (var farm in farms)
                {
                    List<Node> FarmNodes = new List<Node>();
                    var nodes = (from r in farms.Elements("node") select r)
                                .Where(el => el.Parent.Attribute("id").Equals(farm.Attribute("id")))
                                .ToList();

                    foreach (var node in nodes)
                    {
                        Node n = new Node
                        {
                            Id = Convert.ToInt32(node.Attribute("id").Value),
                            IdFarmNode = farm.Attribute("id").Value + "|" + node.Attribute("id").Value,
                            Server = node.Attribute("server").Value,
                            VirtualProxy = node.Attribute("vp").Value,
                            Name = farm.Attribute("name").Value + " - " + node.Value
                        };

                        if (n != null)
                            FarmNodes.Add(n);
                    }
                    Farms.Add(new Farms { Id = farm.Attribute("id").Value, Name = farm.Attribute("name").Value, Nodes = FarmNodes });
                }
            }

            return Farms;
        }

        public static Node GetFarmNode(string id, string node)
        {
            return GetFarmNode(Convert.ToInt32(id), Convert.ToInt32(node));
        }

        public static Node GetFarmNode(int id, int node)
        {
            Node result = new Node();

            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.FarmsXML);
            XDocument root = GetXmlDocument(path);

            if (root != null)
            {
                result = (from f in root.Elements("farms").Elements("farm").Elements("node")
                             where f.Parent.Attribute("id").Value.Equals(id.ToString())
                             select f)
                          .Where(f => f.Attribute("id").Value.Equals(node.ToString()))
                          .Select(f => new Node
                          {
                              Id = Convert.ToInt32(f.Attribute("id").Value),
                              Name = f.Value,
                              Server = f.Attribute("server").Value,
                              VirtualProxy = f.Attribute("vp").Value,
                              Link = f.Attribute("link").Value
                          }).SingleOrDefault();
            }

            return result;
        }

        public static string GetFarmIdByName(string farmName)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.FarmsXML);
            XDocument root = FarmsUtility.GetXmlDocument(path);

            string farmId = null;
            if (root != null)
            {
                farmId = (from f in root.Elements("farms").Elements("farm").Elements("node")
                          where f.Value.Equals(farmName)
                          select f)
                         .Select(f => f.Parent.Attribute("id").Value).SingleOrDefault();
            }
            return farmId;
        }
    }
}