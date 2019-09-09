import os
import pyodbc
from collections import defaultdict
import tkinter as tk
from tkinter import filedialog
from slugify import slugify
from ttkwidgets import CheckboxTreeview

class Graph:
  
    #Constructor
    def __init__(self):
        self.dict = defaultdict(list)
        self.id_to_name = defaultdict(str)

    def add_edge(self, source, terminal):
        self.dict[source].append(terminal)

    def get_children(self, parent):
        return self.dict[parent]

    def set_name_for_id(self, ID, name):
        self.id_to_name[ID] = name

    def get_name_from_id(self, ID):
        return slugify(self.id_to_name[ID])

    def get_raw_graph(self):
        return self.dict            

    def graph_to_DOT(self):
        str_to_print = []
        str_to_print.append("digraph G {\n")
        for parent in self.dict:
            str_to_print.append('\"' + self.get_name_from_id(parent) + '\"')
            str_to_print.append(" -> {")
            #print(str_to_print)
            for child in self.dict[parent]:
                if child:
                    str_to_print.append('\"' + self.get_name_from_id(child) + '\"' + " ")
            str_to_print.append("}\n")
        str_to_print.append("}")
        return ''.join(str_to_print)    

    def dfs_print(self):
        str_to_return = []
        visited = {ID:False for ID in self.id_to_name}
        for root in self.dict:
            if visited[root] == False:
                visited[root] = True                
                str_to_return.append(self.get_name_from_id(root))
                str_to_return.append("\n")
                self.dfs_recursive(root, visited, str_to_return)             
        return ''.join(str_to_return)

    def dfs_recursive(self, root, visited, str_to_return):
        for child in self.dict[root]:
            if child != '':
                str_to_return.append("|--")
                str_to_return.append(self.get_name_from_id(child))
                str_to_return.append("\n")
            if child in self.dict and visited[child]==False:
                visited[child] = True
                str_to_return.append("   ")
                self.dfs_recursive(child, visited, str_to_return)

    



    def generate_folders(self):
        trial_num = 0
        while True:
            try:
                root=tk.Tk()
                root.withdraw()
                start_path = filedialog.askdirectory(initialdir=os.getcwd(), title="Choose a Directory to Save in")
                try:
                    if trial_num==0:
                        os.chdir(start_path)
                        os.mkdir('SQM Folders')
                        root_path = os.getcwd() + "\\SQM Folders"
                    else:
                        os.mkdir("SQM Folders(" + str(trial_num)+")")
                        root_path = os.getcwd() + "\\SQM Folders(" + str(trial_num)+")"
                    break
                except:
                    trial_num+=1
            except:
                pass
     
        os.chdir(root_path)
        visited = {ID:False for ID in self.id_to_name}
        for root in self.dict:
            os.chdir(root_path)
            if visited[root] == False:
                visited[root] = True
                #print(self.get_name_from_id(root))
                os.mkdir(str(root) + '-'+self.get_name_from_id(root))
                cwd = root_path + "\\" + str(root) + '-'+self.get_name_from_id(root)
                #print(cwd)
                self.dfs_generate_recursive(root, visited, cwd)

    def dfs_generate_recursive(self, root, visited, cwd):
        for child in self.dict[root]:
            if child != '':
                os.chdir(cwd)
                if (self.get_name_from_id(child)):
                    os.mkdir(str(child) + '-'+self.get_name_from_id(child))
                else:
                    os.mkdir(str(child))
            if child in self.dict and visited[child]==False:
                visited[child] = True
                self.dfs_generate_recursive(child, visited, cwd + "\\" +str(child) + '-'+self.get_name_from_id(child))         
                
    def generate_tree(self):
        root = tk.Tk()
        tree = CheckboxTreeview(root)
        #root.withdraw()
        

        visited = {ID:False for ID in self.id_to_name}
        for parent in self.dict:
            if visited[parent] ==False:
                visited[parent] = True
                tree.insert("", "end",parent, text=self.get_name_from_id(parent))
                self.generate_tree_dfs(parent, visited, tree)
        def callback():
            print(tree.get_checked())
        submit = tk.Button(root, text="Submit", command=callback)
        
        
        root.geometry('500x500')
        #root.resizeable()
        tree.pack(fill=tk.BOTH)
        submit.pack()
        root.mainloop()
        
        #print(tree.state())

    def generate_tree_dfs(self, root, visited, tree):
        for child in self.dict[root]:
            if child != '':
                try:
                    tree.insert(root,"end",child,text=self.get_name_from_id(child))
                except:
                    pass
            if child in self.dict and visited[child] ==False:
                visited[child]=True
                self.generate_tree_dfs(child, visited, tree)


def setup_cnxn(dataServer=None, db=None):
    toReturn = []
    if (dataServer==None or db==None):
        dataServer = input("Please input the data server: ")
        db= input("Please input the Database: ")
    try:
        cnxn = pyodbc.connect(Driver='{ODBC Driver 17 for SQL Server}',Server=dataServer + ".steton.com",database=db,Trusted_Connection='yes')
        toReturn.append(dataServer)
        toReturn.append(db)
        toReturn.append(cnxn)
        return toReturn
    except:
        print("Connection invalid! Please try again \n")
        #print(e)
        setup_cnxn()



cnxn_settings=setup_cnxn("devopsdata1","Sysco")

