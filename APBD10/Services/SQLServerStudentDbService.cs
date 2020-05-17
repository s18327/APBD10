using APBD10.DTO;
using APBD10.Entities;
using System;
using System.Data.SqlClient;

namespace APBD10.Services
{
    public class SQLServerStudentDbService : IStudentServiceDb
    {
        public EnrollmentResponse EnrollStudent(EnrollmentRequest request)
        {
            using (var connection = new SqlConnection(@"Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    var tran = connection.BeginTransaction();
                    command.Transaction = tran;

                    // Check if studies exists
                    command.CommandText = "select * FROM Studies WHERE Name=@Name";
                    command.Parameters.AddWithValue("Name", request.Studies);

                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        return null;
                    }
                    int idStudies = (int)reader["IdStudy"];

                    reader.Close();

                    command.CommandText = "select * FROM Enrollment WHERE Semester=1 AND IdStudy=@IdStudy";
                    command.Parameters.AddWithValue("IdStudy", idStudies);

                    var idEnrollment = 0;
                    var reader2 = command.ExecuteReader();
                    if (!reader2.Read())
                    {
                        command.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment = (SELECT MAX(IdEnrollment) FROM Enrollment)";
                        reader2.Close();

                        var reader3 = command.ExecuteReader();
                        reader3.Read();
                        idEnrollment = reader3.GetInt32(0);
                        reader3.Close();
                        command.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES(@IdEnrollment, @Semester, @ids, @StartDate);";
                        command.Parameters.AddWithValue("IdEnrollment", idEnrollment + 1);
                        command.Parameters.AddWithValue("Semester", 1);
                        command.Parameters.AddWithValue("ids", idStudies);
                        command.Parameters.AddWithValue("StartDate", DateTime.Now.ToString());

                        var reader4 = command.ExecuteReader();
                        reader4.Close();
                    }
                    else
                    {
                        idEnrollment = (int)reader2["IdEnrollment"];
                        reader2.Close();
                    }

                    // check if student with index number exists
                    command.CommandText = "select * FROM Student WHERE IndexNumber=@IndexNumber";
                    command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                    var reader5 = command.ExecuteReader();
                    if (!reader5.Read())
                    {
                        reader5.Close();
                        // create new student
                        command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, Birthdate, IdEnrollment) VALUES (@IndexNumber2, @FirstName2, @LastName2, @Birthdate2, @IdEnrollment2)";
                        command.Parameters.AddWithValue("IndexNumber2", request.IndexNumber);
                        command.Parameters.AddWithValue("FirstName2", request.FirstName);
                        command.Parameters.AddWithValue("LastName2", request.LastName);
                        command.Parameters.AddWithValue("Birthdate2", request.DateOfBirth.ToString());
                        command.Parameters.AddWithValue("IdEnrollment2", idEnrollment);

                        var reader6 = command.ExecuteReader();
                        reader6.Close();
                    }
                    else
                    {
                        reader5.Close();
                    }

                    tran.Commit();
                }
            }

            var response = new EnrollmentResponse
            {
                LastName = request.LastName,
                Semester = 1
            };

            return response;
        }

        public Student GetStudent(string indexNumber)
        {
            using (var connection = new SqlConnection(@"Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    var tran = connection.BeginTransaction();
                    command.Transaction = tran;

                    // Check if studies exists
                    command.CommandText = @"select *
                                            from Student s
                                            WHERE s.IndexNumber = @indexNumber;";
                    command.Parameters.AddWithValue("indexNumber", indexNumber);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var st = new Student
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                        };

                        return st;
                    }

                    reader.Close();

                    tran.Commit();
                }
            }

            return null;
        }

        public PromoteResponse PromoteStudent(int semester, string studies)
        {
            using (var client = new SqlConnection(@"Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var command = new SqlCommand())

            {
                client.Open();
                command.Connection = client;

                command.CommandText = "SELECT * FROM Enrollment, Studies WHERE Enrollment.IdStudy=Studies.IdStudy AND Enrollment.semester=@semester AND Studies.Name=@Studies";
                command.Parameters.AddWithValue("semester", semester);
                command.Parameters.AddWithValue("Studies", studies);

                var reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    return null;
                }

                reader.Close();

                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.CommandText = "Promotion";
                command.ExecuteNonQuery();

                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = "select * from Enrollment,Studies where Enrollment.IdStudy=Studies.IdStudy and Name=@Studies and Semester=@semestern";

                command.Parameters.AddWithValue("semestern", semester + 1);

                var dr2 = command.ExecuteReader();

                dr2.Read();

                var enrollment = new Enrollment();

                enrollment.IdStudy = (int)dr2["IdStudy"];
                enrollment.Semester = (int)dr2["Semester"];
                var StartDate = dr2["StartDate"];
                enrollment.StartDate = DateTime.Now;

                var promotion = new PromoteResponse(enrollment);

                return promotion;
            }
        }

        public void SaveLogData(string data)
        {
            throw new NotImplementedException();
        }

        Student IStudentServiceDb.GetStudent(string indexNumber)
        {
            throw new NotImplementedException();
        }
    }
}