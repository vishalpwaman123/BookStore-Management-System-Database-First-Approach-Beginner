create table BookDetails(
BookID int identity(1,1) primary key,
InsertionDate datetime default current_timestamp,
UpdateDate datetime ,
BookName varchar(255) not null,
BookType varchar(100) not null,
BookPrice varchar(10) not null,
BookDetails varchar(2055),
BookAuthor varchar(255),
BookImageUrl varchar(512),
PublicId varchar(255),
Quantity int,
IsArchive bit default 0,
IsActive bit default 1
)