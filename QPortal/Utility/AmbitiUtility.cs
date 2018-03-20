using QPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace QPortal.Utility
{
    public class AmbitiUtility
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

        public static List<Ambiti> GetAmbitiById(List<string> ambitiId)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.AmbitiXML);
            XDocument root = GetXmlDocument(path);

            List<Ambiti> Ambiti = new List<Ambiti>();

            if (root != null)
            {

                var ambiti = (from r in root.Elements("ambiti") select r)
                                .SelectMany(r => r.Elements("ambito"))
                                .Where(r => r.Attributes("id").Any(x => ambitiId.Contains(x.Value)))
                                .ToList();

                foreach (var ambito in ambiti)
                {
                    List<Node> AmbitoNodes = new List<Node>();
                    var nodes = (from r in ambiti.Elements("node") select r)
                                .Where(el => el.Parent.Attribute("id").Equals(ambito.Attribute("id")))
                                .ToList();

                    foreach (var node in nodes)
                    {
                        Node n = new Node
                        {
                            Id = Convert.ToInt32(node.Attribute("id").Value),
                            IdAmbitoNode = ambito.Attribute("id").Value + "|" + node.Attribute("id").Value,
                            Server = node.Attribute("server").Value,
                            VirtualProxy = node.Attribute("vp").Value,
                            Name = ambito.Attribute("name").Value + " - " + node.Value,
                            UrlWebTicket = node.Attribute("urlWebTicket").Value,
                            Link = node.Attribute("link").Value,
                            NodeType = node.Attribute("type").Value
                        };

                        if (n != null)
                            AmbitoNodes.Add(n);
                    }
                    Ambiti.Add(new Ambiti
                    {
                        Id = ambito.Attribute("id").Value,
                        Name = ambito.Attribute("name").Value,
                        Nodes = AmbitoNodes,
                        superuserid = ambito.Attribute("superuserid").Value,
                        superuserdom = ambito.Attribute("superuserdom").Value,
                        centralnode = ambito.Attribute("centralnode").Value
                    }
                    );
                }
            }

            return Ambiti;
        }

        public static Ambiti GetAmbitoById(string ambitoId)
        {
            return GetAmbitiById(new List<string>() { ambitoId }).FirstOrDefault();
        }

        public static Node GetAmbitoNode(string id, string node)
        {
            return GetAmbitoNode(Convert.ToInt32(id), Convert.ToInt32(node));
        }

        public static Node GetAmbitoNode(int id, int node)
        {
            Node result = new Node();

            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.AmbitiXML);
            XDocument root = GetXmlDocument(path);

            if (root != null)
            {
                result = (from f in root.Elements("ambiti").Elements("ambito").Elements("node")
                             where f.Parent.Attribute("id").Value.Equals(id.ToString())
                             select f)
                          .Where(f => f.Attribute("id").Value.Equals(node.ToString()))
                          .Select(f => new Node
                          {
                              Id = Convert.ToInt32(f.Attribute("id").Value),
                              Name = f.Value,
                              Server = f.Attribute("server").Value,
                              VirtualProxy = f.Attribute("vp").Value,
                              Link = f.Attribute("link").Value,
                              UrlWebTicket = f.Attribute("urlWebTicket").Value,
                              NodeType = f.Attribute("type").Value
                          }).SingleOrDefault();
            }

            return result;
        }

        public static string GetAmbitoIdByName(string ambitoName)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(FilePaths.AmbitiXML);
            XDocument root = AmbitiUtility.GetXmlDocument(path);

            string ambitoId = null;
            if (root != null)
            {
                ambitoId = (from f in root.Elements("ambiti").Elements("ambito").Elements("node")
                          where f.Value.Equals(ambitoName)
                          select f)
                         .Select(f => f.Parent.Attribute("id").Value).SingleOrDefault();
            }
            return ambitoId;
        }
    }
}