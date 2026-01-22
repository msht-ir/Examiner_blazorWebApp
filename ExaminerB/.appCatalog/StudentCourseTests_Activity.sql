Use msht_eLib2
-- STUDENTS COURSE TESTS
select sct.DateTime, s.StudentName, s.StudentPass, s.StudentNickname, c.CourseName, sc.NumberOfTests NT, sc.CorrectAnswers nAns  
from studentCourseTests sct
inner join StudentCourses sc ON sct.StudentCourseId = sc.StudentCourseId
INNER JOIN Students s ON sc.StudentId = s.StudentId
INNER JOIN Courses c ON sc.CourseId = c.CourseId
order by DateTime desc

-- CHATS
SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, ch.ChatTags, sf.StudentName, sf.StudentNickname, ch.ChatText, st.StudentName, st.StudentNickname
FROM Chats ch 
INNER JOIN Students sf ON ch.FromId = sf.StudentId
INNER JOIN Students st ON ch.ToId = st.StudentId
ORDER BY DateTimeSent DESC
OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY



