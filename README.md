# e-taraba.API
.net core 6 api

## Sql Server data-base:

### Schema
```
create schema taraba
```

### Primary Tables:
##### taraba.tProducts
```
create table taraba.tProducts(
id int identity(1,1) primary key,
name varchar(100) not null,
description varchar(255) not null,
quantity int not null, --update by db, or API?
photoId varchar(255), --guid
photoFolderPath varchar(255),
price decimal(9,2) not null)

alter table taraba.tProducts
alter column description varchar(5000)

```
##### taraba.tOrders
```

create table taraba.tOrders(
id int identity(1,1) primary key,
customerFirstName varchar(100) not null,
customerLastName varchar(100) not null,
customerPhone varchar(20) not null,
total decimal(9,2) not null
)
```
##### taraba.tProductOrderDetails
```

create table taraba.tProductOrderDetails(
id int identity(1,1) primary key,
idProduct int constraint fk_ProductOrder foreign key(idProduct) references taraba.tProducts(id),
idOrder int constraint fk_OrderProduct foreign key(idOrder) references taraba.tOrders(id),
price decimal(9,2) not null, --final price? maybe after discount
quantity int not null
);
```

##### Trigger to update quantity when placing order

```
create trigger taraba.update_quantity_of_product
on taraba.tProductOrderDetails
after insert 
as
begin 
	update taraba.tProducts
	set quantity = P.quantity - I.quantity
	from taraba.tProducts as P inner join inserted as I on P.id = I.idProduct
END
```
### Complementary tables

##### taraba.tUsers
```
create table taraba.tUsers(
id int identity(1,1) constraint pk_tUsers primary key,
username varchar(100) not null constraint uq_tUsers_username unique,
hashedPassword varbinary(32) not null,
hashSalt varbinary(64) not null
)
``` 

##### taraba.tRefreshTokens
```
create table taraba.tRefreshTokens(
id int identity(1,1) constraint pk_RTokens primary key,
RToken varchar(100) not null constraint uq_tRToken_RToken unique,
valid bit not null,
dateCreated smalldatetime not null,
dateExpires smalldatetime not null,
idUser int constraint fk_RToken_Users foreign key(idUser) references taraba.tUsers(id)
)
```
