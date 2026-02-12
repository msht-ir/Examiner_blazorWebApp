using ClosedXML.Excel;
using ExaminerS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;
using Chat = ExaminerS.Models.Chat;
using Group = ExaminerS.Models.Group;

namespace ExaminerB.Services2Backend
    {
    public class BeService : BeIService
        {
        private readonly IConfiguration _config;
        public BeService (IConfiguration config) => _config = config;
        #region Login
        public async Task<User?> LoginTeacherAsync (User user)
            {
            string? connString = _config.GetConnectionString ("cnni");
            string sql = "SELECT UsrId, UsrName, UsrPass, UsrNickname, UsrTags FROM usrs WHERE UsrName=@usr AND UsrPass=@pwd AND ((UsrTags & 1) = 1)";
            using SqlConnection cnn = new (connString);
            User userOut = new ();
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@usr", user.UserName);
                cmd.Parameters.AddWithValue ("@pwd", user.UserPass);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    if ((user.UserName.ToLower () == reader.GetString (1).ToLower ()) && (user.UserPass == reader.GetString (2)))
                        {
                        userOut.UserId = reader.GetInt32 (0);
                        userOut.TeacherId = 0;
                        userOut.UserName = reader.GetString (1);
                        userOut.UserPass = reader.GetString (2);
                        userOut.UserNickname = reader.GetString (3);
                        userOut.UserRole = "teacher";
                        userOut.UserTags = reader.GetInt32 (4);
                        }
                    }
                await cnn.CloseAsync ();

                if (userOut.UserId > 0)
                    {
                    await LogAsync (userOut.UserId, cnn);
                    return userOut;
                    }
                return null;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("LoginAsTeacher error: " + ex.Message);
                cnn.Close ();
                return null;
                }
            }
        public async Task<User?> LoginStudentAsync (User user)
            {
            string? connString = _config.GetConnectionString ("cnni");
            string sql = "SELECT StudentId, TeacherId, StudentName, StudentPass, StudentTags, StudentNickname FROM Students WHERE StudentName=@studentname AND StudentPass=@studentpass AND TeacherId=@teacherid AND (StudentTags & 1) = 1";
            using SqlConnection cnn = new (connString);
            User userOut = new ();
            try
                {
                await cnn.OpenAsync ();
                using SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentname", user.UserName);
                cmd.Parameters.AddWithValue ("@studentpass", user.UserPass);
                cmd.Parameters.AddWithValue ("@teacherid", user.TeacherId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    if ((user.UserName.ToLower () == reader.GetString (2).ToLower ()) && (user.UserPass == reader.GetString (3)))
                        {
                        userOut.UserId = reader.GetInt32 (0);
                        userOut.TeacherId = reader.GetInt32 (1);
                        userOut.UserName = reader.GetString (2);
                        userOut.UserPass = reader.GetString (3);
                        userOut.UserRole = "student";
                        userOut.UserTags = reader.GetInt32 (4);
                        userOut.UserNickname = reader.GetString (5);
                        }
                    }
                if (userOut.UserId > 0)
                    {
                    await cnn.CloseAsync (); //logout because LogAsync tries to open cnn itset 
                    await LogAsync (userOut.UserId, cnn);
                    return userOut;
                    }
                return new User ();
                }
            catch (Exception ex)
                {
                Console.WriteLine ("*BeService: LoginAsStudent error: \n" + ex.Message);
                return new User ();
                }
            }
        public async Task LogAsync (int userId, SqlConnection cnn)
            {
            string sql = "INSERT INTO usrsLogs (UserId, UserLogText, DateTime) VALUES (@userid, @userlogtext, @datetime)";
            await cnn.OpenAsync ();
            SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", userId);
            cmd.Parameters.AddWithValue ("@userlogtext", 21);
            cmd.Parameters.AddWithValue ("@datetime", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
            await cmd.ExecuteNonQueryAsync ();
            }
        #endregion
        #region U:Usrs
        public async Task<int> Create_TeacherAsync (User user)
            {
            string sql = @"INSERT INTO usrs (UsrName, UsrPass, UsrNickname, UsrActive) VALUES (@usrname, @usrpass, @usrnickname, 1); 
                        SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@usrname", user.UserName);
            cmd.Parameters.AddWithValue ("@usrpass", user.UserPass);
            cmd.Parameters.AddWithValue ("@usrnickname", user.UserNickname);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<User>> Read_TeachersAsync ()
            {
            var users = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            using SqlCommand cmd = new ("SELECT UsrId, UsrName, UsrPass, UsrNickname, UsrTags FROM usrs WHERE (UsrTags & 1) = 1", cnn);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                users.Add (new User
                    {
                    UserId = reader.GetInt32 (0),
                    UserName = reader.GetString (1),
                    UserPass = reader.GetString (2),
                    UserNickname = reader.GetString (3),
                    UserTags = Convert.ToInt32 (reader.GetInt32 (4))
                    });
                }
            return users;
            }
        public async Task<bool> Update_TeacherAsync (User user)
            {
            string sql = "UPDATE usrs SET UsrPass=@usrpass, UsrNickname=@usrnickname WHERE UsrId=@userid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd3 = new SqlCommand (sql, cnn);
                cmd3.Parameters.AddWithValue ("@usrpass", user.UserPass);
                cmd3.Parameters.AddWithValue ("@usrnickname", user.UserNickname);
                cmd3.Parameters.AddWithValue ("@userid", user.UserId);
                await cmd3.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                return false;
                }
            }
        public async Task<bool> Delete_TeacherAsync (int userId)
            {
            string sql = @"
                        BEGIN TRANSACTION;
                        BEGIN TRY
                        DELETE FROM TestOptions WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId IN (SELECT CourseId FROM Courses UserId=@teacherid));
                        DELETE FROM Tests WHERE CourseId IN (SELECT CourseId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE StudentId IN (SELECT StudentId FROM Students WHERE UserId=@teacherid));
                        DELETE FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM ExamTests WHERE ExamId IN (SELECT ExamId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM ExamCompositions WHERE ExamId IN (SELECT ExamId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM Exams WHERE CourseId IN (SELECT CourseId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM CourseTopics WHERE CourseId IN (SELECT CourseId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM CourseFolders WHERE CourseId IN (SELECT CourseId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM StudentCourseTests WHERE StudentCourseId IN (SELECT StudentCourseId FROM StudentCourses WHERE CourseId IN (SELECT CourseId FROM Courses UserId=@teacherid));
                        DELETE FROM StudentCourses WHERE CourseId IN (SELECT CourseId FROM Courses WHERE UserId=@teacherid);
                        DELETE FROM Coueses WHERE UserId=@teacherid;
                        DELETE FROM StudentGroups WHERE GroupId IN (SELECT GroupId FROM Groups WHERE UserId=@teacherid);
                        DELETE FROM Groups WHERE UserId=@teacherid;
                        DELETE FROM StudentMessages WHERE MessageId IN (SELECT MessageId FROM messages WHERE UserId=@teacherid);
                        DELETE FROM Messages WHERE UserId=@teacherid;
                        DELETE FROM Students WHERE TeacherId=@teacherid;
                        DELETE FROM usrs WHERE ID=@id;
                        COMMIT TRANSACTION;
                        END TRY
                        BEGIN CATCH
                        ROLLBACK TRANSACTION
                        END CATCH;
                        ";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@teacherid", userId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region S:Students
        public async Task<int> Create_StudentAsync (User student)
            {
            string sql = @"INSERT INTO Students (TeacherId, StudentName, StudentPass, StudentNickname, StudentTags) 
                        VALUES (@teacherid, @studentname, @studentpass, @studentnickname, @studenttags); 
                        SELECT CAST (scope_identity() AS int) ";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.Add ("@teacherid", SqlDbType.Int).Value = student.TeacherId;
                cmd.Parameters.Add ("@studentname", SqlDbType.NVarChar, 50).Value = student.UserName;
                cmd.Parameters.Add ("@studentpass", SqlDbType.NVarChar, 50).Value = student.UserPass;
                cmd.Parameters.Add ("@studentnickname", SqlDbType.NVarChar, 50).Value = student.UserNickname;
                cmd.Parameters.Add ("@studenttags", SqlDbType.Int).Value = student.UserTags;
                int i = (int) await cmd.ExecuteScalarAsync ();
                return i;
                }
            catch (Exception ex)
                {
                await cnn.CloseAsync ();
                Console.WriteLine ($"DB error: {ex.Message}");
                return 0;
                }
            }
        public async Task<List<User>> Read_StudentsByKeywordAsync (int userId, string keyword, int readStudentGCEM)
            {
            //read list of Students by search
            List<User> lstStudents = new List<User> ();
            keyword = "%" + keyword + "%";
            string sql = @"SELECT s.StudentId, s.TeacherId, s.StudentName, s.StudentPass, s.StudentNickname, s.StudentTags 
                    FROM Students s 
                    WHERE s.TeacherId=@userid AND ((s.StudentName LIKE @keyword) OR (s.StudentNickname LIKE @keyword))
                    ORDER BY s.StudentName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", userId);
            cmd.Parameters.Add ("@keyword", SqlDbType.NVarChar, 200).Value = keyword;

            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstStudents.Add (new User
                    {
                    UserId = reader.GetInt32 (0),
                    TeacherId = reader.GetInt32 (1),
                    UserName = reader.GetString (2),
                    UserPass = reader.GetString (3),
                    UserNickname = reader.GetString (4),
                    UserTags = reader.GetInt32 (5),
                    UserRole = "-",
                    StudentGroups = new List<StudentGroup> (),
                    StudentCourses = new List<StudentCourse> (),
                    StudentExams = new List<StudentExam> (),
                    StudentMessages = new List<StudentMessage> (),
                    StudentNotes = new List<Note> ()
                    });
                }
            if ((readStudentGCEM & 1) == 1)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentGroups = await Read_StudentGroupsAsync (student.UserId, "ByStudentId");
                    }
                }
            if ((readStudentGCEM & 2) == 2)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentCourses = await Read_StudentCoursesAsync (student.UserId, "ByStudentId");
                    }
                }
            if ((readStudentGCEM & 4) == 4)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentExams = await Read_StudentExamsAsync (Id: student.UserId, mode: "ByStudentExam"); //3: get {1:inactives + 2:testOptions} 
                    }
                }
            if ((readStudentGCEM & 8) == 8)
                {
                foreach (User student in lstStudents)
                    {
                    student.StudentMessages = await Read_StudentMessagesAsync (student.UserId, "ByStudentIdByStudentId");
                    }
                }
            return lstStudents;
            }
        public async Task<List<User>> Read_StudentsByGCEMSIdAsync (int Id, string mode, int readStudentGCEM)
            {
            //read list of Students by {G/C/E/M}Id
            List<User> lstStudents = new List<User> ();
            string sql = @"SELECT s.StudentId, s.TeacherId, s.StudentName, s.StudentPass, s.StudentNickname, s.StudentTags FROM Students s ";
            switch (mode)
                {
                case "G":
                        {
                        sql += @"INNER JOIN StudentGroups sg ON s.StudentId = sg.StudentId WHERE sg.GroupId=@id ORDER BY s.StudentName";
                        break;
                        }
                case "C":
                        {
                        sql += @"INNER JOIN StudentCourses sc ON s.StudentId = sc.StudentId WHERE sc.CourseId=@id ORDER BY s.StudentName";
                        break;
                        }
                case "E":
                        {
                        sql += @"INNER JOIN StudentExams se ON s.StudentId = se.StudentId WHERE se.ExamId=@id ORDER BY s.StudentName";
                        break;
                        }
                case "M":
                        {
                        sql += @"INNER JOIN StudentMessages sm ON s.StudentId = sm.StudentId WHERE sm.MessageId=@id ORDER BY s.StudentName";
                        break;
                        }
                case "S":
                        {
                        sql += @"WHERE s.StudentId=@id ORDER BY s.StudentName";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    var student = new User
                        {
                        UserId = reader.GetInt32 (0),
                        TeacherId = reader.GetInt32 (1),
                        UserName = reader.GetString (2),
                        UserPass = reader.GetString (3),
                        UserNickname = reader.GetString (4),
                        UserTags = reader.GetInt32 (5),
                        UserRole = "-",
                        StudentGroups = new List<StudentGroup> (),
                        StudentCourses = new List<StudentCourse> (),
                        StudentExams = new List<StudentExam> (),
                        StudentMessages = new List<StudentMessage> (),
                        StudentNotes = new List<Note> ()
                        };
                    lstStudents.Add (student);
                    }
                if ((readStudentGCEM & 1) == 1)
                    {
                    //read studentGroups records
                    foreach (User student in lstStudents)
                        {
                        student.StudentGroups = await Read_StudentGroupsAsync (student.UserId, "ByStudentId");
                        }
                    }
                if ((readStudentGCEM & 2) == 2)
                    {
                    //read studentCourses records
                    foreach (User student in lstStudents)
                        {
                        student.StudentCourses = await Read_StudentCoursesAsync (student.UserId, "ByStudentId");
                        }
                    }
                if ((readStudentGCEM & 4) == 4)
                    {
                    //read studentExams records
                    foreach (User student in lstStudents)
                        {
                        student.StudentExams = await Read_StudentExamsAsync (student.UserId, "ByStudentId"); //1:getActives
                        }
                    }
                if ((readStudentGCEM & 8) == 8)
                    {
                    //read studentMessages records
                    foreach (User student in lstStudents)
                        {
                        student.StudentMessages = await Read_StudentMessagesAsync (student.UserId, "ByStudentId");
                        }
                    }
                return lstStudents;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("****C10 : \n" + ex.ToString ());
                return new List<User> ();
                }
            }
        public async Task<bool> Update_StudentAsync (User student)
            {
            //tags: 1:Active 2:ChangePass            
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            sql = @"Update Students SET 
                StudentName=@studentname, 
                StudentPass=@studentpass, 
                StudentNickname=@studentnickname, 
                StudentTags=@studenttags 
                WHERE StudentId=@studentid";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentname", student.UserName);
            cmd.Parameters.AddWithValue ("@studentpass", student.UserPass);
            cmd.Parameters.AddWithValue ("@studentnickname", student.UserNickname);
            cmd.Parameters.AddWithValue ("@studenttags", student.UserTags);
            cmd.Parameters.AddWithValue ("@studentid", student.UserId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Update_StudentTagsAsync (User student)
            {
            //tags: 1:Active 2:ChangePass            
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            sql = @"Update Students SET 
                StudentTags=@studenttags 
                WHERE StudentId=@studentid";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studenttags", student.UserTags);
            cmd.Parameters.AddWithValue ("@studentid", student.UserId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Remove_StudentFromListAsync (int studentId, string mode)
            {
            List<User> lstStudents = new List<User> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            string sql = "";
            switch (mode)
                {
                case "G":
                        {
                        sql = "DELETE FROM StudentGroups WHERE StudentId=@studentid";
                        break;
                        }
                case "C":
                        {
                        sql = @"DELETE FROM StudentCourseTests WHERE StudentCourseId IN (SELECT StudentCourseId FROM StudentCourses WHERE StudentId=@studentid);
                                DELETE FROM StudentCourses WHERE StudentId=@studentid";
                        break;
                        }
                case "E":
                        {
                        sql = @"DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE StudentId=@studentid);
                                DELETE FROM StudentExams WHERE StudentId=@studentid";
                        break;
                        }
                case "M":
                        {
                        sql = "DELETE FROM StudentMessages WHERE StudentId=@studentid";
                        break;
                        }
                }
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_StudentAsync (int studentId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            await cnn.OpenAsync ();
            sql = @"
                BEGIN TRANSACTION;
                BEGIN TRY
                DELETE FROM StudentGroups WHERE StudentId=@studentid;
                DELETE FROM StudentCourseTests WHERE StudentCourseId IN (SELECT StudentCourseId FROM StudentCourses WHERE StudentId=@studentid);
                DELETE FROM StudentCourses WHERE StudentId=@studentid;
                DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE StudentId=@studentid);
                DELETE FROM StudentExams WHERE StudentId=@studentid;
                DELETE FROM StudentMessages WHERE StudentId=@studentid;
                DELETE FROM Students WHERE StudentId=@studentid; 
                COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                ROLLBACK TRANSACTION;
                END CATCH; 
                ";
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        #endregion
        #region G:Groups
        public async Task<int> Create_GroupAsync (Group group)
            {
            string sql = @"INSERT INTO Groups (GroupName, UserId) VALUES (@groupname, @userid); 
                        SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@groupname", group.GroupName);
            cmd.Parameters.AddWithValue ("@userid", group.UserId);
            int i = (int) await cmd.ExecuteScalarAsync ();
            await cnn.CloseAsync ();
            return i;
            }
        public async Task<List<Group>> Read_GroupsAsync (User user, bool getGroupStudents)
            {
            List<Group> lstGroups = new List<Group> ();
            string sql = "SELECT g.GroupId, g.GroupName, g.UserId FROM Groups g WHERE g.UserId=@userid ORDER BY g.GroupName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", user.UserId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        var group = new Group
                            {
                            GroupId = reader.GetInt32 (0),
                            GroupName = reader.GetString (1),
                            UserId = reader.GetInt32 (2),
                            Students = new List<StudentGroup> (),
                            };
                        lstGroups.Add (group);
                        }
                    }
                if (getGroupStudents)
                    {
                    foreach (Group grp in lstGroups)
                        {
                        grp.Students = await Read_StudentGroupsAsync (grp.GroupId, "ByGroupid");
                        }
                    }
                return lstGroups;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new List<Group> ();
                }
            }
        public async Task<Group> Read_GroupAsync (int groupId)
            {
            //ModelState.Clear (); // disables auto validation
            Group group = new Group ();
            string sql = "SELECT g.GroupId, g.GroupName, g.UserId FROM Groups g WHERE g.GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupid", groupId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        group.GroupId = reader.GetInt32 (0);
                        group.GroupName = reader.GetString (1);
                        group.UserId = reader.GetInt32 (2);
                        group.Students = new List<StudentGroup> ();
                        }
                    }
                group.Students = await Read_StudentGroupsAsync (groupId, "ByGroupId");
                return group;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C10 : \n" + ex.ToString ());
                return new Group ();
                }
            }
        public async Task<bool> Update_GroupAsync (Group group)
            {
            string sql = "UPDATE Groups SET GroupName=@groupname WHERE GroupId=@groupid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@groupname", group.GroupName);
                cmd.Parameters.AddWithValue ("@groupid", group.GroupId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch
                {
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<int> Delete_GroupAsync (int groupId)
            {
            string sql = @"DELETE FROM StudentGroups WHERE GroupId=@groupid;
                        DELETE FROM Groups WHERE GroupId=@groupid ";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@groupid", groupId);
            int i = (int) cmd.ExecuteNonQuery ();
            return i;
            }
        #endregion
        #region SG:StudentGroups
        public async Task<bool> Create_StudentGroupsAsync (int groupId, List<int> lstStudentIds)
            {
            string sql = "INSERT INTO StudentGroups (StudentId, GroupId, DateTimeJoined, StudentGroupTags) VALUES (@studentid, @groupid, @datetimejoined, @studentgrouptags)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            foreach (int st in lstStudentIds)
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", st);
                cmd.Parameters.AddWithValue ("@groupid", groupId);
                cmd.Parameters.AddWithValue ("@datetimejoined", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
                cmd.Parameters.AddWithValue ("@studentgrouptags", 1);
                int i = (int) cmd.ExecuteNonQuery ();
                }
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<List<StudentGroup>> Read_StudentGroupsAsync (int Id, string mode)
            {
            //read SG records
            List<StudentGroup> lstStudentGroups = new List<StudentGroup> ();
            string sql = @"SELECT sg.StudentGroupId, sg.StudentId, sg.GroupId, sg.DateTimeJoined, sg.StudentGroupTags, g.GroupName, s.StudentName, s.StudentNickname, s.StudentTags 
                        FROM StudentGroups sg 
                        INNER JOIN Groups g ON sg.GroupId = g.GroupId
                        INNER JOIN Students s ON sg.StudentId = s.StudentId ";
            switch (mode)
                {
                case "ByStudentId":
                        {
                        sql += " WHERE sg.StudentId=@id ORDER BY s.StudentNickname";
                        break;
                        }
                case "ByGroupId":
                        {
                        sql += " WHERE sg.GroupId=@id ORDER BY s.StudentNickname";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        var studentGroup = new StudentGroup
                            {
                            StudentGroupId = reader.GetInt32 (0),
                            StudentId = reader.GetInt32 (1),
                            GroupId = reader.GetInt32 (2),
                            DateTimeJoined = reader.GetString (3),
                            StudentGroupTags = reader.GetInt32 (4),
                            StudentName = reader.GetString (5),
                            GroupName = reader.GetString (6),
                            StudentNickname = reader.GetString (7),
                            StudentTags = reader.GetInt32 (8),
                            };
                        lstStudentGroups.Add (studentGroup);
                        }
                    }
                return lstStudentGroups;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("be service: Read_StudentGroupsAsync -- error: \n" + ex.ToString ());
                return new List<StudentGroup> ();
                }
            }
        //Update Tags , Delete SG
        #endregion
        #region C:Courses
        public async Task<int> Create_CourseAsync (Course course)
            {
            string sql = "INSERT INTO Courses (UserId, CourseName) VALUES (@userid, @coursename); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", course.UserId);
            cmd.Parameters.AddWithValue ("@coursename", course.CourseName);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<Course>> Read_CoursesAsync (int userId)
            {
            List<Course> lstCourses = new List<Course> ();
            string sql = "SELECT CourseId, UserId, CourseName, CourseUnits, CourseRtl FROM Courses WHERE UserId=@userid ORDER BY CourseName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", userId);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstCourses.Add (new Course
                        {
                        CourseId = reader.GetInt32 (0),
                        UserId = userId,
                        CourseName = reader.GetString (2),
                        CourseUnits = reader.GetInt32 (3),
                        CourseRtl = reader.GetBoolean (4),
                        CourseTopics = new List<CourseTopic> (),
                        CourseFolders = new List<CourseFolder> (),
                        Students = new List<StudentCourse> ()
                        });
                    }
                foreach (Course crs in lstCourses)
                    {
                    var crsTopic = await Read_CourseTopicsAsync (crs.CourseId);
                    if (crsTopic != null)
                        {
                        crs.CourseTopics = crsTopic;
                        }
                    var crsFolder = await Read_CourseFoldersAsync (crs.CourseId);
                    if (crsFolder != null)
                        {
                        crs.CourseFolders = crsFolder;
                        }
                    }
                return lstCourses;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C02 - Error\n" + ex.ToString ());
                return lstCourses;
                }
            }
        public async Task<Course> Read_CourseAsync (int courseId, bool getStudentCourses)
            {
            Course course = new Course ();
            string sql = "SELECT CourseId, UserId, CourseName, CourseUnits, CourseRtl FROM Courses WHERE CourseId=@courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseid", courseId);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    course.CourseId = reader.GetInt32 (0);
                    course.UserId = reader.GetInt32 (1);
                    course.CourseName = reader.GetString (2);
                    course.CourseUnits = reader.GetInt32 (3);
                    course.CourseRtl = reader.GetBoolean (4);
                    course.CourseTopics = new List<CourseTopic> ();
                    course.CourseFolders = new List<CourseFolder> ();
                    course.Students = new List<StudentCourse> ();
                    }
                var crsTopic = await Read_CourseTopicsAsync (course.CourseId);
                if (crsTopic != null)
                    {
                    course.CourseTopics = crsTopic;
                    }
                var crsFolder = await Read_CourseFoldersAsync (course.CourseId);
                if (crsFolder != null)
                    {
                    course.CourseFolders = crsFolder;
                    }
                if (getStudentCourses)
                    {
                    var crsStudents = await Read_StudentCoursesAsync (course.CourseId, "ByCourseId");
                    if (crsStudents != null)
                        {
                        course.Students = crsStudents;
                        }
                    }
                return course;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("C02 - Error\n" + ex.ToString ());
                return course;
                }
            }
        public async Task<bool> Update_CourseAsync (Course course)
            {
            string sql = "UPDATE Courses SET CourseName=@coursename WHERE CourseId = @courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursename", course.CourseName);
            cmd.Parameters.AddWithValue ("@courseid", course.CourseId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseAsync (int courseId)
            {
            string sql = @"DELETE FROM Courses WHERE CourseId = @courseid;
                    DELETE FROM CourseTopics WHERE CourseId = @courseid;
                    DELETE FROM TestOptions WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM ExamTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM StudentExamTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM StudentCourseTests WHERE TestId IN (SELECT TestId FROM Tests WHERE CourseId = @courseid);
                    DELETE FROM Tests WHERE CourseId = @courseid;
                    DELETE FROM ExamCompositions WHERE ExamId IN (SELECT ExamId FROM Exams WHERE CourseId = @courseid);
                    DELETE FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM Exams WHERE CourseId = @courseid);
                    DELETE FROM Exams WHERE CourseId = @courseid;
                    DELETE FROM StudentCourses WHERE CourseId = @courseid;";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region CF:CourseFolders
        public async Task<int> Create_CourseFolderAsync (CourseFolder courseFolder)
            {
            string sql = "INSERT INTO CourseFolders (CourseId, CourseFolderTitle, CourseFolderUrl, CourseFolderActive) VALUES (@courseid, @coursefoldertitle, @coursefolderurl, @coursefolderactive); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseFolder.CourseId);
            cmd.Parameters.AddWithValue ("@coursefoldertitle", courseFolder.CourseFolderTitle);
            cmd.Parameters.AddWithValue ("@coursefolderurl", courseFolder.CourseFolderUrl);
            cmd.Parameters.AddWithValue ("@coursefolderactive", courseFolder.CourseFolderActive);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<CourseFolder>> Read_CourseFoldersAsync (int courseId)
            {
            List<CourseFolder> lstCourseFolders = new ();

            string sql = "SELECT CourseFolderId, CourseId, CourseFolderTitle, CourseFolderUrl, CourseFolderActive FROM CourseFolders WHERE CourseId=@courseId AND CourseFolderActive=1 ORDER BY CourseFolderTitle";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseId", courseId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                int i = 0;
                lstCourseFolders.Clear ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    lstCourseFolders.Add (new CourseFolder
                        {
                        CourseFolderId = reader.GetInt32 (0),
                        CourseId = courseId,
                        CourseFolderTitle = reader.GetString (2),
                        CourseFolderUrl = reader.GetString (3),
                        CourseFolderActive = reader.GetBoolean (4)
                        });
                    }
                return lstCourseFolders;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                return new List<CourseFolder> ();
                }
            }
        public async Task<bool> Update_CourseFolderAsync (CourseFolder courseFolder)
            {
            string sql = "UPDATE CourseFolders SET CourseFolderTitle = @coursefoldertitle, CourseFolderUrl = @coursefolderurl, CourseFolderActive = @coursefolderactive WHERE CourseFolderId = @coursefolderid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursefoldertitle", courseFolder.CourseFolderTitle);
            cmd.Parameters.AddWithValue ("@coursefolderurl", courseFolder.CourseFolderUrl);
            cmd.Parameters.AddWithValue ("@coursefolderactive", courseFolder.CourseFolderActive);
            cmd.Parameters.AddWithValue ("@coursefolderid", courseFolder.CourseFolderId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseFolderAsync (int courseFolderId)
            {
            string sql = @"DELETE FROM CourseFolders WHERE CourseFolderId=@coursefolderid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursefolderid", courseFolderId);
            int i = cmd.ExecuteNonQuery ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region CT:CourseTopics
        public async Task<int> Create_CourseTopicAsync (CourseTopic courseTopic)
            {
            string sql = "INSERT INTO CourseTopics (CourseId, CourseTopicTitle) VALUES (@courseid, @coursetopictitle); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseTopic.CourseId);
            cmd.Parameters.AddWithValue ("@coursetopictitle", courseTopic.CourseTopicTitle);
            int i = (int) cmd.ExecuteScalar ();
            return i;
            }
        public async Task<List<CourseTopic>> Read_CourseTopicsAsync (int courseId)
            {
            List<CourseTopic> lstCourseTopics = new ();

            string sql = "SELECT CourseTopicId, CourseId, CourseTopicTitle FROM CourseTopics WHERE CourseId=@courseId ORDER BY CourseTopicTitle";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@courseId", courseId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                int i = 0;
                lstCourseTopics.Clear ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    lstCourseTopics.Add (new CourseTopic
                        {
                        CourseTopicId = reader.GetInt32 (0),
                        CourseId = courseId,
                        CourseTopicTitle = reader.GetString (2)
                        });
                    }
                return lstCourseTopics;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                return new List<CourseTopic> ();
                }
            }
        public async Task<bool> Update_CourseTopicAsync (CourseTopic courseTopic)
            {
            string sql = "UPDATE CourseTopics SET CourseTopicTitle = @coursetopictitle WHERE CourseTopicId = @id";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursetopictitle", courseTopic.CourseTopicTitle);
            cmd.Parameters.AddWithValue ("@id", courseTopic.CourseTopicId);
            int i = cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_CourseTopicAsync (int courseTopicId)
            {
            string sql = @"DELETE FROM CourseTopics 
                            WHERE CourseTopicId=@coursetopicid 
                            AND NOT EXISTS (SELECT 1 FROM Tests WHERE TopicId=@coursetopicid) 
                            AND NOT EXISTS (SELECT 1 FROM ExamCompositions WHERE TopicId=@coursetopicid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@coursetopicid", courseTopicId);
            int i = cmd.ExecuteNonQuery ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region SC:StudentCourses
        public async Task<bool> Create_StudentCoursesAsync (int courseId, List<int> lstStudentIds)
            {
            string sql = "INSERT INTO StudentCourses (StudentId, CourseId, StudentCourseTags) VALUES (@studentid, @courseid, @studentcoursetags)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            foreach (int st in lstStudentIds)
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", st);
                cmd.Parameters.AddWithValue ("@courseid", courseId);
                cmd.Parameters.AddWithValue ("@studentcoursetags", 3);
                int i = (int) cmd.ExecuteNonQuery ();
                }
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Create_StudentCourseAsync (int studentId, int courseId)
            {
            string sql = "INSERT INTO StudentCourses (StudentId, CourseId) VALUES (@studentid, @courseid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<List<StudentCourse>> Read_StudentCoursesAsync (int Id, string mode)
            {
            //read SC records for one Student
            List<StudentCourse> lstStudentCourses = new List<StudentCourse> ();
            string sql = @"SELECT sc.StudentCourseId, sc.StudentId, sc.CourseId, c.CourseName, s.StudentName, s.StudentNickname, sc.NumberOfTests, sc.CorrectAnswers, sc.StudentCourseTags 
                        FROM StudentCourses sc 
                        INNER JOIN Courses c ON sc.CourseId = c.CourseId 
                        INNER JOIN Students s ON sc.StudentId = s.StudentId ";
            switch (mode)
                {
                case "ByStudentIdIgnoreInactiveCourses":
                        {
                        sql += " WHERE sc.StudentId=@id AND (StudentCourseTags & 1) = 1 ORDER BY s.StudentNickname";
                        break;
                        }
                case "ByStudentId":
                        {
                        sql += " WHERE sc.StudentId=@id ORDER BY s.StudentNickname";
                        break;
                        }
                case "ByCourseId":
                        {
                        sql += " WHERE sc.CourseId=@id ORDER BY s.StudentNickname";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstStudentCourses.Add (new StudentCourse
                        {
                        StudentCourseId = reader.GetInt32 (0),
                        StudentId = reader.GetInt32 (1),
                        CourseId = reader.GetInt32 (2),
                        CourseName = reader.GetString (3),
                        StudentName = reader.GetString (4),
                        StudentNickname = reader.GetString (5),
                        NumberOfTests = reader.GetInt32 (6),
                        CorrectAnswers = reader.GetInt32 (7),
                        StudentCourseTags = reader.GetInt32 (8)
                        });
                    }
                await cnn.CloseAsync ();
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in: region SC: StudentCourses: " + ex);
                await cnn.CloseAsync ();
                }
            return lstStudentCourses;
            }
        public async Task<StudentCourse> Read_StudentCourseAsync (int studentCourseId)
            {
            StudentCourse studentCourse = new StudentCourse ();
            string sql = "SELECT sc.StudentCourseId, sc.StudentId, sc.CourseId, c.CourseName, sc.NumberOfTests, sc.CorrectAnswers FROM StudentCourses sc INNER JOIN Courses c ON sc.CourseId=c.CourseId WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        studentCourse.StudentCourseId = reader.GetInt32 (0);
                        studentCourse.StudentId = reader.GetInt32 (1);
                        studentCourse.CourseId = reader.GetInt32 (2);
                        studentCourse.CourseName = reader.GetString (3);
                        studentCourse.NumberOfTests = reader.GetInt32 (4);
                        studentCourse.CorrectAnswers = reader.GetInt32 (5);
                        }
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in C14 - GetStudentCouses: " + ex);
                await cnn.CloseAsync ();
                }
            return studentCourse;
            }
        public async Task<bool> Update_StudentCourseAsync (StudentCourse studentCourse)
            {
            string sql = "UPDATE StudentCourses SET NumberOfTests=@numberoftests, CorrectAnswers=@correctanswers, StudentCourseTags=@studentcoursetags WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@numberoftests", studentCourse.NumberOfTests);
            cmd.Parameters.AddWithValue ("@correctanswers", studentCourse.CorrectAnswers);
            cmd.Parameters.AddWithValue ("@studentcoursetags", studentCourse.StudentCourseTags);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourse.StudentCourseId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return (i > 0) ? true : false;
            }
        public async Task<bool> Update_StudentCoursesTagsAsync (List<int> lstStudentIds, int courseId, bool activeStatus)
            {
            int i = 0;
            string sql = (activeStatus) ? "UPDATE StudentCourses SET StudentCourseTags = (StudentCourseTags | 3) WHERE StudentId=@studentid AND CourseId=@courseid" : "UPDATE StudentCourses SET StudentCourseTags = (StudentCourseTags & ~7) WHERE StudentId=@studentid AND CourseId=@courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            foreach (int studentId in lstStudentIds)
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", studentId);
                cmd.Parameters.AddWithValue ("@courseid", courseId);
                i = await cmd.ExecuteNonQueryAsync ();
                }
            return (i > 0) ? true : false;
            }
        public async Task<bool> Delete_StudentCoursesAsync (int Id, string mode)
            {
            bool result = true;
            string sql = "";
            switch (mode)
                {
                case "ByStudentId":
                        {
                        sql = @"DELETE FROM StudentCourses WHERE StudentId=@id"; //delete all courses of a student
                        break;
                        }
                case "ByCourseId":
                        {
                        sql = @"DELETE FROM StudentCourses WHERE CourseId=@id"; //delete all students of a course
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            try
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                int i = cmd.ExecuteNonQuery ();
                if (i < 0)
                    {
                    result = false;
                    }
                }
            catch (Exception ex)
                {
                result = false;
                }
            await cnn.CloseAsync ();
            return result;
            }
        public async Task<bool> Delete_StudentCourseAsync (int studentCourseId)
            {
            string sql = "DELETE FROM StudentCourses WHERE StudentCourseId=@studentcourseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> CalculatePoints_StudentCourseAsync (StudentCourse studentCourse)
            {
            try
                {
                string? connString = _config.GetConnectionString ("cnni");
                using SqlConnection cnn = new (connString);
                await cnn.OpenAsync ();
                //count nTests
                string sql1 = "SELECT Count (TestKey) AS t FROM RunningExams WHERE StudentId=@studentid AND CourseId=@courseid";
                SqlCommand cmd1 = new SqlCommand (sql1, cnn);
                cmd1.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd1.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                int n = (int) cmd1.ExecuteScalar ();
                //count nCorrects
                string sql2 = "SELECT Count (TestKey) AS t FROM RunningExams WHERE StudentId=@studentid AND CourseId=@courseid AND TestKey=UserAns";
                SqlCommand cmd2 = new SqlCommand (sql2, cnn);
                cmd2.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd2.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                int c = (int) cmd2.ExecuteScalar ();
                //Save n,c
                string sql3 = "UPDATE StudentCourses SET NumberOfTests=@numberoftests, CorrectAnswers-@correctanswers WHERE StudentId=@studentid AND CourseId=@courseid";
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.Parameters.AddWithValue ("@numberoftests", n);
                cmd3.Parameters.AddWithValue ("@correctanswers", c);
                cmd3.Parameters.AddWithValue ("@studentid", studentCourse.StudentId);
                cmd3.Parameters.AddWithValue ("@courseid", studentCourse.CourseId);
                await cmd3.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in ReCalcRunningPoint:\n" + ex.ToString ());
                return false;
                }
            }
        #endregion
        #region SCT:StudentCourseTests
        public async Task<bool> Create_StudentCourseTestAsync (StudentCourseTest studentCourseTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "INSERT INTO StudentExamTests (UserAns, [DateTime], StudentCourseId, TestId, TestKey) VALUES (@userans, @datetime, @studentcourseid, @testid, @testkey)";
            await cnn.OpenAsync ();
            var cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns.ToString ());
            cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId.ToString ());
            cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId.ToString ());
            cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey.ToString ());
            await cmd.ExecuteNonQueryAsync ();
            //update counts
            sql = "UPDATE StudentCourses SET ";
            sql += (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "NumberOfTests = NumberOfTests +1 ,CorrectAnswers = CorrectAnswers +1 " : "NumberOfTests = NumberOfTests +1 ";
            sql += "WHERE StudentCourseId=@studentcourseid";
            var cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId.ToString ());
            await cmd2.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<List<StudentCourseTest>> Read_StudentCourseTestsAsync (StudentCourse studentCourse, bool readOptions)
            {
            List<StudentCourseTest> lstStudentCourseTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT sct.StudentCourseId, sct.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            sct.TestKey, sct.UserAns, sct.DateTime
            FROM StudentCourseTests sct
            INNER JOIN Tests t ON sct.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE sct.StudentCourseId=@studentcourseid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourse.StudentCourseId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        var sct = new StudentCourseTest
                            {
                            StudentCourseId = reader.GetInt32 (0),
                            TestId = reader.GetInt32 (1),
                            TestTitle = reader.GetString (2),
                            TestType = reader.GetInt32 (3),
                            CourseTopicId = reader.GetInt32 (4),
                            CourseTopicTitle = reader.GetString (5),
                            TestTags = reader.GetInt32 (6),
                            TestLevel = reader.GetInt32 (7),
                            TestKey = reader.GetInt32 (8),
                            UserAns = reader.GetInt32 (9),
                            DateTime = reader.GetString (10),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            };
                        lstStudentCourseTests.Add (sct);
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentCourseTest t in lstStudentCourseTests)
                        {
                        t.TestOptions = await Read_TestOptionsAsync (t.TestId, cnn);
                        }
                    }
                return lstStudentCourseTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentCourseTest> ();
                }
            }
        public async Task<StudentCourseTest> Read_StudentCourseTestRandomAsync (int studentCourseId, bool readOptions, bool retry)
            {
            StudentCourseTest studentCourseTest = new StudentCourseTest ();
            string sql = @"SELECT TOP (1) sc.StudentCourseId, t.TestId, t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel
                         FROM Tests t
                         INNER JOIN Courses c ON t.CourseId = c.CourseId 
                         INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId
                         INNER JOIN StudentCourses sc ON sc.CourseId = c.CourseId AND sc.StudentCourseId = @studentcourseid";
            if (retry)
                {
                sql += " WHERE t.TestId IN (SELECT sct.TestId FROM StudentCourseTests sct WHERE sct.StudentCourseId=@studentcourseid AND sct.UserAns <> sct.TestKey) ORDER BY NEWID()";
                }
            else
                {
                sql += " ORDER BY NEWID()";
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        studentCourseTest.StudentCourseId = reader.GetInt32 (0);
                        studentCourseTest.TestId = reader.GetInt32 (1);
                        studentCourseTest.TestTitle = reader.GetString (2);
                        studentCourseTest.TestType = reader.GetInt32 (3);
                        studentCourseTest.CourseTopicId = reader.GetInt32 (4);
                        studentCourseTest.CourseTopicTitle = reader.GetString (5);
                        studentCourseTest.TestTags = reader.GetInt32 (6);
                        studentCourseTest.TestLevel = reader.GetInt32 (7);
                        studentCourseTest.TestIndex = ++index;
                        studentCourseTest.TestOptions = new List<TestOption> ();
                        }
                    }
                if (readOptions)
                    {
                    studentCourseTest.TestOptions = await Read_TestOptionsAsync (studentCourseTest.TestId, cnn);
                    }
                return studentCourseTest;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new StudentCourseTest ();
                }
            }
        public async Task<bool> Update_StudentCourseTestAsync (StudentCourseTest studentCourseTest, string mode)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            if (mode == "new")
                {
                sql = "INSERT INTO StudentCourseTests (StudentCourseId, TestId, TestKey, UserAns, [DateTime]) VALUES (@studentcourseid, @testid, @testkey, @userans, @datetime)";
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId);
                cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey);
                cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns);
                cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
                await cmd.ExecuteNonQueryAsync ();
                sql = (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "UPDATE StudentCourses SET CorrectAnswers=(CorrectAnswers + 1), NumberOfTests=(NumberOfTests + 1)  WHERE StudentCourseId=@studentcourseid" : "UPDATE StudentCourses SET NumberOfTests=(NumberOfTests + 1) WHERE StudentCourseId=@studentcourseid";
                var cmd2 = new SqlCommand (sql, cnn);
                cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                await cmd2.ExecuteNonQueryAsync ();
                return true;
                }
            else if (mode == "edit")
                {
                sql = "UPDATE StudentCourseTests SET TestKey=@testkey, UserAns=@userans, [DateTime]=@datetime WHERE StudentCourseId=@studentcourseid AND TestId=@testid";
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testkey", studentCourseTest.TestKey);
                cmd.Parameters.AddWithValue ("@userans", studentCourseTest.UserAns);
                cmd.Parameters.AddWithValue ("@datetime", studentCourseTest.DateTime);
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                cmd.Parameters.AddWithValue ("@testid", studentCourseTest.TestId);
                await cmd.ExecuteNonQueryAsync ();
                sql = (studentCourseTest.UserAns == studentCourseTest.TestKey) ? "UPDATE StudentCourses SET CorrectAnswers=(CorrectAnswers + 1) WHERE StudentCourseId=@studentcourseid" : "";
                if (sql != "")
                    {
                    var cmd2 = new SqlCommand (sql, cnn);
                    cmd2.Parameters.AddWithValue ("@studentcourseid", studentCourseTest.StudentCourseId);
                    await cmd2.ExecuteNonQueryAsync ();
                    }
                return true;
                }
            else
                {
                return false;
                }
            //update counts
            }
        public async Task<bool> Delete_StudentCourseTestsAsync (string mode, StudentCourse studentCourse)
            {
            string sql = "";
            switch (mode)
                {
                case "filter":
                        {
                        sql = @"DELETE FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid AND UserAns=TestKey; 
                                UPDATE StudentCourses 
                                SET NumberOfTests=(SELECT COUNT(TestId) FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid), 
                                CorrectAnswers=(SELECT COUNT(TestId) FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid AND TestKey=UserAns) 
                                WHERE StudentCourseId=@studentcourseid";
                        break;
                        }
                case "wipeout":
                        {
                        sql = @"DELETE FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid; 
                                UPDATE StudentCourses SET NumberOfTests=0, CorrectAnswers=0 WHERE StudentCourseId=@studentcourseid;";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            SqlCommand cmd = new SqlCommand (sql, cnn);
            try
                {
                await cnn.OpenAsync ();
                cmd.Parameters.AddWithValue ("@studentcourseid", studentCourse.StudentCourseId);
                cmd.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ($"ERROR in: BeServ-C14_Delete_StudentCourseTestsAsync \n --- mode: {mode}\n courseId:{studentCourse.CourseId}\n student:{studentCourse.StudentId}\n" + ex);
                return false;
                }
            }
        #endregion
        #region T:Tests
        public async Task<int> Create_TestAsync (Test test)
            {
            string sql = "INSERT INTO Tests (CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags) VALUES (@courseid, @topicid, @testtitle, @testtype, @testlevel, @testtags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", test.CourseId);
            cmd.Parameters.AddWithValue ("@topicid", test.TopicId);
            cmd.Parameters.AddWithValue ("@testtitle", test.TestTitle);
            cmd.Parameters.AddWithValue ("@testtype", test.TestType);
            cmd.Parameters.AddWithValue ("@testlevel", test.TestLevel);
            cmd.Parameters.AddWithValue ("@testtags", test.TestTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            await cnn.CloseAsync ();
            foreach (TestOption tstOption in test.TestOptions)
                {
                tstOption.TestId = i;
                int r = await Create_TestOptionAsync (tstOption);
                }
            return i;
            }
        public async Task<Test> Read_TestByTestIdAsync (int testId, bool readOptions)
            {
            Test test = new Test ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    test.TestId = reader.GetInt32 (0);
                    test.CourseId = reader.GetInt32 (1);
                    test.TopicId = reader.GetInt32 (2);
                    test.TestTitle = reader.GetString (3);
                    test.TestType = reader.GetInt32 (4);
                    test.TestLevel = reader.GetInt32 (5);
                    test.TestTags = reader.GetInt32 (6);
                    test.TestOptions = new List<TestOption> ();
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                }
            return test;
            }
        public async Task<Test> Read_TestByStudentExamTestIdAsync (long studentExamTestId, bool readOptions)
            {
            Test test = new Test ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN StudentExamTests est ON t.TestId=est.TestId WHERE est.TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", studentExamTestId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    test.TestId = reader.GetInt32 (0);
                    test.CourseId = reader.GetInt32 (1);
                    test.TopicId = reader.GetInt32 (2);
                    test.TestTitle = reader.GetString (3);
                    test.TestType = reader.GetInt32 (4);
                    test.TestLevel = reader.GetInt32 (5);
                    test.TestTags = reader.GetInt32 (6);
                    test.TestOptions = new List<TestOption> ();
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                }
            return test;
            }
        public async Task<List<Test>> Read_TestsByCourseIdAsync (int courseId, int pageNumber, bool readOptions)
            {
            int pageSize = 20;
            int offset = (pageNumber - 1) * pageSize;
            List<Test> lstTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t WHERE t.CourseId=@courseid ORDER BY t.TestId OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            cmd.Parameters.AddWithValue ("@offset", offset);
            cmd.Parameters.AddWithValue ("@pagesize", pageSize);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstTests;
            }
        public async Task<List<Test>> Read_TestsByCourseTopicIdAsync (int courseTopicId, int pageNumber, bool readOptions)
            {
            int pageSize = 20;
            int offset = (pageNumber - 1) * pageSize;
            List<Test> lstCourseTopicTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN Courses c ON t.CourseId=c.CourseId WHERE t.TopicId=@topicid ORDER BY t.TestId OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@topicid", courseTopicId);
            cmd.Parameters.AddWithValue ("@offset", offset);
            cmd.Parameters.AddWithValue ("@pagesize", pageSize);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstCourseTopicTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstCourseTopicTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstCourseTopicTests;
            }
        public async Task<List<Test>> Read_TestsBySearchAsync (string strSearch, int courseId, bool readOptions)
            {
            List<Test> lstCourseTopicTests = new List<Test> ();
            strSearch = $"%{strSearch}%";
            string sql = "SELECT TOP 50 t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN Courses c ON t.CourseId=c.CourseId WHERE t.TestTitle LIKE @strSearch AND t.CourseId=@courseid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.Add ("@strSearch", SqlDbType.NVarChar, 200).Value = strSearch;
            //cmd.Parameters.AddWithValue ("@strsearch", strSearch);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstCourseTopicTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstCourseTopicTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstCourseTopicTests;
            }
        public async Task<List<Test>> Read_TestsByExamIdAsync (int examId, bool readOptions)
            {
            List<Test> lstExamTests = new List<Test> ();
            string sql = "SELECT t.TestId, t.CourseId, t.TopicId, t.TestTitle, t.TestType, t.TestLevel, t.TestTags FROM Tests t INNER JOIN ExamTests et ON t.TestId=et.TestId WHERE et.ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstExamTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstExamTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstExamTests;
            }
        public async Task<List<Test>> Read_TestsByStudentExamIdAsync (int studentExamId, bool readOptions)
            {
            List<Test> lstStudentExamTests = new List<Test> ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId IN (SELECT TestId FROM StudentExamTests WHERE StudentExamId=@studentexamid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstStudentExamTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstStudentExamTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstStudentExamTests;
            }
        public async Task<List<Test>> Read_TestsByStudentCourseIdAsync (int studentCourseId, bool readOptions)
            {
            List<Test> lstStudentCourseTests = new List<Test> ();
            string sql = "SELECT TestId, CourseId, TopicId, TestTitle, TestType, TestLevel, TestTags FROM Tests WHERE TestId IN (SELECT TestId FROM StudentCourseTests WHERE StudentCourseId=@studentcourseid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentcourseid", studentCourseId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                {
                while (await reader.ReadAsync ())
                    {
                    lstStudentCourseTests.Add (new Test
                        {
                        TestId = reader.GetInt32 (0),
                        CourseId = reader.GetInt32 (1),
                        TopicId = reader.GetInt32 (2),
                        TestTitle = reader.GetString (3),
                        TestType = reader.GetInt32 (4),
                        TestLevel = reader.GetInt32 (5),
                        TestTags = reader.GetInt32 (6)
                        });
                    }
                }
            if (readOptions)
                {
                //with existing open cnn
                foreach (Test test in lstStudentCourseTests)
                    {
                    test.TestOptions = await Read_TestOptionsAsync (test.TestId, cnn);
                    }
                }
            return lstStudentCourseTests;
            }
        public async Task<bool> Update_TestAsync (Test test)
            {
            string sql = "UPDATE Tests SET CourseId=@courseid, TopicId=@topicid, TestTitle=@testtitle, TestType=@testtype, TestLevel=@testlevel, TestTags=@testtags WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", test.CourseId);
            cmd.Parameters.AddWithValue ("@topicid", test.TopicId);
            cmd.Parameters.AddWithValue ("@testtitle", test.TestTitle);
            cmd.Parameters.AddWithValue ("@testtype", test.TestType);
            cmd.Parameters.AddWithValue ("@testlevel", test.TestLevel);
            cmd.Parameters.AddWithValue ("@testtags", test.TestTags);
            cmd.Parameters.AddWithValue ("@testid", test.TestId);
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestAsync (int testId)
            {
            string sql = "DELETE FROM Tests WHERE TestId=@testid; DELETE FROM TestOptions WHERE TestId=@testid;";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> ImportExcelTestsAsync (string filePath, int courseId)
            {
            CourseTopic topic = new CourseTopic ();
            List<TestOption> lstOptions = new List<TestOption> ();
            TestOption option = new TestOption ();
            Test test = new Test ();
            test.CourseId = courseId;
            try
                {
                using (IXLWorkbook WB = new XLWorkbook (filePath))
                    {
                    var WS0 = WB.Worksheets.ElementAtOrDefault (0);
                    int iRow = 0;
                    //Excel columns: {1:Test, 2:testRTL, 3:optRTL, 4:nOpt, 5-9:opt1-5Texts, 10:Ans, 11:forceLast 12:Level 13:Topic} 
                    foreach (IXLRow Rowx in WS0.Rows ())
                        {
                        iRow = iRow + 1;
                        if (iRow > 1)
                            {
                            test.TestTitle = WS0.Cell (iRow, 1).Value.ToString ();
                            test.TestTags = 0;//initialize to 0
                            if (WS0.Cell (iRow, 2).Value.ToString ().ToLower () == "y")
                                {
                                test.TestTags = 1;
                                }
                            if (WS0.Cell (iRow, 3).Value.ToString ().ToLower () == "y")
                                {
                                test.TestTags = test.TestTags + 2;
                                }
                            test.TestType = Convert.ToInt32 (WS0.Cell (iRow, 4).Value.ToString ());   //nOpts
                            test.TestLevel = Convert.ToInt32 (WS0.Cell (iRow, 12).Value.ToString ()); //TestLevel
                            topic.CourseTopicTitle = Strings.Left (WS0.Cell (iRow, 13).Value.ToString (), 30).Trim (); //Topic
                            //test.TopicId = await Read_CourseTopicsAsync (topic);
                            if (test.TopicId == 0)
                                {
                                test.TopicId = await Create_CourseTopicAsync (topic);
                                }
                            //Console.WriteLine ("-----\niRow= " + iRow + "\ntest.CourseId= " + test.CourseId + "\ntopic.CourseTopicTitle= " + topic.CourseTopicTitle + "\ntest.TopicId= " + test.TopicId);
                            //save Test
                            test.TestId = await Create_TestAsync (test);
                            //Options
                            lstOptions.Clear ();
                            for (int col = 5; col <= 9; col++)
                                {
                                lstOptions.Add (new TestOption { TestOptionTitle = WS0.Cell (iRow, col).Value.ToString () });
                                }
                            int answ = Convert.ToInt32 (WS0.Cell (iRow, 10).Value.ToString () ?? "0");
                            lstOptions[answ - 1].TestOptionTags = 2; //IsAns
                            int forceLast = Convert.ToInt32 (WS0.Cell (iRow, 11).Value.ToString () ?? "0");
                            if (forceLast != 0)
                                {
                                lstOptions[forceLast - 1].TestOptionTags += 1; //ForceLast
                                }
                            //Save options
                            for (int i = 0; i < test.TestType; i++)
                                {
                                int addOpt = await Create_TestOptionAsync (lstOptions[i]);
                                if (addOpt != 0)
                                    {
                                    //Console.WriteLine ("testOption " + i + " was not added to db :: " + lstOptions[i].TestOptionTitle);
                                    }
                                }
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in:[ImportExcel] \n" + ex.ToString ());
                }
            return true;
            }
        #endregion
        #region TO:TestOptions
        public async Task<int> Create_TestOptionAsync (TestOption testOption)
            {
            string sql = "INSERT INTO TestOptions (TestId, TestOptionTitle, TestOptionTags) VALUES (@testid, @testoptiontitle, @testoptiontags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testOption.TestId);
            cmd.Parameters.AddWithValue ("@testoptiontitle", testOption.TestOptionTitle);
            cmd.Parameters.AddWithValue ("@testoptiontags", testOption.TestOptionTags);
            int r = (int) await cmd.ExecuteScalarAsync ();
            return r;
            }
        public async Task<List<TestOption>> Read_TestOptionsAsync (int testId, SqlConnection cnn)
            {
            //TestOptions are collected by DTOs: {4:Tests, 8:ExamTests, 12:StudentExamTests, 14:StudentExamTests}
            Random random = new Random ();
            List<TestOption> lstTestOptions = new List<TestOption> ();
            string sql = "SELECT TestOptionId, TestId, TestOptionTitle, TestOptionTags FROM TestOptions WHERE TestId=@testid";
            try
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testid", testId);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        lstTestOptions.Add (new TestOption
                            {
                            TestOptionId = reader.GetInt32 (0),
                            TestId = reader.GetInt32 (1),
                            TestOptionTitle = reader.GetString (2),
                            TestOptionTags = reader.GetInt32 (3)
                            });
                        }
                    }
                //shuffle
                int Nshuffle = lstTestOptions.Count; //shuffle all options
                TestOption tmpx;
                TestOption tmp1 = new TestOption ();
                for (int p = 0; p < lstTestOptions.Count; p++)
                    {
                    if ((lstTestOptions[p].TestOptionTags & 1) == 1)
                        {
                        tmpx = lstTestOptions[p];
                        //send ForceLast to last position
                        tmp1 = lstTestOptions[lstTestOptions.Count - 1];
                        lstTestOptions[lstTestOptions.Count - 1] = tmpx;
                        lstTestOptions[p] = tmp1;
                        Nshuffle = lstTestOptions.Count - 1;
                        break;
                        }
                    }
                //do shuffle
                int rnd = 0;
                for (int i = Nshuffle - 1; i > 0; i--)
                    {
                    int j = random.Next (0, i + 1);
                    var temp = lstTestOptions[i];
                    lstTestOptions[i] = lstTestOptions[j];
                    lstTestOptions[j] = temp;
                    }
                return lstTestOptions;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in BEService C05: Create_TestOptionsAsync: " + ex);
                return new List<TestOption> ();
                }
            }
        public async Task<TestOption> Read_TestOptionAsync (int testOptionId, SqlConnection cnn)
            {
            TestOption testOption = new TestOption ();
            string sql = "SELECT TestOptionId, TestId, TestOptionTitle, TestOptionTags FROM TestOptions WHERE TestOptionId=@testoptionid";
            try
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@testoptionid", testOptionId.ToString ());
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        testOption.TestOptionId = reader.GetInt32 (0);
                        testOption.TestId = reader.GetInt32 (1);
                        testOption.TestOptionTitle = reader.GetString (2);
                        testOption.TestOptionTags = reader.GetInt32 (3);
                        }
                    return testOption;
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in BEService C05: Create_TestOptionsAsync: " + ex);
                return new TestOption ();
                }
            }
        public async Task<bool> Update_TestOptionAsync (TestOption testOption)
            {
            string sql = "UPDATE TestOptions SET TestId=@testid, TestOptionTitle=@testoptiontitle, TestOptionTags=@testoptiontags WHERE TestOptionId=@testoptionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", testOption.TestId);
            cmd.Parameters.AddWithValue ("@testoptiontitle", testOption.TestOptionTitle);
            cmd.Parameters.AddWithValue ("@testoptiontags", testOption.TestOptionTags);
            cmd.Parameters.AddWithValue ("@testoptionid", testOption.TestOptionId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestOptionsAsync (int testId)
            {
            string sql = "DELETE FROM TestOptions WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testoptionid", testId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        public async Task<bool> Delete_TestOptionAsync (int testOptionId)
            {
            string sql = "DELETE FROM TestOptions WHERE TestOptionId=@testoptionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            using SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@testoptionid", testOptionId);
            int i = await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region E:Exams
        public async Task<int> Create_ExamAsync (Exam exam)
            {
            string sql = "INSERT INTO Exams (CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags) VALUES (@courseid, @examtitle, @examdatetime, @examduration, @examntests, @examtags); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", exam.CourseId);
            cmd.Parameters.AddWithValue ("@examtitle", exam.ExamTitle);
            cmd.Parameters.AddWithValue ("@examdatetime", exam.ExamDateTime);
            cmd.Parameters.AddWithValue ("@examduration", exam.ExamDuration);
            cmd.Parameters.AddWithValue ("@examntests", exam.ExamNTests);
            cmd.Parameters.AddWithValue ("@examtags", exam.ExamTags);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<Exam>> Read_ExamsAsync (int courseId)
            {
            List<Exam> lstExams = new List<Exam> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamId, CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags FROM Exams WHERE CourseID=@courseid", cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExams.Add (new Exam
                    {
                    ExamId = reader.GetInt32 (0),
                    CourseId = reader.GetInt32 (1),
                    ExamTitle = reader.GetString (2),
                    ExamDateTime = reader.GetString (3),
                    ExamDuration = reader.GetInt32 (4),
                    ExamNTests = reader.GetInt32 (5),
                    ExamTags = reader.GetInt32 (6)
                    });
                }
            return lstExams;
            }
        public async Task<Exam> Read_ExamAsync (int examId, bool getStudentsList)
            {
            Exam exam = new Exam ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamId, CourseId, ExamTitle, ExamDateTime, ExamDuration, ExamNTests, ExamTags FROM Exams WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                exam.ExamId = reader.GetInt32 (0);
                exam.CourseId = reader.GetInt32 (1);
                exam.ExamTitle = reader.GetString (2);
                exam.ExamDateTime = reader.GetString (3);
                exam.ExamDuration = reader.GetInt32 (4);
                exam.ExamNTests = reader.GetInt32 (5);
                exam.ExamTags = reader.GetInt32 (6);
                exam.Students = new List<StudentExam> ();
                //get students
                if (getStudentsList)
                    {
                    var lstStudents = await Read_StudentExamsAsync (exam.ExamId, "ByExamId");
                    if (lstStudents != null)
                        {
                        exam.Students = lstStudents;
                        }
                    }
                }
            return exam;
            }
        public async Task<bool> Update_ExamAsync (Exam exam)
            {
            string sql = "UPDATE Exams SET CourseId=@courseid, ExamTitle=@examtitle, ExamDateTime=@examdatetime, ExamDuration=@examduration, ExamNTests= @examntests, ExamTags=@examtags WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", exam.CourseId);
            cmd.Parameters.AddWithValue ("@examtitle", exam.ExamTitle);
            cmd.Parameters.AddWithValue ("@examdatetime", exam.ExamDateTime);
            cmd.Parameters.AddWithValue ("@examduration", exam.ExamDuration);
            cmd.Parameters.AddWithValue ("@examntests", exam.ExamNTests);
            cmd.Parameters.AddWithValue ("@examtags", exam.ExamTags);
            cmd.Parameters.AddWithValue ("@examid", exam.ExamId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamsAsync (int courseId)
            {
            string sql = @"BEGIN TRANSACTION;
WITH ExamIds AS (SELECT ExamId FROM Exams WHERE CourseId=@courseid)
DELETE FROM ExamCompositions WHERE ExamId IN (SELECT ExamId FROM ExamIds); 
DELETE FROM ExamTests WHERE ExamId IN (SELECT ExamId FROM ExamIds);
DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM ExamIds)); 
DELETE FROM StudentExams WHERE ExamId IN (SELECT ExamId FROM ExamIds); 
DELETE FROM Exams WHERE ExamId IN (SELECT ExamId From ExamIds);
COMMIT TRANSACTION;
";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@courseid", courseId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamAsync (int examId)
            {
            string sql = @"BEGIN TRANSACTION;
DELETE FROM Exams WHERE ExamId=@examid;
DELETE FROM ExamCompositions WHERE ExamId=@examid; 
DELETE FROM ExamTests WHERE ExamId=@examid; 
DELETE FROM StudentExamTests WHERE StudentExamId IN (SELECT StudentExamId FROM StudentExams WHERE ExamId=@examid); 
DELETE FROM StudentExams WHERE ExamId=@examid; 
COMMIT TRANSACTION;
";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteNonQueryAsync ();
            return true;
            }
        #endregion
        #region EC:ExamCompositions
        public async Task<int> Create_ExamCompositionAsync (ExamComposition examComposition)
            {
            string sql = "INSERT INTO ExamCompositions (ExamId, TopicId, TopicNTests, TestsLevel) VALUES (@examid, @topicid, @topicntests, @testslevel); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examComposition.ExamId);
            cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
            cmd.Parameters.AddWithValue ("@topicntests", examComposition.TopicNTests);
            cmd.Parameters.AddWithValue ("@testslevel", examComposition.TestsLevel);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<ExamComposition>> Read_ExamCompositionsAsync (int examId)
            {
            List<ExamComposition> lstExamComposition = new List<ExamComposition> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamCompositionId, ExamId, TopicId, TopicNTests, TestsLevel FROM ExamCompositions WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExamComposition.Add (new ExamComposition
                    {
                    ExamCompositionId = reader.GetInt32 (0),
                    ExamId = reader.GetInt32 (1),
                    TopicId = reader.GetInt32 (2),
                    TopicNTests = reader.GetInt32 (3),
                    TestsLevel = reader.GetInt32 (4)
                    });
                }
            return lstExamComposition;
            }
        public async Task<ExamComposition> Read_ExamCompositionAsync (int examCompositionId)
            {
            ExamComposition examComposition = new ExamComposition ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamCompositionId, ExamId, TopicId, TopicNTests, TestsLevel FROM ExamCompositions WHERE ExamCompositionId=@examcompositionid", cnn);
            cmd.Parameters.AddWithValue ("@examcompositionid", examCompositionId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                examComposition.ExamCompositionId = reader.GetInt32 (0);
                examComposition.ExamId = reader.GetInt32 (1);
                examComposition.TopicId = reader.GetInt32 (2);
                examComposition.TopicNTests = reader.GetInt32 (3);
                examComposition.TestsLevel = reader.GetInt32 (4);
                }
            return examComposition;
            }
        public async Task<bool> Update_ExamCompositionAsync (ExamComposition examComposition)
            {
            string sql = "UPDATE ExamCompositions SET ExamId=@examid, TopicId=@topicid, TopicNTests=@topicntests, TestsLevel=@testslevel WHERE ExamCompositionId=@examcompositionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examComposition.ExamId);
            cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
            cmd.Parameters.AddWithValue ("@topicntests", examComposition.TopicNTests);
            cmd.Parameters.AddWithValue ("@testslevel", examComposition.TestsLevel);
            cmd.Parameters.AddWithValue ("@examcompositionid", examComposition.ExamCompositionId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamCompositionsAsync (int examId)
            {
            string sql = "DELETE FROM ExamCompositions WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamCompositionAsync (int examCompositionId)
            {
            string sql = "DELETE FROM ExamCompositions WHERE ExamCompositionId=@examcompositionid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examcompositionid", examCompositionId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        #endregion
        #region ET:ExamTests
        public async Task<int> Create_ExamTestsByExamCompositionAsync (ExamComposition examComposition)
            {
            List<int> lstTestIds = new List<int> ();
            string sql = "SELECT Top (@ntests) TestId From Tests WHERE TopicId=@topicid ORDER BY NEWID()";
            string? connString = _config.GetConnectionString ("cnni");
            using (SqlConnection cnn = new (connString))
                {
                using SqlCommand cmd = new (sql, cnn);
                cmd.Parameters.AddWithValue ("@ntests", examComposition.TopicNTests);
                cmd.Parameters.AddWithValue ("@topicid", examComposition.TopicId);
                await cnn.OpenAsync ();
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstTestIds.Add (reader.GetInt32 (0));
                    }
                }
            ExamTest examTest = new ExamTest { ExamId = examComposition.ExamId, TestId = 0, PercentCorrect = 0, PercentIncorrect = 0, PercentHelped = 0 };
            foreach (int testId in lstTestIds)
                {
                examTest.TestId = testId;
                await Create_ExamTestAsync (examTest);
                }
            return 1;
            }
        public async Task<int> Create_ExamTestAsync (ExamTest examTest)
            {
            string sql = "INSERT INTO ExamTests (ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped) VALUES (@examid, @testid, @percentcorrect, @percentincorrect, @percenthelped); SELECT CAST (scope_identity() AS int)"; //get ID of newly added record
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examTest.ExamId);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            cmd.Parameters.AddWithValue ("@percentcorrect", examTest.PercentCorrect);
            cmd.Parameters.AddWithValue ("@percentincorrect", examTest.PercentIncorrect);
            cmd.Parameters.AddWithValue ("@percenthelped", examTest.PercentHelped);
            await cnn.OpenAsync ();
            int i = (int) await cmd.ExecuteScalarAsync ();
            //update number of tests
            sql = @"UPDATE e 
                  SET ExamNTests = COALESCE(t.cnt, 0) 
                  FROM Exams e 
                  LEFT JOIN (SELECT ExamId, COUNT(*) AS cnt 
                             FROM ExamTests 
                             GROUP BY ExamId) t 
                  ON e.ExamId = t.ExamId 
                  WHERE e.ExamId = @examid";
            using SqlCommand cmd2 = new (sql, cnn);
            cmd2.Parameters.AddWithValue ("@examid", examTest.ExamId);
            await cmd2.ExecuteNonQueryAsync ();
            return i;
            }
        public async Task<List<ExamTest>> Read_ExamTestsAsync (int examId)
            {
            List<ExamTest> lstExamTests = new List<ExamTest> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamTestId, ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped FROM ExamTests WHERE ExamId=@examid", cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                lstExamTests.Add (new ExamTest
                    {
                    ExamTestId = reader.GetInt32 (0),
                    ExamId = reader.GetInt32 (1),
                    TestId = reader.GetInt32 (2),
                    PercentCorrect = reader.GetInt32 (3),
                    PercentIncorrect = reader.GetInt32 (4),
                    PercentHelped = reader.GetInt32 (5)
                    });
                }
            return lstExamTests;
            }
        public async Task<ExamTest> Read_ExamTestAsync (int examTestId)
            {
            ExamTest examTest = new ExamTest ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new ("SELECT ExamTestId, ExamId, TestId, PercentCorrect, PercentIncorrect, PercentHelped FROM ExamTests WHERE ExamTestId=@examtestid", cnn);
            cmd.Parameters.AddWithValue ("@examtestid", examTestId);
            await cnn.OpenAsync ();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            while (await reader.ReadAsync ())
                {
                examTest.ExamTestId = reader.GetInt32 (0);
                examTest.ExamId = reader.GetInt32 (1);
                examTest.TestId = reader.GetInt32 (2);
                examTest.PercentCorrect = reader.GetInt32 (3);
                examTest.PercentIncorrect = reader.GetInt32 (4);
                examTest.PercentHelped = reader.GetInt32 (5);
                }
            return examTest;
            }
        public async Task<bool> Update_ExamTestAsync (ExamTest examTest)
            {
            string sql = "UPDATE ExamTests SET ExamId=@examid, TestId=@testid, PercentCorrect=@percentcorrect, PercentIncorrect=@percentincorrect, PercentHelped=@percenthelped";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examTest.ExamId);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            cmd.Parameters.AddWithValue ("@percentcorrect", examTest.PercentCorrect);
            cmd.Parameters.AddWithValue ("@percentincorrect", examTest.PercentIncorrect);
            cmd.Parameters.AddWithValue ("@percenthelped", examTest.PercentHelped);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamTestsAsync (int examId)
            {
            string sql = "DELETE FROM ExamTests WHERE ExamId=@examid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        public async Task<bool> Delete_ExamTestAsync (ExamTest examTest)
            {
            string sql = "DELETE FROM ExamTests WHERE TestId=@testid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", examTest.TestId);
            await cnn.OpenAsync ();
            await cmd.ExecuteReaderAsync ();
            return true;
            }
        #endregion
        #region SE:StudentExams
        public async Task<bool> Create_StudentExamsAsync (int examId, List<int> lstStudentIds)
            {
            Random random = new Random ();
            string sql = "";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            foreach (int studentId in lstStudentIds)
                {
                //1 Add record to: StudentExams
                sql = "INSERT INTO StudentExams (StudentId, ExamId, StartDateTime, FinishDateTime, StudentExamTags, StudentExamPoint) VALUES (@studentid, @examid, @startdatetime, @finishdatetime, @studentexamtags, @studentexampoint); SELECT CAST (scope_identity() AS int)";
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", studentId);
                cmd.Parameters.AddWithValue ("@examid", examId);
                cmd.Parameters.AddWithValue ("@startdatetime", "");
                cmd.Parameters.AddWithValue ("@finishdatetime", "");
                cmd.Parameters.AddWithValue ("@studentexamtags", 1);
                cmd.Parameters.AddWithValue ("@studentexampoint", 0);
                int newStudentExamId = (int) await cmd.ExecuteScalarAsync ();
                //2 read exam tests
                List<Test> lstExamTests = new List<Test> ();
                lstExamTests = await Read_TestsByExamIdAsync (examId, true);
                //3 shuffle tests into: lstExamTests
                int rnd = 0;
                for (int k = 0; k < lstExamTests.Count; k++)
                    {
                    rnd = random.Next (k, (lstExamTests.Count));
                    Test tmpTest = lstExamTests[rnd];
                    lstExamTests[rnd] = lstExamTests[k];
                    lstExamTests[k] = tmpTest;
                    }
                //4 options
                foreach (Test tst in lstExamTests)
                    {
                    StudentExamTest est = new StudentExamTest ();
                    /*NOTICE:
                      If Read_TestOptions is called multiple times in rapid succession (as it is inside the loop over lstExamTests),
                      the Random constructor will use the same seed (based on system time), resulting in identical shuffles of lstTestOptions.
                      So even though you're calling Read_TestOptions(testId) for different tests, the shuffled list ends up in the same order,
                      and the key option (tag 2) is always in the same position, likely the first one found.
                      to fix, use a shared Random instance: pass a single Random object to Read_TestOptions */
                    await Read_TestOptionsAsync (tst.TestId, cnn);
                    est.StudentId = studentId;
                    est.StudentExamId = newStudentExamId;
                    est.TestId = tst.TestId;
                    est.Opt1Id = (tst.TestOptions.Count > 0) ? (tst.TestOptions[0].TestOptionId) : 0;
                    est.Opt2Id = (tst.TestOptions.Count > 1) ? (tst.TestOptions[1].TestOptionId) : 0;
                    est.Opt3Id = (tst.TestOptions.Count > 2) ? (tst.TestOptions[2].TestOptionId) : 0;
                    est.Opt4Id = (tst.TestOptions.Count > 3) ? (tst.TestOptions[3].TestOptionId) : 0;
                    est.Opt5Id = (tst.TestOptions.Count > 4) ? (tst.TestOptions[4].TestOptionId) : 0;
                    foreach (TestOption opt in tst.TestOptions)
                        {
                        if ((opt.TestOptionTags & 2) == 2)
                            {
                            est.StudentExamTestKey = opt.TestOptionId;
                            }
                        }
                    //insert into ExamSheets
                    string sql2 = "INSERT INTO StudentExamTests (StudentExamId, TestId, Opt1Id, Opt2Id, Opt3Id, Opt4Id, Opt5Id, StudentExamTestKey, StudentExamTestAns, StudentExamTestTags) VALUES (@studentexamid, @testid, @opt1id, @opt2id, @opt3id, @opt4id, @opt5id, @key, 0, 0)";
                    var cmd2 = new Microsoft.Data.SqlClient.SqlCommand (sql2, cnn);
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Parameters.AddWithValue ("@studentexamid", newStudentExamId);
                    cmd2.Parameters.AddWithValue ("@testid", est.TestId);
                    cmd2.Parameters.AddWithValue ("@opt1id", est.Opt1Id);
                    cmd2.Parameters.AddWithValue ("@opt2id", est.Opt2Id);
                    cmd2.Parameters.AddWithValue ("@opt3id", est.Opt3Id);
                    cmd2.Parameters.AddWithValue ("@opt4id", est.Opt4Id);
                    cmd2.Parameters.AddWithValue ("@opt5id", est.Opt5Id);
                    cmd2.Parameters.AddWithValue ("@key", est.StudentExamTestKey);
                    await cmd2.ExecuteNonQueryAsync ();
                    }
                }
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<List<StudentExam>> Read_StudentExamsAsync (int Id, string mode)
            {
            int i = 0;
            List<StudentExam> lstStudentsExam = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT se.StudentExamId, se.StudentId, s.StudentName, s.StudentNickname, c.CourseId, c.CourseName,
                e.ExamId, e.ExamTitle, e.ExamDateTime, e.ExamDuration, e.ExamNTests, e.ExamTags,
                se.StartDateTime, se.FinishDateTime, se.StudentExamTags, se.StudentExamPoint
                FROM StudentExams se 
                INNER JOIN Exams e ON se.ExamId = e.ExamId
                INNER JOIN Courses c ON e.CourseId = c.CourseId
                INNER JOIN Students s ON se.StudentId = s.StudentId ";
            switch (mode)
                {
                case "ByStudentId":
                        {
                        sql += " WHERE se.StudentId=@id ORDER BY e.ExamDateTime";
                        break;
                        }
                case "ByExamId":
                        {
                        sql += " WHERE se.ExamId=@id ORDER BY e.ExamDateTime";
                        break;
                        }
                }
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                var reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    i++;
                    var exam = new StudentExam ();
                    exam.StudentExamId = reader.GetInt32 (0);
                    exam.StudentId = reader.GetInt32 (1);
                    exam.StudentName = reader.GetString (2);
                    exam.StudentNickname = reader.GetString (3);
                    exam.CourseId = reader.GetInt32 (4);
                    exam.CourseName = reader.GetString (5);
                    exam.ExamId = reader.GetInt32 (6);
                    exam.ExamIndex = i;
                    exam.ExamTitle = reader.GetString (7);
                    exam.ExamDateTime = reader.GetString (8);
                    exam.ExamDuration = reader.GetInt32 (9);
                    exam.ExamNTests = reader.GetInt32 (10);
                    exam.ExamTags = reader.GetInt32 (11);
                    exam.StartDateTime = reader.GetString (12);
                    exam.FinishDateTime = reader.GetString (13);
                    exam.StudentExamTags = reader.GetInt32 (14);
                    exam.StudentExamPoint = reader.GetDouble (15);
                    lstStudentsExam.Add (exam);
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine ("in be StudentExams: \n" + ex.ToString ());
                }
            await cnn.CloseAsync ();
            return lstStudentsExam;
            }
        public async Task<StudentExam> Read_StudentExamAsync (int studentExamId, bool readInactiveExams)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT TOP 1 se.StudentExamId, se.StudentId, s.StudentName, s.StudentNickname, c.CourseId, c.CourseName,
                 e.ExamId, e.ExamTitle, e.ExamDateTime, e.ExamDuration, e.ExamNTests, e.ExamTags,
                 se.StartDateTime, se.FinishDateTime, se.StudentExamTags, se.StudentExamPoint
                 FROM StudentExams se 
                 INNER JOIN Exams e ON se.ExamId = e.ExamId
                 INNER JOIN Courses c ON e.CourseId = c.CourseId
                 INNER JOIN Students s ON se.StudentId = s.StudentId
                 WHERE se.StudentExamId=@studentexamid ";
            if (!readInactiveExams)
                {
                sql += " AND (e.ExamTags & 1) = 1";
                }
            sql += " ORDER BY e.ExamDateTime";
            try
                {
                var exam = new StudentExam ();
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
                using (var reader = await cmd.ExecuteReaderAsync ())
                    {
                    while (await reader.ReadAsync ())
                        {
                        exam.StudentExamId = reader.GetInt32 (0);
                        exam.StudentId = reader.GetInt32 (1);
                        exam.StudentName = reader.GetString (2);
                        exam.StudentNickname = reader.GetString (3);
                        exam.CourseId = reader.GetInt32 (4);
                        exam.CourseName = reader.GetString (5);
                        exam.ExamId = reader.GetInt32 (6);
                        exam.ExamIndex = 0;
                        exam.ExamTitle = reader.GetString (7);
                        exam.ExamDateTime = reader.GetString (8);
                        exam.ExamDuration = reader.GetInt32 (9);
                        exam.ExamNTests = reader.GetInt32 (10);
                        exam.ExamTags = reader.GetInt32 (11);
                        exam.StartDateTime = reader.GetString (12);
                        exam.FinishDateTime = reader.GetString (13);
                        exam.StudentExamTags = reader.GetInt32 (14);
                        exam.StudentExamPoint = reader.GetDouble (15);
                        exam.StudentExamTests = new List<StudentExamTest> ();
                        }
                    }
                exam.StudentExamTests = await Read_StudentExamTestsAsync (exam.StudentExamId, true, cnn);
                return exam;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("BeService: Read_StudentExam ERROR: \n" + ex.ToString ());
                }
            return new StudentExam ();
            }
        public async Task<bool> Update_StudentExamAsync (StudentExam studentExam)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "UPDATE StudentExams SET StudentId=@studentid, ExamId=@examid, StartDateTime=@startdatetime, FinishDateTime=@finishdatetime, StudentExamTags=@studentexamtags, StudentExamPoint=@studentexampoint WHERE StudentExamId=@studentexamid ";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExam.StudentId);
            cmd.Parameters.AddWithValue ("@examid", studentExam.ExamId);
            cmd.Parameters.AddWithValue ("@startdatetime", studentExam.StartDateTime);
            cmd.Parameters.AddWithValue ("@finishdatetime", studentExam.FinishDateTime);
            cmd.Parameters.AddWithValue ("@studentexamtags", studentExam.StudentExamTags);
            cmd.Parameters.AddWithValue ("@studentexampoint", studentExam.StudentExamPoint);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExam.StudentExamId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Update_StudentsExamTagsAsync (string mode, int examId)
            {
            //Update tags of an exam for all student
            string currentDateTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm");
            string sql = "";
            switch (mode)
                {
                case "startedOn":
                        {
                        sql = "UPDATE StudentExams SET StudentExamTags=(StudentExamTags | 2), StartDateTime=@currectdatetime ";
                        break;
                        }
                case "startedOff":
                        {
                        sql = "UPDATE StudentExams SET StudentExamTags=(StudentExamTags & ~2), StartDateTime='' ";
                        break;
                        }
                case "finishedOn":
                        {
                        sql = "UPDATE StudentExams SET StudentExamTags=(StudentExamTags | 4), FinishDateTime=@currectdatetime ";
                        break;
                        }
                case "finishedOff":
                        {
                        sql = "UPDATE StudentExams SET StudentExamTags=(StudentExamTags & ~4) , FinishDateTime= ''";
                        break;
                        }
                }
            if (sql == "")
                {
                return false;
                }
            else
                {
                sql += " WHERE ExamId=@examid";
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@examid", examId);
                cmd.Parameters.AddWithValue ("@currectdatetime", currentDateTime);
                int c = cmd.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                //get list of students have this exam
                List<StudentExam> lstStudentsExams = await Read_StudentExamsAsync (examId, "ByExamid");
                foreach (StudentExam stdntex in lstStudentsExams)
                    {
                    if (stdntex.FinishDateTime.Length > 0)
                        {
                        bool result = await CalculatePoints_StudentExamsAsync (stdntex.StudentExamId);
                        }
                    }
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in: SetTags \n" + ex.ToString ());
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTagsAsync (StudentExam tempStudentExam)
            {
            //Update tags of an exam for a student
            string sql = "UPDATE StudentExams SET StudentExamTags=@studentexamtags, StartDateTime=@startdatetime, FinishDateTime=@finishdatetime WHERE StudentExamId=@studentexamid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentexamtags", tempStudentExam.StudentExamTags);
                cmd.Parameters.AddWithValue ("@startdatetime", tempStudentExam.StartDateTime);
                cmd.Parameters.AddWithValue ("@finishdatetime", tempStudentExam.FinishDateTime);
                cmd.Parameters.AddWithValue ("@studentexamid", tempStudentExam.StudentExamId);
                int c = cmd.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                if (tempStudentExam.FinishDateTime.Length > 0)
                    {
                    bool result = await CalculatePoints_StudentExamsAsync (tempStudentExam.StudentExamId);
                    }
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error in: SetTagStarted \n" + ex.ToString ());
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Delete_StudentExamsByStudentIdAsync (int studentId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE StudentId=@studentid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamsByExamIdAsync (int examId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE Exam=@examid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@examid", examId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamAsync (int studentExamId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExams WHERE StudentExamId=@studentexamid; DELETE FROM StudentExamTests WHERE StudentExamId=@studentexamid;";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> CalculatePoints_StudentExamsAsync (int studentexamid)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                string sql1 = "SELECT Count (est.StudentExamTestKey) AS t FROM StudentExamTests est INNER JOIN StudentExams se ON est.StudentExamId=se.StudentExamId WHERE est.StudentExamId=@studentexamid";
                await cnn.OpenAsync ();
                var cmdx1 = new SqlCommand (sql1, cnn);
                cmdx1.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int n = (int) cmdx1.ExecuteScalar ();
                string sql2 = "SELECT Count (est.StudentExamTestKey) AS m FROM StudentExamTests est INNER JOIN StudentExams se ON est.StudentExamId=se.StudentExamId WHERE est.StudentExamId=@studentexamid AND StudentExamTestKey=StudentExamTestAns";
                var cmdx2 = new SqlCommand (sql2, cnn);
                cmdx2.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int c = (int) cmdx2.ExecuteScalar ();
                double p = (Math.Abs (2000 * ((1.0 * c) / (1.0 * n))) / 100);
                string sql3 = "UPDATE StudentExams SET StudentExamPoint=@point WHERE StudentExamId=@studentexamid";
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.AddWithValue ("@point", p.ToString ());
                cmd3.Parameters.AddWithValue ("@studentexamid", studentexamid);
                int x = cmd3.ExecuteNonQuery ();
                await cnn.CloseAsync ();
                Console.WriteLine ("n / c / p :" + n + "  -  " + c + "  -  " + p);
                return true;
                }
            catch (Exception ex)
                {
                await cnn.CloseAsync ();
                Console.WriteLine ("CALAC ERROR: \n" + ex.ToString ());
                return false;
                }
            }
        #endregion
        #region SET:StudentExamTests
        public async Task<int> Create_StudentExamTestAsync (StudentExamTest studentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "INSERT INTO StudentExamTests (StudentId, StudentExamId, TestId, Opt1Id, Opt2Id, Opt3Id, Opt4Id, Opt5Id, StudentExamTestKey, StudentExamTestAns, StudentExamTestTags) VALUES (@studentid, @studentexamid, @testid, @opt1id, @opt2id, @opt3id, @opt4id, @opt5id, @StudentExamTestkey, @StudentExamTestans, @StudentExamTesttags); SELECT CAST (scope_identity() AS int)";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExamTest.StudentId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@opt1id", studentExamTest.Opt1Id);
            cmd.Parameters.AddWithValue ("@opt2id", studentExamTest.Opt2Id);
            cmd.Parameters.AddWithValue ("@opt3id", studentExamTest.Opt3Id);
            cmd.Parameters.AddWithValue ("@opt4id", studentExamTest.Opt4Id);
            cmd.Parameters.AddWithValue ("@opt5id", studentExamTest.Opt5Id);
            cmd.Parameters.AddWithValue ("@StudentExamTestkey", studentExamTest.StudentExamTestKey);
            cmd.Parameters.AddWithValue ("@StudentExamTestans", studentExamTest.StudentExamTestAns);
            cmd.Parameters.AddWithValue ("@StudentExamTesttags", studentExamTest.StudentExamTestTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            return i;
            }
        public async Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions)
            {
            //this [overload] has its own cnn
            List<StudentExamTest> lstStudentExamTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamId=@studentexamid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        lstStudentExamTests.Add (new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            });
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentExamTest t in lstStudentExamTests)
                        {
                        int[] optIds = { t.Opt1Id, t.Opt2Id, t.Opt3Id, t.Opt4Id, t.Opt5Id };
                        for (int j = 0; j < t.TestType && j < 5; j++)
                            {
                            t.TestOptions.Add (await Read_TestOptionAsync (optIds[j], cnn));
                            }
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<List<StudentExamTest>> Read_StudentExamTestsAsync (int studentExamId, bool readOptions, SqlConnection cnn)
            {
            //this [overload] is called from another method with an open cnn (see method parameters)
            List<StudentExamTest> lstStudentExamTests = new ();
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamId=@studentexamid";
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        var est = new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            };
                        lstStudentExamTests.Add (est);
                        }
                    }
                if (readOptions)
                    {
                    foreach (StudentExamTest t in lstStudentExamTests)
                        {
                        int[] optIds = { t.Opt1Id, t.Opt2Id, t.Opt3Id, t.Opt4Id, t.Opt5Id };
                        for (int j = 0; j < t.TestType && j < 5; j++)
                            {
                            t.TestOptions.Add (await Read_TestOptionAsync (optIds[j], cnn));
                            }
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<StudentExamTest> Read_StudentExamTestAsync (long studentExamTestId, bool readOptions)
            {
            StudentExamTest studentExamTest = new StudentExamTest ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.StudentExamTestId=@studentexamtestid";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamtestid", studentExamTestId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        studentExamTest.StudentExamTestId = reader.GetInt64 (0);
                        studentExamTest.StudentId = reader.GetInt32 (1);
                        studentExamTest.StudentExamId = reader.GetInt32 (2);
                        studentExamTest.TestId = reader.GetInt32 (3);
                        studentExamTest.TestTitle = reader.GetString (4);
                        studentExamTest.TestType = reader.GetInt32 (5);
                        studentExamTest.CourseTopicId = reader.GetInt32 (6);
                        studentExamTest.CourseTopicTitle = reader.GetString (7);
                        studentExamTest.TestTags = reader.GetInt32 (8);
                        studentExamTest.TestLevel = reader.GetInt32 (9);
                        studentExamTest.Opt1Id = reader.GetInt32 (10);
                        studentExamTest.Opt2Id = reader.GetInt32 (11);
                        studentExamTest.Opt3Id = reader.GetInt32 (12);
                        studentExamTest.Opt4Id = reader.GetInt32 (13);
                        studentExamTest.Opt5Id = reader.GetInt32 (14);
                        studentExamTest.StudentExamTestKey = reader.GetInt32 (15);
                        studentExamTest.StudentExamTestAns = reader.GetInt32 (16);
                        studentExamTest.StudentExamTestTags = reader.GetInt32 (17);
                        studentExamTest.TestIndex = ++index;
                        studentExamTest.TestOptions = new List<TestOption> ();
                        }
                    }
                if (readOptions)
                    {
                    int[] optIds = { studentExamTest.Opt1Id, studentExamTest.Opt2Id, studentExamTest.Opt3Id, studentExamTest.Opt4Id, studentExamTest.Opt5Id };
                    for (int j = 0; j < studentExamTest.TestType && j < 5; j++)
                        {
                        studentExamTest.TestOptions.Add (await Read_TestOptionAsync (optIds[j], cnn));
                        }
                    }
                return studentExamTest;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new StudentExamTest ();
                }
            }
        public async Task<List<StudentExamTest>> Read_StudentsExamTestAsync (StudentExamTest studentExamTest)
            {
            //this [overload] has its own cnn
            List<StudentExamTest> lstStudentExamTests = new ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = @"SELECT est.StudentExamTestId, se.StudentId, est.StudentExamId, est.TestId,
            t.TestTitle, t.TestType, t.TopicId, ct.CourseTopicTitle, t.TestTags, t.TestLevel,
            est.Opt1Id, est.Opt2Id, est.Opt3Id, est.Opt4Id, est.Opt5Id,
            est.StudentExamTestKey, est.StudentExamTestAns, est.StudentExamTestTags
            FROM StudentExams se
            INNER JOIN StudentExamTests est ON se.StudentExamId = est.StudentExamId
            INNER JOIN Tests t ON est.TestId = t.TestId
            INNER JOIN CourseTopics ct ON t.TopicId = ct.CourseTopicId 
            WHERE est.TestId=@testid AND se.ExamId IN (SELECT ExamId FROM StudentExams WHERE StudentExamId=@studentexamid)";
            await cnn.OpenAsync ();
            using SqlCommand cmd = new (sql, cnn);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            try
                {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync ())
                    {
                    int index = 0;
                    while (await reader.ReadAsync ())
                        {
                        lstStudentExamTests.Add (new StudentExamTest
                            {
                            StudentExamTestId = reader.GetInt64 (0),
                            StudentId = reader.GetInt32 (1),
                            StudentExamId = reader.GetInt32 (2),
                            TestId = reader.GetInt32 (3),
                            TestTitle = reader.GetString (4),
                            TestType = reader.GetInt32 (5),
                            CourseTopicId = reader.GetInt32 (6),
                            CourseTopicTitle = reader.GetString (7),
                            TestTags = reader.GetInt32 (8),
                            TestLevel = reader.GetInt32 (9),
                            Opt1Id = reader.GetInt32 (10),
                            Opt2Id = reader.GetInt32 (11),
                            Opt3Id = reader.GetInt32 (12),
                            Opt4Id = reader.GetInt32 (13),
                            Opt5Id = reader.GetInt32 (14),
                            StudentExamTestKey = reader.GetInt32 (15),
                            StudentExamTestAns = reader.GetInt32 (16),
                            StudentExamTestTags = reader.GetInt32 (17),
                            TestIndex = ++index,
                            TestOptions = new List<TestOption> ()
                            });
                        }
                    }
                return lstStudentExamTests;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return new List<StudentExamTest> ();
                }
            }
        public async Task<bool> Update_StudentExamTestAsync (StudentExamTest studentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "UPDATE StudentExamTests SET StudentId=@studentid, StudentExamId= @studentexamid, TestId=@testid, Opt1Id=@opt1id, Opt2Id= @opt2id, Opt3Id=@opt3id, Opt4Id=@opt4id, Opt5Id=@opt5id, StudentExamTestKey=@StudentExamTestkey, StudentExamTestAns=@StudentExamTestans, StudentExamTestTags=@StudentExamTesttags";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentid", studentExamTest.StudentId);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamTest.StudentExamId);
            cmd.Parameters.AddWithValue ("@testid", studentExamTest.TestId);
            cmd.Parameters.AddWithValue ("@opt1id", studentExamTest.Opt1Id);
            cmd.Parameters.AddWithValue ("@opt2id", studentExamTest.Opt2Id);
            cmd.Parameters.AddWithValue ("@opt3id", studentExamTest.Opt3Id);
            cmd.Parameters.AddWithValue ("@opt4id", studentExamTest.Opt4Id);
            cmd.Parameters.AddWithValue ("@opt5id", studentExamTest.Opt5Id);
            cmd.Parameters.AddWithValue ("@StudentExamTestkey", studentExamTest.StudentExamTestKey);
            cmd.Parameters.AddWithValue ("@StudentExamTestans", studentExamTest.StudentExamTestAns);
            cmd.Parameters.AddWithValue ("@StudentExamTesttags", studentExamTest.StudentExamTestTags);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Update_StudentExamTestsTagsAsync (StudentExamTest tempStudentExamTest)
            {
            //tags: 1:Visited 2:Bookmarked 4:Answered 8:Helped 16:Revised 32:Reported
            string sql = "UPDATE StudentExamTests SET StudentExamTestTags=@studentExamTestTags WHERE StudentExamId=@studentexamid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentExamTestTags", tempStudentExamTest.StudentExamTestTags);
                cmd.Parameters.AddWithValue ("@studentexamid", tempStudentExamTest.StudentExamId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTestTagsAsync (StudentExamTest tempStudentExamTest)
            {
            //tags: 1:Visited 2:Bookmarked 4:Answered 8:Helped 16:Revised 32:Reported
            string sql = "UPDATE StudentExamTests SET StudentExamTestTags=@studentExamTestTags WHERE StudentExamTestId=@StudentExamTestid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentExamTestTags", tempStudentExamTest.StudentExamTestTags);
                cmd.Parameters.AddWithValue ("@StudentExamTestid", tempStudentExamTest.StudentExamTestId);
                await cmd.ExecuteNonQueryAsync ();
                return true;
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                return false;
                }
            }
        public async Task<bool> Update_StudentExamTestAnswerAsync (StudentExamTest tempStudentExamTest)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "";
            try
                {
                if (tempStudentExamTest.StudentExamTestAns == 0)
                    {
                    sql = "UPDATE StudentExamTests SET StudentExamTestAns=@answ, StudentExamTestTags = (StudentExamTestTags & ~4) WHERE StudentExamTestId=@StudentExamTestid";
                    }
                else
                    {
                    sql = "UPDATE StudentExamTests SET StudentExamTestAns=@answ, StudentExamTestTags = (StudentExamTestTags | 4) WHERE StudentExamTestId=@StudentExamTestid";
                    }
                await cnn.OpenAsync ();
                var cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@answ", tempStudentExamTest.StudentExamTestAns);
                cmd.Parameters.AddWithValue ("@StudentExamTestid", tempStudentExamTest.StudentExamTestId);
                await cmd.ExecuteNonQueryAsync ();
                await cnn.CloseAsync ();
                return true;
                }
            catch (Exception ex)
                {
                await cnn.CloseAsync ();
                return false;
                }
            }
        public async Task<bool> Delete_StudentExamTestsAsync (int studentExamId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExamTests WHERE StudentExamId=@studentexamid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentexamid", studentExamId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<bool> Delete_StudentExamTestAsync (long studentExamTestId)
            {
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "DELETE FROM StudentExamTests WHERE StudentExamTestId=@StudentExamTestid";
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@StudentExamTestid", studentExamTestId);
            cmd.ExecuteNonQuery ();
            return true;
            }
        public async Task<string> CalculateStats_StudentExamTestsAsync (int examId, int testId)
            {
            string result = "";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            string sql = "SELECT COUNT (se.StudentExamId) AS cnt FROM StudentExams se INNER JOIN StudentExamTests est ON se.StudentExamId=est.StudentExamId WHERE (se.ExamId=@examid) AND (est.TestId=@testid) AND ((se.StudentExamTags & 1) = 1)";
            string sql1 = sql + " AND (est.StudentExamTestKey = est.StudentExamTestAns)"; //with correct answer
            string sql2 = sql + " AND (est.TestId=@testid) AND (est.StudentExamTestKey != est.StudentExamTestAns) AND (est.StudentExamTestAns != 0)"; //has-answer (not ignored)
            string sql3 = sql + " AND (est.TestId=@testid) AND ((est.StudentExamTestTags & 8) = 8)"; //8:Helped
            await cnn.OpenAsync ();
            try
                {
                //sql-n
                var cmd4 = new SqlCommand (sql, cnn);
                cmd4.Parameters.AddWithValue ("@examid", examId);
                cmd4.Parameters.AddWithValue ("@testid", testId);
                int n = (int) cmd4.ExecuteScalar ();
                if (n == 0)
                    {
                    result = "nStudents = 0";
                    return result;
                    }
                //sql1
                var cmd1 = new SqlCommand (sql1, cnn);
                cmd1.Parameters.AddWithValue ("@examid", examId);
                cmd1.Parameters.AddWithValue ("@testid", testId);
                int c = (int) cmd1.ExecuteScalar ();
                //sql2
                var cmd2 = new SqlCommand (sql2, cnn);
                cmd2.Parameters.AddWithValue ("@examid", examId);
                cmd2.Parameters.AddWithValue ("@testid", testId);
                int w = (int) cmd2.ExecuteScalar ();
                //sql3
                var cmd3 = new SqlCommand (sql3, cnn);
                cmd3.Parameters.AddWithValue ("@examid", examId);
                cmd3.Parameters.AddWithValue ("@testid", testId);
                int h = (int) cmd3.ExecuteScalar ();
                if (n != 0)
                    {
                    result = $"result: {h}! +{c} -{w} ?{(n - c - w)} ={n} -- Percent: {(100 * c / n).ToString ("F2")}%";
                    }
                else
                    {
                    result = "";
                    }
                //double p = (Math.Abs (2000 * ((1.0 * c) / (1.0 * c))) / 100);
                }
            catch (Exception ex)
                {
                Console.WriteLine (ex.ToString ());
                }
            return result;
            }
        #endregion
        #region M:Messages
        public async Task<int> Create_MessageAsync (Message message)
            {
            try
                {
                string? connString = _config.GetConnectionString ("cnni");
                using SqlConnection cnn = new (connString);
                string sql = "INSERT INTO Messages (UserId, DateTimeCreated, MessageTitle, MessageBody) VALUES (@userid, @datetimecreated, @messagetitle, @messagebody); SELECT CAST (scope_identity() AS int)";
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", message.UserId);
                cmd.Parameters.AddWithValue ("@datetimecreated", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
                cmd.Parameters.AddWithValue ("@messagetitle", message.MessageTitle);
                cmd.Parameters.AddWithValue ("@messagebody", message.MessageBody);
                int newMessageId = (int) await cmd.ExecuteScalarAsync ();
                await cnn.CloseAsync ();
                return newMessageId;
                }
            catch (Exception ex)
                {
                Console.WriteLine ($"be error (create message failed):\n{ex}");
                return 0;
                }
            }
        public async Task<Message> Read_MessageAsync (int messageId, bool getStudentMessages)
            {
            string sql = "SELECT MessageId, UserId, DateTimeCreated, MessageTitle, MessageBody FROM Messages WHERE MessageId=@messageid";
            Message message = new Message ();
            List<StudentMessage> lstStudentMessages = new List<StudentMessage> ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@messageid", messageId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    message.MessageId = reader.GetInt32 (0);
                    message.UserId = reader.GetInt32 (1);
                    message.DateTimeCreated = reader.GetString (2);
                    message.MessageTitle = reader.GetString (3);
                    message.MessageBody = reader.GetString (4);
                    message.Students = new List<StudentMessage> ();
                    }
                await cnn.CloseAsync ();
                if (getStudentMessages)
                    {
                    lstStudentMessages = await Read_StudentMessagesAsync (message.MessageId, "ByMessageId");
                    message.Students = lstStudentMessages;
                    }
                return message;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new Message ();
                }
            }
        public async Task<List<Message>> Read_MessagesAsync (int userId, bool getStudentMessages)
            {
            List<Message> lstMessages = new List<Message> ();
            string sql = "SELECT MessageId, UserId, DateTimeCreated, MessageTitle, MessageBody FROM Messages WHERE UserId=@userid ORDER BY DateTimeCreated";
            Message message = new Message ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@userid", userId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    lstMessages.Add (new Message
                        {
                        MessageId = reader.GetInt32 (0),
                        UserId = reader.GetInt32 (1),
                        DateTimeCreated = reader.GetString (2),
                        MessageTitle = reader.GetString (3),
                        MessageBody = reader.GetString (4),
                        Students = new List<StudentMessage> ()
                        });
                    }
                await cnn.CloseAsync ();
                if (getStudentMessages)
                    {
                    foreach (Message msg in lstMessages)
                        {
                        msg.Students = await Read_StudentMessagesAsync (msg.MessageId, "ByMessageId");
                        }
                    }
                return lstMessages;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new List<Message> ();
                }
            }
        public async Task<bool> Update_MessageAsync (Message message)
            {
            string sql = "UPDATE Messages SET DateTimeCreated=@datetimecreated, MessageTitle=@messagetitle, MessageBody=@messagebody WHERE MessageId=@messageid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@datetimecreated", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue ("@messagetitle", message.MessageTitle);
            cmd.Parameters.AddWithValue ("@messagebody", message.MessageBody);
            cmd.Parameters.AddWithValue ("@messageid", message.MessageId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_MessagesAsync (string mode, int recipientId)
            {
            string sql = "";
            switch (mode.ToLower ())
                {
                case "bymessageid":
                        {
                        //all instances sent from a message, be deleted. message is also deleted! 
                        sql = @"DELETE FROM StudentMessages WHERE MessageId=@recipientid; DELETE FROM Messages WHERE MessageId=@recipientid";
                        break;
                        }
                case "bystudentid":
                        {
                        //all instances of all messages sent to a student, be deleted. (messages are kept).
                        sql = @"DELETE FROM StudentMessages WHERE StudentId=@recipientid";
                        break;
                        }
                case "bystudentmessageId":
                        {
                        //a messages instance sent to a student, be deleted. (message is kept).
                        sql = @"DELETE FROM StudentMessages WHERE StudentMessageId=@recipientid";
                        break;
                        }
                default:
                        {
                        return false;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@recipientid", recipientId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region SM:StudentMessages
        public async Task<bool> Create_StudentMessagesAsync (int messageId, List<int> lstStudentIds, bool requestFeedback)
            {
            int _tags = requestFeedback ? 8 : 0;
            string sql = "INSERT INTO StudentMessages (StudentId, MessageId, DateTimeSent, StudentMessageTags) VALUES (@studentid, @messageid, @datetimesent, @studentmessagetags)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            foreach (int st in lstStudentIds)
                {
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentid", st);
                cmd.Parameters.AddWithValue ("@messageid", messageId);
                cmd.Parameters.AddWithValue ("@datetimesent", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
                cmd.Parameters.AddWithValue ("@studentmessagetags", _tags);
                int i = (int) cmd.ExecuteNonQuery ();
                }
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<List<StudentMessage>> Read_StudentMessagesAsync (int Id, string mode)
            {
            List<StudentMessage> lstStudentMessages = new List<StudentMessage> ();
            Message message = new Message ();
            string sql = @"SELECT sm.StudentMessageId, sm.MessageId, sm.StudentId, s.StudentName, s.StudentNickname, m.MessageTitle, m.MessageBody, m.DateTimeCreated, sm.DateTimeSent, sm.DateTimeRead, sm.StudentMessageTags
                        FROM StudentMessages sm 
                        INNER JOIN Messages m ON sm.MessageId = m.MessageId 
                        INNER JOIN Students s ON sm.StudentId = s.StudentId";
            switch (mode)
                {
                case "ByStudentIdIgnoreDeletedMessages":
                        {
                        //sort: put new messages at top, FeddbackMessages in mid and simple already-read messages at buttom, finally sorted by date-sent:
                        //sql += " WHERE (sm.StudentId=@id) AND ((sm.StudentMessageTags & 4) = 0) ORDER BY (sm.StudentMessageTags & 1), (sm.StudentMessageTags & 8) DESC, (sm.StudentMessageTags & 48), sm.DateTimeSent DESC"; //48=(16:AnsY+32:AnsN)
                        //sort: by date-sent:
                        sql += " WHERE (sm.StudentId=@id) AND ((sm.StudentMessageTags & 4) = 0) ORDER BY sm.DateTimeSent DESC"; //48=(16:AnsY+32:AnsN)
                        break;
                        }
                case "ByStudentId":
                        {
                        sql += " WHERE sm.StudentId=@id ORDER BY sm.DateTimeSent DESC";
                        break;
                        }
                case "ByMessageId":
                        {
                        sql += " WHERE sm.MessageId=@id ORDER BY m.DateTimeCreated, sm.DateTimeSent ";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@id", Id);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                lstStudentMessages.Clear ();
                while (await reader.ReadAsync ())
                    {
                    lstStudentMessages.Add (new StudentMessage
                        {
                        StudentMessageId = reader.GetInt32 (0),
                        MessageId = reader.GetInt32 (1),
                        StudentId = reader.GetInt32 (2),
                        StudentName = reader.GetString (3),
                        StudentNickname = reader.GetString (4),
                        MessageTitle = reader.GetString (5),
                        MessageBody = reader.GetString (6),
                        DateTimeCreated = reader.GetString (7),
                        DateTimeSent = reader.GetString (8),
                        DateTimeRead = reader.GetString (9),
                        StudentMessageTags = reader.GetInt32 (10)
                        });
                    }
                await cnn.CloseAsync ();
                return lstStudentMessages;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new List<StudentMessage> ();
                }
            }
        public async Task<Message> Read_StudentMessageAsync (int studentMessageId)
            {
            //by reading a studentMessage, its message (title, body) is also needed. so, a message (containing a studentMessage) is retured.
            string sql = "SELECT StudentMessageId, StudentId, MessageId, DateTimeSent, DateTimeRead, StudentMessageTags FROM StudentMessages WHERE StudentMessageId=@studentmessageid";
            StudentMessage studentMessage = new StudentMessage ();
            Message message = new Message ();
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            try
                {
                await cnn.OpenAsync ();
                SqlCommand cmd = new SqlCommand (sql, cnn);
                cmd.Parameters.AddWithValue ("@studentmessageid", studentMessageId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync ();
                while (await reader.ReadAsync ())
                    {
                    studentMessage.StudentMessageId = reader.GetInt32 (0);
                    studentMessage.StudentId = reader.GetInt32 (1);
                    studentMessage.MessageId = reader.GetInt32 (2);
                    studentMessage.DateTimeSent = reader.GetString (3);
                    studentMessage.DateTimeRead = reader.GetString (4);
                    studentMessage.StudentMessageTags = reader.GetInt32 (5);
                    }
                await cnn.CloseAsync ();
                message = await Read_MessageAsync (studentMessage.MessageId, false);
                return message;
                }
            catch (Exception ex)
                {
                Console.WriteLine ("Error: " + ex.ToString ());
                await cnn.CloseAsync ();
                return new Message ();
                }
            }
        public async Task<bool> Update_StudentMessageTagsAsync (StudentMessage studentMessage)
            {
            string sql = "UPDATE StudentMessages SET StudentMessageTags=@studentmessagetags WHERE StudentMessageId=@studentmessageid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentmessagetags", studentMessage.StudentMessageTags);
            cmd.Parameters.AddWithValue ("@studentmessageid", studentMessage.StudentMessageId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Update_StudentMessageSetAsReadAsync (StudentMessage studentMessage)
            {
            string sql = "UPDATE StudentMessages SET StudentMessageTags=(StudentMessagetags | 1), DateTimeRead=@datetimeread WHERE StudentMessageId=@studentmessageid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@datetimeread", DateTime.Now.ToString ("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue ("@studentmessageid", studentMessage.StudentMessageId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_StudentMessageAsync (int studentMessageId)
            {
            string sql = @"DELETE FROM StudentMessages WHERE StudentMessageId=@studentmessageid)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@studentmessageid", studentMessageId);
            int i = cmd.ExecuteNonQuery ();
            await cnn.CloseAsync ();
            return (i > 0) ? true : false;
            }
        #endregion
        #region CH:Chat
        public async Task<int> Create_ChatAsync (Chat chat)
            {
            string sql = "INSERT INTO Chats (FromId, ToId, DateTimeSent, ChatText, ChatTags) VALUES (@fromid, @toid, @datetimesent, @chattext, @chattags); SELECT CAST (scope_identity() AS int)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@fromid", chat.FromId);
            cmd.Parameters.AddWithValue ("@toid", chat.ToId);
            cmd.Parameters.AddWithValue ("@datetimesent", chat.DateTimeSent);
            cmd.Parameters.AddWithValue ("@chattext", chat.ChatText);
            cmd.Parameters.AddWithValue ("@chattags", chat.ChatTags);
            int i = (int) await cmd.ExecuteScalarAsync ();
            await cnn.CloseAsync ();
            return i;
            }
        public async Task<List<Chat>> Read_ChatsAsync (int studentId)
            {
            //1 get netList of chatMates
            List<int> lstMateIds = new List<int> ();
            string sql = @"SELECT DISTINCT ch.FromId mateId
                        FROM Chats ch INNER JOIN Students sf ON ch.FromId = sf.StudentId
                        WHERE ch.ToId=@meId
                        UNION
                        SELECT DISTINCT ch.ToId mateId
                        FROM Chats ch INNER JOIN Students st ON ch.ToId = st.StudentId
                        WHERE ch.FromId = @meId";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd1 = new SqlCommand (sql, cnn);
            cmd1.Parameters.AddWithValue ("@meid", studentId);
            SqlDataReader reader1 = await cmd1.ExecuteReaderAsync ();
            lstMateIds.Clear ();
            while (await reader1.ReadAsync ())
                {
                lstMateIds.Add (reader1.GetInt32 (0));
                }
            await reader1.CloseAsync ();
            //2 get display data
            List<Chat> lstChats = new List<Chat> ();
            lstChats.Clear ();
            foreach (int mateId in lstMateIds)
                {
                sql = @$"SELECT TOP 1 ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, Left(ch.ChatText, 10), ch.ChatTags, sf.StudentNickname, st.StudentNickname
                    FROM Chats ch 
                    INNER JOIN Students sf ON ch.FromId = sf.StudentId
                    INNER JOIN Students st ON ch.ToId = st.StudentId
                    WHERE (FromId={mateId} AND ToId={studentId}) OR (FromId={studentId} AND ToId={mateId})
                    ORDER BY (ch.ChatTags & 1) ASC, ch.DateTimeSent DESC";
                SqlCommand cmd2 = new SqlCommand (sql, cnn);
                SqlDataReader reader2 = await cmd2.ExecuteReaderAsync ();
                while (await reader2.ReadAsync ())
                    {
                    lstChats.Add (new Chat
                        {
                        ChatId = reader2.GetInt32 (0),
                        FromId = reader2.GetInt32 (1),
                        ToId = reader2.GetInt32 (2),
                        DateTimeSent = reader2.GetString (3),
                        ChatText = reader2.GetString (4),
                        ChatTags = reader2.GetInt32 (5),
                        FromName = reader2.GetString (6),
                        ToName = reader2.GetString (7)
                        });
                    }
                await reader2.CloseAsync ();
                }
            await cnn.CloseAsync ();
            lstChats = lstChats
                .OrderBy (chat => (chat.ChatTags & 1))   // 0 comes before 1  → bit=0 first
                .ThenByDescending (chat => chat.DateTimeSent)
                .ToList ();
            return lstChats;
            }
        public async Task<List<Chat>> Read_ChatsWithOneMateAsync (int studentId, int mateId)
            {
            List<Chat> lstChats = new List<Chat> ();
            string sql = @"SELECT ch.ChatId, ch.FromId, ch.ToId, ch.DateTimeSent, ch.ChatText, ch.ChatTags, sf.StudentNickname, st.StudentNickname
                        FROM Chats ch 
                        INNER JOIN Students sf ON ch.FromId = sf.StudentId
                        INNER JOIN Students st ON ch.ToId = st.StudentId
                        WHERE ((ch.FromId=@meid) AND (ch.ToId=@mateid)) OR ((ch.FromId=@mateid) AND (ch.ToId=@meid)) 
                        ORDER BY DateTimeSent DESC
                        OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY ";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@meid", studentId);
            cmd.Parameters.AddWithValue ("@mateid", mateId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            lstChats.Clear ();
            while (await reader.ReadAsync ())
                {
                lstChats.Add (new Chat
                    {
                    ChatId = reader.GetInt32 (0),
                    FromId = reader.GetInt32 (1),
                    ToId = reader.GetInt32 (2),
                    DateTimeSent = reader.GetString (3),
                    ChatText = reader.GetString (4),
                    ChatTags = reader.GetInt32 (5),
                    FromName = reader.GetString (6),
                    ToName = reader.GetString (7)
                    });
                }
            await cnn.CloseAsync ();
            return lstChats;
            }
        public async Task<bool> Update_ChatAsync (Chat chat)
            {
            string sql = "UPDATE Chats SET FromId=@fromid, ToId=@toid, DateTimeSent=@datetimesent, ChatText=@chattext, ChatTags=@chattags WHERE ChatId=@chatid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@fromid", chat.FromId);
            cmd.Parameters.AddWithValue ("@toid", chat.ToId);
            cmd.Parameters.AddWithValue ("@datetimesent", chat.DateTimeSent);
            cmd.Parameters.AddWithValue ("@chattext", chat.ChatText);
            cmd.Parameters.AddWithValue ("@chattags", chat.ChatTags);
            cmd.Parameters.AddWithValue ("@chatid", chat.ChatId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Update_ChatTagsAsync (Chat chat)
            {
            //1:IsRead 2:IsImp 3:IsBookmarked 4:Deleted
            string sql = "UPDATE Chats SET ChatTags=@chattags WHERE ChatId=@chatid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@chattags", chat.ChatTags);
            cmd.Parameters.AddWithValue ("@chatid", chat.ChatId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_ChatAsync (int chatId)
            {
            string sql = "DELETE FROM Chats WHERE ChatId=@chatid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@chatid", chatId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        #endregion
        #region P:Projects
        public async Task<int> Create_ProjectAsync (Project project)
            {
            string sql = "INSERT INTO Projects (UserId, ProjectName, ProjectTags ) VALUES (@userid, @projectname, @projecttags)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            //Create
            await cnn.OpenAsync ();
            SqlCommand cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@userid", project.UserId);
            cmd2.Parameters.AddWithValue ("@projectname", project.ProjectName);
            cmd2.Parameters.AddWithValue ("@active", project.ProjectTags);
            await cmd2.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return 1;
            }
        public async Task<List<Project>> Read_ProjectsAsync (int userId, string mode)
            {
            List<Project> lstProjects = new List<Project> ();
            string sql = @"SELECT ProjectId, UserId, ProjectName, ProjectTags FROM Projects ";
            switch (mode)
                {
                case "all":
                        {
                        sql += " WHERE UserId=@userid ORDER BY ProjectName";
                        break;
                        }
                case "active":
                        {
                        sql += " WHERE UserId=@userid AND (ProjectTags & 1)=1 ORDER BY ProjectName";
                        break;
                        }
                case "inactive":
                        {
                        sql += " WHERE UserId=@userid AND (ProjectTags & 1)=0 ORDER BY ProjectName";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@userid", userId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            lstProjects.Clear ();
            while (await reader.ReadAsync ())
                {
                lstProjects.Add (new Project
                    {
                    ProjectId = reader.GetInt32 (0),
                    UserId = reader.GetInt32 (1),
                    ProjectName = reader.GetString (2),
                    ProjectTags = reader.GetInt32 (3),
                    Subprojects = new List<Subproject> ()
                    });
                }
            await cnn.CloseAsync ();
            foreach (Project prj in lstProjects)
                {
                prj.Subprojects = await Read_SubprojectsAsync (prj.ProjectId, false);
                }
            return lstProjects;
            }
        public async Task<Project> Read_ProjectAsync (int projectId)
            {
            string sql = "SELECT ProjectId, UserId, ProjectName, ProjectTags FROM Projects WHERE ProjectId=@projectid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@projectid", projectId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            Project project = new Project ();
            while (await reader.ReadAsync ())
                {
                project.ProjectId = reader.GetInt32 (0);
                project.UserId = reader.GetInt32 (1);
                project.ProjectName = reader.GetString (2);
                project.ProjectTags = reader.GetInt32 (3);
                project.Subprojects = new List<Subproject> ();
                }
            await cnn.CloseAsync ();
            project.Subprojects = await Read_SubprojectsAsync (project.ProjectId, false);
            return project;
            }
        public async Task<bool> Update_ProjectAsync (Project project)
            {
            string sql = "UPDATE Projects SET ProjectName=@projectname, ProjectTags=@projecttags WHERE ProjectId=@projectid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@projectname", project.ProjectName);
            cmd.Parameters.AddWithValue ("@projecttags", project.ProjectTags);
            cmd.Parameters.AddWithValue ("@projectid", project.ProjectId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        #endregion
        #region SP:Subprojects
        public async Task<int> Create_SubprojectAsync (Subproject subProject)
            {
            string sql = "INSERT INTO SubProjects (ProjectId, SubProjectName, SubProjectTags) VALUES (@projectid, @subprojectname, 1)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            //Create
            await cnn.OpenAsync ();
            SqlCommand cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@projectid", subProject.ProjectId);
            cmd2.Parameters.AddWithValue ("@subprojectname", subProject.SubprojectName);
            await cmd2.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return 1;
            }
        public async Task<List<Subproject>> Read_SubprojectsAsync (int projectId, bool readNotes)
            {
            List<Subproject> lstSubprojects = new List<Subproject> ();
            string sql = "SELECT SubprojectId, ProjectId, SubProjectName, SubprojectTags FROM SubProjects WHERE ProjectId=@projectid ORDER BY SubProjectName";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@projectid", projectId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            lstSubprojects.Clear ();
            while (await reader.ReadAsync ())
                {
                lstSubprojects.Add (new Subproject
                    {
                    SubprojectId = reader.GetInt32 (0),
                    ProjectId = reader.GetInt32 (1),
                    SubprojectName = reader.GetString (2),
                    Notes = new List<Note> ()
                    });
                }
            await cnn.CloseAsync ();
            if (readNotes)
                {
                foreach (Subproject subprj in lstSubprojects)
                    {
                    subprj.Notes = await Read_NotesAsync (subprj.SubprojectId, 2); //2:read SP notes (parentTypes> 1:U 2:SP 3:S 4:G 5:C 6:E)
                    }
                }
            return lstSubprojects;
            }
        public async Task<Subproject> Read_SubprojectAsync (int subProjectId, bool readNotes)
            {
            string sql = "SELECT SubprojectId, ProjectId, SubProjectName FROM SubProjects WHERE SubprojectId=@subprojectid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@subprojectid", subProjectId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            Subproject subProject = new Subproject ();
            while (await reader.ReadAsync ())
                {
                subProject.SubprojectId = reader.GetInt32 (0);
                subProject.ProjectId = reader.GetInt32 (1);
                subProject.SubprojectName = reader.GetString (2);
                subProject.Notes = new List<Note> ();
                }
            await cnn.CloseAsync ();
            if (readNotes)
                {
                subProject.Notes = await Read_NotesAsync (subProject.SubprojectId, 1); //1:read sp notes --parentTypes 1:subprojects 2:students 3:groups 4:courses 5:exams
                }
            return subProject;
            }
        public async Task<bool> Update_SubprojectAsync (Subproject subProject)
            {
            string sql = "UPDATE subprojects SET SubprojectName=@subprojectname, SubprojectTags=@subprojecttags WHERE SubprojectId=@subprojectid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@subprojectname", subProject.SubprojectName);
            cmd.Parameters.AddWithValue ("@subprojecttags", subProject.SubprojectTags);
            cmd.Parameters.AddWithValue ("@subprojectid", subProject.SubprojectId);
            await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_SubprojectAsync (int subProjectId)
            {
            string sql = "DELETE FROM Notes WHERE ParentId=@subprojectid; DELETE FROM SubProjects WHERE SubprojectId=@subprojectid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@subprojectid", subProjectId);
            int i = await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        #endregion
        #region N:Notes
        public async Task<int> Create_NoteAsync (Note note)
            {
            string sql = "INSERT INTO Notes (ParentId, ParentType, NoteDatum, NoteText, NoteTags) VALUES (@parentid, @parenttype, @notedatum, @notetext, @notetags)";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@parentid", note.ParentId);
            cmd2.Parameters.AddWithValue ("@parenttype", note.ParentType);
            cmd2.Parameters.AddWithValue ("@notedatum", note.NoteDatum);
            cmd2.Parameters.AddWithValue ("@notetext", note.NoteText);
            cmd2.Parameters.AddWithValue ("@notetags", note.NoteTags); //1:rtl 2:done 4:shared 8:readonly
            await cmd2.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return 1;
            }
        public async Task<List<Note>> Read_NotesAsync (int parentId, int parentType)
            {
            //parentTypes 1:user(U) 2:subprojects(SP) 3:students(S) 4:groups(G) 5:courses(C) 6:exams(E) 7:studentnotes(SN)
            string sql = "";
            List<Note> lstNotes = new List<Note> ();
            switch (parentType)
                {
                case 1:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, u.UsrName FROM Notes n INNER JOIN Usrs u ON n.ParentId = u.UsrId WHERE n.ParentId=@parentid ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 2:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, sp.SubprojectName FROM Notes n INNER JOIN Subprojects sp ON n.ParentId = sp.SubprojectId WHERE n.ParentId=@parentid ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 3:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, s.StudentName FROM Notes n INNER JOIN Students s ON n.ParentId = s.StudentId WHERE (n.ParentId=@parentid) AND (NoteTags & 16 = 0) ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 4:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, g.GroupName FROM Notes n INNER JOIN Groups g ON n.ParentId = g.GroupId WHERE n.ParentId=@parentid ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 5:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, c.CourseName FROM Notes n INNER JOIN Courses c ON n.ParentId = c.CourseId WHERE n.ParentId=@parentid ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 6:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, e.ExamTitle FROM Notes n INNER JOIN Exams e ON n.ParentId = e.ExamId WHERE n.ParentId=@parentid ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                case 7:
                        {
                        sql = "SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags, s.StudentName FROM Notes n INNER JOIN Students s ON n.ParentId = s.StudentId WHERE (n.ParentId=@parentid) AND (NoteTags & 16 = 16) ORDER BY NoteDatum DESC, NoteId DESC ";
                        break;
                        }
                }
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@parentid", parentId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            lstNotes.Clear ();
            while (await reader.ReadAsync ())
                {
                Note note = new Note ();
                note.NoteId = reader.GetInt32 (0);
                note.ParentId = reader.GetInt32 (1);
                note.ParentType = reader.GetInt32 (2);
                note.NoteDatum = reader.GetString (3);
                note.NoteText = reader.GetString (4);
                note.NoteTags = reader.GetInt32 (5);
                note.ParentName = reader.GetString (6);
                lstNotes.Add (note);
                }
            await cnn.CloseAsync ();
            return lstNotes;
            }
        public async Task<List<Note>> Read_NotesBySearchKeyAsync (string searchKey, int parentId, string mode)
            {
            searchKey = "%" + searchKey + "%";
            List<Note> lstNotes = new List<Note> ();
            string sql = @"SELECT n.NoteId, n.ParentId, n.ParentType, n.NoteDatum, n.NoteText, n.NoteTags FROM Notes n";
            switch (mode)
                {
                case "U":
                        {
                        sql += " INNER JOIN usrs u ON n.ParentId = u.UsrId WHERE u.UserId=@id ";
                        break;
                        }
                case "SP":
                        {
                        sql += " INNER JOIN Subprojects sp ON n.ParentId = sp.SubprojectId WHERE sp.ProjectId IN (SELECT ProjectId FROM Projects WHERE UserId=@id) ";
                        break;
                        }
                case "S":
                        {
                        sql += " INNER JOIN Students s ON n.ParentId = s.StudentId WHERE (s.StudentId=@id) AND (NoteTags & 16 = 0)";
                        break;
                        }
                case "G":
                        {
                        sql += " INNER JOIN Groups g ON n.ParentId = g.GroupId WHERE g.GroupId=@id ";
                        break;
                        }
                case "C":
                        {
                        sql += " INNER JOIN Courses c ON n.ParentId = c.CourseId WHERE c.CourseId=@id ";
                        break;
                        }
                case "E":
                        {
                        sql += " INNER JOIN Exams e ON n.ParentId = e.ExamId WHERE e.ExamId=@id ";
                        break;
                        }
                case "SN":
                        {
                        sql += " INNER JOIN Students s ON n.ParentId = s.StudentId WHERE (s.StudentId=@id) AND (NoteTags & 16 = 16)";
                        break;
                        }
                }
            sql += @" AND NoteText LIKE @key
                    ORDER BY NoteDatum DESC 
                    OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY ";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@key", searchKey);
            cmd.Parameters.AddWithValue ("@id", parentId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            lstNotes.Clear ();
            while (await reader.ReadAsync ())
                {
                Note note = new Note ();
                note.NoteId = reader.GetInt32 (0);
                note.ParentId = reader.GetInt32 (1);
                note.ParentType = reader.GetInt32 (2);
                note.NoteDatum = reader.GetString (3);
                note.NoteText = reader.GetString (4);
                note.NoteTags = reader.GetInt32 (5);
                lstNotes.Add (note);
                }
            await cnn.CloseAsync ();
            return lstNotes;
            }
        public async Task<Note> Read_NoteAsync (int noteId)
            {
            string sql = "SELECT ID, NoteDatum, Note, Parent_ID, ParentType, Rtl, Done, User_ID, Shared, ReadOnly FROM Notes WHERE ID=@noteid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@noteid", noteId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync ();
            Note note = new Note ();
            while (await reader.ReadAsync ())
                {
                note.NoteId = reader.GetInt32 (0);
                note.NoteDatum = reader.GetString (1);
                note.NoteText = reader.GetString (2);
                note.ParentId = reader.GetInt32 (3);
                note.ParentType = reader.GetInt32 (4);
                note.NoteTags = reader.GetInt32 (5);
                }
            await cnn.CloseAsync ();
            return note;
            }
        public async Task<bool> Update_NoteAsync (Note note)
            {
            string sql = "UPDATE Notes SET ParentId=@parentid, ParentType=@parenttype, NoteDatum=@notedatum, NoteText=@notetext, NoteTags=@notetags WHERE NoteId=@noteid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@parentid", note.ParentId);
            cmd2.Parameters.AddWithValue ("@parenttype", note.ParentType);
            cmd2.Parameters.AddWithValue ("@notedatum", note.NoteDatum);
            cmd2.Parameters.AddWithValue ("@notetext", note.NoteText);
            cmd2.Parameters.AddWithValue ("@notetags", note.NoteTags); //1:rtl 2:done 4:shared 8:readonly
            cmd2.Parameters.AddWithValue ("@noteid", note.NoteId);
            await cmd2.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Update_NoteParentAsync (Note note)
            {
            string sql = "UPDATE Notes SET ParentId=@parentid, ParentType=@parenttype WHERE NoteId=@noteid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd2 = new SqlCommand (sql, cnn);
            cmd2.Parameters.AddWithValue ("@parentid", note.ParentId);
            cmd2.Parameters.AddWithValue ("@parenttype", note.ParentType);
            cmd2.Parameters.AddWithValue ("@noteid", note.NoteId);
            await cmd2.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_NotesAsync (int parentId, int parentType)
            {
            //parentTypes 1:user(U) 2:subprojects(SP) 3:students(S) 4:groups(G) 5:courses(C) 6:exams(E) 7:studentnotes(SN)
            string sql = "DELETE FROM Notes WHERE Parent_ID=@parentid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@parentid", parentId);
            int i = await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        public async Task<bool> Delete_NoteAsync (int noteId)
            {
            string sql = "DELETE FROM Notes WHERE NoteId=@noteid";
            string? connString = _config.GetConnectionString ("cnni");
            using SqlConnection cnn = new SqlConnection (connString);
            await cnn.OpenAsync ();
            SqlCommand cmd = new SqlCommand (sql, cnn);
            cmd.Parameters.AddWithValue ("@noteid", noteId);
            int i = await cmd.ExecuteNonQueryAsync ();
            await cnn.CloseAsync ();
            return true;
            }
        #endregion
        }
    }
