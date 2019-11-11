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
                SubmitButton.Enabled = false;
                TreeViewBox.BeginUpdate();
                // TreeViewBox = cnxn.getTree();
                CopyTree(cnxn.getAccountTree(),TreeViewBox);
                //TreeViewBox.Nodes.Add("This Worked");
                TreeViewBox.EndUpdate();
                SubmitButton.Visible = false;
                SubmitAccounts.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
                DBBox.Enabled = true;
                DataServerBox.Enabled = true;
            }

        }

        private void CopyTree(TreeView source, TreeView copy)
        {
            TreeNode newTN;
            foreach (TreeNode TN in source.Nodes)
            {
                newTN = new TreeNode(TN.Text);
                newTN.Name = TN.Name;
                CopyTreeChildren(newTN, TN);
                copy.Nodes.Add(newTN);
            }
        }

        private void CopyTreeChildren(TreeNode parent, TreeNode toCopy)
        {
            TreeNode newTN;
            foreach (TreeNode tn in toCopy.Nodes)
            {
                newTN = new TreeNode(tn.Text);
                newTN.Name = tn.Name;
                parent.Nodes.Add(newTN);
                CopyTreeChildren(newTN, tn);
            }
        }

        private void SubmitAccounts_Click(object sender, EventArgs e)
        {

            try
            {
                SubmitAccounts.Enabled = false;
                TreeViewBox.Enabled = false;
                List<string> checkedAccounts = new List<string>();
                foreach (TreeNode node in TreeViewBox.Nodes)
                {
                    if (node.Checked)
                        checkedAccounts.Add(node.Name);
                }
                cnxn.getForms(checkedAccounts);                
            }
            catch (Exception)
            {                
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
                SubmitAccounts.Enabled = true;
                TreeViewBox.Enabled = true;
            }
        }
    }
}
