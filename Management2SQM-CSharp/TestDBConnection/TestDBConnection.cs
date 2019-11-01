using DB_Interface;
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
            DB2SQM cnxn = new DB2SQM("csrhstest","stagdata1");
            cnxn.getManagement();
            cnxn.getAccounts();
        }
    }
}
