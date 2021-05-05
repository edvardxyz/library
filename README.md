# A complete library in C# using OOP for school
(all user facing text is in Danish)
In this library program you can create a library user, login as the user, and login as library admin.

The password to admin is "sikkerkode" and is hardcoded into the program
When a user has created his account he can login and borrow books, return books, delete his account,
show all the books he has borrowed or show all the books the library has available.

The admin can create new books and delete any user and see all the users.
The admin does not have access to the users passwords they are hashed in SHA256.

When the program starts it checks if there is a database for users and books.
If there is then, it deserializes the files into the correct dictionary of objects.
In the same way the program will save the state of the dictionaries when the program closes.


