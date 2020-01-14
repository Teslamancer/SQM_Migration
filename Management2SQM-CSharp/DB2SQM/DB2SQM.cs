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
    /// Represents a connection to a database and data server that can pull and push Management, Account, and SQM information
    /// </summary>
    public class DBConnection
    {
        

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
        private Graph AccountFormResultTree = new Graph();

        private Graph overallTree = new Graph();
        private Dictionary<string, string> suppliers = new Dictionary<string, string>();
        private Dictionary<string, string> locations = new Dictionary<string, string>();
        private Dictionary<string, string> materials = new Dictionary<string, string>();
        private Dictionary<string, HashSet<string>> IDtoBFS = new Dictionary<string, HashSet<string>>();
        public HashSet<string> AccountsForForms = new HashSet<string>();
        public DBConnection(string db, string dataserver)
        {
            this.DB = db;
            this.DataServer = dataserver;
        }
        /// <summary>
        /// Adds all Management Records into a graph and a dictionary that can retrieve their name from their ID
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
            return managementTree.generateTree("");
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
                        
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(AuditGlobalIDColNum))
                            {
                                AccountFormTree.addEdge(ID, reader.GetGuid(AuditGlobalIDColNum).ToString());
                                AccountFormTree.setNameForID(reader.GetGuid(AuditGlobalIDColNum).ToString(), reader.GetString(AuditNameColNum));
                                AccountFormTree.setNameForID(ID, overallTree.getNameFromID(ID));
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
            overallTree = managementTree.Clone();
            foreach (string ID in toRemove)
                managementTree.remove(ID);
            return managementTree.generateTree("");
        }

        public TreeView getResultsTree(Graph AccountForms)
        {
            //TODO: Implement getting Tree of top 10 (make this configurable) results for each form for each location
            throw new NotImplementedException();
        }

        private Graph getResults(Graph AccountForms)
        {//TODO: make this function return graph of Account->Form->Results
            throw new NotImplementedException();
            //Graph results = new Graph();

            //foreach (string ID in AccountForms.getRoots())
            //{
            //    foreach(string auditID in AccountForms.getChildren(ID))
            //    {
            //        string getResults = "select distinct aal.AuditGlobalID, al.AuditName from AuditAccountLookup aal join AuditLanguage al on aal.AuditGlobalID = al.AuditGlobalID where aal.AccountID = \'" + ID + "\'";

            //    }
            //    //StringBuilder sb = new StringBuilder("(");
            //    for (int i = 0; i < 10; i++)
            //    {

            //    } 
            //}
            //foreach (string ID in accounts)
            //{
            //    // sb.Append("\'" + ID + "\',");

            //    //sb.Append(")");
            //    //string getFormNames = "select * from auditlanguage where AuditGlobalID=\'";
            //    using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
            //    {
            //        SqlCommand command = new SqlCommand(getForms, cnxn);
            //        cnxn.Open();
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {

            //            int AuditGlobalIDColNum = reader.GetOrdinal("AuditGlobalID");
            //            int AuditNameColNum = reader.GetOrdinal("AuditName");
            //            //int AccountIDColNum = reader.GetOrdinal("AccountID");

            //            while (reader.Read())
            //            {
            //                if (!reader.IsDBNull(AuditGlobalIDColNum))
            //                {
            //                    AccountFormTree.addEdge(ID, reader.GetGuid(AuditGlobalIDColNum).ToString());
            //                    AccountFormTree.setNameForID(reader.GetGuid(AuditGlobalIDColNum).ToString(), reader.GetString(AuditNameColNum));
            //                    AccountFormTree.setNameForID(ID, overallTree.getNameFromID(ID));
            //                }
            //                else
            //                    continue;
            //            }
            //        }
            //    }
            //}
        }
    }
}
