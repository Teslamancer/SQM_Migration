using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DB2SQM
{
    /// <summary>
    /// Represents a connection to a database and data server that can pull and push Management, Account, and SQM information CHANGEDYCHANGE
    /// </summary>
    public class DBConnection
    {
        private class Graph
        {
            private const int TVIF_STATE = 0x8;
            private const int TVIS_STATEIMAGEMASK = 0xF000;
            private const int TV_FIRST = 0x1100;
            private const int TVM_SETITEM = TV_FIRST + 63;

            [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
            private struct TVITEM
            {
                public int mask;
                public IntPtr hItem;
                public int state;
                public int stateMask;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpszText;
                public int cchTextMax;
                public int iImage;
                public int iSelectedImage;
                public int cChildren;
                public IntPtr lParam;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
                                                     ref TVITEM lParam);

            /// <summary>
            /// Hides the checkbox for the specified node on a TreeView control.
            /// </summary>
            private void HideCheckBox(TreeView tvw, TreeNode node)
            {
                TVITEM tvi = new TVITEM();
                tvi.hItem = node.Handle;
                tvi.mask = TVIF_STATE;
                tvi.stateMask = TVIS_STATEIMAGEMASK;
                tvi.state = 0;
                SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
            }
            //private class Node
            //{
            //    public string Name
            //    {
            //        get; private set;
            //    }
            //    public string ID
            //    {
            //        get; private set;
            //    }
            //    public Node parent
            //    {
            //        get;private set;
            //    }
            //    public List<Node> children
            //    {
            //        get; private set;
            //    }
            //}
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
                foreach (string parent in edgeList[""])
                {
                    
                        if (!visited.ContainsKey(parent))
                            visited.Add(parent, false);
                        if (visited[parent] == false)
                        {
                            visited[parent] = true;
                            TreeNode parentNode = new TreeNode(getNameFromID(parent));
                            parentNode.Name = parent;
                            if (edgeList.ContainsKey(parent))                        
                                recursiveNodeGen(parent, visited, parentNode);                         
                            toReturn.Nodes.Add(parentNode);
                        }
                   
                }
                return toReturn;
            }
            private void recursiveNodeGen(string root, Dictionary<string, bool> visited, TreeNode parent)
            {
                foreach (string child in edgeList[root])
                {
                    //Console.WriteLine(tree.Nodes.IndexOfKey(root));
                    
                    if (!child.Equals(""))
                    {
                        TreeNode childNode = new TreeNode(getNameFromID(child));
                        childNode.Name = child;
                        parent.Nodes.Add(childNode);
                        if (edgeList.ContainsKey(child) && ((visited.ContainsKey(child) && visited[child] == false) || (!visited.ContainsKey(child))))
                        {
                            visited.Add(child, true);
                            recursiveNodeGen(child, visited, childNode);                            
                        }
                    }
                }                
            }

            public void remove(string ID)
            {
                string parentSetToModify="";
                foreach (string parent in edgeList.Keys)
                {
                    foreach (string child in edgeList[parent])
                    {
                        if (child.Equals(ID))
                        {
                            parentSetToModify = parent;
                            break;
                        }                            
                    }
                }
                edgeList[parentSetToModify].Remove(ID);
                if (edgeList.ContainsKey(ID))
                {                          
                    foreach (string child in edgeList[ID])
                        edgeList[parentSetToModify].Add(child);
                    edgeList.Remove(ID);                                                        
                }             
                if (IDtoName.ContainsKey(ID))
                    IDtoName.Remove(ID);
                               
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
        private Graph AccountFormTree = new Graph();

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
        
        public TreeView getAccountTree()
        {
            return managementTree.generateTree();
        }

        public TreeView getFormTree()
        {
            return AccountFormTree.generateTree();
        }

        public void getFiles()
        {
            string getFiles = "";
        }
        //THIS WILL RUN FOR MANAGEMENTS TOO
        public void getForms(IEnumerable<string> accounts)
        {
            //StringBuilder sb = new StringBuilder("(");
            foreach (string ID in accounts)
            {
               // sb.Append("\'" + ID + "\',");

                //sb.Append(")");
                string getForms = "select distinct aal.AuditGlobalID, al.AuditName from AuditAccountLookup aal join AuditLanguage al on aal.AuditGlobalID = al.AuditGlobalID where aal.AccountID = \'" + ID + "\'";
                //string getFormNames = "select * from auditlanguage where AuditGlobalID=\'";
                using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
                {
                    SqlCommand command = new SqlCommand(getForms, cnxn);
                    cnxn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        int AuditGlobalIDColNum = reader.GetOrdinal("AuditGlobalID");
                        int AuditNameColNum = reader.GetOrdinal("AuditName");
                        //int AccountIDColNum = reader.GetOrdinal("AccountID");
                        AccountFormTree.setNameForID(ID, managementTree.getNameFromID(ID));
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(AuditGlobalIDColNum))
                            {
                                AccountFormTree.addEdge(ID, reader.GetGuid(AuditGlobalIDColNum).ToString());
                                AccountFormTree.setNameForID(reader.GetGuid(AuditGlobalIDColNum).ToString(), reader.GetString(AuditNameColNum));                                
                            }                                
                            else
                                continue;
                        }
                    }
                }
            }
        }

        public TreeView getSQMLocationOptionsTree(IEnumerable<string> toRemove)
        {
            foreach (string ID in toRemove)
                managementTree.remove(ID);
            return managementTree.generateTree();
        }
    }
}
