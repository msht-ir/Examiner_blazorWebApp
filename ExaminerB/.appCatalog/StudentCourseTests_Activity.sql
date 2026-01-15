use msht_eLib2
--select * from usrsLogs ORDER BY DateTime DESC --WHERE UserId NOT IN (1, 47)
--select * from Students Where StudentId in (178, 145, 113, 111)

--DELETE FROM usrsLogs WHERE DateTime LIKE '1404%'

select sct.DateTime, s.StudentNickname, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers, s.StudentName, s.StudentPass 

from studentCourseTests sct
inner join StudentCourses sc ON sct.StudentCourseId = sc.StudentCourseId
INNER JOIN Students s ON sc.StudentId = s.StudentId
INNER JOIN Courses c ON sc.CourseId = c.CourseId
order by DateTime desc

