To Build (from root)
dotnet build

To Test (from root)
dotnet test JSONClientServerTest

SQL Databases

use lobby_server;
CREATE TABLE users (
    username varchar(32), 
    salt varchar(128),
    password varchar(128), 
    iterations integer,
    email varchar(128), 
    status varchar(32)
);

sudo service mysql start