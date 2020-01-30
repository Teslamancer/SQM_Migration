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
        public string BFSServer
        {
            get;
            private set;
        }

        private Graph managementTree = new Graph();
        private Graph AccountFormTree = new Graph();
        private Graph AccountFormResultTree = new Graph();
        private Graph AccountFormResultFileTree = new Graph();

        private Graph overallTree = new Graph();
        private Dictionary<string, string> suppliers = new Dictionary<string, string>();
        private Dictionary<string, string> locations = new Dictionary<string, string>();
        private Dictionary<string, string> materials = new Dictionary<string, string>();
        private Dictionary<string, HashSet<string>> IDtoBFS = new Dictionary<string, HashSet<string>>();
        public HashSet<string> AccountsForForms = new HashSet<string>();
        
        public DBConnection(string db, string dataserver, int Environment)
        {
            this.DB = db;
            this.DataServer = dataserver;
            switch (Environment)
            {
                case 0:
                    this.BFSServer = "dmsdata5";
                    break;
                case 1:
                    this.BFSServer = "stagdata2";
                    break;
                case 2:
                    this.BFSServer = "devopsdata1";
                    break;
                default:
                    this.BFSServer = "dmsdata5";
            }

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

        public TreeView getResultsTree(Graph AccountToForms)
        {
            string getResults;

            AccountFormResultTree = AccountToForms.Clone();
            foreach(string AccountID in AccountToForms.getRoots())
            {
                foreach(string formID in AccountToForms.getChildren(AccountID))
                {
                    getResults= "select top 10 AuditResultGlobalID, StartDateLocal from auditresult where accountID = \'" + AccountID + "\' and auditglobalid = \'" + formID + "\'Order by StartDateLocal desc";
                    using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
                    {
                        SqlCommand command = new SqlCommand(getResults, cnxn);
                        cnxn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccountFormResultTree.setNameForID(reader.GetGuid(0).ToString(), reader.GetDateTime(1).ToString());
                                AccountFormResultTree.addEdge(formID, reader.GetGuid(0).ToString());
                            }
                        }
                    }
                }
            }
            return AccountFormResultTree.generateTree();
        }

        public Graph getAccountFormResultGraph()
        {
            return AccountFormResultTree.Clone();
        }

        public TreeView getFileTree(Graph FormsToResults)
        {
            Graph toReturn = FormsToResults.Clone();
            foreach (string AccountID in FormsToResults.getRoots())
            {
                foreach (string formID in FormsToResults.getChildren(AccountID))
                {
                    foreach(string ARGUID in FormsToResults.getChildren(formID))//TODO: make this query return publicID, filename
                    {
                        string getFiles = "select f.PublicId from QuestionResult r join QuestionResultBinaryFile f on r.QuestionResultSK = f.QuestionResultSK where r.AuditResultGlobalID = \'" + ARGUID + "\'";
                        using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + this.DataServer + ";" + "database=" + DB + ";"))
                        {
                            SqlCommand command = new SqlCommand(getFiles, cnxn);
                            cnxn.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    toReturn.setNameForID(reader.GetGuid(0).ToString(), reader.GetDateTime(1).ToString());
                                    toReturn.addEdge(formID, reader.GetGuid(0).ToString());
                                }
                            }
                        }

                    }
                }
            }
            return toReturn.generateTree();
        }
    }
}
