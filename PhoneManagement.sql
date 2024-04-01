create database PhoneManagement
go
use PhoneManagement
go

create table Vendor( -- Nhà cung cấp
	VendorID nvarchar(30) primary key not null,
	VendorName nvarchar(30),
	Address nvarchar(30),
	PhoneNumber nvarchar(20),
	Status int, -- 0: Ngừng hợp tác, 1: Đang hợp tác
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Category( -- Danh mục sản phẩm
	CategoryID nvarchar(30) primary key not null,
	CategoryName nvarchar(30),
	CategoryImage nvarchar(100),
	Status int, -- 0: Ngừng kinh doanh, 1: Đang kinh doanh
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Color( -- Màu sản phẩm
	ColorName nvarchar(50) primary key,
	ColorImage nvarchar(100),
	ColorPrice int, 
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Storage( -- Dung lượng bộ nhớ
	StorageGB int primary key,
	StoragePrice int,
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Product( -- Sản phẩm
	ProductID nvarchar(30) primary key not null,
	ProductName nvarchar(30),
	StorageGB int,
	ColorName nvarchar(50),
	Amount int,
	Price int,
	Discount int, -- Giảm giá
	CategoryID nvarchar(30),
	VendorID nvarchar(30),
	Detail nvarchar(max),
	Status int, -- "Hết hàng", "Còn hàng", "Sắp ra mắt", ...
	CreateAt datetime default getdate(),
	UpdateAt datetime,

	constraint FK_Product_StorageGB foreign key(StorageGB) references Storage(StorageGB),
	constraint FK_Product_ColorName foreign key(ColorName) references Color(ColorName),
	constraint FK_Product_CategoryID foreign key(CategoryID) references Category(CategoryID),
    constraint FK_Product_VendorID foreign key(VendorID) references Vendor(VendorID),
)
go

create table ProductImage( -- Các hình ảnh của sản phẩm ProductID
	ProductImageID nvarchar(100) primary key,
	ProductID nvarchar(30),
	ImagePath nvarchar(100),
	IsPrimary bit, -- Đánh dấu hình ảnh xuất hiện đầu tiên
	CreateAt datetime default getdate(),
	UpdateAt datetime,

	constraint FK_ProductImage_ProductID foreign key (ProductID) references Product(ProductID)
)
go

create table Customer( -- Khách hàng
	CustomerID nvarchar(30) primary key not null,
	CustomerName nvarchar(30),
	Email nvarchar(50) UNIQUE, -- Username là Email
	Password nvarchar(100),
	PhoneNumber nvarchar(20),
	Address nvarchar(50),
	Status int, -- 0: Đã khóa, 1: Đã kích hoạt
	LastLogin datetime,
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Account( -- Tài khoản hệ thống
	Username nvarchar(30) primary key,
	Password nvarchar(100),
	LastLogin datetime,
	Status int, -- 0: Đã khóa, 1: Đã kích hoạt
	CreateAt datetime default getdate(),
	UpdateAt datetime,
)
go

create table Bill ( -- Đơn hàng
	BillID nvarchar(30) primary key not null,
	CustomerID nvarchar(30),
	DateBill datetime default getdate(),
	DeliveryAddress nvarchar(100),
	Status int null, -- Trạng thái đơn hàng
	TotalBill int, -- Tổng tiền hóa đơn
	UpdateAt datetime,

	constraint FK_Bill_CustomerID foreign key(CustomerID) references Customer(CustomerID),
)
go

create table BillDetail( -- Chi tiết đơn hàng
	BillID nvarchar(30),
	ProductID nvarchar(30),
	Amount int,
	Price int,
	Discount int,
	Total int, --Thành tiền
	CreateAt datetime default getdate(),
	UpdateAt datetime,

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
	CreateAt datetime default getdate(),
	UpdateAt datetime,

	constraint FK_ShoppingCart_CustomerID foreign key(CustomerID) references Customer(CustomerID)
)
go

create table ShoppingCartDetail( -- Chi tiết giỏ hàng
	ShoppingCartID nvarchar(30),
	ProductID nvarchar(30),
	Amount int,
	Total int,
	CreateAt datetime default getdate(),
	UpdateAt datetime,
	
	primary key (ShoppingCartID, ProductID),
	constraint FK_ShoppingCartDetail_ShoppingCartID foreign key(ShoppingCartID) references ShoppingCart(ShoppingCartID),
	constraint FK_ShoppingCartDetail_ProductID foreign key(ProductID) references Product(ProductID)
)
go

create table SystemNotification ( -- Thông báo hệ thống
    NotificationID int primary key not null,
    Title nvarchar(255),
    Message nvarchar(max),
    NotificationType nvarchar(50),  -- Ví dụ: 'General', 'Update', 'Warning', 'Error'
    CreatedAt datetime default getdate(),
    IsActive bit default 1  -- Kích hoạt
)
go

create table SystemNotificationRead ( -- Tài khoản đã đọc
    ReadID int primary key not null,
	NotificationID int,
    CustomerID nvarchar(30),
    ReadAt datetime default getdate(),

    constraint FK_SystemNotificationRead_NotificationID foreign key (NotificationID) references SystemNotification(NotificationID),
    constraint FK_SystemNotificationRead_CustomerID foreign key(CustomerID) references Customer(CustomerID) 
)
go
alter table Bill
add Note nvarchar(max)
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


INSERT [dbo].[Category] ([CategoryID], [CategoryName], [Status]) VALUES (N'CT001', N'Iphone', 1)
INSERT [dbo].[Category] ([CategoryID], [CategoryName], [Status]) VALUES (N'CT002', N'SamSung', 1)
INSERT [dbo].[Category] ([CategoryID], [CategoryName], [Status]) VALUES (N'CT003', N'Oppo', 1)
GO

INSERT [dbo].[Color] ([ColorName], [ColorPrice]) VALUES (N'Xanh', 10)
INSERT [dbo].[Color] ([ColorName], [ColorPrice]) VALUES (N'Đỏ', 12)
INSERT [dbo].[Color] ([ColorName], [ColorPrice]) VALUES (N'Tím', 14)
INSERT [dbo].[Color] ([ColorName], [ColorPrice]) VALUES (N'Vàng', 16)

INSERT [dbo].[Storage] ([StorageGB], [StoragePrice]) VALUES (64, 5)
INSERT [dbo].[Storage] ([StorageGB], [StoragePrice]) VALUES (128, 10)
INSERT [dbo].[Storage] ([StorageGB], [StoragePrice]) VALUES (256, 15)
INSERT [dbo].[Storage] ([StorageGB], [StoragePrice]) VALUES (512, 20)
GO
INSERT [dbo].[Vendor] ([VendorID], [VendorName], [Address], [PhoneNumber], [Status]) VALUES (N'VD001', N'Nhà cung cấp A', N'Hà Nội', N'011111111', 1)
INSERT [dbo].[Vendor] ([VendorID], [VendorName], [Address], [PhoneNumber], [Status]) VALUES (N'VD002', N'Nhà cung cấp B', N'Ðà Nẵng', N'022222222', 1)
INSERT [dbo].[Vendor] ([VendorID], [VendorName], [Address], [PhoneNumber], [Status]) VALUES (N'VD003', N'Nhà cung cấp C', N'TP HCM', N'033333333', 1)
GO

INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH001', NULL, N'trang', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH002', NULL, N'dung', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH003', NULL, N'khanh', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH004', NULL, N'viet', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH005', NULL, N'ha', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH006', NULL, N'nghien', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 0)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH007', NULL, N'duy', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
INSERT [dbo].[Customer] ([CustomerID], [CustomerName], [Email], [Password], [PhoneNumber], [Address], [Status]) VALUES (N'MKH008', NULL, N'xuantrangdo', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', NULL, NULL, 1)
GO
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC001', N'MKH001', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC002', N'MKH002', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC003', N'MKH003', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC004', N'MKH004', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC005', N'MKH005', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC006', N'MKH006', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC007', N'MKH007', 0, 1)
INSERT [dbo].[ShoppingCart] ([ShoppingCartID], [CustomerID], [TotalCart], [Status]) VALUES (N'SPC008', N'MKH008', 0, 1)
GO

INSERT INTO [dbo].[Product] 
([ProductID], [ProductName], [StorageGB], [ColorName], [Amount], [Price],[Discount], [CategoryID], [VendorID], [Detail], [Status]) 
VALUES 
(N'PRD001', N'IphoneX', 64, N'Vàng', 10, 10, 5, N'CT001', N'VD002', N'Tần số quét 60Hz', 1),
(N'PRD002', N'Iphone 11prm', 64, N'Xanh', 8, 11, 5, N'CT001', N'VD003', N'Tần số quét 60Hz, kích thước màn hình 6.5 inch', 1),
(N'PRD003', N'Iphone 12prm', 128, N'Xanh', 10, 12, 5, N'CT001', N'VD003', N'Tần số quét 60Hz', 1),
(N'PRD004', N'Iphone 13prm', 128, N'Tím', 13, 13, 5, N'CT001', N'VD003', N'Tần số quét 120Hz', 1),
(N'PRD005', N'Iphone 14prm', 512, N'Tím', 16, 14, 5, N'CT001', N'VD001', N'Tai thỏ', 1),
(N'PRD006', N'Iphone 15prm', 256, N'Vàng', 20, 15, 5, N'CT001', N'VD002', N'Tần số quét 120Hz', 1)
GO

INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC001', N'PRD001', 1, 13)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC001', N'PRD002', 2, 24)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC001', N'PRD003', 1, 13)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC001', N'PRD004', 2, 30)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC001', N'PRD006', 1, 11)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC002', N'PRD002', 2, 24)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC002', N'PRD003', 2, 26)
INSERT [dbo].[ShoppingCartDetail] ([ShoppingCartID], [ProductID], [Amount], [Total]) VALUES (N'SPC002', N'PRD005', 1, 14)


