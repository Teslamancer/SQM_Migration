using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DB2SQM;

namespace GUI
{
    public partial class Form1 : Form
    {
        public static DBConnection cnxn;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                cnxn = new DBConnection(DBBox.Text, DataServerBox.Text);
                cnxn.getManagement();
                cnxn.getAccounts();
                DBBox.Enabled = false;
                DataServerBox.Enabled = false;

                
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
            }
        }

        private TreeView generateTree(DBConnection dbc)
        {
            DBConnection cnxn = dbc;
            foreach (string parent in cnxn.getEdgeList().Keys)
            {                
                foreach (string child in cnxn.getEdgeList()[parent])
                {
                    if (!child.Equals(""))
                    {
                        List<TreeNode> childrenList = new List<TreeNode>();
                        TreeNode leaf = new TreeNode(child);
                        childrenList.Add(leaf);
                    }
                    childrenList
                }
            }
        }
    }
}
