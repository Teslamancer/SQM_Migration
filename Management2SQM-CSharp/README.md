Changed implementation. 
Instead of selecting Suppliers and locations in one go, will go:
select suppliers>
select locations for those suppliers>
select Materials for those locations? >
select forms for either supplier or location or material (appropriately)
Select Results for those forms
select files from those results
Insert into DB
Profit


w/ Tix
https://stackoverflow.com/questions/28483980/creating-a-treeview-with-checkboxes

//These PublicIds cannot be found in BinaryFileStorage

use sysco
select pat.AccountID, pat.ProgramID, pat.TaskID, pttl.Name from ProgramAccountTask pat
join ProgramTaskTypeLanguage pttl on pttl.TaskTypeID=pat.TaskTypeID
where pttl.Name = 'Certification'

select * from ProgramAccountTask pat
join ProgramTaskTypeLanguage pttl on pttl.TaskTypeID=pat.TaskTypeID
where pttl.Name = 'Certification'



select * from questionresult qr
join QuestionLanguage ql on qr.QuestionGlobalID=ql.QuestionGlobalID
where AuditResultGlobalID='BB57EC57-7CFF-4E06-9ECE-CB7D382D2140'

select f.PublicId from QuestionResult r
    join QuestionResultBinaryFile f on r.QuestionResultSK = f.QuestionResultSK
where r.AuditResultGlobalID = 'BB57EC57-7CFF-4E06-9ECE-CB7D382D2140'

--devopsdata1
use BinaryFileStorage
select top 100 * from BinaryFileMetaData where PublicId in ( 
'9148A71D-A3AC-4455-AD5D-9FE03F8A5739',
'7F5B7A67-AAC4-445E-8A14-F530337D1239')

So Inserting with the first publicId above worked, but the data was corrupted, probably because it inserted as a text file and it's really something else, but I can't find it in BFS to get the correct MetaData

TODO: Find out where they live so I can get MetaData

The issue was BFS is different between QA, Staging, and Prod, whoops! If I insert a oublicid from the correct BFS, it works!
Prod: dmsdata5
Stag: stagdata2
QA: devopsdata1