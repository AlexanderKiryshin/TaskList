USE taskList_Kiryshin

delete Users
delete Tasks
dbcc checkident('Tasks', RESEED, 0)
delete	Tasks_Marks


insert into Users (user_login) values
	 ('demo')


set identity_insert Tasks on 

insert into Tasks (id_task,name_task,deadline,user_login,deleted,time_when_task_completed ) values
     (1, 'Все задачи отсортированы по алфавиту',null,'demo', 0, null) 
	 ,(2, 'Задачи в базе данных не удаляются,а делаются невидимыми для пользователя', '18.07.2017', 'demo', 1,null)	 
	 ,(3, '5 последних выполненных задач  видны пользователю,остальные остаются в базе данных,но пользователю не видны','18.03.2017','demo', 0, '09.01.2017')
	 ,(4, 'Весь sql-код написан с использованием переменных,что исключает sql инъекции','18.03.2017','demo', 0,null)
	 ,(5, 'метки для задач имеют ограничение по длине в 19 символов,описание задачи-512 символов.При превышении лимита выдается предупреждение.Также предупреждение выдается при вводе неккоректной даты','23.01.2017','demo', 0, null)

 set identity_insert Tasks off
  
insert into Tasks_marks(id_task,name_mark) values
	 (1, 'метка 1')
	 ,(1, 'метка 3')
	 ,(2, 'метка 2')
	 ,(4, 'метка 2')
	 ,(5, 'метка 3')
	 ,(5, 'самая длинная метка')



