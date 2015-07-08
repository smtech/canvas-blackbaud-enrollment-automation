SELECT distinct
COALESCE(Null,' ') "Course_Id",
r.importid "User_Id",
'Student' "Role",
c.importid "Section_Id",
CASE es.changetype
     WHEN 1 THEN 'Active'
     WHEN 3 THEN 'Deleted'         
     WHEN 4 THEN 'Deleted' 
END Status
FROM 
ea7schedulingchanges es,
ea7Students S,
ea7records r,
users u,
ea7classes c,
ea7classterms ct,
ea7blocks b
WHERE 
es.ea7studentsid = s.ea7studentsid 
and s.ea7recordsid = r.ea7recordsid 
and es.changedbyid = u.usersid
and (c.ea7classesid =es.previousclass
 or c.ea7classesid=es.newclass)
and ct.ea7classesid =c.ea7classesid
and ct.ea7blocksid=b.ea7blocksid
and es.datechanged >=  (select max(starttime) from StMarks.dbo.processstats where processid=5
and upper(processstatus)='COMPLETE' and upper(statusmessage)='STARTED')
and es.changetype in (1,3)
union
select 
COALESCE(Null,' ') "Course_Id",
r.importid "User_Id",
'Student' "Role",
c.importid "Section_Id",
'deleted' Status
from ea7schedulingchanges sc, ea7classes c, ea7students s,ea7records r
where c.ea7classesid=sc.previousclass
and s.ea7studentsid=sc.ea7studentsid
and s.ea7recordsid=r.ea7recordsid
and sc.datechanged   >=  (select max(starttime) from StMarks.dbo.processstats where processid=5
and upper(processstatus)='COMPLETE' and upper(statusmessage)='STARTED')
and sc.changetype=4
union
select  
COALESCE(Null,' ') "Course_Id",
r.importid "User_Id",
'Student' "Role",
c.importid "Section_Id",
'active' Status
from ea7schedulingchanges sc, ea7classes c, ea7students s,ea7records r
where c.ea7classesid=sc.newclass
and s.ea7studentsid=sc.ea7studentsid
and s.ea7recordsid=r.ea7recordsid
and sc.datechanged  >=  (select max(starttime) from StMarks.dbo.processstats where processid=5
and upper(processstatus)='COMPLETE' and upper(statusmessage)='STARTED')
and sc.changetype=4