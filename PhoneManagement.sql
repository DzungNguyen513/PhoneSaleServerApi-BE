create database PhoneManagement
go
use PhoneManagement
go

create table Vendor( -- Nhà cung cấp
	VendorID nvarchar(30) primary key not null,
	VendorName nvarchar(30),
	Address nvarchar(30),
	PhoneNumber nvarchar(20),
	Status int 
)
go

create table Category( -- Danh mục sản phẩm
	CategoryID nvarchar(30) primary key not null,
	CategoryName nvarchar(30),
	Status int
)
go

create table Color( -- Màu sản phẩm
	ColorName nvarchar(50) primary key,
	ColorPrice int 
)
go

create table Storage( -- Dung lượng bộ nhớ
	StorageGB int primary key,
	StoragePrice int
)
go

create table Product( -- Sản phẩm
	ProductID nvarchar(30) primary key not null,
	ProductName nvarchar(30),
	StorageGB int,
	ColorName nvarchar(50),
	Amount int,
	Price int,
	CategoryID nvarchar(30),
	VendorID nvarchar(30),
	Detail nvarchar(max),
	Img nvarchar(50), -- Ảnh sản phẩm
	Status int, -- "Hết hàng", "Còn hàng", "Sắp ra mắt", ...

	constraint FK_Product_StorageGB foreign key(StorageGB) references Storage(StorageGB),
	constraint FK_Product_ColorName foreign key(ColorName) references Color(ColorName),
	constraint FK_Product_CategoryID foreign key(CategoryID) references Category(CategoryID),
    constraint FK_Product_VendorID foreign key(VendorID) references Vendor(VendorID)
)
go

create table Customer( -- Khách hàng
	CustomerID nvarchar(30) primary key not null,
	CustomerName nvarchar(30),
	Email nvarchar(50) UNIQUE, -- Username là Email
	Password nvarchar(100),
	PhoneNumber nvarchar(20),
	Address nvarchar(50)
)
go

CREATE TABLE Employee( -- Nhân viên
    EmployeeID NVARCHAR(30) PRIMARY KEY,
    EmployeeName NVARCHAR(30),
    PhoneNumber NVARCHAR(30),
    Status int
)
go

create table Account( -- Tài khoản nhân viên
	Username nvarchar(30) primary key,
	Password nvarchar(100)
)
go

create table Bill ( -- Đơn hàng
	BillID nvarchar(30) primary key not null,
	CustomerID nvarchar(30),
	EmployeeID nvarchar(30),
	DateBill date,
	Status int null, -- Trạng thái đơn hàng
	TotalBill int, -- Tổng tiền hóa đơn

	constraint FK_Bill_CustomerID foreign key(CustomerID) references Customer(CustomerID),
	constraint FK_Bill_EmployeeID foreign key(EmployeeID) references Employee(EmployeeID),
)
go

create table BillDetail( -- Chi tiết đơn hàng
	BillID nvarchar(30),
	ProductID nvarchar(30),
	Amount int,
	Price int,
	Total int, --Thành tiền

	constraint PK_BillDetail primary key clustered -- Cặp khóa chính 
	(
		BillID ASC,
		ProductID ASC
	),
	constraint FK_BillDetail_BillID foreign key(BillID) references Bill(BillID),
	constraint FK_BillDetail_ProductID foreign key(ProductID) references Product(ProductID)
)
go
                                                                  
create table ShoppingCart( -- Giỏ hàng
	ShoppingCartID nvarchar(30) primary key,
	CustomerID nvarchar(30) unique not null,
	TotalCart int, -- Tổng tiền giỏ hàng		
	Status int, -- Trạng thái giỏ hàng

	constraint FK_ShoppingCart_CustomerID foreign key(CustomerID) references Customer(CustomerID)
)
go

create table ShoppingCartDetail( -- Chi tiết giỏ hàng
	ShoppingCartID nvarchar(30),
	ProductID nvarchar(30),
	Amount int,
	Price int,
	Total int,
	
	primary key (ShoppingCartID, ProductID),
	constraint FK_ShoppingCartDetail_ShoppingCartID foreign key(ShoppingCartID) references ShoppingCart(ShoppingCartID),
	constraint FK_ShoppingCartDetail_ProductID foreign key(ProductID) references Product(ProductID)
)
go
alter table Customer
add Status int
alter table ShoppingCartDetail
drop column Price
go

create or alter trigger UpdateTotalOnAmountChange
on ShoppingCartDetail
after insert, update
as
begin
	set nocount on;
	IF TRIGGER_NESTLEVEL() > 1 RETURN;

    update ShoppingCartDetail
	set Total = Product.Price*ShoppingCartDetail.Amount
	from inserted
	inner join ShoppingCartDetail on ShoppingCartDetail.ShoppingCartID = inserted.ShoppingCartID 
								  and inserted.ProductID = ShoppingCartDetail.ProductID
	inner join Product on Product.ProductID = ShoppingCartDetail.ProductID
end
go


