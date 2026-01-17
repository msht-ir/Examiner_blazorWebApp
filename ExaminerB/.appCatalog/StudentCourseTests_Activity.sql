use msht_eLib2
--update usrs set usrName='user4', UsrNickname='user4', usrPass='123456', usrTags=0 WHERE UsrId=16

--SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, Left(ch.ChatText, 10) msg, ch.ChatTags, sf.StudentName, st.StudentName 
--FROM Chats ch 
--INNER JOIN Students sf ON ch.FromId = sf.StudentId
--INNER JOIN Students st ON ch.ToId = st.StudentId
--WHERE ch.FromId=47 OR ch.ToId=47 
--ORDER BY DateTimeSent 
--OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY

-- REPORT STUDENTS COURSE TESTS
select sct.DateTime, s.StudentNickname, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers, s.StudentName, s.StudentNickname, s.StudentPass 
from studentCourseTests sct
inner join StudentCourses sc ON sct.StudentCourseId = sc.StudentCourseId
INNER JOIN Students s ON sc.StudentId = s.StudentId
INNER JOIN Courses c ON sc.CourseId = c.CourseId
order by DateTime desc

-- REPORT CHATS
SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, ch.ChatTags, sf.StudentName, sf.StudentNickname, ch.ChatText, st.StudentName, st.StudentNickname
FROM Chats ch 
INNER JOIN Students sf ON ch.FromId = sf.StudentId
INNER JOIN Students st ON ch.ToId = st.StudentId
ORDER BY DateTimeSent DESC
OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY


Select * from chats ORDER BY DateTimeSent Desc




