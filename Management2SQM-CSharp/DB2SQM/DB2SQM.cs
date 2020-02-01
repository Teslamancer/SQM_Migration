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

        private Graph SQMTree = new Graph();
        private Graph AccountFormResultTree = new Graph();
        private Graph AccountFormResultFileTree = new Graph();

        private Graph SupplierLocationMaterial = new Graph();

        private Graph overallTree = new Graph();
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
                    break;
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
            overallTree = managementTree.Clone();
            //SQMTree = overallTree.Clone();
        }

        public void updateSQMTree(IEnumerable<string> checkedItems, int type)
        {
            switch (type)
            {
                case 1:
                    foreach(string SupplierID in checkedItems)
                    {
                        SQMTree.setNameForID(SupplierID, overallTree.getNameFromID(SupplierID));
                        SQMTree.addEdge("", SupplierID);
                    }
                    break;
                case 2:
                    foreach(string supplier in SQMTree.getChildren(""))
                    {
                        if(supplier != "")
                        {
                            foreach(string locationID in checkedItems)
                            {
                                if (overallTree.getChildren(supplier).Contains(locationID))
                                {
                                    SQMTree.setNameForID(locationID, overallTree.getNameFromID(locationID));
                                    SQMTree.addEdge(supplier, locationID);
                                }
                            }
                        }
                    }
                    break;
                case 3:
                    foreach (string supplier in SQMTree.getChildren(""))
                    {
                        if (supplier != "")
                        {
                            foreach (string location in SQMTree.getChildren(supplier))
                            {
                                foreach(string materialID in checkedItems)
                                {
                                    if (overallTree.getChildren(location) != null && overallTree.getChildren(location).Contains(materialID))
                                    {
                                        SQMTree.setNameForID(materialID, overallTree.getNameFromID(materialID));
                                        SQMTree.addEdge(location, materialID);
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public void printDOT()
        {
            Console.Write(this.managementTree.toDOT());
        }       

        //public void addSuppliers(IEnumerable<string> suppliers)
        //{
        //    foreach(string supplierID in suppliers)
        //    {
        //        SupplierLocationMaterial.setNameForID(supplierID, managementTree.getNameFromID(supplierID));
        //        SupplierLocationMaterial.addEdge(supplierID, "");
        //    }
        //}

        //public void addLocations(IEnumerable<string> Locations)
        //{
        //    foreach(string LocationID in Locations)
        //    {
        //        SupplierLocationMaterial.setNameForID(LocationID, managementTree.getNameFromID(LocationID));
        //        foreach(string supplier in SupplierLocationMaterial.getRoots())
        //        {
        //            if (managementTree.getChildren(supplier).Contains(LocationID))
        //            {
        //                SupplierLocationMaterial.addEdge(supplier, LocationID);
        //            }
        //        }
        //    }
        //}


        
        public TreeView getAccountTree()
        {
            return managementTree.generateTree("");
        }

        public TreeView getFormTree()
        {
            return AccountFormTree.generateTree();
        }



        public void sendtoSQM()
        {
            //foreach(string root in overallTree.getRoots())
            //{
            //    if (managementTree.getRoots().Contains(root) && root !="")
            //    {
            //        SQMTree.remove(root);
            //    }
            //foreach(string child in overallTree.getChildren(root))
            //{
            //    if (managementTree.getChildren(root) != null && managementTree.getChildren(root).Contains(child))
            //    {
            //        SQMTree.remove(child);
            //    }
            //}
            //}
            //string insertTree = "";
            using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
            {
                cnxn.Open();
                foreach(string supplier in SQMTree.getChildren(""))
                {
                    string insertSupplier = "INSERT INTO Target (TargetName, TargetTypeId, TargetStatusId) VALUES('" + SQMTree.getNameFromID(supplier)+"','3','13');SELECT CAST(Scope_Identity() AS int);";
                    SqlCommand insertSupplierCommand = new SqlCommand(insertSupplier, cnxn);
                    int supplierSQLID = (int)insertSupplierCommand.ExecuteScalar();
                    if (SQMTree.getChildren(supplier) != null)
                        foreach (string location in SQMTree.getChildren(supplier))
                        {
                            string insertLocation = "INSERT INTO Target (TargetName, TargetTypeId,TargetParentId, TargetStatusId) VALUES(\'" + SQMTree.getNameFromID(location) + "\',1,"+supplierSQLID+ ",13);SELECT CAST(Scope_Identity() AS int);";
                            SqlCommand insertLocationCommand = new SqlCommand(insertLocation, cnxn);
                            int LocationSQLID = (int)insertLocationCommand.ExecuteScalar();
                            if(SQMTree.getChildren(location) != null)
                                foreach(string material in SQMTree.getChildren(location))
                                {
                                    string insertMaterial = "INSERT INTO Target (TargetName, TargetTypeId,TargetParentId, TargetStatusId) VALUES(\'" + SQMTree.getNameFromID(material) + "\',2," + LocationSQLID + ",13);";
                                    SqlCommand insertMaterialCommand = new SqlCommand(insertMaterial, cnxn);
                                    //string LocationSQLID = (string)insertLocationCommand.ExecuteScalar();
                                    insertMaterialCommand.ExecuteNonQuery();
                                }
                        }
                }
                
                
            }
        }
        //public void getFiles()
        //{
        //    string getFiles = "";
        //}
        //THIS WILL RUN FOR MANAGEMENTS TOO
        //public void getForms(IEnumerable<string> accounts)
        //{
        //    //StringBuilder sb = new StringBuilder("(");
        //    foreach (string ID in accounts)
        //    {
        //       // sb.Append("\'" + ID + "\',");

        //        //sb.Append(")");
        //        string getForms = "select distinct aal.AuditGlobalID, al.AuditName from AuditAccountLookup aal join AuditLanguage al on aal.AuditGlobalID = al.AuditGlobalID where aal.AccountID = \'" + ID + "\'";
        //        //string getFormNames = "select * from auditlanguage where AuditGlobalID=\'";
        //        using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
        //        {
        //            SqlCommand command = new SqlCommand(getForms, cnxn);
        //            cnxn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {

        //                int AuditGlobalIDColNum = reader.GetOrdinal("AuditGlobalID");
        //                int AuditNameColNum = reader.GetOrdinal("AuditName");
        //                //int AccountIDColNum = reader.GetOrdinal("AccountID");
                        
        //                while (reader.Read())
        //                {
        //                    if (!reader.IsDBNull(AuditGlobalIDColNum))
        //                    {
        //                        AccountFormTree.addEdge(ID, reader.GetGuid(AuditGlobalIDColNum).ToString());
        //                        AccountFormTree.setNameForID(reader.GetGuid(AuditGlobalIDColNum).ToString(), reader.GetString(AuditNameColNum));
        //                        AccountFormTree.setNameForID(ID, overallTree.getNameFromID(ID));
        //                    }                                
        //                    else
        //                        continue;
        //                }
        //            }
        //        }
        //    }
        //}

        public TreeView getSQMLocationOptionsTree(IEnumerable<string> toRemove)
        {

            //Console.WriteLine(SQMTree.toDOT());
            foreach (string ID in toRemove)
                managementTree.remove(ID);
            return managementTree.generateTree("");
        }

        //public TreeView getResultsTree(Graph AccountToForms)
        //{
        //    string getResults;

        //    AccountFormResultTree = AccountToForms.Clone();
        //    foreach(string AccountID in AccountToForms.getRoots())
        //    {
        //        foreach(string formID in AccountToForms.getChildren(AccountID))
        //        {
        //            getResults= "select top 10 AuditResultGlobalID, StartDateLocal from auditresult where accountID = \'" + AccountID + "\' and auditglobalid = \'" + formID + "\'Order by StartDateLocal desc";
        //            using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + DataServer + ";" + "database=" + DB + ";"))
        //            {
        //                SqlCommand command = new SqlCommand(getResults, cnxn);
        //                cnxn.Open();
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        AccountFormResultTree.setNameForID(reader.GetGuid(0).ToString(), reader.GetDateTime(1).ToString());
        //                        AccountFormResultTree.addEdge(formID, reader.GetGuid(0).ToString());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return AccountFormResultTree.generateTree();
        //}

        //public Graph getAccountFormResultGraph()
        //{
        //    return AccountFormResultTree.Clone();
        //}

        //public TreeView getImageTree(Graph FormsToResults)
        //{
        //    Graph toReturn = FormsToResults.Clone();
        //    foreach (string AccountID in FormsToResults.getRoots())
        //    {
        //        foreach (string formID in FormsToResults.getChildren(AccountID))
        //        {
        //            foreach(string ARGUID in FormsToResults.getChildren(formID))//TODO: make this query return publicID, filename, Base64 if Image?
        //            {
        //                string getFiles = "select f.PublicId from QuestionResult r join QuestionResultBinaryFile f on r.QuestionResultSK = f.QuestionResultSK where r.AuditResultGlobalID = \'" + ARGUID + "\'";
        //                using (SqlConnection cnxn = new SqlConnection("Integrated Security=true;" + "Server=" + this.DataServer + ";" + "database=" + DB + ";"))
        //                {
        //                    SqlCommand command = new SqlCommand(getFiles, cnxn);
        //                    cnxn.Open();
        //                    using (SqlDataReader reader = command.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            toReturn.setNameForID(reader.GetGuid(0).ToString(), reader.GetDateTime(1).ToString());
        //                            toReturn.addEdge(formID, reader.GetGuid(0).ToString());
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    return toReturn.generateTree();
        //}
    }
}