dataServer = cnxn_settings[0]
db = cnxn_settings[1]
cnxn = cnxn_settings[2]
cursor = cnxn.cursor()

pull_mgmt = ('''
select * from Management
where Deleted=0
order by ParentID asc
''')

cursor.execute(pull_mgmt)

rows = cursor.fetchall()
management_tree=Graph()

for row in rows:
    management_tree.set_name_for_id(row.ManagementID, row.ManagementName)
    if row.ParentID is None:
        management_tree.add_edge(row.ManagementID, '')
    else:
         management_tree.add_edge(row.ParentID, row.ManagementID)
#print(management_tree.graph_to_DOT())



pull_accounts = ('''
select a.AccountID, a.AccountName, a.Active, a.Deleted, mal.ManagementID from Account a
join ManagementAccountList mal
on a.AccountID = mal.AccountID
where a.Active = 1
and a.Deleted=0
''')
cursor.execute(pull_accounts)
rows=cursor.fetchall()
cursor.close()
#NOTE THIS CAN BREAK IF A MANAGEMENT AND ACCOUNT HAVE THE SAME ID
for row in rows:
    management_tree.set_name_for_id(row.AccountID, row.AccountName)
    management_tree.add_edge(row.ManagementID, row.AccountID)

#print(management_tree.graph_to_DOT())
#print(management_tree.dfs_print())

management_tree.generate_tree()
#root = tk.Tk()

#tree = CheckboxTreeview(root)
#tree.pack()

#tree.insert("", "end", "1", text="1")
#tree.insert("1", "end", "11", text="11")
#tree.insert("1", "end", "12",  text="12")
#tree.insert("11", "end", "111", text="111")
#tree.insert("", "end", "2", text="2")

#root.mainloop()

#get_certs = ('''
#select pat.AccountID, qrb.PublicId from QuestionResult qr
#	join QuestionResultBinaryFile qrb on qr.QuestionResultSK = qrb.QuestionResultSK
#	join ProgramAccountTask pat on pat.AuditResultGlobalID = qr.AuditResultGlobalID
#where pat.TaskTypeID=5
#and pat.AuditResultGlobalID is not null
#''')

#get_cert_names = ('''
#    select PublicId, OriginalFileName from BinaryFileMetaData
#''')
#cnxn_settings=setup_cnxn('devopsdata1','BinaryFileStorage')

#dataServer = cnxn_settings[0]
#db = cnxn_settings[1]
#cnxn = cnxn_settings[2]
#cursor = cnxn.cursor()
#cursor.execute(get_cert_names)
#rows = cursor.fetchall()
#cert_name_from_ID = {}
#for row in rows:
#    cert_name_from_ID[str(row.PublicId)] = row.OriginalFileName

#cnxn_settings=setup_cnxn('devopsdata1','Sysco')

#dataServer = cnxn_settings[0]
#db = cnxn_settings[1]
#cnxn = cnxn_settings[2]
#cursor = cnxn.cursor()
#cert_tree = Graph()
#cursor.execute(get_certs)
#rows = cursor.fetchall()
#for row in rows:
#    cert_tree.add_edge(row.AccountID, row.PublicId)

#management_tree.create_checkboxes()
##management_tree.generate_folders()
#print("Folders Generated.\n Please delete all extraneous folders, making all folders under the original the suppliers, then all folders under each supplier their location.\n An attempt will be made to automatically populate the locations with their certifications from Program Compliance.")
#input("Press Enter to Select the Suppliers to Import (The SQM Folders folder)...")
#root=tk.Tk()
#root.withdraw()
#root_folder = filedialog.askdirectory(initialdir=os.getcwd(), title="Choose the root folder containing all suppliers")
#print(os.walk(root_folder))

#def create_insert(ObjectName, FileName, PublicId):
#    pass
##Call this with Params (Object Name, File Name, PublicID)
#insert_stuff=('''
#SET XACT_ABORT ON;
#BEGIN TRANSACTION
#   DECLARE @TargetID int;
#   INSERT INTO  Target (TargetName, TargetTypeId, TargetStatusId) VALUES (?,3,13);
#   SELECT @TargetID = @@IDENTITY;
#   DECLARE @TargetRequirementID int;
#   INSERT INTO TargetRequirement(RequirementName, TargetId, TargetRequirementType, Active, Deleted) VALUES (?, @TargetID,1,1,0);
#   SELECT @TargetRequirementID = @@IDENTITY;
#   INSERT INTO TargetRequirementDocument(TargetRequirementId,PublicId,TargetRequirementDocumentStatusId,Active, Deleted) VALUES (@TargetRequirementID, ?,1,1,0)
#COMMIT
#''')

#insert_supplier=('''
#'''  )
#cnxn_settings=setup_cnxn("stagdata1","csrhstest")

#dataServer = cnxn_settings[0]
#db = cnxn_settings[1]
#cnxn = cnxn_settings[2]
#cursor = cnxn.cursor()

#cursor.execute(insert_stuff, 'ParamTest')
#cursor.commit()

#This is the SQL I was working on
#use sysco
#select top 10 ProgramID, pat.TaskID, ptl.Name, AccountID from programaccounttask pat
#join ProgramTaskLanguage ptl on ptl.TaskID=pat.TaskID

#select distinct Name, TaskID from ProgramTaskLanguage
