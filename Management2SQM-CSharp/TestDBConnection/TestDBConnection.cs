using DB2SQM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestDBConnection
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DBConnection cnxn = new DBConnection("csrhstest","stagdata1");
            cnxn.getManagement();
            cnxn.getAccounts();
            cnxn.printDOT();
        }
    }
}
