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
                SubmitSuppliers.Visible = true;
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

        private void SubmitSuppliers_Click(object sender, EventArgs e)
        {
            TreeView backup = new TreeView();
            try
            {
                SubmitSuppliers.Enabled = false;
                TreeViewBox.Enabled = false;          
                
                CopyTree(TreeViewBox, backup);
                TreeViewBox.BeginUpdate();
                TreeView LocationTree = cnxn.getSQMLocationOptionsTree(TreeViewBox.getChecked());
                TreeViewBox.Nodes.Clear();
                CopyTree(LocationTree, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                SelectLabel.Text = "Please select Locations for SQM";
                SubmitSuppliers.Visible = false;
                SubmitLocationsButton.Enabled = true;
                SubmitLocationsButton.Visible = true;
            }
            catch (Exception)
            {                
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
                SubmitSuppliers.Enabled = true;
                TreeViewBox.BeginUpdate();
                TreeViewBox.Nodes.Clear();
                CopyTree(backup, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                TreeViewBox.Enabled = true;
                SubmitSuppliers.Visible = true;
            }
        }

        private void SubmitLocationsButton_Click(object sender, EventArgs e)
        {        
            TreeView backup = new TreeView();
            try
            {
                SubmitLocationsButton.Enabled = false;
                SubmitLocationsButton.Visible = false;
                TreeViewBox.Enabled = false;

                CopyTree(TreeViewBox, backup);
                TreeViewBox.BeginUpdate();
                TreeView LocationTree = cnxn.getSQMLocationOptionsTree(TreeViewBox.getChecked());
                TreeViewBox.Nodes.Clear();
                CopyTree(LocationTree, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                SelectLabel.Text = "Please select Materials for SQM";
                SubmitSuppliers.Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
                SubmitLocationsButton.Enabled = true;
                SubmitLocationsButton.Visible = true;
                TreeViewBox.BeginUpdate();
                TreeViewBox.Nodes.Clear();
                CopyTree(backup, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                TreeViewBox.Enabled = true;                
            }
        }

        private void SubmitMaterialsButton_Click(object sender, EventArgs e)
        {
            TreeView backup = new TreeView();
            try
            {
                SubmitMaterialsButton.Enabled = false;
                SubmitMaterialsButton.Visible = false;
                TreeViewBox.Enabled = false;

                CopyTree(TreeViewBox, backup);
                TreeViewBox.BeginUpdate();
                TreeView LocationTree = cnxn.getSQMLocationOptionsTree(TreeViewBox.getChecked());
                TreeViewBox.Nodes.Clear();
                CopyTree(LocationTree, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                SelectLabel.Text = "Please select Forms for each level to search for Certs";
                SubmitSuppliers.Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Settings!", "Error", MessageBoxButtons.OK);
                SubmitMaterialsButton.Enabled = true;
                SubmitMaterialsButton.Visible = true;
                TreeViewBox.BeginUpdate();
                TreeViewBox.Nodes.Clear();
                CopyTree(backup, TreeViewBox);
                TreeViewBox.EndUpdate();
                TreeViewBox.Enabled = true;
                TreeViewBox.Enabled = true;
            }
        }
    }
    public static class extTreeView
    {
        public static IEnumerable<string> getChecked(this TreeView tv)
        {
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            HashSet<string> toReturn = new HashSet<string>();
            foreach(TreeNode parent in tv.Nodes)
            {
                if (!visited.ContainsKey(parent.Name))
                    visited.Add(parent.Name, false);
                if (visited[parent.Name] == false)
                {
                    visited[parent.Name] = true;
                    if (parent.Checked)
                        toReturn.Add(parent.Name);
                    recursiveNodeChecker(visited, parent, toReturn);
                }
            }
            return toReturn;
        }

        private static void recursiveNodeChecker(Dictionary<string, bool> visited, TreeNode parent, HashSet<string> checkedList)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                //Console.WriteLine(tree.Nodes.IndexOfKey(root));

                if ((visited.ContainsKey(child.Name) && !visited[child.Name]) || !visited.ContainsKey(child.Name))
                {
                    visited[child.Name] = true;
                    if (!child.Name.Equals(""))
                    {
                        if (child.Checked)
                            checkedList.Add(child.Name);

                        recursiveNodeChecker(visited, child, checkedList);
                    } 
                }
            }
        }
    }
}
