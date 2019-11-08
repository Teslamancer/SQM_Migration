﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DB2SQM
{
    /// <summary>
    /// Represents a connection to a database and data server that can pull and push Management, Account, and SQM information
    /// </summary>
    public class DBConnection
    {
        private class Graph
        {

            private class Node
            {
                public string Name
                {
                    get; private set;
                }
                public string ID
                {
                    get; private set;
                }
                public Node parent
                {
                    get;private set;
                }
                public List<Node> children
                {
                    get; private set;
                }
            }
            //This dictionary maps ID's to Names for Management Records and Accounts
            private Dictionary<string, string> IDtoName = new Dictionary<string, string>();
            //This is an adjecency list of edges from parent to child nodes
            private Dictionary<string, HashSet<string>> edgeList = new Dictionary<string, HashSet<string>>();

            /// <summary>
            /// Adds an edge to the graph from a source ID to a terminal ID (both should be ManagementID or AccountID
            /// </summary>
            /// <param name="source">Source ID (Management or Account)</param>
            /// <param name="terminal">Terminal ID (Management or Account)</param>
            public void addEdge(string source, string terminal)
            {
                if (edgeList.ContainsKey(source))
                {
                    edgeList[source].Add(terminal);
                }
                else
                {
                    edgeList.Add(source, new HashSet<string>());
                    edgeList[source].Add(terminal);
                }
            }

            public TreeView generateTree()
            {
                Dictionary<string, bool> visited = new Dictionary<string, bool>();
                TreeView toReturn = new TreeView();
                foreach (string parent in edgeList.Keys)
                {
                    if (!visited.ContainsKey(parent))
                        visited.Add(parent, false);
                    if(visited[parent] == false)
                    {
                        visited[parent] = true;
                        toReturn.Nodes.Add(getNameFromID(parent));
                        recursiveTreeGen(parent, visited, toReturn);                        
                    }                    
                }
                return toReturn;
            }
            private void recursiveTreeGen(string root, Dictionary<string, bool> visited, TreeView tree)
            {
                foreach (string child in edgeList[root])
                {
                    Console.WriteLine(tree.Nodes.IndexOfKey(root));
                    int treelevel = tree.Nodes.IndexOfKey(root)+1;
                    if (!child.Equals(""))
                    {
                        tree.Nodes[treelevel].Nodes.Add(getNameFromID(child));
                    }
                    if (edgeList.ContainsKey(child) && (visited.ContainsKey(child) && visited[child]==false))
                    {
                        visited.Add(child, true);
                        recursiveTreeGen(child, visited, tree);
                    }

                }
            }
            
            /// <summary>
            /// Gets all of the children as an IEnumerable for a specified Parent Node
            /// </summary>
            /// <param name="parentID">ManagementID to get children of</param>
            /// <returns></returns>
            public IEnumerable<string> getChildren(string parentID)
            {
                if (edgeList.ContainsKey(parentID))
                {
                    return edgeList[parentID];
                }
                return null;
            }
            /// <summary>
            /// Returns the name of an Account or Management Record given its ID
            /// </summary>
            /// <param name="ID">ID to get name of</param>
            /// <returns></returns>
            public string getNameFromID(string ID)
            {
                if (IDtoName.ContainsKey(ID))
                {
                    return IDtoName[ID];
                }
                else
                {
                    return null;
                }
            }
            /// <summary>
            /// Sets name for ID (Management or Account)
            /// </summary>
            /// <param name="ID">ID</param>
            /// <param name="name">Name to set</param>
            public void setNameForID(string ID, string name)
            {
                IDtoName[ID] = name;
            }

            public string toDOT()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("digraph G {\n");
                foreach (string parent in edgeList.Keys)
                {
                    if (!parent.Equals(""))
                    {
                        sb.Append('\"' + getNameFromID(parent) + '\"');
                        sb.Append(" -> {");
                        foreach (string child in edgeList[parent])
                        {
                            if (!child.Equals(""))
                            {
                                sb.Append('\"' + getNameFromID(child) + '\"');
                            }
                        }
                        sb.Append("}\n");
                    }
                }
                sb.Append("}");
                return sb.ToString();
            }

        }

        /// <summary>
        /// Database used for connection
        /// </summary>
        public string DB
        {
            get;
            private set;
        }
        /// <summary>
        /// Data Server Used for Connection
        /// </summary>
        public string DataServer
        {
            get;
            private set;
        }
        private Graph managementTree = new Graph();

        public DBConnection(string db, string dataserver)
        {
            this.DB = db;
            this.DataServer = dataserver;
        }
        /// <summary>
        /// Adds all Management REcords into a graph and a dictionary that can retrieve their name from their ID
        /// </summary>
        public void getManagement()
        {
            string getManagements = "select * from Management\nwhere Deleted=0\norder by ParentID asc";
            using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
            {
                SqlCommand command = new SqlCommand(getManagements, cnxn);
                cnxn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    int ManagementIDColNum = reader.GetOrdinal("ManagementID");
                    int ManagementNameColNum = reader.GetOrdinal("ManagementName");
                    int ParentIDColNum = reader.GetOrdinal("ParentID");
                    while (reader.Read())
                    {
                        managementTree.setNameForID(reader.GetString(ManagementIDColNum), reader.GetString(ManagementNameColNum));
                        if (reader.IsDBNull(ParentIDColNum))
                            managementTree.addEdge("", reader.GetString(ManagementIDColNum));
                        else
                            managementTree.addEdge(reader.GetString(ParentIDColNum), reader.GetString(ManagementIDColNum));
                    }
                }
            }
        }
        public void getAccounts()
        {
            string getAccounts = "select a.AccountID, a.AccountName, a.Active, a.Deleted, mal.ManagementID from Account a join ManagementAccountList mal on a.AccountID = mal.AccountID where a.Active = 1 and a.Deleted = 0";
            using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
            {
                SqlCommand command = new SqlCommand(getAccounts, cnxn);
                cnxn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    int ManagementIDColNum = reader.GetOrdinal("ManagementID");
                    int AccountNameColNum = reader.GetOrdinal("AccountName");
                    int AccountIDColNum = reader.GetOrdinal("AccountID");
                    while (reader.Read())
                    {
                        managementTree.setNameForID(reader.GetString(AccountIDColNum), reader.GetString(AccountNameColNum));
                        if (reader.IsDBNull(ManagementIDColNum))
                            managementTree.addEdge("", reader.GetString(AccountIDColNum));
                        else
                            managementTree.addEdge(reader.GetString(ManagementIDColNum), reader.GetString(AccountIDColNum));
                    }
                }
            }
        }
        public void printDOT()
        {
            Console.Write(this.managementTree.toDOT());
        }       
        
        public TreeView getTree()
        {
            return managementTree.generateTree();
        }
    }
}