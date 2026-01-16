use msht_eLib2
--update usrs set usrName='user4', UsrNickname='user4', usrPass='123456', usrTags=0 WHERE UsrId=16
--update usrs set UsrTags=1 WHERE UsrId=12
--INSERT INTO Chats (FromId, ToId, DateTimeSent, ChatText, ChatTags) 
--Values (48, 47, '2026-01.16 . 10:53', 'Salam Majid! Whats up?', 0) 

--update chats set chattags = 1 where chatid=1
--Select * from chats Order by datetimesent

--SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, Left(ch.ChatText, 10) msg, ch.ChatTags, sf.StudentName, st.StudentName 
--FROM Chats ch 
--INNER JOIN Students sf ON ch.FromId = sf.StudentId
--INNER JOIN Students st ON ch.ToId = st.StudentId
--WHERE ch.FromId=47 OR ch.ToId=47 
--ORDER BY DateTimeSent 
--OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY


select sct.DateTime, s.StudentNickname, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers, s.StudentName, s.StudentPass 
from studentCourseTests sct
inner join StudentCourses sc ON sct.StudentCourseId = sc.StudentCourseId
INNER JOIN Students s ON sc.StudentId = s.StudentId
INNER JOIN Courses c ON sc.CourseId = c.CourseId
order by DateTime desc


SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, ch.ChatText, ch.ChatTags, sf.StudentName, st.StudentName
FROM Chats ch 
INNER JOIN Students sf ON ch.FromId = sf.StudentId
INNER JOIN Students st ON ch.ToId = st.StudentId
WHERE ((ch.FromId=47) AND (ch.ToId=48)) OR ((ch.FromId=48) AND (ch.ToId=47)) 
ORDER BY DateTimeSent DESC
OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY


Select * from chats ORDER BY DateTimeSent Desc




