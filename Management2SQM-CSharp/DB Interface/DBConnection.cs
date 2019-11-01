using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DB_Interface
{
    /// <summary>
    /// Represents a connection to a database and data server
    /// </summary>
    public class DB2SQM
    {
        private class Graph
        {
            private Dictionary<string, string> IDtoName = new Dictionary<string, string>();
            private Dictionary<string, HashSet<string>> edgeList = new Dictionary<string, HashSet<string>>();

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

            public IEnumerable<string> getChildren(string parentID)
            {
                if (edgeList.ContainsKey(parentID))
                {
                    return edgeList[parentID];
                }
                return null;
            }

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
            public void setNameForID(string ID, string name)
            {
                IDtoName[ID] = name;
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

        public DB2SQM(string db, string dataserver)
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
            using(SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
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
                            managementTree.addEdge(reader.GetString(ManagementIDColNum), null);
                        else
                            managementTree.addEdge(reader.GetString(ManagementIDColNum), reader.GetString(ParentIDColNum));
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
                            managementTree.addEdge(reader.GetString(AccountIDColNum), null);
                        else
                            managementTree.addEdge(reader.GetString(AccountIDColNum), reader.GetString(ManagementIDColNum));
                    }
                }
            }
        }
    }

    

}
