using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TaskList.Models;

namespace TaskList.Global
{
    /// <summary>
    /// Осуществляет работу с данными
    /// </summary>
    public class Data
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["ConnectionToDatabase"].ConnectionString;

        /// <summary>
        /// Выполняет запрос на поиск всех  дат пользователя для заполнения меню
        /// </summary>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        public IEnumerable<DateTime> GetAllDates(string user)
        {
            var listDate = new List<DateTime>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
                        select 
                              distinct  deadline                         
                            from 
                                Tasks
                            where 
                                (
                                user_login =@login
                                and deleted  = 0
                                and deadline is not null
                                and time_when_task_completed is null
                                )
                                or
                                (
                                    deadline is not null
                                    and deadline in
                                     (
						            select deadline                        
                                    from 
                                    Tasks
                                    where 
                                    user_login = @login
                                    and deleted  = 0                                 
                                    and time_when_task_completed is not null
					                order by   time_when_task_completed desc
						            offset 0 rows
                                    fetch next 5 rows only
                                       )
                                 )
                            ";
                    cmd.Parameters.AddWithValue("@login", user);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listDate.Add(reader.GetDateTime(reader.GetOrdinal("deadline")));

                        }
                    }
                }
            }
            return listDate;
        }

        /// <summary>
        /// Выполняет запрос на поиск всех  дат пользователя для заполнения меню
        /// </summary>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        public IEnumerable<string> GetAllMarks(string user)
        {
            var listMarks = new List<string>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.CommandText = @"
                         select 
                          distinct name_mark                         
                        from 
                            Tasks,Tasks_marks
                        where 
                            Tasks.id_task=Tasks_marks.id_task
                            and user_login = @login
                            and deleted  = 0
                    ";
                    cmd.Parameters.AddWithValue("@login", user);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listMarks.Add(reader.GetString(reader.GetOrdinal("name_mark")));
                        }
                    }
                }
            }
            var count = listMarks.Count;
            return listMarks;
        }
        
        /// <summary>
        /// Выполняет запрос на показ всех задач пользователя    
        /// </summary>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        /// <returns></returns>
        public IEnumerable<Task> ShowAllTasks(string user)
        {
            var listTasks = new List<Task>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
                         select 
                            id_task
                            ,name_task
                            ,time_when_task_completed
                         from 
                            Tasks
                         where 
                            (
                            user_login =@login
                            and deleted  = 0
                            and time_when_task_completed is null
                            )
                            or
                            ( 
                                Tasks.id_task in
                                (
                                select 
                                    id_task
							    from
							        Tasks
                                where
                                    user_login = @login
                                    and deleted  = 0
                                    and time_when_task_completed is not null
					            order by   time_when_task_completed desc
						            offset 0 rows
                                    fetch next 5 rows only
                                )
                            )
						 ";
                
                    cmd.Parameters.AddWithValue("@login", user);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Task();
                            task.Id = reader.GetInt32(reader.GetOrdinal("id_task"));
                            task.Name = reader.GetString(reader.GetOrdinal("name_task"));
                            if (reader["time_when_task_completed"] == DBNull.Value)
                                task.TimeWhenTaskCompleted = null;
                            else
                                task.TimeWhenTaskCompleted =
                                    reader.GetDateTime(reader.GetOrdinal("time_when_task_completed"));

                            listTasks.Add(task);
                        };
                    }
                }
            }
            IEnumerable<Task> result = listTasks.OrderBy(x => x.Name);
            return result;
        }

        /// <summary>
        /// Осуществляет поиск всех задач пользователя без даты
        /// </summary>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        /// <returns></returns>
        public IEnumerable<Task> SearchTasksByNullDate(string user)
        {
            var listTasks = new List<Task>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.CommandText = @"                      
						select 
                            id_task
                            ,name_task
                            ,time_when_task_completed
                        from 
                            Tasks
                        where 
                            (
                            user_login = @login
                            and deleted  = 0
                            and deadline is null
							and time_when_task_completed is null
                            )
						    or
                            (       
                                deadline is null
							    and id_task in
                                (
							    select 
                                    id_task
							    from
							        Tasks
							    where user_login = @login
                                    and deleted  = 0
							    order by   time_when_task_completed desc
						            offset 0 rows
                                    fetch next 5 rows only
                                 )
                             )
                    ";
                    cmd.Parameters.AddWithValue("@login", user);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Task();
                            task.Id = reader.GetInt32(reader.GetOrdinal("id_task"));
                            task.Name = reader.GetString(reader.GetOrdinal("name_task"));
                            if (reader["time_when_task_completed"] == DBNull.Value)
                                task.TimeWhenTaskCompleted = null;
                            else
                                task.TimeWhenTaskCompleted =
                                    reader.GetDateTime(reader.GetOrdinal("time_when_task_completed"));
                            listTasks.Add(task);
                        }
                    }
                }
            }
            IEnumerable<Task> result = listTasks.OrderBy(x => x.Name);
            return result;
        }

        /// <summary>
        /// Осуществляет поиск всех задач пользователя по дате
        /// </summary>
        /// <param name="dateStr">Дата по которой искать задачи</param>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        /// <returns></returns>
        public IEnumerable<Task> SearchTasksByDate(string dateStr, string user)
        {
            DateTime date;
            if (!DateTime.TryParse(dateStr, out date))
            {
                return null;
            };

            var listTasks = new List<Task>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
                    select 
                            id_task
                            ,name_task
                            ,time_when_task_completed
                        from 
                            Tasks
                        where 
                            (
                            user_login = @login
                            and deleted  = 0
                            and deadline=@date
							and time_when_task_completed is null
                            )
                            or
                            (
							 deadline=@date
							 and id_task in(
							 select 
                                id_task
							from
							    Tasks
							where 
                                user_login = @login
                                and deleted  = 0
							order by   time_when_task_completed desc
						         offset 0 rows
                                 fetch next 5 rows only)
                             )
                                ";
                             
                    cmd.Parameters.AddWithValue("@login", user);
                    cmd.Parameters.AddWithValue("@date", date);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Task();
                            task.Id = reader.GetInt32(reader.GetOrdinal("id_task"));
                            task.Name = reader.GetString(reader.GetOrdinal("name_task"));
                            if (reader["time_when_task_completed"] == DBNull.Value)
                                task.TimeWhenTaskCompleted = null;
                            else
                                task.TimeWhenTaskCompleted =
                                    reader.GetDateTime(reader.GetOrdinal("time_when_task_completed"));

                            listTasks.Add(task);
                        }
                    }
                }
            }
            if (listTasks.Count == 0)
            {
                return null;
            }
            IEnumerable<Task> result = listTasks.OrderBy(x => x.Name);
            return result;
        }

        /// <summary>
        /// Осуществляет поиск всех задач пользователя по метке
        /// </summary>
        /// <param name="mark">Имя метки</param>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        /// <returns></returns>
        public IEnumerable<Task> SearchTasksByMarks(string mark, string user)
        {
            var listTasks = new List<Task>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.CommandText = @"
                         select 
                            Tasks.id_task
                            ,name_task
                            ,time_when_task_completed
                        from 
                            Tasks
                            ,Tasks_marks
                        where 
                            (
                            Tasks.id_task=Tasks_marks.id_task
                            and user_login = @login
                            and deleted  = 0
                            and name_mark=@mark
							and time_when_task_completed is null
                            )
                            or
                            (
							     Tasks.id_task=Tasks_marks.id_task
							     and name_mark=@mark
							     and Tasks.id_task in
                                (
							    select 
                                    id_task
							    from
							        Tasks
							    where 
                                    user_login =@login
                                    and deleted  = 0
							    order by   time_when_task_completed desc
						        offset 0 rows
                                fetch next 5 rows only
                                )
                            )
                    ";
                    cmd.Parameters.AddWithValue("@login", user);
                    cmd.Parameters.AddWithValue("@mark", mark);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Task();
                            task.Id = reader.GetInt32(reader.GetOrdinal("id_task"));
                            task.Name = reader.GetString(reader.GetOrdinal("name_task"));
                            if (reader["time_when_task_completed"] == DBNull.Value)
                                task.TimeWhenTaskCompleted = null;
                            else
                                task.TimeWhenTaskCompleted =
                                    reader.GetDateTime(reader.GetOrdinal("time_when_task_completed"));
                            listTasks.Add(task);
                        }
                    }
                }
            }
            if (listTasks.Count == 0)
            {
                return null;
            }
            IEnumerable<Task> result = listTasks.OrderBy(x => x.Name);
            return result;
        }

        /// <summary>
        /// Добавляет новую задачу в базу данных
        /// </summary>
        /// <param name="name">Имя задачи</param>
        /// <param name="deadline">дата выполнения задачи</param>
        /// <param name="marks">Строка меток</param>
        /// <param name="user">Логин пользовотеля для которого выполняется запрос</param>
        public void AddTask(string name, DateTime? deadline, string marks, string user)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                char[] delimiterChars = { ',', '.', ';' };
                string[] sSplitedMarks = marks != null ? marks.Split(delimiterChars) : new string[1];
                const string insertTaskMarks = @"
                    insert into 
                        Tasks_Marks 
                                    (
                                    id_task
                                    ,name_mark
                                    )
                            values
                                    (
                                    IDENT_CURRENT( 'tasks' )
                                    ,@mark
                                    )
                ";
                string insertTask;
                connection.Open();

                if (deadline != null)
                {
                    insertTask = @"
                        insert into 
                            Tasks 
                                ( 
                                name_task
                                ,deadline
                                ,user_login
                                ,deleted  
                                )
                        values 
                            (
                            @name
                            ,@deadline
                            ,@user
                            ,0
                            )
                    ";
                    using (var cmd = new SqlCommand(insertTask, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@deadline", deadline);
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    insertTask = @"
                       insert into 
                            Tasks 
                                ( 
                                name_task
                                ,user_login
                                ,deleted
                                )
                        values 
                            (
                            @name
                            ,@user
                            ,0
                            )
                    ";
                    using (var cmd = new SqlCommand(insertTask, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.ExecuteNonQuery();
                    }
                }
                for (int i = 0; i < sSplitedMarks.Length; i++)
                    for (int j = 0; j < sSplitedMarks.Length; j++)
                    {
                        if ((i != j) && (sSplitedMarks[i] == sSplitedMarks[j]))
                        {
                            sSplitedMarks[j] = "";
                        }
                    }
                for (var i = 0; i < sSplitedMarks.Length; i++)
                {
              //      sSplitedMarks[i] = HttpUtility.HtmlEncode(sSplitedMarks[i]);
                    using (var cmd = new SqlCommand(insertTaskMarks, connection))
                    {
                        if (!string.IsNullOrEmpty(sSplitedMarks[i]))
                        {
                            cmd.Parameters.AddWithValue("@mark", sSplitedMarks[i]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Спрятать задачу
        /// </summary>
        /// <param name="id">id задачи</param>
        public void HideTask(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
                       update Tasks
                       set deleted=1
                       where id_task = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ищет задачу в базе по id
        /// </summary>
        /// <param name="id">id задачи</param>    
        /// <returns></returns>
        public Task FindTask(int id)
        {
            var task = new Task();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = @"
                            select 
                                name_task
                                ,deadline
                            from
                                Tasks   
                            where 
                                id_task=@id";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            task.Name = reader.GetString(reader.GetOrdinal("name_task"));                  
                            if (reader["deadline"] == DBNull.Value)
                                task.Deadline = null;
                            else
                                task.Deadline = reader.GetDateTime(reader.GetOrdinal("deadline"));
                        }
                    }
                    var marksList = new List<string>();
                    cmd.CommandText = @"
                            select 
                                name_mark
                            from 
                                Tasks_Marks
                            where 
                                id_task=@id";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            marksList.Add(reader.GetString(reader.GetOrdinal("name_mark")));
                        }
                    }
                    string marks = string.Empty;
                    foreach (var mark in marksList)
                    {
                        marks += mark + ",";
                    }
                    task.Id = id;
                    task.Marks = marks;

                }
            }
            return task;
        }

        /// <summary>
        /// Изменяет задачу 
        /// </summary>
        /// <param name="id">id задачи</param>
        /// <param name="name">имя задачи</param>
        /// <param name="deadline">время завершения выполнения задачи</param>
        /// <param name="marks">метки</param>
        public void EditTask(int id, string name, DateTime? deadline, string marks)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@id", id);
                    if (deadline != null)
                    {
                        cmd.CommandText = @"
                       update Tasks
                       set 
                            name_task=@name
                            ,deadline=@deadline
                       where id_task = @id
                    ";

                        cmd.Parameters.AddWithValue("@deadline", deadline);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"
                       update Tasks
                       set 
                        name_task=@name
                       where id_task = @id
                    ";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = @"
                        delete from Tasks_marks
                        where id_task=@id";
                    cmd.ExecuteNonQuery();
                }
                char[] delimiterChars = { ',', '.', ';' };
                string[] sSplitedMarks = marks != null ? marks.Split(delimiterChars) : new string[1];
                const string insertTaskMarks = @"
                                    insert into 
                                        Tasks_Marks 
                                                 (
                                                  id_task
                                                  ,name_mark
                                                  )
                                         values
                                                  (
                                                    @id
                                                   ,@mark
                                                   )";
                for (int i = 0; i < sSplitedMarks.Length; i++)
                    for (int j = 0; j < sSplitedMarks.Length; j++)
                    {
                        if ((i != j) && (sSplitedMarks[i] == sSplitedMarks[j]))
                        {
                            sSplitedMarks[j] = "";
                        }
                    }
                for (var i = 0; i < sSplitedMarks.Length; i++)
                {
                  //  sSplitedMarks[i] = HttpUtility.HtmlEncode(sSplitedMarks[i]);
                //    sSplitedMarks[i] = HttpUtility.HtmlEncode(sSplitedMarks[i]);
                    using (var cmd = new SqlCommand(insertTaskMarks, connection))
                    {
                        if (!string.IsNullOrEmpty(sSplitedMarks[i]))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@mark", sSplitedMarks[i]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        public void Authorization(string userName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
                        if not exists
                        (
                            select user_login
                            from users
                            where user_login=@userName
                        ) 
						insert into  Users
                                            (user_login)
                                        values
								            (@userName)
                        ";
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}