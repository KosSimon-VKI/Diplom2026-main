USE [master]
GO
/****** Object:  Database [db_demo2]    Script Date: 07.05.2026 22:11:02 ******/
CREATE DATABASE [db_demo2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'db_demo2', FILENAME = N'C:\Users\Admin\Desktop\fh\SQL Manegment\MSSQL16.SQLEXPRESS\MSSQL\DATA\db_demo2.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'db_demo2_log', FILENAME = N'C:\Users\Admin\Desktop\fh\SQL Manegment\MSSQL16.SQLEXPRESS\MSSQL\DATA\db_demo2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [db_demo2] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_demo2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [db_demo2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [db_demo2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [db_demo2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [db_demo2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [db_demo2] SET ARITHABORT OFF 
GO
ALTER DATABASE [db_demo2] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [db_demo2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [db_demo2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [db_demo2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [db_demo2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [db_demo2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [db_demo2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [db_demo2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [db_demo2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [db_demo2] SET  ENABLE_BROKER 
GO
ALTER DATABASE [db_demo2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [db_demo2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [db_demo2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [db_demo2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [db_demo2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [db_demo2] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [db_demo2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [db_demo2] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [db_demo2] SET  MULTI_USER 
GO
ALTER DATABASE [db_demo2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [db_demo2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [db_demo2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [db_demo2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [db_demo2] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [db_demo2] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [db_demo2] SET QUERY_STORE = ON
GO
ALTER DATABASE [db_demo2] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [db_demo2]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Password] [nvarchar](100) NULL,
	[ClientCategoryId] [int] NULL,
	[OrderCount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discounts]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[DiscountPercent] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DishCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dishes]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dishes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[CategoryId] [int] NULL,
	[UnitOfMeasureId] [int] NULL,
	[CostRub] [decimal](10, 2) NULL,
	[MarkupPercent] [decimal](10, 2) NULL,
	[PriceRub] [decimal](10, 2) NULL,
	[CostPercent] [decimal](5, 2) NULL,
	[TechnicalCardId] [int] NULL,
	[FatsG] [decimal](8, 2) NULL,
	[ProteinsG] [decimal](8, 2) NULL,
	[CarbsG] [decimal](8, 2) NULL,
	[CaloriesKcal] [decimal](8, 2) NULL,
	[Kilojoules] [decimal](8, 2) NULL,
	[ImageUrl] [nvarchar](500) NULL,
	[IsAvailable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishToppings]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DishToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ToppingId] [int] NULL,
	[OrderDishItemId] [int] NULL,
	[Quantity] [decimal](8, 2) NULL,
	[FinalPrice] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DrinkCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DrinkCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Drinks]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Drinks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Quantity] [decimal](8, 2) NULL,
	[UnitOfMeasureId] [int] NULL,
	[CategoryId] [int] NULL,
	[CostRub] [decimal](10, 2) NULL,
	[MarkupPercent] [decimal](10, 2) NULL,
	[PriceRub] [decimal](10, 2) NULL,
	[CostPercent] [decimal](5, 2) NULL,
	[TechnicalCardId] [int] NULL,
	[FatsG] [decimal](8, 2) NULL,
	[ProteinsG] [decimal](8, 2) NULL,
	[CarbsG] [decimal](8, 2) NULL,
	[CaloriesKcal] [decimal](8, 2) NULL,
	[Kilojoules] [decimal](8, 2) NULL,
	[ImageUrl] [nvarchar](500) NULL,
	[IsAvailable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DrinkToppings]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DrinkToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ToppingId] [int] NULL,
	[OrderDrinkItemId] [int] NULL,
	[Quantity] [decimal](8, 2) NULL,
	[FinalPrice] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ingredients]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ingredients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Stock] [decimal](10, 2) NULL,
	[UnitOfMeasureId] [int] NULL,
	[CostRub] [decimal](10, 2) NULL,
	[CategoryId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientSupplyActItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientSupplyActItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplyActId] [int] NOT NULL,
	[IngredientId] [int] NOT NULL,
	[Quantity] [decimal](10, 2) NOT NULL,
	[UnitOfMeasureId] [int] NULL,
 CONSTRAINT [PK_IngredientSupplyActItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientSupplyActs]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientSupplyActs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_IngredientSupplyActs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientWriteOffActItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientWriteOffActItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WriteOffActId] [int] NOT NULL,
	[IngredientId] [int] NOT NULL,
	[Quantity] [decimal](10, 2) NOT NULL,
	[UnitOfMeasureId] [int] NULL,
	[WriteOffTypeId] [int] NOT NULL,
 CONSTRAINT [PK_IngredientWriteOffActItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MenuItemPortionLimits]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItemPortionLimits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemType] [nvarchar](16) NOT NULL,
	[ItemId] [int] NOT NULL,
	[RemainingPortions] [decimal](10, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_MenuItemPortionLimits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDishItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDishItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[DishId] [int] NULL,
	[Quantity] [decimal](8, 2) NULL,
	[FinalPrice] [decimal](10, 2) NULL,
	[IsCompleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDrinkItemModifiers]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDrinkItemModifiers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderDrinkItemId] [int] NOT NULL,
	[MilkIngredientId] [int] NULL,
	[CoffeeIngredientId] [int] NULL,
 CONSTRAINT [PK_OrderDrinkItemModifiers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDrinkItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDrinkItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[DrinkId] [int] NULL,
	[Quantity] [decimal](8, 2) NULL,
	[FinalPrice] [decimal](10, 2) NULL,
	[IsCompleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[ClientId] [int] NULL,
	[OrderTypeId] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[TotalCalories] [decimal](10, 2) NULL,
	[DiscountId] [int] NULL,
	[TotalPrice] [decimal](10, 2) NULL,
	[StatusID] [int] NULL,
	[PickupAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatuses]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderToppingItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderToppingItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ToppingId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[IsCompleted] [bit] NOT NULL,
 CONSTRAINT [PK_OrderToppingItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderTypes]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Preparations]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Preparations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[SemiFinishedId] [int] NULL,
	[StockGrams] [decimal](10, 2) NULL,
	[ProductionDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreparationTasks]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreparationTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SemiFinishedId] [int] NULL,
	[Comment] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[TaskText] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_PreparationTasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemiFinished]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemiFinished](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[CostRub] [decimal](10, 2) NULL,
	[CategoryId] [int] NULL,
	[UnitOfMeasureId] [int] NULL,
	[TechnicalCardId] [int] NULL,
	[FatsG] [decimal](8, 2) NULL,
	[ProteinsG] [decimal](8, 2) NULL,
	[CarbsG] [decimal](8, 2) NULL,
	[CaloriesKcal] [decimal](8, 2) NULL,
	[Kilojoules] [decimal](8, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemiFinishedCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemiFinishedCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemiFinishedWriteOffActItems]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemiFinishedWriteOffActItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WriteOffActId] [int] NOT NULL,
	[SemiFinishedId] [int] NOT NULL,
	[Quantity] [decimal](10, 2) NOT NULL,
	[UnitOfMeasureId] [int] NULL,
	[WriteOffTypeId] [int] NOT NULL,
 CONSTRAINT [PK_SemiFinishedWriteOffActItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Staff]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staff](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[FullName] [nvarchar](255) NULL,
	[Login] [nvarchar](100) NULL,
	[Password] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaffRoles]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaffRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TechnicalCardIngredientComposition]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TechnicalCardIngredientComposition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TechnicalCardId] [int] NULL,
	[IngredientId] [int] NULL,
	[UnitOfMeasureId] [int] NULL,
	[GrossWeight] [decimal](10, 6) NULL,
	[ColdLossPercent] [decimal](10, 2) NULL,
	[NetWeight] [decimal](10, 6) NULL,
	[HotLossPercent] [decimal](10, 2) NULL,
	[OutputWeight] [decimal](10, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TechnicalCards]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TechnicalCards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TechnicalCardSemiFinishedComposition]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TechnicalCardSemiFinishedComposition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TechnicalCardId] [int] NULL,
	[SemiFinishedId] [int] NULL,
	[UnitOfMeasureId] [int] NULL,
	[GrossWeight] [decimal](10, 6) NULL,
	[ColdLossPercent] [decimal](10, 2) NULL,
	[NetWeight] [decimal](10, 6) NULL,
	[HotLossPercent] [decimal](10, 2) NULL,
	[OutputWeight] [decimal](10, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToppingCategories]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToppingCategories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToppingsAndSyrups]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToppingsAndSyrups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Quantity] [decimal](8, 2) NULL,
	[UnitOfMeasureId] [int] NULL,
	[CostRub] [decimal](10, 2) NULL,
	[MarkupPercent] [decimal](10, 2) NULL,
	[PriceRub] [decimal](10, 2) NULL,
	[CostPercent] [decimal](5, 2) NULL,
	[TechnicalCardId] [int] NULL,
	[FatsG] [decimal](8, 2) NULL,
	[ProteinsG] [decimal](8, 2) NULL,
	[CarbsG] [decimal](8, 2) NULL,
	[CaloriesKcal] [decimal](8, 2) NULL,
	[Kilojoules] [decimal](8, 2) NULL,
	[CategoryID] [int] NULL,
	[IsAvailable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnitsOfMeasure]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnitsOfMeasure](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WriteOffActs]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WriteOffActs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Comment] [nvarchar](500) NULL,
	[StaffId] [int] NULL,
 CONSTRAINT [PK_WriteOffActs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WriteOffTypes]    Script Date: 07.05.2026 22:11:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WriteOffTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_WriteOffTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260305143433_Baseline_ExistingSchema', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260305143616_AddIsAvailableStopList', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260309154559_AddOrderStatusesStatusId', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260318050658_AddOrderDrinkItemModifiers', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260326091640_AddOrdersPickupAt', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260409120000_AddKitchenOrderItemIsCompleted', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260414100928_AddPreparationTasks', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260414110943_AddPreparationTaskTextAndOptionalSemiFinished', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260414161539_RestorePreparationTaskTextAndOptionalSemiFinished', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260422073207_AddMenuItemPortionLimits', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260423152815_AddWriteOffActs', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260427161423_ReworkWriteOffActStructure', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260505120000_AddIngredientSupplyActs', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260508120000_FillTechnicalCardRecipesAndAlgorithms', N'8.0.0')
GO
SET IDENTITY_INSERT [dbo].[ClientCategories] ON 

INSERT [dbo].[ClientCategories] ([Id], [Name]) VALUES (1, N'Новый')
INSERT [dbo].[ClientCategories] ([Id], [Name]) VALUES (2, N'Постоянный')
INSERT [dbo].[ClientCategories] ([Id], [Name]) VALUES (3, N'Особый')
INSERT [dbo].[ClientCategories] ([Id], [Name]) VALUES (4, N'Без категории')
SET IDENTITY_INSERT [dbo].[ClientCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Clients] ON 

INSERT [dbo].[Clients] ([Id], [FullName], [PhoneNumber], [Password], [ClientCategoryId], [OrderCount]) VALUES (16, N'test1', N'79991234567', N'123', 2, 101)
INSERT [dbo].[Clients] ([Id], [FullName], [PhoneNumber], [Password], [ClientCategoryId], [OrderCount]) VALUES (17, N'Simon', N'79130615770', N'123', 3, 0)
INSERT [dbo].[Clients] ([Id], [FullName], [PhoneNumber], [Password], [ClientCategoryId], [OrderCount]) VALUES (18, N'Гость WPF', N'79990000000', N'wpf-guest', 4, 0)
INSERT [dbo].[Clients] ([Id], [FullName], [PhoneNumber], [Password], [ClientCategoryId], [OrderCount]) VALUES (19, N'какашкинскс', N'72886325535', N'ооо', 3, 0)
INSERT [dbo].[Clients] ([Id], [FullName], [PhoneNumber], [Password], [ClientCategoryId], [OrderCount]) VALUES (20, N'какашка', N'79137208326', N'ооо', 1, 1)
SET IDENTITY_INSERT [dbo].[Clients] OFF
GO
SET IDENTITY_INSERT [dbo].[Discounts] ON 

INSERT [dbo].[Discounts] ([Id], [Name], [DiscountPercent]) VALUES (1, N'Для новых клиентов', CAST(15.00 AS Decimal(5, 2)))
INSERT [dbo].[Discounts] ([Id], [Name], [DiscountPercent]) VALUES (2, N'Для постоянных клиентов', CAST(7.00 AS Decimal(5, 2)))
INSERT [dbo].[Discounts] ([Id], [Name], [DiscountPercent]) VALUES (3, N'Для особых клиентов', CAST(20.00 AS Decimal(5, 2)))
INSERT [dbo].[Discounts] ([Id], [Name], [DiscountPercent]) VALUES (4, N'Для сотрудников', CAST(20.00 AS Decimal(5, 2)))
INSERT [dbo].[Discounts] ([Id], [Name], [DiscountPercent]) VALUES (5, N'На день рождения', CAST(15.00 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[Discounts] OFF
GO
SET IDENTITY_INSERT [dbo].[DishCategories] ON 

INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (1, N'Боулы')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (2, N'Вафли')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (3, N'Витрина')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (4, N'Горячие блюда')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (5, N'Супы')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (6, N'Десерты')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (7, N'Завтрки')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (8, N'Торты')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (9, N'Салаты')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (10, N'Сендвичи')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (11, N'Закуски')
INSERT [dbo].[DishCategories] ([Id], [Name]) VALUES (12, N'Неактивные')
SET IDENTITY_INSERT [dbo].[DishCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Dishes] ON 

INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (1, N'Боул с креветкой, нутом, киноа, авокадо', 12, 1, CAST(186.47 AS Decimal(10, 2)), CAST(221.23 AS Decimal(10, 2)), CAST(599.00 AS Decimal(10, 2)), CAST(31.13 AS Decimal(5, 2)), NULL, CAST(24.00 AS Decimal(8, 2)), CAST(12.00 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1470.00 AS Decimal(8, 2)), N'photo1.png', 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (2, N'Боул с тофу, нутом, киноа, авокадо', 12, 1, CAST(139.55 AS Decimal(10, 2)), CAST(264.74 AS Decimal(10, 2)), CAST(509.00 AS Decimal(10, 2)), CAST(27.42 AS Decimal(5, 2)), NULL, CAST(17.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(19.00 AS Decimal(8, 2)), CAST(270.00 AS Decimal(8, 2)), CAST(1120.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (3, N'Безглютеновая  вафля с креветкой и яйцом пашот', 2, 1, CAST(102.44 AS Decimal(10, 2)), CAST(348.07 AS Decimal(10, 2)), CAST(459.00 AS Decimal(10, 2)), CAST(22.32 AS Decimal(5, 2)), 442, CAST(24.00 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(11.00 AS Decimal(8, 2)), CAST(320.00 AS Decimal(8, 2)), CAST(1340.00 AS Decimal(8, 2)), N'vaffle_shrimp.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (4, N'Вафля со свекольным сыром и имбирным соусом', 2, 1, CAST(88.25 AS Decimal(10, 2)), CAST(374.79 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(21.06 AS Decimal(5, 2)), 457, CAST(48.00 AS Decimal(8, 2)), CAST(7.50 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(520.00 AS Decimal(8, 2)), CAST(2190.00 AS Decimal(8, 2)), N'vaffle_svek.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (5, N'Кабачковая вафля с песто,индейкой и яйцом пашот', 2, 1, CAST(88.59 AS Decimal(10, 2)), CAST(406.83 AS Decimal(10, 2)), CAST(449.00 AS Decimal(10, 2)), CAST(19.73 AS Decimal(5, 2)), 476, CAST(22.00 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(29.00 AS Decimal(8, 2)), CAST(400.00 AS Decimal(8, 2)), CAST(1650.00 AS Decimal(8, 2)), N'vaffle_kabach.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (6, N'Наполеон VEG c ванильным кремом ', 12, 1, CAST(52.10 AS Decimal(10, 2)), CAST(435.51 AS Decimal(10, 2)), CAST(279.00 AS Decimal(10, 2)), CAST(18.67 AS Decimal(5, 2)), NULL, CAST(25.00 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(50.00 AS Decimal(8, 2)), CAST(450.00 AS Decimal(8, 2)), CAST(1870.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (7, N'Чиа пудинг Ваниль-лимон', 12, 1, CAST(73.01 AS Decimal(10, 2)), CAST(309.53 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(24.42 AS Decimal(5, 2)), NULL, CAST(21.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(26.00 AS Decimal(8, 2)), CAST(310.00 AS Decimal(8, 2)), CAST(1290.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (8, N'Чиа пудинг Йогурт - маракуйя', 3, 1, CAST(134.82 AS Decimal(10, 2)), CAST(181.12 AS Decimal(10, 2)), CAST(379.00 AS Decimal(10, 2)), CAST(35.57 AS Decimal(5, 2)), 594, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (9, N'Чиа пудинг Облепиха манго', 3, 1, CAST(85.69 AS Decimal(10, 2)), CAST(260.60 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(27.73 AS Decimal(5, 2)), 595, CAST(9.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(61.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1480.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (10, N'Чиа пудинг Сникерс', 3, 1, CAST(87.25 AS Decimal(10, 2)), CAST(311.46 AS Decimal(10, 2)), CAST(359.00 AS Decimal(10, 2)), CAST(24.30 AS Decimal(5, 2)), 596, CAST(31.00 AS Decimal(8, 2)), CAST(14.00 AS Decimal(8, 2)), CAST(37.00 AS Decimal(8, 2)), CAST(480.00 AS Decimal(8, 2)), CAST(2000.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (11, N'Healthy Оливье', 12, 1, CAST(40.53 AS Decimal(10, 2)), CAST(341.65 AS Decimal(10, 2)), CAST(179.00 AS Decimal(10, 2)), CAST(22.64 AS Decimal(5, 2)), NULL, CAST(17.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(7.00 AS Decimal(8, 2)), CAST(200.00 AS Decimal(8, 2)), CAST(820.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (12, N'Healthy Шуба', 12, 1, CAST(33.19 AS Decimal(10, 2)), CAST(409.19 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(19.64 AS Decimal(5, 2)), NULL, CAST(16.00 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(11.00 AS Decimal(8, 2)), CAST(190.00 AS Decimal(8, 2)), CAST(810.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (13, N'Борщ овощной', 12, 1, CAST(27.12 AS Decimal(10, 2)), CAST(1039.38 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(8.78 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (14, N'Борщ с щечками', 12, 1, CAST(59.93 AS Decimal(10, 2)), CAST(465.66 AS Decimal(10, 2)), CAST(339.00 AS Decimal(10, 2)), CAST(17.68 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (16, N'Буррито с индейкой и овощами', 4, 1, CAST(97.30 AS Decimal(10, 2)), CAST(340.90 AS Decimal(10, 2)), CAST(429.00 AS Decimal(10, 2)), CAST(22.68 AS Decimal(5, 2)), 451, CAST(22.00 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(37.00 AS Decimal(8, 2)), CAST(410.00 AS Decimal(8, 2)), CAST(1700.00 AS Decimal(8, 2)), N'burrito_turkey.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (17, N'Буррито с креветкой в соусе сладкий чили', 4, 1, CAST(116.17 AS Decimal(10, 2)), CAST(286.50 AS Decimal(10, 2)), CAST(449.00 AS Decimal(10, 2)), CAST(25.87 AS Decimal(5, 2)), 452, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), N'burrito_shrimp.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (18, N'Буррито с растительным фаршем', 12, 1, CAST(94.66 AS Decimal(10, 2)), CAST(342.64 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(22.59 AS Decimal(5, 2)), NULL, CAST(35.00 AS Decimal(8, 2)), CAST(6.00 AS Decimal(8, 2)), CAST(36.00 AS Decimal(8, 2)), CAST(480.00 AS Decimal(8, 2)), CAST(2010.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (19, N'Буррито Санрайз', 12, 1, CAST(77.34 AS Decimal(10, 2)), CAST(364.18 AS Decimal(10, 2)), CAST(359.00 AS Decimal(10, 2)), CAST(21.54 AS Decimal(5, 2)), NULL, CAST(19.00 AS Decimal(8, 2)), CAST(13.00 AS Decimal(8, 2)), CAST(47.00 AS Decimal(8, 2)), CAST(410.00 AS Decimal(8, 2)), CAST(1740.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (20, N'Гречневая соба с креветками', 4, 1, CAST(124.33 AS Decimal(10, 2)), CAST(341.57 AS Decimal(10, 2)), CAST(549.00 AS Decimal(10, 2)), CAST(22.65 AS Decimal(5, 2)), 466, CAST(37.00 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(42.00 AS Decimal(8, 2)), CAST(560.00 AS Decimal(8, 2)), CAST(2340.00 AS Decimal(8, 2)), N'soba.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (21, N'Кесадия 1/2 с растительным мясом ', 12, 1, CAST(65.37 AS Decimal(10, 2)), CAST(372.69 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(21.16 AS Decimal(5, 2)), NULL, CAST(8.50 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(24.00 AS Decimal(8, 2)), CAST(190.00 AS Decimal(8, 2)), CAST(800.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (22, N'Кесадия с индейкой', 12, 1, CAST(115.33 AS Decimal(10, 2)), CAST(376.03 AS Decimal(10, 2)), CAST(549.00 AS Decimal(10, 2)), CAST(21.01 AS Decimal(5, 2)), NULL, CAST(22.00 AS Decimal(8, 2)), CAST(19.00 AS Decimal(8, 2)), CAST(43.00 AS Decimal(8, 2)), CAST(440.00 AS Decimal(8, 2)), CAST(1850.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (23, N'Кесадия с растительным мясом томатами и сыром ', 12, 1, CAST(122.10 AS Decimal(10, 2)), CAST(308.68 AS Decimal(10, 2)), CAST(499.00 AS Decimal(10, 2)), CAST(24.47 AS Decimal(5, 2)), NULL, CAST(16.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(43.00 AS Decimal(8, 2)), CAST(360.00 AS Decimal(8, 2)), CAST(1500.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (24, N'Пряный томатный суп', 5, 1, CAST(56.02 AS Decimal(10, 2)), CAST(469.44 AS Decimal(10, 2)), CAST(319.00 AS Decimal(10, 2)), CAST(17.56 AS Decimal(5, 2)), 542, CAST(23.00 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(12.00 AS Decimal(8, 2)), CAST(280.00 AS Decimal(8, 2)), CAST(1180.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (25, N'Спагетти с морепродуктами в соусе том ям', 12, 1, CAST(147.69 AS Decimal(10, 2)), CAST(298.81 AS Decimal(10, 2)), CAST(589.00 AS Decimal(10, 2)), CAST(25.07 AS Decimal(5, 2)), NULL, CAST(12.00 AS Decimal(8, 2)), CAST(19.00 AS Decimal(8, 2)), CAST(38.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1450.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (26, N'Суп-пюре грибной', 5, 1, CAST(92.73 AS Decimal(10, 2)), CAST(265.58 AS Decimal(10, 2)), CAST(339.00 AS Decimal(10, 2)), CAST(27.35 AS Decimal(5, 2)), 560, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), N'soup_mushroom.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (27, N'Томленные щёчки с овощами', 12, 1, CAST(148.25 AS Decimal(10, 2)), CAST(304.05 AS Decimal(10, 2)), CAST(599.00 AS Decimal(10, 2)), CAST(24.75 AS Decimal(5, 2)), NULL, CAST(13.00 AS Decimal(8, 2)), CAST(11.00 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(240.00 AS Decimal(8, 2)), CAST(980.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (28, N'Фетучини с томатным соусом растительным фаршем, овощами и салатом', 4, 1, CAST(100.40 AS Decimal(10, 2)), CAST(317.33 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(23.96 AS Decimal(5, 2)), 576, CAST(28.00 AS Decimal(8, 2)), CAST(7.00 AS Decimal(8, 2)), CAST(34.00 AS Decimal(8, 2)), CAST(430.00 AS Decimal(8, 2)), CAST(1820.00 AS Decimal(8, 2)), N'fetuch.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (29, N'Арахисовое печенье', 6, 1, CAST(29.91 AS Decimal(10, 2)), CAST(331.29 AS Decimal(10, 2)), CAST(129.00 AS Decimal(10, 2)), CAST(23.19 AS Decimal(5, 2)), 438, CAST(17.00 AS Decimal(8, 2)), CAST(9.50 AS Decimal(8, 2)), CAST(18.00 AS Decimal(8, 2)), CAST(260.00 AS Decimal(8, 2)), CAST(1090.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (30, N'Баунти', 12, 1, CAST(37.43 AS Decimal(10, 2)), CAST(351.51 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(22.15 AS Decimal(5, 2)), NULL, CAST(19.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(9.50 AS Decimal(8, 2)), CAST(220.00 AS Decimal(8, 2)), CAST(920.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (31, N'Большой кукис с шоколадом', 12, 1, CAST(45.39 AS Decimal(10, 2)), CAST(382.49 AS Decimal(10, 2)), CAST(219.00 AS Decimal(10, 2)), CAST(20.73 AS Decimal(5, 2)), NULL, CAST(19.00 AS Decimal(8, 2)), CAST(4.00 AS Decimal(8, 2)), CAST(35.00 AS Decimal(8, 2)), CAST(320.00 AS Decimal(8, 2)), CAST(1350.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (33, N'Имбирное печенье 1шт', 12, 1, CAST(26.43 AS Decimal(10, 2)), CAST(350.25 AS Decimal(10, 2)), CAST(119.00 AS Decimal(10, 2)), CAST(22.21 AS Decimal(5, 2)), NULL, CAST(12.00 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(18.00 AS Decimal(8, 2)), CAST(200.00 AS Decimal(8, 2)), CAST(830.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (34, N'Конфета Garden Ferrero', 12, 1, CAST(37.50 AS Decimal(10, 2)), CAST(270.67 AS Decimal(10, 2)), CAST(139.00 AS Decimal(10, 2)), CAST(26.98 AS Decimal(5, 2)), NULL, CAST(10.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(8.50 AS Decimal(8, 2)), CAST(140.00 AS Decimal(8, 2)), CAST(590.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (35, N'Конфета Garden green', 12, 1, CAST(14.01 AS Decimal(10, 2)), CAST(606.64 AS Decimal(10, 2)), CAST(99.00 AS Decimal(10, 2)), CAST(14.15 AS Decimal(5, 2)), NULL, CAST(3.50 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(9.50 AS Decimal(8, 2)), CAST(75.00 AS Decimal(8, 2)), CAST(310.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (36, N'Конфета Манго', 12, 1, CAST(16.97 AS Decimal(10, 2)), CAST(483.38 AS Decimal(10, 2)), CAST(99.00 AS Decimal(10, 2)), CAST(17.14 AS Decimal(5, 2)), NULL, CAST(6.00 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(11.00 AS Decimal(8, 2)), CAST(110.00 AS Decimal(8, 2)), CAST(440.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (37, N'Медовик', 12, 1, CAST(57.60 AS Decimal(10, 2)), CAST(419.10 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(19.26 AS Decimal(5, 2)), NULL, CAST(19.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(61.00 AS Decimal(8, 2)), CAST(430.00 AS Decimal(8, 2)), CAST(1810.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (38, N'Морковный пирог', 12, 1, CAST(42.38 AS Decimal(10, 2)), CAST(605.52 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(14.17 AS Decimal(5, 2)), NULL, CAST(8.00 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), CAST(720.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (39, N'Муравейник с грецким орехом ', 12, 1, CAST(22.64 AS Decimal(10, 2)), CAST(469.79 AS Decimal(10, 2)), CAST(129.00 AS Decimal(10, 2)), CAST(17.55 AS Decimal(5, 2)), NULL, CAST(9.00 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(150.00 AS Decimal(8, 2)), CAST(620.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (40, N'Овсяное печенье ', 12, 1, CAST(15.06 AS Decimal(10, 2)), CAST(623.77 AS Decimal(10, 2)), CAST(109.00 AS Decimal(10, 2)), CAST(13.82 AS Decimal(5, 2)), NULL, CAST(8.50 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), CAST(700.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (42, N'Печенье Бискоти', 12, 1, CAST(15.40 AS Decimal(10, 2)), CAST(737.66 AS Decimal(10, 2)), CAST(129.00 AS Decimal(10, 2)), CAST(11.94 AS Decimal(5, 2)), NULL, CAST(6.00 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(85.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (43, N'Пирожное Картошка', 6, 1, CAST(47.67 AS Decimal(10, 2)), CAST(233.54 AS Decimal(10, 2)), CAST(159.00 AS Decimal(10, 2)), CAST(29.98 AS Decimal(5, 2)), 541, CAST(12.00 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(29.00 AS Decimal(8, 2)), CAST(250.00 AS Decimal(8, 2)), CAST(1040.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (44, N'Птичье молоко ', 6, 1, CAST(14.74 AS Decimal(10, 2)), CAST(571.64 AS Decimal(10, 2)), CAST(99.00 AS Decimal(10, 2)), CAST(14.89 AS Decimal(5, 2)), 543, CAST(2.50 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(65.00 AS Decimal(8, 2)), CAST(270.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (45, N'Творожный кешью сырок с фундучным пралине', 12, 1, CAST(38.88 AS Decimal(10, 2)), CAST(360.39 AS Decimal(10, 2)), CAST(179.00 AS Decimal(10, 2)), CAST(21.72 AS Decimal(5, 2)), NULL, CAST(49.00 AS Decimal(8, 2)), CAST(19.00 AS Decimal(8, 2)), CAST(23.00 AS Decimal(8, 2)), CAST(600.00 AS Decimal(8, 2)), CAST(2520.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (46, N'Трубочка безглютеновая с кокосовой  сгущенкой', 12, 1, CAST(58.49 AS Decimal(10, 2)), CAST(377.00 AS Decimal(10, 2)), CAST(279.00 AS Decimal(10, 2)), CAST(20.96 AS Decimal(5, 2)), NULL, CAST(17.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(250.00 AS Decimal(8, 2)), CAST(1050.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (48, N'Чизкейк Сан Себастьян', 12, 1, CAST(97.61 AS Decimal(10, 2)), CAST(308.77 AS Decimal(10, 2)), CAST(399.00 AS Decimal(10, 2)), CAST(24.46 AS Decimal(5, 2)), NULL, CAST(16.00 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(26.00 AS Decimal(8, 2)), CAST(260.00 AS Decimal(8, 2)), CAST(1080.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (49, N'Шоколадно-банановый трайфл с малиной', 12, 1, CAST(91.36 AS Decimal(10, 2)), CAST(216.33 AS Decimal(10, 2)), CAST(289.00 AS Decimal(10, 2)), CAST(31.61 AS Decimal(5, 2)), NULL, CAST(25.00 AS Decimal(8, 2)), CAST(8.00 AS Decimal(8, 2)), CAST(41.00 AS Decimal(8, 2)), CAST(420.00 AS Decimal(8, 2)), CAST(1750.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (50, N'Шоколадное печенье с апельсином', 12, 1, CAST(28.18 AS Decimal(10, 2)), CAST(428.74 AS Decimal(10, 2)), CAST(149.00 AS Decimal(10, 2)), CAST(18.91 AS Decimal(5, 2)), NULL, CAST(9.00 AS Decimal(8, 2)), CAST(2.50 AS Decimal(8, 2)), CAST(28.00 AS Decimal(8, 2)), CAST(200.00 AS Decimal(8, 2)), CAST(860.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (51, N'Шоколадный тарт со смородиной', 12, 1, CAST(85.49 AS Decimal(10, 2)), CAST(308.23 AS Decimal(10, 2)), CAST(349.00 AS Decimal(10, 2)), CAST(24.50 AS Decimal(5, 2)), NULL, CAST(22.00 AS Decimal(8, 2)), CAST(4.00 AS Decimal(8, 2)), CAST(23.00 AS Decimal(8, 2)), CAST(310.00 AS Decimal(8, 2)), CAST(1280.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (52, N'Яблочный пирог', 12, 1, CAST(43.05 AS Decimal(10, 2)), CAST(548.08 AS Decimal(10, 2)), CAST(279.00 AS Decimal(10, 2)), CAST(15.43 AS Decimal(5, 2)), NULL, CAST(7.00 AS Decimal(8, 2)), CAST(0.30 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(160.00 AS Decimal(8, 2)), CAST(650.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (53, N'Брускетта с креветкой,авокадо кремом и яйцом пашот', 12, 1, CAST(89.10 AS Decimal(10, 2)), CAST(291.69 AS Decimal(10, 2)), CAST(349.00 AS Decimal(10, 2)), CAST(25.53 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (54, N'Брускетта с ростбифом и кешью сыром ', 12, 1, CAST(62.43 AS Decimal(10, 2)), CAST(378.94 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(20.88 AS Decimal(5, 2)), NULL, CAST(15.00 AS Decimal(8, 2)), CAST(6.00 AS Decimal(8, 2)), CAST(23.00 AS Decimal(8, 2)), CAST(250.00 AS Decimal(8, 2)), CAST(1050.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (56, N'Брускетта с щёчками', 12, 1, CAST(67.09 AS Decimal(10, 2)), CAST(345.67 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(22.44 AS Decimal(5, 2)), NULL, CAST(12.00 AS Decimal(8, 2)), CAST(7.50 AS Decimal(8, 2)), CAST(24.00 AS Decimal(8, 2)), CAST(240.00 AS Decimal(8, 2)), CAST(1000.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (57, N'Завтрак Скрембл Конструктор', 7, 1, CAST(75.77 AS Decimal(10, 2)), CAST(492.58 AS Decimal(10, 2)), CAST(449.00 AS Decimal(10, 2)), CAST(16.88 AS Decimal(5, 2)), 468, CAST(40.00 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(440.00 AS Decimal(8, 2)), CAST(1830.00 AS Decimal(8, 2)), N'skrembl.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (58, N'Завтрак Яичница Конструктор', 12, 1, CAST(50.25 AS Decimal(10, 2)), CAST(733.83 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(11.99 AS Decimal(5, 2)), NULL, CAST(38.00 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(2.50 AS Decimal(8, 2)), CAST(420.00 AS Decimal(8, 2)), CAST(1750.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (59, N'Зеленая гречка с вялеными томатами, гуакомоле и яйцом пашот', 12, 1, CAST(82.71 AS Decimal(10, 2)), CAST(297.78 AS Decimal(10, 2)), CAST(329.00 AS Decimal(10, 2)), CAST(25.14 AS Decimal(5, 2)), NULL, CAST(15.00 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(39.00 AS Decimal(8, 2)), CAST(420.00 AS Decimal(8, 2)), CAST(1740.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (60, N'Каша из киноа на кокосовом молоке с вареньем', 12, 1, CAST(78.01 AS Decimal(10, 2)), CAST(283.28 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(26.09 AS Decimal(5, 2)), NULL, CAST(18.00 AS Decimal(8, 2)), CAST(8.50 AS Decimal(8, 2)), CAST(39.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1460.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (61, N'Каша кукурузная с кокосовым йогуртом и мармеладом', 12, 1, CAST(65.00 AS Decimal(10, 2)), CAST(360.00 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(21.74 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (62, N'Каша овсяная', 7, 1, CAST(15.96 AS Decimal(10, 2)), CAST(833.58 AS Decimal(10, 2)), CAST(149.00 AS Decimal(10, 2)), CAST(10.71 AS Decimal(5, 2)), 486, CAST(2.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), CAST(140.00 AS Decimal(8, 2)), CAST(570.00 AS Decimal(8, 2)), N'porrige_oat.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (63, N'Круассан с индейкой с горчично сырным соусом', 12, 1, CAST(145.04 AS Decimal(10, 2)), CAST(306.09 AS Decimal(10, 2)), CAST(589.00 AS Decimal(10, 2)), CAST(24.62 AS Decimal(5, 2)), NULL, CAST(6.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(160.00 AS Decimal(8, 2)), CAST(680.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (64, N'Круассан с ростбифом и творожным кремом', 12, 1, CAST(160.72 AS Decimal(10, 2)), CAST(272.70 AS Decimal(10, 2)), CAST(599.00 AS Decimal(10, 2)), CAST(26.83 AS Decimal(5, 2)), NULL, CAST(19.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(28.00 AS Decimal(8, 2)), CAST(370.00 AS Decimal(8, 2)), CAST(1550.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (65, N'Нутовый омлет с творожным кремом и вешенками', 12, 1, CAST(101.19 AS Decimal(10, 2)), CAST(294.31 AS Decimal(10, 2)), CAST(399.00 AS Decimal(10, 2)), CAST(25.36 AS Decimal(5, 2)), NULL, CAST(26.00 AS Decimal(8, 2)), CAST(17.00 AS Decimal(8, 2)), CAST(31.00 AS Decimal(8, 2)), CAST(430.00 AS Decimal(8, 2)), CAST(1780.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (66, N'Овсянка с индейкой и пармезаном', 12, 1, CAST(104.68 AS Decimal(10, 2)), CAST(338.48 AS Decimal(10, 2)), CAST(459.00 AS Decimal(10, 2)), CAST(22.81 AS Decimal(5, 2)), NULL, CAST(18.00 AS Decimal(8, 2)), CAST(27.00 AS Decimal(8, 2)), CAST(36.00 AS Decimal(8, 2)), CAST(420.00 AS Decimal(8, 2)), CAST(1760.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (67, N'Пшенная каша с тыквой и облепиховым мусом', 7, 1, CAST(56.10 AS Decimal(10, 2)), CAST(343.85 AS Decimal(10, 2)), CAST(249.00 AS Decimal(10, 2)), CAST(22.53 AS Decimal(5, 2)), 545, CAST(11.00 AS Decimal(8, 2)), CAST(6.50 AS Decimal(8, 2)), CAST(48.00 AS Decimal(8, 2)), CAST(320.00 AS Decimal(8, 2)), CAST(1320.00 AS Decimal(8, 2)), N'porrige_pump.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (68, N'Свекольный хумус', 12, 1, CAST(64.83 AS Decimal(10, 2)), CAST(237.81 AS Decimal(10, 2)), CAST(219.00 AS Decimal(10, 2)), CAST(29.60 AS Decimal(5, 2)), NULL, CAST(27.00 AS Decimal(8, 2)), CAST(8.00 AS Decimal(8, 2)), CAST(42.00 AS Decimal(8, 2)), CAST(440.00 AS Decimal(8, 2)), CAST(1850.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (69, N'Сырники с черникой и кешью сметаной', 12, 1, CAST(92.87 AS Decimal(10, 2)), CAST(351.17 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(22.16 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (71, N'Сырники со сгущенкой и манговым кремом', 7, 1, CAST(108.71 AS Decimal(10, 2)), CAST(285.43 AS Decimal(10, 2)), CAST(419.00 AS Decimal(10, 2)), CAST(25.95 AS Decimal(5, 2)), 563, CAST(16.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(47.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1480.00 AS Decimal(8, 2)), N'syrniki.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (72, N'Сэндвич на бриоши с ростбифом', 12, 1, CAST(145.03 AS Decimal(10, 2)), CAST(264.75 AS Decimal(10, 2)), CAST(529.00 AS Decimal(10, 2)), CAST(27.42 AS Decimal(5, 2)), NULL, CAST(6.00 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(29.00 AS Decimal(8, 2)), CAST(180.00 AS Decimal(8, 2)), CAST(760.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (73, N'Сэндвич с ветчиной на бриоши ', 12, 1, CAST(103.25 AS Decimal(10, 2)), CAST(383.29 AS Decimal(10, 2)), CAST(499.00 AS Decimal(10, 2)), CAST(20.69 AS Decimal(5, 2)), NULL, CAST(24.00 AS Decimal(8, 2)), CAST(7.00 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(330.00 AS Decimal(8, 2)), CAST(1360.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (75, N'Шакшука с томатами в собственном соку', 12, 1, CAST(86.15 AS Decimal(10, 2)), CAST(421.18 AS Decimal(10, 2)), CAST(449.00 AS Decimal(10, 2)), CAST(19.19 AS Decimal(5, 2)), NULL, CAST(10.00 AS Decimal(8, 2)), CAST(14.00 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(220.00 AS Decimal(8, 2)), CAST(900.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (76, N'Зеленый салат VEGAN', 9, 1, CAST(78.53 AS Decimal(10, 2)), CAST(293.48 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(25.41 AS Decimal(5, 2)), 471, CAST(20.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(4.00 AS Decimal(8, 2)), CAST(210.00 AS Decimal(8, 2)), CAST(890.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (77, N'Зеленый салат с индейкой', 12, 1, CAST(99.62 AS Decimal(10, 2)), CAST(270.41 AS Decimal(10, 2)), CAST(369.00 AS Decimal(10, 2)), CAST(27.00 AS Decimal(5, 2)), NULL, CAST(23.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(4.00 AS Decimal(8, 2)), CAST(260.00 AS Decimal(8, 2)), CAST(1100.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (78, N'Салат Veg c тыквой и киноа ', 9, 1, CAST(59.94 AS Decimal(10, 2)), CAST(415.52 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(19.40 AS Decimal(5, 2)), 554, CAST(11.00 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(190.00 AS Decimal(8, 2)), CAST(810.00 AS Decimal(8, 2)), N'salat_pump.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (79, N'Салат с креветками и тыквой и киноа ', 12, 1, CAST(110.72 AS Decimal(10, 2)), CAST(260.37 AS Decimal(10, 2)), CAST(399.00 AS Decimal(10, 2)), CAST(27.75 AS Decimal(5, 2)), NULL, CAST(11.00 AS Decimal(8, 2)), CAST(8.50 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(220.00 AS Decimal(8, 2)), CAST(940.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (80, N'Салат с ростбифом и картофелем', 9, 1, CAST(106.07 AS Decimal(10, 2)), CAST(285.59 AS Decimal(10, 2)), CAST(409.00 AS Decimal(10, 2)), CAST(25.93 AS Decimal(5, 2)), 556, CAST(22.00 AS Decimal(8, 2)), CAST(4.50 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(280.00 AS Decimal(8, 2)), CAST(1150.00 AS Decimal(8, 2)), N'salat_rost.png', 0)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (81, N'Печеночный паштет с чиабаттой,смородиновым мармеладом', 12, 1, CAST(90.99 AS Decimal(10, 2)), CAST(338.51 AS Decimal(10, 2)), CAST(399.00 AS Decimal(10, 2)), CAST(22.80 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (82, N'Медовик (целый)', 12, 1, CAST(616.19 AS Decimal(10, 2)), CAST(378.59 AS Decimal(10, 2)), CAST(2949.00 AS Decimal(10, 2)), CAST(20.89 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Dishes] ([Id], [Name], [CategoryId], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (84, N'Тарт шоколадный со смородиной (целый)', 8, 1, CAST(593.54 AS Decimal(10, 2)), CAST(329.46 AS Decimal(10, 2)), CAST(2549.00 AS Decimal(10, 2)), CAST(23.29 AS Decimal(5, 2)), 566, CAST(116.00 AS Decimal(8, 2)), CAST(18.00 AS Decimal(8, 2)), CAST(163.00 AS Decimal(8, 2)), CAST(1770.00 AS Decimal(8, 2)), CAST(7390.00 AS Decimal(8, 2)), NULL, 1)
SET IDENTITY_INSERT [dbo].[Dishes] OFF
GO
SET IDENTITY_INSERT [dbo].[DishToppings] ON 

INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (5, 22, 21, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (6, 22, 22, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (7, 14, 23, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (8, 19, 23, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (9, 19, 25, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (10, 19, 28, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (11, 22, 28, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (12, 22, 32, CAST(3.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (13, 6, 33, CAST(4.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (14, 11, 33, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (15, 14, 33, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (16, 19, 33, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (17, 19, 34, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (18, 14, 38, CAST(3.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (19, 14, 39, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (20, 19, 39, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (40, 4, 59, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (41, 19, 61, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (42, 22, 63, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (43, 14, 66, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (44, 22, 77, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (45, 22, 79, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (46, 4, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (47, 6, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (48, 7, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (49, 11, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (50, 14, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (51, 19, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (52, 22, 86, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (53, 11, 87, CAST(4.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (54, 4, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (55, 6, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (56, 7, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (57, 11, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (58, 14, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (59, 19, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (60, 22, 89, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (61, 19, 90, CAST(3.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (62, 14, 91, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (63, 22, 91, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (64, 4, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (65, 6, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (66, 7, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (67, 11, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (68, 14, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (69, 19, 95, CAST(2.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (70, 22, 95, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (71, 19, 97, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (72, 22, 97, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (73, 4, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (74, 6, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (75, 7, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (76, 11, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (77, 14, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (78, 22, 98, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (79, 4, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (80, 6, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (81, 7, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (82, 11, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (83, 14, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (84, 19, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (85, 22, 101, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (86, 4, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (87, 6, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (88, 7, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (89, 11, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (90, 14, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (91, 19, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (92, 22, 102, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (93, 7, 104, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (94, 11, 104, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (95, 14, 104, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (96, 19, 104, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (97, 4, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (98, 6, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (99, 7, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (100, 11, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (101, 14, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (102, 19, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (103, 22, 107, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (104, 4, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (105, 6, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (106, 7, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (107, 11, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (108, 14, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (109, 19, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (110, 22, 108, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (111, 4, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (112, 6, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (113, 7, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (114, 11, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (115, 14, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (116, 19, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (117, 22, 114, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (118, 4, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (119, 6, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (120, 7, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (121, 11, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (122, 14, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (123, 19, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (124, 22, 115, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (125, 19, 116, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (126, 22, 116, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (127, 22, 124, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (128, 11, 125, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (129, 14, 125, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (130, 22, 128, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (131, 11, 129, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (132, 19, 129, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (133, 19, 131, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (134, 22, 131, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (135, 19, 133, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (136, 22, 133, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (137, 14, 137, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (138, 22, 137, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (139, 11, 138, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (140, 11, 139, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (141, 7, 142, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (142, 22, 152, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (158, 6, 182, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DishToppings] ([Id], [ToppingId], [OrderDishItemId], [Quantity], [FinalPrice]) VALUES (159, 19, 183, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[DishToppings] OFF
GO
SET IDENTITY_INSERT [dbo].[DrinkCategories] ON 

INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (1, N'Кофе')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (2, N'Не кофе')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (3, N'Фреши')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (4, N'Чаи')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (5, N'Смузи')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (6, N'Лимонады')
INSERT [dbo].[DrinkCategories] ([Id], [Name]) VALUES (7, N'Неактивные')
SET IDENTITY_INSERT [dbo].[DrinkCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Drinks] ON 

INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (1, N'Айс Латте солёная карамель', CAST(350.00 AS Decimal(8, 2)), 3, 1, CAST(17.01 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 429, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), N'ice_latte.png', 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (2, N'Айс Латте Фундук ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(36.71 AS Decimal(10, 2)), CAST(551.05 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(15.36 AS Decimal(5, 2)), NULL, CAST(4.00 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(33.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), CAST(720.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (3, N'Айс Латте Черника Тимьян', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(52.78 AS Decimal(10, 2)), CAST(352.82 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(22.08 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (4, N'Айс Матча ваниль ', CAST(300.00 AS Decimal(8, 2)), 3, 7, CAST(48.58 AS Decimal(10, 2)), CAST(391.97 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(20.33 AS Decimal(5, 2)), NULL, CAST(3.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), CAST(130.00 AS Decimal(8, 2)), CAST(550.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (5, N'Американо   ', CAST(150.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(149.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 434, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (6, N'Американо   ', CAST(250.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(199.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (7, N'Апельсиновый Какао  ', CAST(350.00 AS Decimal(8, 2)), 3, 2, CAST(54.38 AS Decimal(10, 2)), CAST(265.94 AS Decimal(10, 2)), CAST(199.00 AS Decimal(10, 2)), CAST(27.33 AS Decimal(5, 2)), 435, CAST(5.00 AS Decimal(8, 2)), CAST(7.50 AS Decimal(8, 2)), CAST(52.00 AS Decimal(8, 2)), CAST(280.00 AS Decimal(8, 2)), CAST(1180.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (8, N'Бамбл   ', CAST(310.00 AS Decimal(8, 2)), 3, 7, CAST(103.13 AS Decimal(10, 2)), CAST(277.19 AS Decimal(10, 2)), CAST(389.00 AS Decimal(10, 2)), CAST(26.51 AS Decimal(5, 2)), NULL, CAST(0.40 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(26.00 AS Decimal(8, 2)), CAST(120.00 AS Decimal(8, 2)), CAST(480.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (9, N'Банановый раф  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(27.46 AS Decimal(10, 2)), CAST(770.36 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(11.49 AS Decimal(5, 2)), NULL, CAST(5.00 AS Decimal(8, 2)), CAST(2.50 AS Decimal(8, 2)), CAST(22.00 AS Decimal(8, 2)), CAST(150.00 AS Decimal(8, 2)), CAST(620.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (10, N'Ванильный раф  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(12.05 AS Decimal(10, 2)), CAST(1883.40 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(5.04 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(14.00 AS Decimal(8, 2)), CAST(55.00 AS Decimal(8, 2)), CAST(240.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (11, N'Ванильный раф  ', CAST(450.00 AS Decimal(8, 2)), 3, 7, CAST(13.42 AS Decimal(10, 2)), CAST(1680.92 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(5.62 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (12, N'Двойной эспрессо  ', CAST(60.00 AS Decimal(8, 2)), 3, 7, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(119.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (13, N'Какао   ', CAST(250.00 AS Decimal(8, 2)), 3, 2, CAST(9.92 AS Decimal(10, 2)), CAST(1502.82 AS Decimal(10, 2)), CAST(159.00 AS Decimal(10, 2)), CAST(6.24 AS Decimal(5, 2)), 478, CAST(1.00 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(30.00 AS Decimal(8, 2)), CAST(120.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (14, N'Какао   ', CAST(350.00 AS Decimal(8, 2)), 3, 2, CAST(14.17 AS Decimal(10, 2)), CAST(1163.23 AS Decimal(10, 2)), CAST(179.00 AS Decimal(10, 2)), CAST(7.92 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (15, N'Какао Нутелла  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(32.24 AS Decimal(10, 2)), CAST(579.28 AS Decimal(10, 2)), CAST(219.00 AS Decimal(10, 2)), CAST(14.72 AS Decimal(5, 2)), NULL, CAST(14.00 AS Decimal(8, 2)), CAST(9.50 AS Decimal(8, 2)), CAST(18.00 AS Decimal(8, 2)), CAST(240.00 AS Decimal(8, 2)), CAST(1000.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (16, N'Капучино   ', CAST(250.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(179.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 482, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), N'kapuch.png', 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (17, N'Капучино   ', CAST(350.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(199.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, N'kapuch.png', 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (18, N'Капучино   ', CAST(450.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(219.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, N'kapuch.png', 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (19, N'Кедровый какао  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(31.01 AS Decimal(10, 2)), CAST(541.73 AS Decimal(10, 2)), CAST(199.00 AS Decimal(10, 2)), CAST(15.58 AS Decimal(5, 2)), NULL, CAST(1.00 AS Decimal(8, 2)), CAST(2.50 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(40.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (20, N'Кедровый латте  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(21.89 AS Decimal(10, 2)), CAST(991.82 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(9.16 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (21, N'Куркума латте  ', CAST(250.00 AS Decimal(8, 2)), 3, 7, CAST(1.74 AS Decimal(10, 2)), CAST(7888.51 AS Decimal(10, 2)), CAST(139.00 AS Decimal(10, 2)), CAST(1.25 AS Decimal(5, 2)), NULL, CAST(0.50 AS Decimal(8, 2)), CAST(0.30 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(45.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (22, N'Куркума латте  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(2.28 AS Decimal(10, 2)), CAST(7312.28 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(1.35 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (23, N'Кэроб   ', CAST(250.00 AS Decimal(8, 2)), 3, 7, CAST(4.52 AS Decimal(10, 2)), CAST(2975.22 AS Decimal(10, 2)), CAST(139.00 AS Decimal(10, 2)), CAST(3.25 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (24, N'Кэроб   ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(7.23 AS Decimal(10, 2)), CAST(2237.48 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(4.28 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (25, N'Латте   ', CAST(250.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(179.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 518, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (26, N'Латте   ', CAST(350.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(219.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (27, N'Латте   ', CAST(450.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(279.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (28, N'Ореховый раф  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(25.42 AS Decimal(10, 2)), CAST(840.20 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(10.64 AS Decimal(5, 2)), NULL, CAST(13.00 AS Decimal(8, 2)), CAST(7.50 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(210.00 AS Decimal(8, 2)), CAST(880.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (29, N'Пуровер   ', CAST(300.00 AS Decimal(8, 2)), 3, 7, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (30, N'Раф Апельсин в шоколаде', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(20.44 AS Decimal(10, 2)), CAST(1069.28 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(8.55 AS Decimal(5, 2)), NULL, CAST(0.50 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), CAST(110.00 AS Decimal(8, 2)), CAST(470.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (31, N'Раф Апельсин в шоколаде', CAST(450.00 AS Decimal(8, 2)), 3, 7, CAST(23.44 AS Decimal(10, 2)), CAST(919.62 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(9.81 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (32, N'Раф Малина под Шубой', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(40.41 AS Decimal(10, 2)), CAST(639.92 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(13.52 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (33, N'Раф Соленая карамель ', CAST(350.00 AS Decimal(8, 2)), 3, 1, CAST(17.01 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(269.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 550, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (34, N'Раф Соленая карамель ', CAST(450.00 AS Decimal(8, 2)), 3, 7, CAST(19.85 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(319.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (35, N'Раф Соленый арахис ', CAST(350.00 AS Decimal(8, 2)), 3, 1, CAST(19.35 AS Decimal(10, 2)), CAST(1135.14 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(8.10 AS Decimal(5, 2)), 551, CAST(11.00 AS Decimal(8, 2)), CAST(7.00 AS Decimal(8, 2)), CAST(18.00 AS Decimal(8, 2)), CAST(200.00 AS Decimal(8, 2)), CAST(840.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (36, N'Тыквенный Латте  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(16.23 AS Decimal(10, 2)), CAST(1495.81 AS Decimal(10, 2)), CAST(259.00 AS Decimal(10, 2)), CAST(6.27 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (37, N'Фильтр кофе  ', CAST(180.00 AS Decimal(8, 2)), 3, 7, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(159.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (38, N'Фильтр кофе  ', CAST(300.00 AS Decimal(8, 2)), 3, 7, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(199.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (39, N'Фильтр черника лимон ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(34.30 AS Decimal(10, 2)), CAST(596.79 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(14.35 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (40, N'Флэт уайт  ', CAST(250.00 AS Decimal(8, 2)), 3, 1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(189.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), 580, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (41, N'Цикорий латте  ', CAST(250.00 AS Decimal(8, 2)), 3, 7, CAST(6.31 AS Decimal(10, 2)), CAST(2102.85 AS Decimal(10, 2)), CAST(139.00 AS Decimal(10, 2)), CAST(4.54 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (42, N'Цикорий латте  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(10.10 AS Decimal(10, 2)), CAST(1573.27 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(5.98 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (43, N'Эспрессо   ', CAST(18.00 AS Decimal(8, 2)), 3, 7, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (44, N'Апельсиновый фреш  ', CAST(200.00 AS Decimal(8, 2)), 3, 3, CAST(98.53 AS Decimal(10, 2)), CAST(142.57 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(41.23 AS Decimal(5, 2)), 437, CAST(0.30 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(12.00 AS Decimal(8, 2)), CAST(55.00 AS Decimal(8, 2)), CAST(230.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (45, N'Апельсиновый фреш  ', CAST(400.00 AS Decimal(8, 2)), 3, 3, CAST(197.05 AS Decimal(10, 2)), CAST(117.71 AS Decimal(10, 2)), CAST(429.00 AS Decimal(10, 2)), CAST(45.93 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (46, N'Грейпфрутовый фреш  ', CAST(200.00 AS Decimal(8, 2)), 3, 7, CAST(94.61 AS Decimal(10, 2)), CAST(226.60 AS Decimal(10, 2)), CAST(309.00 AS Decimal(10, 2)), CAST(30.62 AS Decimal(5, 2)), NULL, CAST(0.30 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(90.00 AS Decimal(8, 2)), CAST(370.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (47, N'Грейпфрутовый фреш  ', CAST(400.00 AS Decimal(8, 2)), 3, 7, CAST(189.23 AS Decimal(10, 2)), CAST(200.69 AS Decimal(10, 2)), CAST(569.00 AS Decimal(10, 2)), CAST(33.26 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (48, N'Кедровый смузи  ', CAST(300.00 AS Decimal(8, 2)), 3, 7, CAST(50.52 AS Decimal(10, 2)), CAST(630.40 AS Decimal(10, 2)), CAST(369.00 AS Decimal(10, 2)), CAST(13.69 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (49, N'Морковный фреш  ', CAST(200.00 AS Decimal(8, 2)), 3, 3, CAST(35.20 AS Decimal(10, 2)), CAST(493.75 AS Decimal(10, 2)), CAST(209.00 AS Decimal(10, 2)), CAST(16.84 AS Decimal(5, 2)), 528, CAST(0.50 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(19.00 AS Decimal(8, 2)), CAST(90.00 AS Decimal(8, 2)), CAST(380.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (50, N'Морковный фреш  ', CAST(400.00 AS Decimal(8, 2)), 3, 3, CAST(58.67 AS Decimal(10, 2)), CAST(545.99 AS Decimal(10, 2)), CAST(379.00 AS Decimal(10, 2)), CAST(15.48 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (51, N'Яблочный фреш  ', CAST(200.00 AS Decimal(8, 2)), 3, 3, CAST(74.45 AS Decimal(10, 2)), CAST(247.88 AS Decimal(10, 2)), CAST(259.00 AS Decimal(10, 2)), CAST(28.75 AS Decimal(5, 2)), 611, CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(90.00 AS Decimal(8, 2)), CAST(370.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (52, N'Яблочный фреш  ', CAST(400.00 AS Decimal(8, 2)), 3, 3, CAST(149.48 AS Decimal(10, 2)), CAST(153.55 AS Decimal(10, 2)), CAST(379.00 AS Decimal(10, 2)), CAST(39.44 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (53, N'Голубая матча  ', CAST(250.00 AS Decimal(8, 2)), 3, 7, CAST(9.78 AS Decimal(10, 2)), CAST(1832.52 AS Decimal(10, 2)), CAST(189.00 AS Decimal(10, 2)), CAST(5.17 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (54, N'Голубая матча  ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(14.67 AS Decimal(10, 2)), CAST(1324.68 AS Decimal(10, 2)), CAST(209.00 AS Decimal(10, 2)), CAST(7.02 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (55, N'Иван чай  ', CAST(500.00 AS Decimal(8, 2)), 3, 7, CAST(5.84 AS Decimal(10, 2)), CAST(2793.84 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(3.46 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (56, N'Масала   ', CAST(370.00 AS Decimal(8, 2)), 3, 7, CAST(30.62 AS Decimal(10, 2)), CAST(778.51 AS Decimal(10, 2)), CAST(269.00 AS Decimal(10, 2)), CAST(11.38 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (57, N'Матча латте  ', CAST(250.00 AS Decimal(8, 2)), 3, 7, CAST(9.32 AS Decimal(10, 2)), CAST(1927.90 AS Decimal(10, 2)), CAST(189.00 AS Decimal(10, 2)), CAST(4.93 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), N'matcha_latte.png', 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (58, N'Матча латте  ', CAST(350.00 AS Decimal(8, 2)), 3, 4, CAST(13.98 AS Decimal(10, 2)), CAST(1394.99 AS Decimal(10, 2)), CAST(209.00 AS Decimal(10, 2)), CAST(6.69 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, N'matcha_latte.png', 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (59, N'Матча латте гранат ', CAST(350.00 AS Decimal(8, 2)), 3, 7, CAST(30.04 AS Decimal(10, 2)), CAST(662.32 AS Decimal(10, 2)), CAST(229.00 AS Decimal(10, 2)), CAST(13.12 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), CAST(40.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (60, N'Облепиха манго чай ', CAST(370.00 AS Decimal(8, 2)), 3, 7, CAST(31.85 AS Decimal(10, 2)), CAST(650.39 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(13.33 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (61, N'Смородина- Ежевика Чай ', CAST(370.00 AS Decimal(8, 2)), 3, 7, CAST(30.41 AS Decimal(10, 2)), CAST(587.27 AS Decimal(10, 2)), CAST(209.00 AS Decimal(10, 2)), CAST(14.55 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (62, N'Хвойный Чай  ', CAST(370.00 AS Decimal(8, 2)), 3, 7, CAST(31.52 AS Decimal(10, 2)), CAST(563.07 AS Decimal(10, 2)), CAST(209.00 AS Decimal(10, 2)), CAST(15.08 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (63, N'Церемониал матча  ', CAST(150.00 AS Decimal(8, 2)), 3, 7, CAST(9.32 AS Decimal(10, 2)), CAST(1498.71 AS Decimal(10, 2)), CAST(149.00 AS Decimal(10, 2)), CAST(6.26 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (64, N'Чай гречишный  ', CAST(500.00 AS Decimal(8, 2)), 3, 7, CAST(8.54 AS Decimal(10, 2)), CAST(1878.92 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(5.05 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (65, N'Чай Жасмин  ', CAST(500.00 AS Decimal(8, 2)), 3, 7, CAST(5.62 AS Decimal(10, 2)), CAST(2729.18 AS Decimal(10, 2)), CAST(159.00 AS Decimal(10, 2)), CAST(3.53 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (70, N'Чай Молочный улун ', CAST(500.00 AS Decimal(8, 2)), 3, 7, CAST(5.86 AS Decimal(10, 2)), CAST(2783.96 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(3.47 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (74, N'Чай Ромашковый  ', CAST(500.00 AS Decimal(8, 2)), 3, 4, CAST(13.10 AS Decimal(10, 2)), CAST(1190.08 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(7.75 AS Decimal(5, 2)), 590, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (77, N'Чай травяной  ', CAST(500.00 AS Decimal(8, 2)), 3, 4, CAST(5.59 AS Decimal(10, 2)), CAST(2923.26 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(3.31 AS Decimal(5, 2)), 591, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (78, N'Чай черный  ', CAST(500.00 AS Decimal(8, 2)), 3, 4, CAST(6.80 AS Decimal(10, 2)), CAST(2385.29 AS Decimal(10, 2)), CAST(169.00 AS Decimal(10, 2)), CAST(4.02 AS Decimal(5, 2)), 592, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (79, N'Шиповник Барбарис горячий ', CAST(370.00 AS Decimal(8, 2)), 3, 7, CAST(24.85 AS Decimal(10, 2)), CAST(861.77 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(10.40 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (80, N'Ягодный Глинтвейн  ', CAST(380.00 AS Decimal(8, 2)), 3, 7, CAST(67.24 AS Decimal(10, 2)), CAST(255.44 AS Decimal(10, 2)), CAST(239.00 AS Decimal(10, 2)), CAST(28.13 AS Decimal(5, 2)), NULL, CAST(1.00 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(51.00 AS Decimal(8, 2)), CAST(230.00 AS Decimal(8, 2)), CAST(950.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (81, N'Зеленый смузи  ', CAST(330.00 AS Decimal(8, 2)), 3, 5, CAST(56.88 AS Decimal(10, 2)), CAST(425.67 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(19.02 AS Decimal(5, 2)), 473, CAST(0.50 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(14.00 AS Decimal(8, 2)), CAST(70.00 AS Decimal(8, 2)), CAST(300.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (82, N'Клубника-вишня смузи  ', CAST(330.00 AS Decimal(8, 2)), 3, 7, CAST(71.99 AS Decimal(10, 2)), CAST(315.34 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(24.08 AS Decimal(5, 2)), NULL, CAST(0.30 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(33.00 AS Decimal(8, 2)), CAST(140.00 AS Decimal(8, 2)), CAST(580.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (83, N'Кокосовый снежок с манго', CAST(330.00 AS Decimal(8, 2)), 3, 5, CAST(101.40 AS Decimal(10, 2)), CAST(273.77 AS Decimal(10, 2)), CAST(379.00 AS Decimal(10, 2)), CAST(26.75 AS Decimal(5, 2)), 499, CAST(30.00 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(27.00 AS Decimal(8, 2)), CAST(400.00 AS Decimal(8, 2)), CAST(1670.00 AS Decimal(8, 2)), NULL, 0)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (84, N'Лимонад Малина-маракуйя  ', CAST(270.00 AS Decimal(8, 2)), 3, 7, CAST(69.23 AS Decimal(10, 2)), CAST(331.89 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(23.15 AS Decimal(5, 2)), NULL, CAST(0.10 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(60.00 AS Decimal(8, 2)), CAST(260.00 AS Decimal(8, 2)), NULL, 1)
INSERT [dbo].[Drinks] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CategoryId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [ImageUrl], [IsAvailable]) VALUES (85, N'Мохито   ', CAST(270.00 AS Decimal(8, 2)), 3, 7, CAST(51.82 AS Decimal(10, 2)), CAST(477.00 AS Decimal(10, 2)), CAST(299.00 AS Decimal(10, 2)), CAST(17.33 AS Decimal(5, 2)), NULL, CAST(0.10 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(85.00 AS Decimal(8, 2)), CAST(360.00 AS Decimal(8, 2)), NULL, 1)
SET IDENTITY_INSERT [dbo].[Drinks] OFF
GO
SET IDENTITY_INSERT [dbo].[DrinkToppings] ON 

INSERT [dbo].[DrinkToppings] ([Id], [ToppingId], [OrderDrinkItemId], [Quantity], [FinalPrice]) VALUES (3, 12, 4, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DrinkToppings] ([Id], [ToppingId], [OrderDrinkItemId], [Quantity], [FinalPrice]) VALUES (4, 17, 4, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DrinkToppings] ([Id], [ToppingId], [OrderDrinkItemId], [Quantity], [FinalPrice]) VALUES (5, 18, 4, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
INSERT [dbo].[DrinkToppings] ([Id], [ToppingId], [OrderDrinkItemId], [Quantity], [FinalPrice]) VALUES (8, 17, 88, CAST(1.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[DrinkToppings] OFF
GO
SET IDENTITY_INSERT [dbo].[IngredientCategories] ON 

INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (1, N'Грибы')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (2, N'Кофе')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (3, N'Животное происхождение')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (4, N'Заморозка')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (5, N'Зелень')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (6, N'Консервация')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (7, N'Крупы')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (8, N'Мука')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (9, N'Масло')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (10, N'Молочные продукты')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (11, N'Напитки')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (12, N'Овощи')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (13, N'Орехи, сухофрукты и семена')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (14, N'Питание персонал')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (15, N'Соевые продукты')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (16, N'Топинги')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (17, N'Соусы')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (18, N'Специи')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (19, N'Кондитерка')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (20, N'Фрукты')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (21, N'Хлебобулочные')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (22, N'Чай')
INSERT [dbo].[IngredientCategories] ([Id], [Name]) VALUES (23, N'Ягоды')
SET IDENTITY_INSERT [dbo].[IngredientCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Ingredients] ON 

INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (1, N'Кофе Бразилия зерно молочка', CAST(10.23 AS Decimal(10, 2)), 5, CAST(1654.94 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (2, N'Кофе Бразилия Колумбия черный', CAST(0.00 AS Decimal(10, 2)), 5, NULL, 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (3, N'Кофе в зернах ТАВ 200г', CAST(0.00 AS Decimal(10, 2)), 4, CAST(327.86 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (4, N'Кофе в зернах ТАВ Costa Rica Tarrazu', CAST(-0.20 AS Decimal(10, 2)), 5, CAST(2025.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (5, N'Кофе в зернах ТАВ Ethiopia Anasora', CAST(-0.08 AS Decimal(10, 2)), 5, CAST(2354.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (6, N'Кофе в зернах ТАВ Galaxy', CAST(999.94 AS Decimal(10, 2)), 5, CAST(1900.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (7, N'Кофе в зернах ТАВ Tanzania Umoja ', CAST(-0.03 AS Decimal(10, 2)), 5, CAST(2325.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (8, N'Кофе Гватемала', CAST(0.94 AS Decimal(10, 2)), 5, CAST(1733.65 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (9, N'Кофе Декаф COLOMBIA', CAST(0.98 AS Decimal(10, 2)), 5, CAST(2341.34 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (10, N'Кофе Индонезия Фринса', CAST(0.00 AS Decimal(10, 2)), 5, NULL, 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (11, N'Кофе Кения', CAST(0.00 AS Decimal(10, 2)), 5, CAST(2188.33 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (12, N'Кофе Колумбия', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1352.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (13, N'Кофе Руанда', CAST(5.23 AS Decimal(10, 2)), 5, CAST(2142.82 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (14, N'Кофе Суматра', CAST(0.00 AS Decimal(10, 2)), 5, NULL, 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (15, N'Кофе Эфиопия', CAST(0.00 AS Decimal(10, 2)), 5, CAST(5.00 AS Decimal(10, 2)), 2)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (16, N'ВЕШЕНКИ', CAST(0.36 AS Decimal(10, 2)), 5, CAST(359.99 AS Decimal(10, 2)), 1)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (17, N'ШАМПИНЬОНЫ', CAST(-0.61 AS Decimal(10, 2)), 5, CAST(323.60 AS Decimal(10, 2)), 1)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (18, N'ГЛАЗНОЙ МУСКУЛ ГОВЯДИНА', CAST(23.70 AS Decimal(10, 2)), 5, CAST(1363.73 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (19, N'ИНДЕЙКА', CAST(19.25 AS Decimal(10, 2)), 5, CAST(540.06 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (20, N'КАЛЬМАР С/М', CAST(2.41 AS Decimal(10, 2)), 5, CAST(717.00 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (21, N'КРЕВЕТКИ', CAST(2.68 AS Decimal(10, 2)), 5, CAST(1112.70 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (22, N'КУРИЦА ПЕРС', CAST(1.66 AS Decimal(10, 2)), 5, CAST(250.17 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (23, N'ПЕЧЕНЬ КУРИНАЯ', CAST(1.45 AS Decimal(10, 2)), 5, CAST(234.12 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (24, N'УТКА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(769.24 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (25, N'ЩЕЧКИ ГОВЯЖЬИ', CAST(17.28 AS Decimal(10, 2)), 5, CAST(1415.63 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (26, N'ЯЙЦО', CAST(81.46 AS Decimal(10, 2)), 4, CAST(9.76 AS Decimal(10, 2)), 3)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (27, N'АВОКАДО С/М', CAST(0.74 AS Decimal(10, 2)), 5, CAST(929.09 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (28, N'АНАНАС С/М', CAST(0.00 AS Decimal(10, 2)), 5, CAST(357.04 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (29, N'БРОККОЛИ С/М', CAST(1.63 AS Decimal(10, 2)), 5, CAST(199.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (30, N'БРУСНИКА С/М', CAST(1.24 AS Decimal(10, 2)), 5, CAST(517.24 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (31, N'ВИШНЯ С/М', CAST(2.24 AS Decimal(10, 2)), 5, CAST(725.01 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (32, N'ЕЖЕВИКА С/М', CAST(2.60 AS Decimal(10, 2)), 5, CAST(472.23 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (33, N'ЖИМОЛОСТЬ С/М', CAST(0.00 AS Decimal(10, 2)), 5, CAST(442.83 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (34, N'КЛУБНИКА С/М', CAST(2.13 AS Decimal(10, 2)), 5, CAST(289.41 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (35, N'КЛЮКВА С/М', CAST(1.93 AS Decimal(10, 2)), 5, CAST(640.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (36, N'КРУАССАН', CAST(76.00 AS Decimal(10, 2)), 4, CAST(81.09 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (37, N'МАЛИНА С/М', CAST(2.27 AS Decimal(10, 2)), 5, CAST(714.82 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (38, N'МАНГО С/М', CAST(1.73 AS Decimal(10, 2)), 5, CAST(371.01 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (39, N'ОБЛЕПИХА С/М', CAST(3.09 AS Decimal(10, 2)), 5, CAST(277.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (40, N'СМОРОДИНА С/М', CAST(3.87 AS Decimal(10, 2)), 5, CAST(728.58 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (41, N'ТЫКВА С/М', CAST(3.00 AS Decimal(10, 2)), 5, CAST(199.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (42, N'ФАСОЛЬ СТРУЧКОВАЯ С/М', CAST(0.00 AS Decimal(10, 2)), 5, CAST(195.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (43, N'ЧЕРНИКА С/М', CAST(7.54 AS Decimal(10, 2)), 5, CAST(713.53 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (44, N'ЭДАМАМЕ С/М', CAST(1.06 AS Decimal(10, 2)), 5, CAST(462.00 AS Decimal(10, 2)), 4)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (45, N'БАЗИЛИК СВЕЖИЙ', CAST(0.29 AS Decimal(10, 2)), 5, CAST(1783.17 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (46, N'КИНЗА', CAST(-0.09 AS Decimal(10, 2)), 5, CAST(480.03 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (47, N'ЛУК ЗЕЛЁНЫЙ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(356.00 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (48, N'МИКРОЗЕЛЕНЬ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3023.81 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (49, N'МЯТА СВЕЖАЯ', CAST(0.89 AS Decimal(10, 2)), 5, CAST(1147.51 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (50, N'ПЕТРУШКА СВЕЖАЯ', CAST(-0.12 AS Decimal(10, 2)), 5, CAST(259.99 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (51, N'РОЗМАРИН СВЕЖИЙ', CAST(0.73 AS Decimal(10, 2)), 5, CAST(1521.35 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (52, N'РОМАНО', CAST(0.09 AS Decimal(10, 2)), 5, CAST(379.08 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (53, N'РУККОЛА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1232.56 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (54, N'САЛАТ АЙСБЕРГ', CAST(-0.05 AS Decimal(10, 2)), 5, CAST(230.38 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (55, N'ТИМЬЯН СВЕЖИЙ', CAST(0.32 AS Decimal(10, 2)), 5, CAST(1706.20 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (56, N'УКРОП СВЕЖИЙ', CAST(-0.50 AS Decimal(10, 2)), 5, CAST(330.01 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (57, N'ШПИНАТ СВЕЖИЙ', CAST(6.94 AS Decimal(10, 2)), 5, CAST(610.01 AS Decimal(10, 2)), 5)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (58, N'КАПЕРСЫ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(985.06 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (59, N'КАПУСТА КВАШЕННАЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(242.91 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (60, N'ОГУРЦЫ МАРИНОВАННЫЕ', CAST(-4.80 AS Decimal(10, 2)), 5, CAST(400.00 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (61, N'ОЛИВКИ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1222.43 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (62, N'ПЮРЕ МАНГО', CAST(6.89 AS Decimal(10, 2)), 5, CAST(653.24 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (63, N'ПЮРЕ МАРАКУЙЯ', CAST(7.99 AS Decimal(10, 2)), 5, CAST(922.39 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (64, N'Пюре ревень', CAST(0.00 AS Decimal(10, 2)), 5, CAST(765.00 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (65, N'ПЮРЕ ЯБЛОЧНОЕ', CAST(-4.47 AS Decimal(10, 2)), 5, CAST(243.33 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (66, N'ТОМАТЫ В С/С', CAST(4.26 AS Decimal(10, 2)), 5, CAST(250.92 AS Decimal(10, 2)), 6)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (67, N'БУЛГУР', CAST(0.00 AS Decimal(10, 2)), 5, CAST(235.44 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (68, N'ВОДОРОСЛИ ВАКАМЕ', CAST(-0.20 AS Decimal(10, 2)), 5, CAST(1638.46 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (69, N'ГЕРКУЛЕС', CAST(5.59 AS Decimal(10, 2)), 5, CAST(160.61 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (70, N'ГЕРКУЛЕС МОНАСТЫРСКИЙ', CAST(-6.71 AS Decimal(10, 2)), 5, CAST(98.07 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (71, N'Горох колотый', CAST(-0.40 AS Decimal(10, 2)), 5, CAST(72.03 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (72, N'ГРЕЧКА', CAST(-0.01 AS Decimal(10, 2)), 5, CAST(63.00 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (73, N'ГРЕЧНЕВАЯ СОБА', CAST(-2.73 AS Decimal(10, 2)), 5, CAST(305.00 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (74, N'ЗЕЛЁНАЯ ГРЕЧКА', CAST(-4.05 AS Decimal(10, 2)), 5, CAST(89.64 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (75, N'КИНОА ТРИКОЛОР', CAST(2.87 AS Decimal(10, 2)), 5, CAST(493.79 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (76, N'КРАХМАЛ КАРТОФЕЛЬНЫЙ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(250.00 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (77, N'КРАХМАЛ КУКУРУЗНЫЙ', CAST(5.00 AS Decimal(10, 2)), 5, CAST(277.81 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (78, N'КРУПА МАННАЯ', CAST(7.13 AS Decimal(10, 2)), 5, CAST(87.14 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (79, N'КРУПА ПЕРЛОВАЯ', CAST(0.62 AS Decimal(10, 2)), 5, CAST(48.47 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (80, N'КРУПА ПШЕНО', CAST(3.31 AS Decimal(10, 2)), 5, CAST(71.64 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (81, N'КУКУРУЗНАЯ КРУПА', CAST(4.29 AS Decimal(10, 2)), 5, CAST(69.79 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (82, N'МАНКА ИЗ ПОЛБЫ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(128.45 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (83, N'НУТ', CAST(4.74 AS Decimal(10, 2)), 5, CAST(452.34 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (84, N'ПАСТА ЦЕЛЬНОЗЕРНОВАЯ', CAST(-2.57 AS Decimal(10, 2)), 5, CAST(111.49 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (85, N'РИС', CAST(1.10 AS Decimal(10, 2)), 5, CAST(132.66 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (86, N'РИС БАСМАТИ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(323.19 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (87, N'РИСОВАЯ ЛАПША', CAST(0.00 AS Decimal(10, 2)), 5, CAST(441.98 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (88, N'СПАГЕТТИ', CAST(1.25 AS Decimal(10, 2)), 5, CAST(117.62 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (89, N'СПАГЕТТИ ЧЕРНЫЕ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(723.46 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (90, N'ФАСОЛЬ КРАСНАЯ СУХАЯ', CAST(0.28 AS Decimal(10, 2)), 5, CAST(244.99 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (91, N'ФУНЧЕЗА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(441.99 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (92, N'ЧЕЧЕВИЦА КРАСНАЯ', CAST(2.66 AS Decimal(10, 2)), 5, CAST(170.00 AS Decimal(10, 2)), 7)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (93, N'МУКА БЕЗГЛЮТЕНОВАЯ СМЕСЬ', CAST(-0.24 AS Decimal(10, 2)), 5, CAST(389.86 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (94, N'МУКА ИЗ ЗЕЛЁНОЙ ГРЕЧКИ', CAST(-0.84 AS Decimal(10, 2)), 5, CAST(96.00 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (95, N'МУКА КУКУРУЗНАЯ', CAST(2.33 AS Decimal(10, 2)), 5, CAST(150.97 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (96, N'Мука льняная', CAST(0.00 AS Decimal(10, 2)), 5, CAST(139.82 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (97, N'МУКА МИНДАЛЬНАЯ', CAST(5.33 AS Decimal(10, 2)), 5, CAST(1270.15 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (98, N'МУКА ПШЕНИЧНАЯ', CAST(1.64 AS Decimal(10, 2)), 5, CAST(59.50 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (99, N'МУКА РИСОВАЯ', CAST(-4.01 AS Decimal(10, 2)), 5, CAST(182.00 AS Decimal(10, 2)), 8)
GO
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (100, N'МУКА ЦЕЛЬНОЗЕРНОВАЯ ', CAST(5.30 AS Decimal(10, 2)), 5, CAST(59.07 AS Decimal(10, 2)), 8)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (101, N'КАКАО МАСЛО', CAST(0.73 AS Decimal(10, 2)), 5, CAST(2583.30 AS Decimal(10, 2)), 9)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (102, N'КОКОСОВОЕ МАСЛО', CAST(7.64 AS Decimal(10, 2)), 5, CAST(587.02 AS Decimal(10, 2)), 9)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (103, N'КУНЖУТНОЕ МАСЛО', CAST(0.00 AS Decimal(10, 2)), 5, CAST(802.12 AS Decimal(10, 2)), 9)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (104, N'ОЛИВКОВОЕ МАСЛО', CAST(2.67 AS Decimal(10, 2)), 5, CAST(1456.43 AS Decimal(10, 2)), 9)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (105, N'ПОДСОЛНЕЧНОЕ МАСЛО', CAST(113.94 AS Decimal(10, 2)), 5, CAST(193.71 AS Decimal(10, 2)), 9)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (106, N'GREEN MILK КОКОСОВОЕ МОЛОКО', CAST(49.52 AS Decimal(10, 2)), 5, CAST(154.47 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (107, N'GREEN MILK МИНДАЛЬНОЕ МОЛОКО', CAST(20.83 AS Decimal(10, 2)), 5, CAST(153.95 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (108, N'GREEN MILK ФУНДУЧНОЕ МОЛОКО', CAST(19.30 AS Decimal(10, 2)), 5, CAST(155.96 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (109, N'БАНАНОВОЕ МОЛОКО', CAST(9.87 AS Decimal(10, 2)), 5, CAST(145.79 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (110, N'БЕЗЛАКТОЗНОЕ МОЛОКО', CAST(26.42 AS Decimal(10, 2)), 5, CAST(153.11 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (111, N'КОКОСОВОЕ МОЛОКО AROY-D', CAST(58.96 AS Decimal(10, 2)), 5, CAST(418.00 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (112, N'КОКОСОВЫЕ СЛИВКИ СУХИЕ', CAST(-0.60 AS Decimal(10, 2)), 5, CAST(1074.88 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (113, N'КОРОВЬЕ МОЛОКО', CAST(85.95 AS Decimal(10, 2)), 5, CAST(105.43 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (114, N'МАСЛО СЛИВОЧНОЕ', CAST(2.79 AS Decimal(10, 2)), 5, CAST(979.88 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (115, N'ОВСЯНОЕ МОЛОКО', CAST(34.87 AS Decimal(10, 2)), 5, CAST(118.45 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (116, N'ПАРМЕЗАН', CAST(-0.95 AS Decimal(10, 2)), 5, CAST(1464.20 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (117, N'СЛИВКИ 33 %', CAST(5.96 AS Decimal(10, 2)), 5, CAST(455.04 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (118, N'СОЕВОЕ МОЛОКО', CAST(26.74 AS Decimal(10, 2)), 5, CAST(117.89 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (119, N'СЫР ТВОРОЖНЫЙ', CAST(2.99 AS Decimal(10, 2)), 5, CAST(539.75 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (120, N'ТВОРОГ', CAST(2.24 AS Decimal(10, 2)), 5, CAST(336.46 AS Decimal(10, 2)), 10)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (121, N'ВОДА ГАЗИРОВАННАЯ', CAST(5.71 AS Decimal(10, 2)), 6, CAST(38.99 AS Decimal(10, 2)), 11)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (122, N'АВОКАДО', CAST(0.00 AS Decimal(10, 2)), 5, CAST(740.01 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (123, N'БАКЛАЖАН', CAST(0.00 AS Decimal(10, 2)), 5, CAST(326.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (124, N'БАТАТ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(430.65 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (125, N'КАБАЧКИ', CAST(4.87 AS Decimal(10, 2)), 5, CAST(197.72 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (126, N'КАПУСТА БЕЛОКОЧАННАЯ', CAST(0.53 AS Decimal(10, 2)), 5, CAST(46.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (127, N'КАПУСТА КРАСНОКОЧАННАЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(100.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (128, N'КАПУСТА ПЕКИНСКАЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(164.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (129, N'КАРТОФЕЛЬ ВАКУУМ', CAST(-2.42 AS Decimal(10, 2)), 5, CAST(81.83 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (130, N'КОРЕНЬ ИМБИРЯ СВЕЖИЙ', CAST(0.22 AS Decimal(10, 2)), 5, CAST(408.40 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (131, N'КОРЕНЬ СЕЛЬДЕРЕЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(300.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (132, N'ЛУК КРАСНЫЙ', CAST(2.69 AS Decimal(10, 2)), 5, CAST(89.68 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (133, N'ЛУК РЕПЧАТЫЙ ВАКУУМ', CAST(12.37 AS Decimal(10, 2)), 5, CAST(69.46 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (134, N'МОРКОВЬ ВАКУУМ ', CAST(-3.56 AS Decimal(10, 2)), 5, CAST(70.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (135, N'ОГУРЕЦ СВЕЖИЙ', CAST(9.26 AS Decimal(10, 2)), 5, CAST(131.07 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (136, N'ПЕРЕЦ БОЛГАРСКИЙ КРАСНЫЙ', CAST(4.08 AS Decimal(10, 2)), 5, CAST(251.61 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (137, N'ПЕРЕЦ ЧИЛИ СВЕЖИЙ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(980.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (138, N'ПОМИДОРЫ СВЕЖИЕ', CAST(4.82 AS Decimal(10, 2)), 5, CAST(239.22 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (139, N'РЕДИС', CAST(-0.89 AS Decimal(10, 2)), 5, CAST(155.01 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (140, N'РЕДЬКА / ДАЙКОН', CAST(0.73 AS Decimal(10, 2)), 5, CAST(139.97 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (141, N'РОСТКИ СОИ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(345.89 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (142, N'СВЁКЛА', CAST(4.14 AS Decimal(10, 2)), 5, CAST(47.66 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (143, N'СТЕБЕЛЬ СЕЛЬДЕРЕЯ', CAST(5.60 AS Decimal(10, 2)), 5, CAST(207.02 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (144, N'ТОМАТЫ ЧЕРРИ', CAST(5.33 AS Decimal(10, 2)), 5, CAST(389.55 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (145, N'ТЫКВА СВЕЖАЯ', CAST(3.21 AS Decimal(10, 2)), 5, CAST(111.00 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (146, N'ЧЕСНОК СВЕЖИЙ', CAST(1.22 AS Decimal(10, 2)), 5, CAST(244.98 AS Decimal(10, 2)), 12)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (147, N'АРАХИС', CAST(0.91 AS Decimal(10, 2)), 5, CAST(269.72 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (148, N'ГРЕЦКИЙ ОРЕХ', CAST(-0.05 AS Decimal(10, 2)), 5, CAST(550.09 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (149, N'ИЗЮМ', CAST(2.06 AS Decimal(10, 2)), 5, CAST(397.56 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (150, N'КЕДРОВЫЙ ЖМЫХ', CAST(3.42 AS Decimal(10, 2)), 5, CAST(1684.16 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (151, N'КЕДРОВЫЙ ОРЕХ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1637.60 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (152, N'КЕШЬЮ', CAST(22.79 AS Decimal(10, 2)), 5, CAST(956.39 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (153, N'КОКОСОВАЯ СТРУЖКА', CAST(3.18 AS Decimal(10, 2)), 5, CAST(516.60 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (154, N'КОКОСОВЫЕ ЧИПСЫ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(307.37 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (155, N'КУНЖУТ БЕЛЫЙ', CAST(2.60 AS Decimal(10, 2)), 5, CAST(350.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (156, N'КУНЖУТ ЧЕРНЫЙ', CAST(-0.23 AS Decimal(10, 2)), 5, CAST(360.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (157, N'КУРАГА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(270.82 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (158, N'МАНГО (СУХОФРУКТ)', CAST(0.83 AS Decimal(10, 2)), 5, CAST(644.33 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (159, N'МАРАКУЙА (СУХОФРУКТ)', CAST(0.00 AS Decimal(10, 2)), 5, CAST(892.71 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (160, N'МИНДАЛЬ', CAST(2.05 AS Decimal(10, 2)), 5, CAST(794.91 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (161, N'МИНДАЛЬНЫЕ ЛЕПЕСТКИ', CAST(1.46 AS Decimal(10, 2)), 5, CAST(1617.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (162, N'МУСКАТНЫЙ ОРЕХ', CAST(0.52 AS Decimal(10, 2)), 5, CAST(654.97 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (163, N'СЕМЕНА ЛЬНА', CAST(3.23 AS Decimal(10, 2)), 5, CAST(242.87 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (164, N'СЕМЕНА ПОДСОЛНЕЧНИКА', CAST(0.52 AS Decimal(10, 2)), 5, CAST(108.35 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (165, N'СЕМЕНА ТЫКВЫ', CAST(1.03 AS Decimal(10, 2)), 5, CAST(480.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (166, N'СЕМЕНА ЧИА', CAST(-1.50 AS Decimal(10, 2)), 5, CAST(490.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (167, N'ТОМАТЫ ВЯЛЕНЫЕ', CAST(-1.34 AS Decimal(10, 2)), 5, CAST(1188.26 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (168, N'ФИНИКИ', CAST(6.73 AS Decimal(10, 2)), 5, CAST(139.04 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (169, N'ФИСТАШКА', CAST(-0.51 AS Decimal(10, 2)), 5, CAST(2094.87 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (170, N'ФУНДУК', CAST(1.22 AS Decimal(10, 2)), 5, CAST(984.57 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (171, N'ЧЕРНОСЛИВ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(200.00 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (172, N'ШИПОВНИК', CAST(4.95 AS Decimal(10, 2)), 5, CAST(331.24 AS Decimal(10, 2)), 13)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (173, N'КРУПА ПШЕНИЧНАЯ (ПЕРС)', CAST(0.00 AS Decimal(10, 2)), 5, CAST(79.67 AS Decimal(10, 2)), 14)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (174, N'МАКАРОНЫ (ПЕРС)', CAST(0.36 AS Decimal(10, 2)), 5, CAST(145.47 AS Decimal(10, 2)), 14)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (175, N'САХАР БЕЛЫЙ', CAST(11.50 AS Decimal(10, 2)), 5, CAST(89.18 AS Decimal(10, 2)), 14)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (176, N'СОСИСКИ', CAST(4.35 AS Decimal(10, 2)), 5, CAST(370.26 AS Decimal(10, 2)), 14)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (177, N'ТОФУ РЫНОК', CAST(-0.38 AS Decimal(10, 2)), 5, CAST(680.00 AS Decimal(10, 2)), 15)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (178, N'ТОФУ ЯСО КОПЧЕНЫЙ', CAST(2.88 AS Decimal(10, 2)), 5, CAST(445.54 AS Decimal(10, 2)), 15)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (179, N'ТОФУ ЯСО НАТУР', CAST(0.00 AS Decimal(10, 2)), 5, CAST(500.00 AS Decimal(10, 2)), 15)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (180, N'ФАРШ СОЕВЫЙ ХАЙБИФ', CAST(-0.09 AS Decimal(10, 2)), 5, CAST(672.95 AS Decimal(10, 2)), 15)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (181, N'МЁД', CAST(4.79 AS Decimal(10, 2)), 5, CAST(262.65 AS Decimal(10, 2)), 16)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (182, N'АРАХИСОВАЯ ПАСТА', CAST(6.44 AS Decimal(10, 2)), 5, CAST(530.00 AS Decimal(10, 2)), 16)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (183, N'СИРОП ТОПИНАМБУРА', CAST(81.15 AS Decimal(10, 2)), 5, CAST(261.92 AS Decimal(10, 2)), 16)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (184, N'БАЛЬЗАМИК-КРЕМ', CAST(1.58 AS Decimal(10, 2)), 5, CAST(922.16 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (185, N'ГОРЧИЦА ДИЖОНСКАЯ', CAST(0.67 AS Decimal(10, 2)), 5, CAST(303.28 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (186, N'КОНЦЕНТРАТ МАЛИНЫ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1200.00 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (187, N'ЛЕЦИТИН СОЕВЫЙ', CAST(-0.05 AS Decimal(10, 2)), 5, CAST(1463.96 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (188, N'МИСО ПАСТА', CAST(-1.07 AS Decimal(10, 2)), 5, CAST(270.31 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (189, N'ПАСТА ТОМ ЯМ', CAST(0.22 AS Decimal(10, 2)), 5, CAST(762.94 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (190, N'ПАСТА ТОМАТНАЯ', CAST(3.82 AS Decimal(10, 2)), 5, CAST(410.59 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (191, N'СОЕВЫЙ СОУС', CAST(27.09 AS Decimal(10, 2)), 5, CAST(142.37 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (192, N'СОК ЛИМОНА', CAST(3.91 AS Decimal(10, 2)), 5, CAST(257.49 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (193, N'СОУС НАРШАРАБ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(704.52 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (194, N'СОУС ТОМАТНЫЙ ДЛЯ ПИЦЦЫ', CAST(5.80 AS Decimal(10, 2)), 6, CAST(240.90 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (195, N'УКСУС ВИННЫЙ', CAST(-0.53 AS Decimal(10, 2)), 5, CAST(332.08 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (196, N'УКСУС СТОЛОВЫЙ 9%', CAST(2.23 AS Decimal(10, 2)), 5, CAST(70.35 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (197, N'УКСУС ЯБЛОЧНЫЙ 6%', CAST(0.65 AS Decimal(10, 2)), 5, CAST(140.39 AS Decimal(10, 2)), 17)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (198, N'БАДЬЯН', CAST(0.01 AS Decimal(10, 2)), 5, CAST(1844.58 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (199, N'БАРБАРИС', CAST(0.64 AS Decimal(10, 2)), 5, CAST(1000.01 AS Decimal(10, 2)), 18)
GO
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (200, N'ВИТГРАСС', CAST(-0.03 AS Decimal(10, 2)), 5, CAST(8000.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (201, N'ГВОЗДИКА', CAST(0.35 AS Decimal(10, 2)), 5, CAST(2000.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (202, N'ГОРЧИЦА СЕМЕНА', CAST(0.18 AS Decimal(10, 2)), 5, CAST(549.99 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (203, N'ГРАНАТ ПОРОШОК', CAST(-0.31 AS Decimal(10, 2)), 5, CAST(3211.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (204, N'ДРОЖЖИ СЫРНЫЕ ПИЩЕВЫЕ', CAST(-1.28 AS Decimal(10, 2)), 5, CAST(1721.51 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (205, N'ЗИРА', CAST(0.11 AS Decimal(10, 2)), 5, CAST(925.40 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (206, N'ИМБИРЬ СУШЕНЫЙ', CAST(0.36 AS Decimal(10, 2)), 5, CAST(523.43 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (207, N'КАКАО ПОРОШОК', CAST(9.98 AS Decimal(10, 2)), 5, CAST(1416.83 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (208, N'КАРДАМОН', CAST(0.66 AS Decimal(10, 2)), 5, CAST(2987.72 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (209, N'КАРРИ', CAST(0.22 AS Decimal(10, 2)), 5, CAST(862.28 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (210, N'КОПЧЕНАЯ ПАПРИКА', CAST(1.32 AS Decimal(10, 2)), 5, CAST(1175.83 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (211, N'КОРИАНДР МОЛОТЫЙ', CAST(-0.35 AS Decimal(10, 2)), 5, CAST(379.99 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (212, N'КОРИЦА МОЛОТАЯ', CAST(0.75 AS Decimal(10, 2)), 5, CAST(505.70 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (213, N'КОРИЦА ПАЛОЧКИ', CAST(0.24 AS Decimal(10, 2)), 5, CAST(1333.33 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (214, N'КСАНТОВАЯ КАМЕДЬ (КСАНТАН)', CAST(0.52 AS Decimal(10, 2)), 5, CAST(1120.68 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (215, N'КУРКУМА', CAST(1.47 AS Decimal(10, 2)), 5, CAST(540.32 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (216, N'КЭРОБ СРЕД.ОБЖАРКИ', CAST(0.51 AS Decimal(10, 2)), 5, CAST(904.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (217, N'ЛАВАНДА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3500.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (218, N'ЛИМОННАЯ КИСЛОТА (ПОРОШОК)', CAST(0.00 AS Decimal(10, 2)), 5, CAST(668.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (219, N'ЛУК ФРИ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(565.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (220, N'МАЛИНА (СУХОЙ СОК)', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3463.26 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (221, N'МАЛИНА СУБЛИМИР.', CAST(0.00 AS Decimal(10, 2)), 5, CAST(9420.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (222, N'МАНГО (СУХОЙ СОК)', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3581.40 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (223, N'МАТЧА ГОЛУБАЯ', CAST(0.16 AS Decimal(10, 2)), 5, CAST(4890.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (224, N'МАТЧА ПРЕМ. ЗЕЛЁНАЯ', CAST(0.71 AS Decimal(10, 2)), 5, CAST(4660.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (225, N'МОЖЖЕВЕЛЬНИК СУХ.', CAST(0.88 AS Decimal(10, 2)), 5, CAST(2122.93 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (226, N'ПАПРИКА СЛАДКАЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(600.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (227, N'ПЕКТИН NH ', CAST(0.92 AS Decimal(10, 2)), 5, CAST(3692.50 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (228, N'ПЕКТИН ЯБЛ.', CAST(-0.09 AS Decimal(10, 2)), 5, CAST(3590.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (229, N'ПЕРЕЦ ДУШИС.ГОР.', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1500.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (230, N'ПЕРЕЦ ЧЕРН.ГОРОШЕК', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1945.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (231, N'ПЕРЕЦ ЧЕРНЫЙ МОЛОТ', CAST(-0.13 AS Decimal(10, 2)), 5, CAST(655.99 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (232, N'ПРЕБИОТИК', CAST(0.06 AS Decimal(10, 2)), 5, CAST(6238.32 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (233, N'Приправа Kotanyi Classic', CAST(0.00 AS Decimal(10, 2)), 5, CAST(4700.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (234, N'ПРОВАНСКИЕ ТРАВЫ', CAST(-0.06 AS Decimal(10, 2)), 5, CAST(1294.59 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (235, N'РАЗРЫХЛИТЕЛЬ', CAST(2.27 AS Decimal(10, 2)), 5, CAST(392.62 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (236, N'РОМАШКА СУШЕНАЯ', CAST(0.36 AS Decimal(10, 2)), 5, CAST(2619.97 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (237, N'САХАР ТРОСТНИКОВЫЙ', CAST(51.95 AS Decimal(10, 2)), 5, CAST(382.73 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (238, N'СВЕКОЛЬНЫЙ ПОРОШОК', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3237.35 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (239, N'СЕМЕНА УКРОПА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(500.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (240, N'СОЛЬ', CAST(6.23 AS Decimal(10, 2)), 5, CAST(20.71 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (241, N'СОЛЬ РОЗОВАЯ', CAST(1.41 AS Decimal(10, 2)), 5, CAST(239.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (242, N'СОЛЬ ЧЕРНАЯ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(900.97 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (243, N'СУШЕНЫЕ ПОМИДОРЫ', CAST(0.17 AS Decimal(10, 2)), 5, CAST(580.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (244, N'ТМИН', CAST(-0.02 AS Decimal(10, 2)), 5, CAST(799.71 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (245, N'ХВОЙНЫЙ ПИХТОВО-КЕДРОВЫЙ ЭКСТРАКТ', CAST(0.98 AS Decimal(10, 2)), 5, CAST(535.17 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (246, N'Цедра лимона молотая', CAST(0.00 AS Decimal(10, 2)), 5, CAST(4500.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (247, N'ЦИКОРИЙ', CAST(0.36 AS Decimal(10, 2)), 5, CAST(1262.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (248, N'ЧЕСНОК ПОРОШОК', CAST(0.00 AS Decimal(10, 2)), 5, CAST(7100.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (249, N'ЧИЛИ ПЕРЕЦ МОЛОТЫЙ', CAST(0.35 AS Decimal(10, 2)), 5, CAST(5800.00 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (250, N'АГАР-АГАР', CAST(1.11 AS Decimal(10, 2)), 5, CAST(3224.80 AS Decimal(10, 2)), 18)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (251, N'ВАНИЛЬНАЯ ПАСТА', CAST(1.34 AS Decimal(10, 2)), 5, CAST(6532.20 AS Decimal(10, 2)), 19)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (252, N'ОВСЯНЫЕ ХЛОПЬЯ КЦ', CAST(14.01 AS Decimal(10, 2)), 5, CAST(200.28 AS Decimal(10, 2)), 19)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (253, N'ШОКОЛАД ТЕМН.БЕЛЬГ.', CAST(4.18 AS Decimal(10, 2)), 5, CAST(1349.93 AS Decimal(10, 2)), 19)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (254, N'АБРИКОС', CAST(1.10 AS Decimal(10, 2)), 5, CAST(249.77 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (255, N'АПЕЛЬСИН', CAST(43.32 AS Decimal(10, 2)), 5, CAST(174.69 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (256, N'БАНАНЫ', CAST(13.41 AS Decimal(10, 2)), 5, CAST(187.05 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (257, N'ГРЕЙПФРУТ', CAST(3.69 AS Decimal(10, 2)), 5, CAST(210.25 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (258, N'ГРУША', CAST(0.00 AS Decimal(10, 2)), 5, CAST(403.23 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (259, N'КИВИ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(349.55 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (260, N'ЛАЙМ', CAST(0.77 AS Decimal(10, 2)), 5, CAST(504.09 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (261, N'ЛИМОН', CAST(5.00 AS Decimal(10, 2)), 5, CAST(246.25 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (262, N'МАНДАРИНЫ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(248.55 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (263, N'ПЕРСИК', CAST(0.00 AS Decimal(10, 2)), 5, CAST(313.65 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (264, N'ХУРМА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(300.17 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (265, N'ЯБЛОКИ ГРЕННИ СМИТ', CAST(24.29 AS Decimal(10, 2)), 5, CAST(204.77 AS Decimal(10, 2)), 20)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (266, N'БАГЕТ ЦЕЛЬНОЗЕРНОВОЙ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(304.35 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (267, N'БРИОШЬ', CAST(1.36 AS Decimal(10, 2)), 5, CAST(1437.52 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (268, N'БУЛОЧКА Д/БУРГЕРА', CAST(0.00 AS Decimal(10, 2)), 4, CAST(50.63 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (269, N'ГРЕЧНЕВЫЙ ХЛЕБ', CAST(-0.17 AS Decimal(10, 2)), 5, CAST(342.26 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (270, N'ТОРТИЛЬЯ ЦЕЛЬНОЗЕРНОВАЯ', CAST(-5.61 AS Decimal(10, 2)), 5, CAST(238.33 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (271, N'Хлеб', CAST(-0.10 AS Decimal(10, 2)), 5, CAST(221.30 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (272, N'ХЛЕБ ЧЁРНЫЙ', CAST(2.30 AS Decimal(10, 2)), 5, CAST(342.85 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (273, N'ЧИАБАТТА', CAST(3.22 AS Decimal(10, 2)), 5, CAST(295.73 AS Decimal(10, 2)), 21)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (274, N'ИВАН ЧАЙ', CAST(0.81 AS Decimal(10, 2)), 5, CAST(1168.73 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (275, N'КАРКАДЭ ЧАЙ', CAST(0.96 AS Decimal(10, 2)), 5, CAST(813.77 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (276, N'МОЛОЧНЫЙ УЛУН', CAST(0.51 AS Decimal(10, 2)), 5, CAST(1171.88 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (277, N'ПОЧКИ СОСНЫ', CAST(0.12 AS Decimal(10, 2)), 5, CAST(3825.17 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (278, N'ТАЁЖНЫЙ СБОР ПРЕМ.', CAST(0.00 AS Decimal(10, 2)), 5, CAST(2300.00 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (279, N'ТРАВЯНОЙ ЧАЙ', CAST(0.75 AS Decimal(10, 2)), 5, CAST(1117.33 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (280, N'ЧАЙ ГРЕЧИШНЫЙ', CAST(0.83 AS Decimal(10, 2)), 5, CAST(853.75 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (281, N'ЧАЙ ЖАСМИН ЗЕЛЁНЫЙ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(1124.68 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (282, N'ЧАЙ ЛАПСАНГ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(3800.00 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (283, N'ЧАЙ МАСАЛА', CAST(0.14 AS Decimal(10, 2)), 5, CAST(5426.79 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (284, N'ЧАЙ РОЙБУШ', CAST(0.43 AS Decimal(10, 2)), 5, CAST(1000.00 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (285, N'ЧАЙ ЧЕРНЫЙ АССАМ', CAST(0.33 AS Decimal(10, 2)), 5, CAST(1360.03 AS Decimal(10, 2)), 22)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (286, N'ГОЛУБИКА', CAST(0.00 AS Decimal(10, 2)), 5, CAST(2551.23 AS Decimal(10, 2)), 23)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (287, N'ГРАНАТ', CAST(0.00 AS Decimal(10, 2)), 5, CAST(393.26 AS Decimal(10, 2)), 23)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (288, N'КЛУБНИКА СВЕЖАЯ', CAST(-0.11 AS Decimal(10, 2)), 5, CAST(880.48 AS Decimal(10, 2)), 23)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (304, N'Пряная смесь  Бар', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (305, N'Тунец', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (309, N'Паста ланч отварная', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (313, N'Глазурь для творожного сырка', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (314, N'Глазурь', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (316, N'Картошка нью', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (317, N'Кулич Безглютеновый Творожный тесто', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (318, N'Овсяное печенье смесь', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (319, N'Основа Чизкейк Маракуйа', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (320, N'Сухая смесь на пирог', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (321, N'Сухая смесь на шоколадное печенье', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
GO
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (322, N'Сухая смесь на шоколадный тарт со смородиной', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (323, N'Тесто на муравейник', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (324, N'Хрустящая крошка', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (327, N'Лемонграсовый сироп', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (329, N'Сливочная пенка', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (331, N'Лёд', CAST(0.00 AS Decimal(10, 2)), 5, NULL, NULL)
INSERT [dbo].[Ingredients] ([Id], [Name], [Stock], [UnitOfMeasureId], [CostRub], [CategoryId]) VALUES (334, N'ВОДА', CAST(0.00 AS Decimal(10, 2)), 6, CAST(38.99 AS Decimal(10, 2)), 11)
SET IDENTITY_INSERT [dbo].[Ingredients] OFF
GO
SET IDENTITY_INSERT [dbo].[IngredientWriteOffActItems] ON 

INSERT [dbo].[IngredientWriteOffActItems] ([Id], [WriteOffActId], [IngredientId], [Quantity], [UnitOfMeasureId], [WriteOffTypeId]) VALUES (1, 1, 26, CAST(4.00 AS Decimal(10, 2)), 4, 2)
INSERT [dbo].[IngredientWriteOffActItems] ([Id], [WriteOffActId], [IngredientId], [Quantity], [UnitOfMeasureId], [WriteOffTypeId]) VALUES (2, 1, 57, CAST(0.01 AS Decimal(10, 2)), 5, 2)
INSERT [dbo].[IngredientWriteOffActItems] ([Id], [WriteOffActId], [IngredientId], [Quantity], [UnitOfMeasureId], [WriteOffTypeId]) VALUES (3, 2, 176, CAST(0.50 AS Decimal(10, 2)), 5, 1)
INSERT [dbo].[IngredientWriteOffActItems] ([Id], [WriteOffActId], [IngredientId], [Quantity], [UnitOfMeasureId], [WriteOffTypeId]) VALUES (4, 2, 26, CAST(10.00 AS Decimal(10, 2)), 4, 2)
SET IDENTITY_INSERT [dbo].[IngredientWriteOffActItems] OFF
GO
SET IDENTITY_INSERT [dbo].[MenuItemPortionLimits] ON 

INSERT [dbo].[MenuItemPortionLimits] ([Id], [ItemType], [ItemId], [RemainingPortions], [CreatedAt], [UpdatedAt]) VALUES (10, N'drink', 16, CAST(0.00 AS Decimal(10, 2)), CAST(N'2026-05-01T11:21:46.7926744' AS DateTime2), CAST(N'2026-05-05T22:28:48.6011721' AS DateTime2))
SET IDENTITY_INSERT [dbo].[MenuItemPortionLimits] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDishItems] ON 

INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (20, 12, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (21, 13, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (22, 14, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (23, 14, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (24, 16, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (25, 16, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (26, 17, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (27, 17, 40, CAST(1.00 AS Decimal(8, 2)), CAST(109.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (28, 18, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (29, 19, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (30, 20, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (31, 20, 12, CAST(1.00 AS Decimal(8, 2)), CAST(169.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (32, 20, 38, CAST(1.00 AS Decimal(8, 2)), CAST(299.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (33, 20, 82, CAST(1.00 AS Decimal(8, 2)), CAST(2949.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (34, 20, 42, CAST(1.00 AS Decimal(8, 2)), CAST(129.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (35, 20, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (36, 21, 11, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (37, 21, 12, CAST(1.00 AS Decimal(8, 2)), CAST(169.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (38, 21, 82, CAST(1.00 AS Decimal(8, 2)), CAST(2949.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (39, 22, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (59, 47, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (60, 107, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (61, 108, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (62, 109, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (63, 110, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (64, 111, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (65, 112, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (66, 113, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (67, 114, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (68, 115, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (69, 116, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (70, 117, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (71, 118, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (72, 119, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (73, 120, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (74, 121, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (75, 122, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (76, 123, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (77, 124, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (78, 125, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (79, 126, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (80, 127, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (81, 128, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (82, 129, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (83, 130, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (84, 131, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (85, 132, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (86, 132, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (87, 132, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (88, 133, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (89, 134, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (90, 134, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (91, 134, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (92, 135, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (93, 136, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (94, 137, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (95, 138, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (96, 139, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (97, 139, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (98, 139, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (99, 140, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (100, 141, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (101, 142, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (102, 143, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (103, 143, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (104, 143, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (105, 144, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (106, 145, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (107, 146, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (108, 146, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (109, 147, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (110, 148, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (111, 149, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (112, 150, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (113, 151, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (114, 151, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (115, 151, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (116, 158, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (117, 158, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (118, 158, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (119, 159, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (120, 160, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (121, 161, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (122, 162, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (123, 163, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (124, 164, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (125, 165, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (126, 166, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (127, 167, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (128, 167, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (129, 167, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (130, 168, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (131, 168, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (132, 169, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (133, 170, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (134, 170, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (135, 171, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (136, 172, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (137, 173, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
GO
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (138, 173, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (139, 174, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (140, 175, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (141, 176, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (142, 176, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (143, 177, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (144, 178, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (147, 180, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (149, 182, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (150, 183, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (151, 184, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (152, 185, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (153, 185, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (154, 186, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (155, 187, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (177, 188, 84, CAST(1.00 AS Decimal(8, 2)), CAST(2549.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (181, 189, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (182, 189, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (183, 189, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDishItems] ([Id], [OrderId], [DishId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (184, 190, 24, CAST(1.00 AS Decimal(8, 2)), CAST(319.00 AS Decimal(10, 2)), 1)
SET IDENTITY_INSERT [dbo].[OrderDishItems] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDrinkItemModifiers] ON 

INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (1, 9, NULL, NULL)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (71, 79, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (72, 80, 113, 8)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (73, 81, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (74, 82, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (75, 83, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (76, 84, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (77, 85, 106, 8)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (78, 86, 106, 8)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (79, 87, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (80, 88, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (81, 89, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (82, 90, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (83, 91, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (92, 100, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (94, 102, 113, 6)
INSERT [dbo].[OrderDrinkItemModifiers] ([Id], [OrderDrinkItemId], [MilkIngredientId], [CoffeeIngredientId]) VALUES (95, 103, 113, 6)
SET IDENTITY_INSERT [dbo].[OrderDrinkItemModifiers] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDrinkItems] ON 

INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (4, 15, 3, CAST(1.00 AS Decimal(8, 2)), CAST(239.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (5, 16, 3, CAST(1.00 AS Decimal(8, 2)), CAST(239.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (9, 27, 3, CAST(1.00 AS Decimal(8, 2)), CAST(239.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (79, 98, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (80, 99, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (81, 100, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (82, 101, 44, CAST(1.00 AS Decimal(8, 2)), CAST(239.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (83, 102, 74, CAST(1.00 AS Decimal(8, 2)), CAST(169.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (84, 103, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (85, 104, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (86, 105, 40, CAST(1.00 AS Decimal(8, 2)), CAST(189.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (87, 106, 45, CAST(1.00 AS Decimal(8, 2)), CAST(429.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (88, 185, 16, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (89, 185, 13, CAST(1.00 AS Decimal(8, 2)), CAST(159.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (90, 186, 16, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (91, 187, 16, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 1)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (100, 188, 16, CAST(1.00 AS Decimal(8, 2)), CAST(179.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (102, 189, 17, CAST(1.00 AS Decimal(8, 2)), CAST(199.00 AS Decimal(10, 2)), 0)
INSERT [dbo].[OrderDrinkItems] ([Id], [OrderId], [DrinkId], [Quantity], [FinalPrice], [IsCompleted]) VALUES (103, 191, 74, CAST(1.00 AS Decimal(8, 2)), CAST(169.00 AS Decimal(10, 2)), 0)
SET IDENTITY_INSERT [dbo].[OrderDrinkItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (9, CAST(N'2026-03-02T17:11:59.2292077' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 1, CAST(0.00 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (10, CAST(N'2026-03-02T17:12:18.9983022' AS DateTime2), 16, 2, N'Я очень хочу какать', CAST(0.00 AS Decimal(10, 2)), 1, CAST(0.00 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (11, CAST(N'2026-03-05T10:17:12.8234781' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 1, CAST(0.00 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (12, CAST(N'2026-03-05T22:17:42.8884521' AS DateTime2), 16, 1, NULL, CAST(200.00 AS Decimal(10, 2)), 1, CAST(152.15 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (13, CAST(N'2026-03-05T22:29:24.2479590' AS DateTime2), 16, 1, NULL, CAST(365.00 AS Decimal(10, 2)), 1, CAST(271.15 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (14, CAST(N'2026-03-09T23:22:07.9545823' AS DateTime2), 16, 2, N'я сисика', CAST(646.00 AS Decimal(10, 2)), 2, CAST(593.34 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (15, CAST(N'2026-03-10T16:38:15.6442822' AS DateTime2), 16, 1, NULL, CAST(172.00 AS Decimal(10, 2)), 2, CAST(222.27 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (16, CAST(N'2026-03-10T17:19:07.6320448' AS DateTime2), 16, 1, NULL, CAST(498.00 AS Decimal(10, 2)), 2, CAST(685.41 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (17, CAST(N'2026-03-10T17:26:46.4567560' AS DateTime2), 16, 1, NULL, CAST(370.00 AS Decimal(10, 2)), 2, CAST(267.84 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (18, CAST(N'2026-03-10T17:27:39.0977304' AS DateTime2), 16, 1, NULL, CAST(452.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (19, CAST(N'2026-03-11T13:40:54.4502953' AS DateTime2), 16, 2, N'хочу передать вам всем привет', CAST(200.00 AS Decimal(10, 2)), 2, CAST(166.47 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (20, CAST(N'2026-03-11T13:41:38.9302102' AS DateTime2), 16, 1, N'НЕ ЛОЖИТЬ С СОБОЙ ЛУК И КАРТОФЕЛЬ', CAST(1583.00 AS Decimal(10, 2)), 2, CAST(3760.92 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (21, CAST(N'2026-03-11T14:59:54.9356179' AS DateTime2), 16, 2, N'cvbghnjmk,lgfhjkjhyhjKJHKJKL', CAST(390.00 AS Decimal(10, 2)), 2, CAST(3066.21 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (22, CAST(N'2026-03-16T11:20:29.8337224' AS DateTime2), 16, 1, N'упвапа', CAST(281.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (25, CAST(N'2026-03-18T12:13:38.1477687' AS DateTime2), 16, 1, NULL, CAST(15.00 AS Decimal(10, 2)), 2, CAST(0.00 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (27, CAST(N'2026-03-18T12:29:04.0214422' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(222.27 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (47, CAST(N'2026-03-18T14:21:05.3436382' AS DateTime2), 16, 1, NULL, CAST(360.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (98, CAST(N'2026-03-18T16:21:51.9485191' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (99, CAST(N'2026-03-18T16:22:51.8347456' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (100, CAST(N'2026-03-18T16:25:50.3253336' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (101, CAST(N'2026-03-19T10:58:52.1419147' AS DateTime2), 16, 1, NULL, CAST(55.00 AS Decimal(10, 2)), 2, CAST(222.27 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (102, CAST(N'2026-03-19T10:59:39.2864949' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(157.17 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (103, CAST(N'2026-03-19T11:08:06.3295014' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (104, CAST(N'2026-03-19T11:08:55.8615986' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (105, CAST(N'2026-03-19T11:10:09.9733583' AS DateTime2), 16, 1, NULL, CAST(1.00 AS Decimal(10, 2)), 2, CAST(175.77 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (106, CAST(N'2026-03-19T11:10:42.8088734' AS DateTime2), 16, 1, NULL, CAST(0.00 AS Decimal(10, 2)), 2, CAST(398.97 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (107, CAST(N'2026-03-26T15:11:09.4111286' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (108, CAST(N'2026-03-26T16:33:10.6644301' AS DateTime2), 16, 2, NULL, CAST(282.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T17:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (109, CAST(N'2026-03-26T16:33:46.6314816' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (110, CAST(N'2026-03-26T16:43:18.5724104' AS DateTime2), 16, 2, N'fdfdsfsdf', CAST(1855.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:00:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (111, CAST(N'2026-03-26T16:58:48.5603899' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (112, CAST(N'2026-03-26T17:19:44.3612338' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (113, CAST(N'2026-03-26T17:21:00.4653140' AS DateTime2), 16, 2, N'weqweqw', CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T17:45:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (114, CAST(N'2026-03-26T17:21:29.3263174' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:00:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (115, CAST(N'2026-03-26T17:22:17.7254221' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (116, CAST(N'2026-03-26T17:22:23.4143976' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T17:45:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (117, CAST(N'2026-03-26T17:22:38.4085751' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (118, CAST(N'2026-03-26T17:22:47.1543265' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T17:45:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (119, CAST(N'2026-03-26T17:26:28.2274270' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (120, CAST(N'2026-03-26T17:26:35.7337690' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T17:45:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (121, CAST(N'2026-03-26T17:26:50.0465902' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (122, CAST(N'2026-03-26T17:26:54.4360018' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (123, CAST(N'2026-03-26T17:26:58.8892497' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (124, CAST(N'2026-03-26T17:31:17.7785717' AS DateTime2), 16, 1, NULL, CAST(1855.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (125, CAST(N'2026-03-26T17:31:27.4243088' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:00:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (126, CAST(N'2026-03-26T17:31:35.3825843' AS DateTime2), 16, 1, NULL, CAST(1855.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (127, CAST(N'2026-03-26T17:31:45.6149066' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:00:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (128, CAST(N'2026-03-26T17:53:34.2161958' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (129, CAST(N'2026-03-26T17:53:40.5785075' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (130, CAST(N'2026-03-26T17:53:52.0143175' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (131, CAST(N'2026-03-26T17:54:18.9922139' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (132, CAST(N'2026-03-26T17:55:08.8674202' AS DateTime2), 16, 2, N'sdsadasdasdasdasdasdasdsada', CAST(6436.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (133, CAST(N'2026-03-26T17:56:28.8548124' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (134, CAST(N'2026-03-26T17:56:51.8847431' AS DateTime2), 16, 2, N'fdgdfgdfgdfg', CAST(5809.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 2, CAST(N'2026-03-26T18:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (135, CAST(N'2026-03-26T18:02:29.1791639' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (136, CAST(N'2026-03-26T22:46:10.7255765' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (137, CAST(N'2026-03-26T22:58:55.2233985' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (138, CAST(N'2026-03-26T22:59:05.7809687' AS DateTime2), 16, 1, NULL, CAST(2097.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (139, CAST(N'2026-03-26T22:59:25.8618114' AS DateTime2), 16, 1, NULL, CAST(5721.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (140, CAST(N'2026-03-26T23:07:18.9465252' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (141, CAST(N'2026-03-26T23:07:28.2655015' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (142, CAST(N'2026-03-26T23:08:17.5386451' AS DateTime2), 16, 1, NULL, CAST(2096.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (143, CAST(N'2026-03-26T23:08:33.1556632' AS DateTime2), 16, 1, NULL, CAST(5837.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (144, CAST(N'2026-03-26T23:25:00.6751822' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (145, CAST(N'2026-04-01T12:49:35.1014650' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (146, CAST(N'2026-04-01T12:52:33.9025355' AS DateTime2), 16, 2, NULL, CAST(4192.00 AS Decimal(10, 2)), 2, CAST(4741.14 AS Decimal(10, 2)), 2, CAST(N'2026-04-01T13:15:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (147, CAST(N'2026-04-01T12:52:49.4552159' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (148, CAST(N'2026-04-01T12:52:56.9530328' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (149, CAST(N'2026-04-01T12:53:03.9725183' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (150, CAST(N'2026-04-01T12:53:10.7749347' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (151, CAST(N'2026-04-01T12:53:31.9929191' AS DateTime2), 16, 1, NULL, CAST(5962.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (158, CAST(N'2026-04-09T23:14:53.1112557' AS DateTime2), 16, 1, N'fbfdgfg', CAST(2416.00 AS Decimal(10, 2)), 2, CAST(2963.91 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (159, CAST(N'2026-04-12T21:42:46.2773209' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (160, CAST(N'2026-04-12T21:42:57.8049031' AS DateTime2), 16, 2, N'rewrwr', CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (161, CAST(N'2026-04-12T21:55:33.0823320' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (162, CAST(N'2026-04-12T21:55:42.9691518' AS DateTime2), 16, 2, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (163, CAST(N'2026-04-12T22:18:04.4802756' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (164, CAST(N'2026-04-12T22:39:33.9103157' AS DateTime2), 16, 1, NULL, CAST(365.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (165, CAST(N'2026-04-12T22:40:13.4506637' AS DateTime2), 16, 2, N'5hgh', CAST(480.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (166, CAST(N'2026-04-13T13:23:13.2240502' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (167, CAST(N'2026-04-13T13:23:46.2078021' AS DateTime2), 16, 2, N'dfdsfdsf', CAST(5596.00 AS Decimal(10, 2)), 2, CAST(7111.71 AS Decimal(10, 2)), 3, CAST(N'2026-04-13T13:45:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (168, CAST(N'2026-04-13T13:25:01.4550292' AS DateTime2), 16, 1, NULL, CAST(646.00 AS Decimal(10, 2)), 2, CAST(593.34 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (169, CAST(N'2026-04-13T16:01:52.0892202' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (170, CAST(N'2026-04-13T21:00:57.8963969' AS DateTime2), 16, 1, NULL, CAST(646.00 AS Decimal(10, 2)), 2, CAST(593.34 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (171, CAST(N'2026-04-14T22:26:00.4929722' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (172, CAST(N'2026-04-14T22:44:48.4166514' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (173, CAST(N'2026-04-14T23:52:41.5934905' AS DateTime2), 16, 2, NULL, CAST(845.00 AS Decimal(10, 2)), 2, CAST(593.34 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (174, CAST(N'2026-04-16T13:10:27.9135083' AS DateTime2), 16, 2, N'больше супа', CAST(480.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 1, CAST(N'2026-04-16T14:00:00.0000000' AS DateTime2))
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (175, CAST(N'2026-04-16T13:12:07.3397676' AS DateTime2), 16, 1, NULL, CAST(1770.00 AS Decimal(10, 2)), 2, CAST(2370.57 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (176, CAST(N'2026-04-16T13:38:52.7524762' AS DateTime2), 16, 1, NULL, CAST(560.00 AS Decimal(10, 2)), 2, CAST(593.34 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (177, CAST(N'2026-04-29T12:23:43.7061842' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (178, CAST(N'2026-04-29T12:23:53.9853138' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (180, CAST(N'2026-04-29T12:24:13.9959491' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (182, CAST(N'2026-04-29T13:22:14.3527202' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (183, CAST(N'2026-04-29T13:22:25.6028229' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (184, CAST(N'2026-04-29T13:22:33.9227098' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (185, CAST(N'2026-04-29T22:28:09.5175522' AS DateTime2), 16, 1, NULL, CAST(847.00 AS Decimal(10, 2)), 2, CAST(907.68 AS Decimal(10, 2)), 2, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (186, CAST(N'2026-04-29T22:33:57.3253996' AS DateTime2), 16, 1, NULL, CAST(281.00 AS Decimal(10, 2)), 2, CAST(463.14 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (187, CAST(N'2026-04-30T21:39:34.7666964' AS DateTime2), 16, 1, NULL, CAST(1771.00 AS Decimal(10, 2)), 2, CAST(2537.04 AS Decimal(10, 2)), 3, NULL)
GO
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (188, CAST(N'2026-04-30T21:40:36.8036869' AS DateTime2), 16, 1, NULL, CAST(1771.00 AS Decimal(10, 2)), 2, CAST(2537.04 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (189, CAST(N'2026-05-04T11:50:01.6963052' AS DateTime2), 18, 1, N'redacted', CAST(841.00 AS Decimal(10, 2)), 3, CAST(924.80 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (190, CAST(N'2026-05-07T12:30:32.6466348' AS DateTime2), 16, 1, NULL, CAST(280.00 AS Decimal(10, 2)), 2, CAST(296.67 AS Decimal(10, 2)), 3, NULL)
INSERT [dbo].[Orders] ([Id], [CreatedAt], [ClientId], [OrderTypeId], [Comment], [TotalCalories], [DiscountId], [TotalPrice], [StatusID], [PickupAt]) VALUES (191, CAST(N'2026-05-07T15:19:30.8021927' AS DateTime2), 20, 1, N'без лука пж', CAST(1.00 AS Decimal(10, 2)), 1, CAST(143.65 AS Decimal(10, 2)), 2, NULL)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderStatuses] ON 

INSERT [dbo].[OrderStatuses] ([Id], [Name]) VALUES (1, N'В процессе')
INSERT [dbo].[OrderStatuses] ([Id], [Name]) VALUES (2, N'Отменен')
INSERT [dbo].[OrderStatuses] ([Id], [Name]) VALUES (3, N'Готов')
SET IDENTITY_INSERT [dbo].[OrderStatuses] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderToppingItems] ON 

INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (8, 9, 1, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (9, 10, 3, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (10, 11, 1, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (11, 16, 2, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (12, 16, 14, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (13, 16, 14, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (14, 22, 7, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (15, 25, 2, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (16, 185, 1, 1, CAST(0.00 AS Decimal(18, 2)), 0)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (17, 185, 12, 1, CAST(0.00 AS Decimal(18, 2)), 1)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (18, 186, 14, 1, CAST(0.00 AS Decimal(18, 2)), 1)
INSERT [dbo].[OrderToppingItems] ([Id], [OrderId], [ToppingId], [Quantity], [TotalPrice], [IsCompleted]) VALUES (19, 186, 17, 1, CAST(0.00 AS Decimal(18, 2)), 1)
SET IDENTITY_INSERT [dbo].[OrderToppingItems] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderTypes] ON 

INSERT [dbo].[OrderTypes] ([Id], [Name]) VALUES (1, N'В ресторане')
INSERT [dbo].[OrderTypes] ([Id], [Name]) VALUES (2, N'С собой')
SET IDENTITY_INSERT [dbo].[OrderTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Preparations] ON 

INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (8, N' Тесто на чечевич.вафлю ', 35, CAST(1000.00 AS Decimal(10, 2)), CAST(N'2026-04-14' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (10, N' Кокосовая сгущенка ', 44, CAST(940.00 AS Decimal(10, 2)), CAST(N'2026-04-06' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (21, N' Тесто на вафлю из кабачка ', 34, CAST(1500.00 AS Decimal(10, 2)), CAST(N'2026-04-29' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (22, N' Зеленое масло ', 53, CAST(280.00 AS Decimal(10, 2)), CAST(N'2026-04-29' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (29, N' Апельсиновый сироп ', 245, CAST(1000.00 AS Decimal(10, 2)), CAST(N'2026-05-01' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (30, N'Песто', 294, CAST(1990.00 AS Decimal(10, 2)), CAST(N'2026-05-01' AS Date))
INSERT [dbo].[Preparations] ([Id], [Name], [SemiFinishedId], [StockGrams], [ProductionDate]) VALUES (48, N' Томатный суп с арахисовой пастой основа ', 41, CAST(3000.00 AS Decimal(10, 2)), CAST(N'2026-05-07' AS Date))
SET IDENTITY_INSERT [dbo].[Preparations] OFF
GO
SET IDENTITY_INSERT [dbo].[PreparationTasks] ON 

INSERT [dbo].[PreparationTasks] ([Id], [SemiFinishedId], [Comment], [CreatedAt], [TaskText]) VALUES (73, 44, N'Рекомендовано системой', CAST(N'2026-04-30T22:56:00.8857219' AS DateTime2), N' Кокосовая сгущенка ')
INSERT [dbo].[PreparationTasks] ([Id], [SemiFinishedId], [Comment], [CreatedAt], [TaskText]) VALUES (76, 35, N'Рекомендовано системой', CAST(N'2026-04-30T22:56:00.8857219' AS DateTime2), N' Тесто на чечевич.вафлю ')
INSERT [dbo].[PreparationTasks] ([Id], [SemiFinishedId], [Comment], [CreatedAt], [TaskText]) VALUES (88, 141, NULL, CAST(N'2026-04-29T17:31:33.4487733' AS DateTime2), N'Гречневая соба отварная')
SET IDENTITY_INSERT [dbo].[PreparationTasks] OFF
GO
SET IDENTITY_INSERT [dbo].[SemiFinished] ON 

INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (6, N' Кешью замоченный ', CAST(735.68 AS Decimal(10, 2)), 12, 5, 103, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (9, N' Орехи с чесноком ', CAST(345.42 AS Decimal(10, 2)), 12, 5, 232, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (10, N' Семена подсолнечника ', CAST(86.68 AS Decimal(10, 2)), 12, 5, 275, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (13, N' Апельсин очищенный ', CAST(320.57 AS Decimal(10, 2)), 11, 5, 9, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (14, N' Апельсиновые чипсы сушеные ', CAST(1455.98 AS Decimal(10, 2)), 11, 5, 11, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (15, N' Бананы очищенные ', CAST(317.99 AS Decimal(10, 2)), 11, 5, 26, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (23, N' Лимон ', CAST(403.69 AS Decimal(10, 2)), 11, 5, 154, CAST(0.00 AS Decimal(8, 2)), CAST(0.20 AS Decimal(8, 2)), CAST(0.50 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (27, N' Цедра апельсина', CAST(0.00 AS Decimal(10, 2)), 11, 5, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (34, N' Тесто на вафлю из кабачка ', CAST(175.54 AS Decimal(10, 2)), 13, 5, 343, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (35, N' Тесто на чечевич.вафлю ', CAST(147.40 AS Decimal(10, 2)), 13, 5, 348, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (39, N' Суп грибной ', CAST(260.10 AS Decimal(10, 2)), 9, 5, 325, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (41, N' Томатный суп с арахисовой пастой основа ', CAST(176.35 AS Decimal(10, 2)), 9, 5, 350, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (44, N' Кокосовая сгущенка ', CAST(1059.15 AS Decimal(10, 2)), 14, 5, 274, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (45, N' Облепиховый гель ', CAST(393.96 AS Decimal(10, 2)), 14, 5, 212, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (47, N' Соус "Манго-мята" ', CAST(649.28 AS Decimal(10, 2)), 14, 5, 310, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (49, N' Сироп топинамбура ', CAST(275.71 AS Decimal(10, 2)), 14, 5, 290, CAST(0.00 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(13.00 AS Decimal(8, 2)), CAST(55.00 AS Decimal(8, 2)), CAST(220.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (50, N' Арахисовая паста ', CAST(556.50 AS Decimal(10, 2)), 14, 5, 20, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (51, N' Заправка для салата с тыквой ', CAST(826.56 AS Decimal(10, 2)), 8, 5, 75, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (52, N' Заправка из сока лимона ', CAST(1098.57 AS Decimal(10, 2)), 8, 5, 76, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (53, N' Зеленое масло ', CAST(327.50 AS Decimal(10, 2)), 8, 5, 79, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (57, N' Майонез ', CAST(188.97 AS Decimal(10, 2)), 8, 5, 163, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (62, N' Соус горчично-сырный ', CAST(391.59 AS Decimal(10, 2)), 8, 5, 312, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (63, N' Соус для буритто ', CAST(196.78 AS Decimal(10, 2)), 8, 5, 313, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (64, N' Соус имбирный ', CAST(243.74 AS Decimal(10, 2)), 8, 5, 314, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (66, N' Соус Сладкий Чили ', CAST(176.65 AS Decimal(10, 2)), 8, 5, 316, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (69, N' Соус томатный на пасту ', CAST(208.76 AS Decimal(10, 2)), 8, 5, 319, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (72, N' Брокколи ', CAST(306.15 AS Decimal(10, 2)), 7, 5, 41, CAST(28.00 AS Decimal(8, 2)), CAST(11.00 AS Decimal(8, 2)), CAST(14.00 AS Decimal(8, 2)), CAST(350.00 AS Decimal(8, 2)), CAST(1470.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (74, N' Вешенки ', CAST(467.99 AS Decimal(10, 2)), 7, 5, 49, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (76, N' Кабачки ', CAST(220.32 AS Decimal(10, 2)), 7, 5, 89, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (77, N' Капуста белокачанная очищенная ', CAST(69.00 AS Decimal(10, 2)), 7, 5, 92, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (78, N' Капуста с морковью ферментированная ', CAST(71.99 AS Decimal(10, 2)), 7, 5, 94, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (79, N' Картофель отварной ', CAST(117.45 AS Decimal(10, 2)), 7, 5, 96, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (80, N' Картофель очищенный ', CAST(96.27 AS Decimal(10, 2)), 7, 5, 97, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (81, N' Корень имбиря очищенный ', CAST(530.92 AS Decimal(10, 2)), 7, 5, 136, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (82, N' Лук красный маринованный ', CAST(213.87 AS Decimal(10, 2)), 7, 5, 158, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (83, N' Лук красный очищенный ', CAST(116.58 AS Decimal(10, 2)), 7, 5, 159, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (85, N' Лук репчатый очищенный ', CAST(77.18 AS Decimal(10, 2)), 7, 5, 161, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (86, N' Морковь маринованная ', CAST(139.33 AS Decimal(10, 2)), 7, 5, 191, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (88, N' Морковь очищенная ', CAST(73.33 AS Decimal(10, 2)), 7, 5, 193, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (92, N' Огурец свежий ', CAST(165.15 AS Decimal(10, 2)), 7, 5, 225, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (94, N' Перец болгарский запечный ', CAST(454.86 AS Decimal(10, 2)), 7, 5, 239, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (95, N' Перец болгарский очищ ', CAST(324.66 AS Decimal(10, 2)), 7, 5, 240, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (96, N' Перец запечный маринованный ', CAST(493.33 AS Decimal(10, 2)), 7, 5, 241, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (98, N' Редис очищ ', CAST(193.76 AS Decimal(10, 2)), 7, 5, 255, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (100, N' Свекла запеченая ', CAST(92.52 AS Decimal(10, 2)), 7, 5, 268, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (102, N' Свекла очищенная ', CAST(71.49 AS Decimal(10, 2)), 7, 5, 270, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (104, N' Стебель сельдерея ', CAST(236.59 AS Decimal(10, 2)), 7, 5, 324, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (105, N' Томаты черри ', CAST(396.90 AS Decimal(10, 2)), 7, 5, 352, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (106, N' Черри конфи ', CAST(762.84 AS Decimal(10, 2)), 7, 5, 395, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (107, N' Чеснок очищенный ', CAST(318.47 AS Decimal(10, 2)), 7, 5, 396, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (109, N' Шампиньоны очищенные ', CAST(436.86 AS Decimal(10, 2)), 7, 5, 406, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (111, N' Свекольный сыр ', CAST(365.85 AS Decimal(10, 2)), 7, 5, 271, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (125, N' Глазной мускул ', CAST(1835.79 AS Decimal(10, 2)), 3, 5, 57, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (126, N' Индейка ', CAST(600.07 AS Decimal(10, 2)), 3, 5, 84, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (127, N' Индейка су-вид ', CAST(703.06 AS Decimal(10, 2)), 3, 5, 87, CAST(3.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), CAST(0.20 AS Decimal(8, 2)), CAST(50.00 AS Decimal(8, 2)), CAST(210.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (129, N' Креветки ', CAST(1450.72 AS Decimal(10, 2)), 3, 5, 142, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (132, N' Ростбиф ', CAST(1206.86 AS Decimal(10, 2)), 3, 5, 262, CAST(2.00 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(1.50 AS Decimal(8, 2)), CAST(25.00 AS Decimal(8, 2)), CAST(110.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (137, N' Супермясо ', CAST(623.57 AS Decimal(10, 2)), 3, 5, 327, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (139, N' Геркулес отварной ', CAST(42.83 AS Decimal(10, 2)), 6, 5, 56, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (141, N' Гречневая соба отварная ', CAST(144.55 AS Decimal(10, 2)), 6, 5, 66, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (144, N' Каша пшенная отварная ', CAST(21.66 AS Decimal(10, 2)), 6, 5, 99, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (145, N' Киноа отварная ', CAST(154.31 AS Decimal(10, 2)), 6, 5, 111, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (154, N' Цельнозерновая паста отварная ', CAST(70.27 AS Decimal(10, 2)), 6, 5, 387, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (155, N' Чечевица замочен. ', CAST(91.07 AS Decimal(10, 2)), 6, 5, 397, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (161, N' Арахисовый слой ', CAST(416.77 AS Decimal(10, 2)), 4, 5, 21, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (163, N' Бисквит на торт прагу ', CAST(579.16 AS Decimal(10, 2)), 4, 5, 37, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (166, N' Ганаш взбитый  ', CAST(892.67 AS Decimal(10, 2)), 4, 5, 55, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (168, N' Глазурь ', CAST(1462.05 AS Decimal(10, 2)), 4, 5, 59, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (170, N' Заварной крем ', CAST(331.24 AS Decimal(10, 2)), 4, 5, 73, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (172, N' Йогурт v 1.1 ', CAST(509.38 AS Decimal(10, 2)), 4, 5, 88, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (173, N' Карамель ', CAST(508.91 AS Decimal(10, 2)), 4, 5, 95, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (180, N' Конфитюр из маракуйи ', CAST(788.12 AS Decimal(10, 2)), 4, 5, 135, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (190, N' Масса на арахисовое печенье ', CAST(560.48 AS Decimal(10, 2)), 4, 5, 172, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (200, N' Основа пирожное картошка ', CAST(571.34 AS Decimal(10, 2)), 4, 5, 234, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (209, N' Суфле ', CAST(258.40 AS Decimal(10, 2)), 4, 5, 330, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (227, N' Чиа масса кокосовая  ', CAST(270.11 AS Decimal(10, 2)), 4, 5, 398, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (228, N' Чиа масса овсяная ', CAST(158.26 AS Decimal(10, 2)), 4, 5, 399, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (238, N' Мята свежая ', CAST(1232.51 AS Decimal(10, 2)), 1, 5, 203, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (239, N' Петрушка ', CAST(316.51 AS Decimal(10, 2)), 1, 5, 245, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (240, N' Романо ', CAST(561.60 AS Decimal(10, 2)), 1, 5, 259, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (242, N' Укроп ', CAST(425.71 AS Decimal(10, 2)), 1, 5, 366, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (243, N' Шпинат ', CAST(704.90 AS Decimal(10, 2)), 1, 5, 605, CAST(0.00 AS Decimal(8, 2)), CAST(0.30 AS Decimal(8, 2)), CAST(0.20 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (245, N' Апельсиновый сироп ', CAST(249.17 AS Decimal(10, 2)), 16, 5, 14, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(13.00 AS Decimal(8, 2)), CAST(50.00 AS Decimal(8, 2)), CAST(220.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (264, N' Пюре манго ', CAST(725.82 AS Decimal(10, 2)), 16, 5, 253, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (265, N' Пюре маракуйа ', CAST(1024.88 AS Decimal(10, 2)), 16, 5, 254, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (273, N' Сок лимона ', CAST(980.90 AS Decimal(10, 2)), 16, 5, 306, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (274, N' Солёная карамель ', CAST(567.13 AS Decimal(10, 2)), 16, 5, 309, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (286, N' Вода ', CAST(0.00 AS Decimal(10, 2)), 17, 5, 53, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (287, N' Кокосовое молоко Aroy-D ', CAST(435.42 AS Decimal(10, 2)), 17, 5, 128, CAST(185.00 AS Decimal(8, 2)), CAST(16.00 AS Decimal(8, 2)), CAST(20.00 AS Decimal(8, 2)), CAST(1810.00 AS Decimal(8, 2)), CAST(7570.00 AS Decimal(8, 2)))
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (288, N' Лёд ', CAST(0.00 AS Decimal(10, 2)), 17, 5, 150, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[SemiFinished] ([Id], [Name], [CostRub], [CategoryId], [UnitOfMeasureId], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules]) VALUES (294, N'Песто', NULL, 8, 5, 612, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SemiFinished] OFF
GO
SET IDENTITY_INSERT [dbo].[SemiFinishedCategories] ON 

INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (1, N'Зелень')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (2, N'Заморозка')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (3, N'Мясо')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (4, N'Кондитерка')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (5, N'Консервация')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (6, N'Крупы и каши')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (7, N'Овощные')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (8, N'Соуса')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (9, N'Супы')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (10, N'Хлеб')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (11, N'Фрукты и ягоды')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (12, N'Орехи')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (13, N'Тесто')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (14, N'Топинги')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (15, N'Напитки')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (16, N'Бар')
INSERT [dbo].[SemiFinishedCategories] ([Id], [Name]) VALUES (17, N'Другое')
SET IDENTITY_INSERT [dbo].[SemiFinishedCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[SemiFinishedWriteOffActItems] ON 

INSERT [dbo].[SemiFinishedWriteOffActItems] ([Id], [WriteOffActId], [SemiFinishedId], [Quantity], [UnitOfMeasureId], [WriteOffTypeId]) VALUES (1, 2, 44, CAST(50.00 AS Decimal(10, 2)), 2, 2)
SET IDENTITY_INSERT [dbo].[SemiFinishedWriteOffActItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Staff] ON 

INSERT [dbo].[Staff] ([Id], [RoleId], [FullName], [Login], [Password]) VALUES (1, 1, N'Косухин Семён', N'cook', N'123')
INSERT [dbo].[Staff] ([Id], [RoleId], [FullName], [Login], [Password]) VALUES (2, 2, N'Настя Шутова', N'admin', N'123')
INSERT [dbo].[Staff] ([Id], [RoleId], [FullName], [Login], [Password]) VALUES (3, 3, N'Арина Высоцкая', N'bar', N'123')
SET IDENTITY_INSERT [dbo].[Staff] OFF
GO
SET IDENTITY_INSERT [dbo].[StaffRoles] ON 

INSERT [dbo].[StaffRoles] ([Id], [Name]) VALUES (1, N'Повар')
INSERT [dbo].[StaffRoles] ([Id], [Name]) VALUES (2, N'Администратор')
INSERT [dbo].[StaffRoles] ([Id], [Name]) VALUES (3, N'Бариста')
SET IDENTITY_INSERT [dbo].[StaffRoles] OFF
GO
SET IDENTITY_INSERT [dbo].[TechnicalCardIngredientComposition] ON 

INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (4, 14, 237, 5, CAST(0.651042 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.651000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.651000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (6, 21, 240, 5, CAST(0.004054 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.004000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (10, 26, 256, 5, CAST(1.700000 AS Decimal(10, 6)), CAST(41.18 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (25, 37, 207, 5, CAST(0.070588 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.071000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.071000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (26, 37, 93, 5, CAST(0.411765 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.412000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.412000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (27, 37, 235, 5, CAST(0.017647 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.018000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.018000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (28, 37, 97, 5, CAST(0.123529 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.124000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.124000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (29, 37, 118, 5, CAST(0.705882 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.706000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.706000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (30, 37, 105, 5, CAST(0.076471 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.076000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.076000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (35, 41, 29, 5, CAST(1.538462 AS Decimal(10, 6)), CAST(33.50 AS Decimal(10, 2)), CAST(1.023000 AS Decimal(10, 6)), CAST(2.26 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (63, 434, 6, 2, CAST(18.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(18.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(18.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (64, 580, 6, 2, CAST(18.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(18.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(18.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (65, 580, 113, 3, CAST(170.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(170.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(170.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (71, 103, 152, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (77, 11, 255, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (83, 154, 261, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (89, 20, 182, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (93, 49, 16, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (94, 89, 125, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (95, 92, 126, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (96, 97, 129, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (97, 136, 130, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (98, 159, 132, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (100, 161, 133, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (101, 193, 134, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (102, 225, 135, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (103, 239, 136, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (104, 240, 136, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (106, 255, 139, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (108, 270, 142, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (109, 324, 143, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (110, 352, 144, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (111, 396, 146, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (112, 406, 17, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (114, 57, 18, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (115, 84, 19, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (117, 142, 21, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (122, 56, 69, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (123, 66, 73, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (125, 111, 75, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (131, 397, 92, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (153, 203, 49, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (154, 245, 50, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (155, 259, 52, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (157, 366, 56, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (158, 605, 57, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (165, 53, 334, 6, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (166, 128, 111, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (167, 150, 331, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (171, 75, 104, 5, CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (172, 75, 185, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (173, 75, 191, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (174, 75, 211, 5, CAST(0.008000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.008000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.008000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (176, 254, 63, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (177, 274, 111, 5, CAST(3.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(3.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(3.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (178, 274, 237, 5, CAST(0.600000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.600000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.600000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (180, 399, 106, 5, CAST(2.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(2.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(2.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (181, 399, 111, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (182, 399, 166, 5, CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (183, 59, 314, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (184, 253, 62, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (185, 290, 183, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (191, 476, 26, 4, CAST(2.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(2.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(2.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (192, 476, 100, 5, CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (193, 476, 125, 5, CAST(0.700000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.700000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.700000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (194, 476, 211, 5, CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (195, 476, 215, 5, CAST(0.008000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.008000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.008000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (196, 476, 240, 5, CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (197, 612, 164, 5, CAST(0.150000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.150000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.150000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (198, 612, 57, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (199, 612, 45, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (200, 612, 104, 5, CAST(0.250000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.250000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.250000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (201, 612, 204, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (202, 612, 192, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (203, 612, 334, 5, CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (204, 612, 150, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (205, 612, 240, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardIngredientComposition] ([Id], [TechnicalCardId], [IngredientId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (206, 9, 255, 2, CAST(1000.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1000.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(990.000000 AS Decimal(10, 6)))
SET IDENTITY_INSERT [dbo].[TechnicalCardIngredientComposition] OFF
GO
SET IDENTITY_INSERT [dbo].[TechnicalCards] ON 

INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (9, N'+ Апельсин очищенный п/ф', N'Рецептура: апельсин свежий. Алгоритм приготовления: 1. Промыть апельсин, обсушить и снять кожуру с белыми прожилками. 2. Разделить на дольки или нарезать по назначению. 3. Хранить в закрытой гастроемкости в холодильнике.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (11, N'+ Апельсиновые чипсы сушеные п/ф', N'+ Апельсиновые чипсы сушеные п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (14, N'+ Апельсиновый сироп п/ф', N'Рецептура: апельсиновый сок, сахар, цедра апельсина. Алгоритм приготовления: 1. Соединить сок с сахаром и цедрой, прогреть до растворения сахара. 2. Уварить до легкой сиропной консистенции. 3. Процедить, охладить и хранить в чистой таре.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (20, N'+ Арахисовая паста п/ф', N'+ Арахисовая паста п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (21, N'+ Арахисовый слой п/ф', N'+ Арахисовый слой п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (26, N'+ Бананы очищенные п/ф', N'Рецептура: бананы спелые. Алгоритм приготовления: 1. Промыть бананы, очистить от кожуры и удалить поврежденные участки. 2. Нарезать по требуемому формату или оставить целыми для дальнейшей обработки. 3. Использовать сразу либо хранить кратковременно в закрытой таре.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (37, N'+ Бисквит на торт прагу п/ф', N'+ Бисквит на торт прагу п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (41, N'+ Брокколи п/ф', N'Рецептура: брокколи свежая или замороженная, вода, соль. Алгоритм приготовления: 1. Разобрать брокколи на соцветия и промыть. 2. Бланшировать в подсоленной воде до полуготовности. 3. Быстро охладить, обсушить и хранить в холодильнике.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (49, N'+ Вешенки п/ф', N'+ Вешенки п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (53, N'+ Вода п/ф', N'+ Вода п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (55, N'+ Ганаш взбитый  п/ф', N'+ Ганаш взбитый  п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (56, N'+ Геркулес отварной п/ф', N'+ Геркулес отварной п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (57, N'+ Глазной мускул п/ф', N'+ Глазной мускул п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (59, N'+ Глазурь п/ф', N'+ Глазурь п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (66, N'+ Гречневая соба отварная п/ф', N'Рецептура: лапша соба, вода, соль, растительное масло. Алгоритм приготовления: 1. Отварить собу в кипящей подсоленной воде до готовности. 2. Промыть холодной водой и дать стечь. 3. Заправить небольшим количеством масла, перемешать и охладить.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (73, N'+ Заварной крем п/ф', N'+ Заварной крем п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (75, N'+ Заправка для салата с тыквой п/ф', N'+ Заправка для салата с тыквой п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (76, N'+ Заправка из сока лимона п/ф', N'+ Заправка из сока лимона п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (79, N'+ Зеленое масло п/ф', N'+ Зеленое масло п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (84, N'+ Индейка п/ф', N'+ Индейка п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (87, N'+ Индейка су-вид п/ф', N'+ Индейка су-вид п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (88, N'+ Йогурт v 1.1 п/ф', N'+ Йогурт v 1.1 п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (89, N'+ Кабачки п/ф', N'+ Кабачки п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (92, N'+ Капуста белокачанная очищенная п/ф', N'+ Капуста белокачанная очищенная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (94, N'+ Капуста с морковью ферментированная п/ф', N'+ Капуста с морковью ферментированная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (95, N'+ Карамель п/ф', N'+ Карамель п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (96, N'+ Картофель отварной п/ф', N'Рецептура: картофель, вода, соль. Алгоритм приготовления: 1. Промыть и очистить картофель, удалить дефекты. 2. Отварить в подсоленной воде до готовности. 3. Слить воду, охладить и хранить в закрытой таре.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (97, N'+ Картофель очищенный п/ф', N'+ Картофель очищенный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (99, N'+ Каша пшенная отварная п/ф', N'+ Каша пшенная отварная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (103, N'+ Кешью замоченный п/ф', N'Рецептура: кешью, питьевая вода. Алгоритм приготовления: 1. Перебрать кешью и промыть. 2. Залить холодной питьевой водой и выдержать до размягчения. 3. Слить воду, промыть и использовать для соусов, кремов или начинок.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (111, N'+ Киноа отварная п/ф', N'Рецептура: киноа, вода, соль. Алгоритм приготовления: 1. Тщательно промыть киноа до прозрачной воды. 2. Отварить в подсоленной воде до мягкости и раскрытия зерна. 3. Дать настояться, разрыхлить и охладить.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (128, N'+ Кокосовое молоко Aroy-D п/ф', N'+ Кокосовое молоко Aroy-D п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (135, N'+ Конфитюр из маракуйи п/ф', N'+ Конфитюр из маракуйи п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (136, N'+ Корень имбиря очищенный п/ф', N'+ Корень имбиря очищенный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (142, N'+ Креветки п/ф', N'+ Креветки п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (150, N'+ Лёд п/ф', N'+ Лёд п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (154, N'+ Лимон п/ф', N'Рецептура: лимон свежий. Алгоритм приготовления: 1. Промыть лимон с щеткой и обсушить. 2. Нарезать дольками, кружками или подготовить цедру по назначению. 3. Хранить в закрытой таре отдельно от продуктов с сильным запахом.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (158, N'+ Лук красный маринованный п/ф', N'+ Лук красный маринованный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (159, N'+ Лук красный очищенный п/ф', N'+ Лук красный очищенный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (161, N'+ Лук репчатый очищенный п/ф', N'+ Лук репчатый очищенный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (163, N'+ Майонез п/ф', N'+ Майонез п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (172, N'+ Масса на арахисовое печенье п/ф', N'+ Масса на арахисовое печенье п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (191, N'+ Морковь маринованная п/ф', N'+ Морковь маринованная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (193, N'+ Морковь очищенная п/ф', N'Рецептура: морковь свежая. Алгоритм приготовления: 1. Промыть морковь, очистить и удалить поврежденные участки. 2. Повторно ополоснуть и обсушить. 3. Нарезать по назначению либо хранить целой в закрытой таре.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (203, N'+ Мята свежая п/ф', N'+ Мята свежая п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (212, N'+ Облепиховый гель п/ф', N'+ Облепиховый гель п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (225, N'+ Огурец свежий п/ф', N'+ Огурец свежий п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (232, N'+ Орехи с чесноком п/ф', N'+ Орехи с чесноком п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (234, N'+ Основа пирожное картошка п/ф', N'+ Основа пирожное картошка п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (239, N'+ Перец болгарский запечный п/ф', N'+ Перец болгарский запечный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (240, N'+ Перец болгарский очищ п/ф', N'+ Перец болгарский очищ п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (241, N'+ Перец запечный маринованный п/ф', N'+ Перец запечный маринованный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (245, N'+ Петрушка п/ф', N'+ Петрушка п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (253, N'+ Пюре манго п/ф', N'+ Пюре манго п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (254, N'+ Пюре маракуйа п/ф', N'+ Пюре маракуйа п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (255, N'+ Редис очищ п/ф', N'+ Редис очищ п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (259, N'+ Романо п/ф', N'+ Романо п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (262, N'+ Ростбиф п/ф', N'+ Ростбиф п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (268, N'+ Свекла запеченая п/ф', N'+ Свекла запеченая п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (270, N'+ Свекла очищенная п/ф', N'+ Свекла очищенная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (271, N'+ Свекольный сыр п/ф', N'+ Свекольный сыр п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (274, N'+ Сгущенка 30 г', N'+ Сгущенка 30 г')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (275, N'+ Семена подсолнечника п/ф', N'+ Семена подсолнечника п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (290, N'+ Сироп топинамбура п/ф', N'+ Сироп топинамбура п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (306, N'+ Сок лимона п/ф', N'+ Сок лимона п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (309, N'+ Солёная карамель п/ф', N'+ Солёная карамель п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (310, N'+ Соус "Манго-мята" п/ф', N'+ Соус "Манго-мята" п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (312, N'+ Соус горчично-сырный п/ф', N'+ Соус горчично-сырный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (313, N'+ Соус для буритто п/ф', N'+ Соус для буритто п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (314, N'+ Соус имбирный п/Ф', N'+ Соус имбирный п/Ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (316, N'+ Соус Сладкий Чили п/ф', N'+ Соус Сладкий Чили п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (319, N'+ Соус томатный на пасту п/ф', N'+ Соус томатный на пасту п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (324, N'+ Стебель сельдерея п/ф', N'+ Стебель сельдерея п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (325, N'+ Суп грибной п/ф', N'+ Суп грибной п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (327, N'+ Супермясо п/ф', N'+ Супермясо п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (330, N'+ Суфле п/ф', N'+ Суфле п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (343, N'+ Тесто на вафлю из кабачка п/ф', N'+ Тесто на вафлю из кабачка п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (348, N'+ Тесто на чечевич.вафлю п/ф', N'+ Тесто на чечевич.вафлю п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (350, N'+ Томатный суп с арахисовой пастой основа п/ф', N'+ Томатный суп с арахисовой пастой основа п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (352, N'+ Томаты черри п/ф', N'+ Томаты черри п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (366, N'+ Укроп п/ф', N'+ Укроп п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (387, N'+ Цельнозерновая паста отварная п/ф', N'+ Цельнозерновая паста отварная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (395, N'+ Черри конфи п/ф', N'+ Черри конфи п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (396, N'+ Чеснок очищенный п/ф', N'+ Чеснок очищенный п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (397, N'+ Чечевица замочен. п/ф', N'+ Чечевица замочен. п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (398, N'+ Чиа масса кокосовая  п/ф', N'+ Чиа масса кокосовая  п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (399, N'+ Чиа масса овсяная п/ф', N'+ Чиа масса овсяная п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (406, N'+ Шампиньоны очищенные п/ф', N'+ Шампиньоны очищенные п/ф')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (429, N'Айс Латте солёная карамель', N'Рецептура: эспрессо, молоко, лед, сироп соленая карамель. Алгоритм приготовления: 1. Наполнить стакан льдом и влить сироп. 2. Добавить охлажденное молоко. 3. Приготовить эспрессо и аккуратно влить сверху, перемешать перед отдачей.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (434, N'Американо 250 мл', N'Рецептура: эспрессо, горячая вода. Алгоритм приготовления: 1. Прогреть чашку. 2. Влить горячую воду. 3. Приготовить порцию эспрессо и добавить в чашку, подать сразу.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (435, N'Апельсиновый Какао 350 мл', N'Апельсиновый Какао 350 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (437, N'Апельсиновый фреш 400 мл', N'Апельсиновый фреш 400 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (438, N'Арахисовое печенье', N'Арахисовое печенье')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (442, N'Безглютеновая  вафля с креветкой и яйцом пашот', N'Рецептура: безглютеновая вафля, креветки, яйцо пашот, соус, зелень. Алгоритм приготовления: 1. Выпечь или разогреть вафлю до хрустящей корочки. 2. Обжарить креветки до готовности. 3. Выложить креветки и яйцо пашот на вафлю, добавить соус и зелень.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (451, N'Буррито с индейкой и овощами', N'Рецептура: тортилья, индейка, овощи, соус для буррито, зелень. Алгоритм приготовления: 1. Прогреть тортилью и подготовить начинку. 2. Выложить индейку, овощи и соус, свернуть плотный ролл. 3. Подрумянить буррито на гриле и разрезать перед подачей.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (452, N'Буррито с креветкой в соусе сладкий чили', N'Буррито с креветкой в соусе сладкий чили')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (457, N'Вафля со свекольным сыром и имбирным соусом', N'Вафля со свекольным сыром и имбирным соусом')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (466, N'Гречневая соба с креветками', N'Гречневая соба с креветками')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (468, N'Завтрак Скрембл Конструктор', N'Завтрак Скрембл Конструктор')
GO
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (471, N'Зеленый салат VEGAN', N'Зеленый салат VEGAN')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (473, N'Зеленый смузи', N'Зеленый смузи')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (476, N'Кабачковая вафля с песто,индейкой и яйцом пашот', N'Кабачковая вафля с песто,индейкой и яйцом пашот')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (478, N'Какао 350 мл', N'Рецептура: молоко, какао-порошок, сахар или сироп. Алгоритм приготовления: 1. Смешать какао с небольшим количеством горячего молока до однородности. 2. Добавить остальное молоко и прогреть паром. 3. Перелить в стакан и подать горячим.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (482, N'Капучино 450 мл', N'Рецептура: эспрессо, молоко. Алгоритм приготовления: 1. Приготовить эспрессо в прогретую чашку. 2. Взбить молоко паром до плотной мелкой пены. 3. Влить молоко в эспрессо, сформировать ровный слой пены и подать.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (486, N'Каша овсяная', N'Каша овсяная')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (499, N'Кокосовый снежок с манго', N'Кокосовый снежок с манго')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (518, N'Латте 450 мл', N'Рецептура: эспрессо, молоко. Алгоритм приготовления: 1. Приготовить эспрессо. 2. Взбить молоко до мягкой эластичной пены. 3. Соединить молоко с эспрессо в высоком стакане, оставить тонкий слой пены сверху.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (528, N'Морковный фреш 400 мл', N'Морковный фреш 400 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (541, N'Пирожное Картошка', N'Пирожное Картошка')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (542, N'Пряный томатный суп', N'Рецептура: томатная основа, овощи, специи, зелень, масло. Алгоритм приготовления: 1. Прогреть томатную основу с овощами и специями. 2. Довести до кипения и проварить до насыщенного вкуса. 3. При необходимости пробить блендером, довести по соли и подать с зеленью.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (543, N'Птичье молоко', N'Птичье молоко')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (545, N'Пшенная каша с тыквой и облепиховым мусом', N'Пшенная каша с тыквой и облепиховым мусом')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (550, N'Раф Соленая карамель 450 мл', N'Раф Соленая карамель 450 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (551, N'Раф Соленый арахис 350 мл', N'Раф Соленый арахис 350 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (554, N'Салат Veg c тыквой и киноа', N'Салат Veg c тыквой и киноа')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (556, N'Салат с ростбифом и картофелем', N'Салат с ростбифом и картофелем')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (560, N'Суп-пюре грибной', N'Суп-пюре грибной')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (563, N'Сырники со сгущенкой и манговым кремом', N'Сырники со сгущенкой и манговым кремом')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (566, N'Тарт шоколадный со смородиной (целый)', N'Тарт шоколадный со смородиной (целый)')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (576, N'Фетучини с томатным соусом растительным фаршем, овощами и салатом', N'Фетучини с томатным соусом растительным фаршем, овощами и салатом')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (580, N'Флэт уайт 250 мл', N'Флэт уайт 250 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (590, N'Чай Ромашковый 500 мл', N'Чай Ромашковый 500 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (591, N'Чай травяной 500 мл', N'Чай травяной 500 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (592, N'Чай черный 500 мл', N'Чай черный 500 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (594, N'Чиа пудинг Йогурт - маракуйя', N'Рецептура: семена чиа, йогурт, молочная или растительная основа, конфитюр маракуйи. Алгоритм приготовления: 1. Смешать чиа с йогуртовой основой и оставить для набухания. 2. Перемешать до однородной текстуры. 3. Выложить порцию слоями с маракуйей и охладить перед подачей.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (595, N'Чиа пудинг Облепиха манго', N'Чиа пудинг Облепиха манго')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (596, N'Чиа пудинг Сникерс', N'Рецептура: семена чиа, молочная или растительная основа, арахисовая паста, карамель, орехи, шоколад. Алгоритм приготовления: 1. Замочить чиа в основе до густой консистенции. 2. Добавить арахисовый слой и карамель. 3. Оформить орехами и шоколадом, охладить перед отдачей.')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (605, N'Шпинат 10 г', N'Шпинат 10 г')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (611, N'Яблочный фреш 400 мл', N'Яблочный фреш 400 мл')
INSERT [dbo].[TechnicalCards] ([Id], [Name], [Description]) VALUES (612, N'Песто', N'Песто')
SET IDENTITY_INSERT [dbo].[TechnicalCards] OFF
GO
SET IDENTITY_INSERT [dbo].[TechnicalCardSemiFinishedComposition] ON 

INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (12, 14, 286, 5, CAST(0.348958 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.349000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.349000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (17, 21, 15, 5, CAST(0.472973 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.473000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.473000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (18, 21, 287, 5, CAST(0.222973 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.223000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.223000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (19, 21, 50, 5, CAST(0.304054 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.304000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.304000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (28, 37, 49, 5, CAST(0.205882 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.206000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.206000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (51, 55, 287, 5, CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (67, 73, 287, 5, CAST(0.510204 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.510000 AS Decimal(10, 6)), CAST(4.00 AS Decimal(10, 2)), CAST(0.490000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (68, 73, 49, 5, CAST(0.122449 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.122000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (69, 76, 273, 5, CAST(0.214286 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.214000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.214000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (71, 79, 239, 5, CAST(0.142857 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.143000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (72, 79, 242, 5, CAST(0.142857 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.143000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (85, 87, 126, 5, CAST(1.125000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.125000 AS Decimal(10, 6)), CAST(11.11 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (86, 87, 107, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (87, 88, 287, 5, CAST(1.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.100000 AS Decimal(10, 6)), CAST(9.09 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (90, 94, 81, 5, CAST(0.012017 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.012000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.012000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (91, 94, 77, 5, CAST(0.600858 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.601000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.601000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (92, 94, 88, 5, CAST(0.128755 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.129000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.129000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (93, 94, 286, 5, CAST(0.214592 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.215000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.215000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (94, 95, 49, 5, CAST(0.461538 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.462000 AS Decimal(10, 6)), CAST(33.33 AS Decimal(10, 2)), CAST(0.308000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (95, 95, 287, 5, CAST(0.538462 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.538000 AS Decimal(10, 6)), CAST(28.57 AS Decimal(10, 2)), CAST(0.385000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (96, 96, 80, 5, CAST(1.220000 AS Decimal(10, 6)), CAST(9.09 AS Decimal(10, 2)), CAST(1.109000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.998000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (103, 99, 286, 5, CAST(0.906977 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.907000 AS Decimal(10, 6)), CAST(22.86 AS Decimal(10, 2)), CAST(0.700000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (140, 135, 265, 5, CAST(0.666667 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.667000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.667000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (141, 135, 286, 5, CAST(0.133333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.133000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.133000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (142, 135, 49, 5, CAST(0.333333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.333000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.267000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (175, 158, 83, 5, CAST(1.180000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(1.062000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.062000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (176, 158, 273, 5, CAST(0.250000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.250000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (178, 163, 273, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (185, 172, 49, 5, CAST(0.320000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.320000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.320000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (186, 172, 50, 5, CAST(0.266000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.266000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.266000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (200, 191, 88, 5, CAST(0.838710 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.839000 AS Decimal(10, 6)), CAST(7.69 AS Decimal(10, 2)), CAST(0.774000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (201, 191, 273, 5, CAST(0.064516 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.065000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.065000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (238, 212, 49, 5, CAST(0.625000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.625000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (239, 212, 286, 5, CAST(0.225000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.225000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (258, 232, 10, 5, CAST(0.256410 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.256000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.256000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (263, 234, 163, 5, CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)), CAST(5.00 AS Decimal(10, 2)), CAST(0.475000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (264, 234, 287, 5, CAST(0.158333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.158000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.143000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (265, 234, 49, 5, CAST(0.166667 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.167000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.167000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (271, 241, 94, 5, CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (288, 262, 125, 5, CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)), CAST(12.50 AS Decimal(10, 2)), CAST(0.438000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (289, 262, 107, 5, CAST(0.021875 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.022000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (290, 262, 85, 5, CAST(0.218750 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.219000 AS Decimal(10, 6)), CAST(42.86 AS Decimal(10, 2)), CAST(0.125000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (299, 268, 102, 5, CAST(1.294118 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.294000 AS Decimal(10, 6)), CAST(22.73 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (301, 271, 6, 5, CAST(0.400000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.400000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.400000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (302, 271, 78, 5, CAST(0.160000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.160000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.160000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (303, 271, 286, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (304, 271, 100, 5, CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (314, 275, 286, 5, CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.200000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (344, 306, 23, 5, CAST(3.983333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(3.983000 AS Decimal(10, 6)), CAST(74.90 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (345, 309, 286, 5, CAST(0.074074 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.074000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (346, 310, 264, 5, CAST(0.625000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.625000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.563000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (347, 310, 287, 5, CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)), CAST(7.00 AS Decimal(10, 2)), CAST(0.279000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (348, 310, 49, 5, CAST(0.175000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.175000 AS Decimal(10, 6)), CAST(6.86 AS Decimal(10, 2)), CAST(0.163000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (349, 310, 238, 5, CAST(0.009375 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.009000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (352, 312, 6, 5, CAST(0.416667 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.417000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.375000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (353, 312, 78, 5, CAST(0.208333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.208000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.208000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (354, 312, 107, 5, CAST(0.012500 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.013000 AS Decimal(10, 6)), CAST(33.33 AS Decimal(10, 2)), CAST(0.008000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (355, 312, 273, 5, CAST(0.041667 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.042000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.033000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (356, 312, 286, 5, CAST(0.283333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.283000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.283000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (357, 313, 57, 5, CAST(0.769231 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.769000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.769000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (358, 314, 81, 5, CAST(0.057333 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.052000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.052000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (359, 314, 107, 5, CAST(0.057333 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(0.052000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.052000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (362, 316, 107, 5, CAST(0.125000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.125000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.125000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (363, 316, 286, 5, CAST(0.375000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.375000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.375000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (364, 316, 286, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (369, 319, 85, 5, CAST(0.323077 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.323000 AS Decimal(10, 6)), CAST(30.00 AS Decimal(10, 2)), CAST(0.226000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (370, 319, 104, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (371, 319, 286, 5, CAST(0.230769 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.231000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.231000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (379, 325, 85, 5, CAST(0.156522 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.157000 AS Decimal(10, 6)), CAST(16.67 AS Decimal(10, 2)), CAST(0.130000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (380, 325, 80, 5, CAST(0.382609 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.383000 AS Decimal(10, 6)), CAST(18.18 AS Decimal(10, 2)), CAST(0.313000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (381, 325, 109, 5, CAST(0.278261 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.278000 AS Decimal(10, 6)), CAST(25.00 AS Decimal(10, 2)), CAST(0.209000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (382, 325, 286, 5, CAST(0.173913 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.174000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.174000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (383, 325, 287, 5, CAST(0.173913 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.174000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.174000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (388, 327, 85, 5, CAST(0.307692 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.308000 AS Decimal(10, 6)), CAST(25.00 AS Decimal(10, 2)), CAST(0.231000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (389, 327, 88, 5, CAST(0.307692 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.308000 AS Decimal(10, 6)), CAST(25.00 AS Decimal(10, 2)), CAST(0.231000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (396, 330, 286, 5, CAST(0.480000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.480000 AS Decimal(10, 6)), CAST(44.00 AS Decimal(10, 2)), CAST(0.269000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (397, 330, 273, 5, CAST(0.013333 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.013000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.013000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (398, 330, 49, 5, CAST(0.680000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.680000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.680000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (423, 343, 76, 5, CAST(0.625000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.625000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.625000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (428, 348, 155, 5, CAST(0.694444 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.694000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.694000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (429, 348, 286, 5, CAST(0.111111 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.111000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.111000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (430, 348, 287, 5, CAST(0.180556 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.181000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.181000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (431, 350, 85, 5, CAST(0.086957 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.087000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.087000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (432, 350, 95, 5, CAST(0.086957 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.087000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (433, 350, 104, 5, CAST(0.058261 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.058000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.047000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (434, 350, 107, 5, CAST(0.006957 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.007000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.007000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (435, 350, 50, 5, CAST(0.043478 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.043000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.043000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (436, 350, 286, 5, CAST(0.391304 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.391000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.391000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (476, 387, 286, 5, CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.500000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (486, 395, 105, 5, CAST(1.111111 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(1.111000 AS Decimal(10, 6)), CAST(10.00 AS Decimal(10, 2)), CAST(1.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (487, 395, 107, 5, CAST(0.111111 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.111000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (488, 398, 287, 5, CAST(0.303030 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.303000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.303000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (511, 429, 288, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (512, 429, 274, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (519, 435, 245, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (520, 435, 14, 5, CAST(0.003000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.003000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.003000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (521, 438, 190, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (528, 442, 35, 5, CAST(0.120000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (529, 442, 129, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)))
GO
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (530, 442, 66, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (531, 442, 240, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (532, 442, 243, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (533, 442, 92, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (578, 451, 62, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (579, 451, 240, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (580, 451, 243, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (581, 451, 86, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (582, 451, 96, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (583, 451, 92, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (584, 451, 127, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(20.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (585, 452, 63, 5, CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (586, 452, 240, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (587, 452, 243, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (588, 452, 96, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (589, 452, 92, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (590, 452, 106, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (591, 452, 129, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(14.29 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (592, 452, 66, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(50.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (606, 457, 35, 5, CAST(0.120000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)), CAST(16.67 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (607, 457, 64, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(14.29 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (608, 457, 240, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (609, 457, 243, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (610, 457, 105, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(14.29 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (611, 457, 111, 5, CAST(0.075000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.075000 AS Decimal(10, 6)), CAST(6.67 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (612, 457, 53, 5, CAST(0.002000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.002000 AS Decimal(10, 6)), CAST(100.00 AS Decimal(10, 2)), CAST(0.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (618, 466, 141, 5, CAST(0.120000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)), CAST(16.67 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (619, 466, 88, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (620, 466, 129, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (621, 466, 92, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (622, 466, 66, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (623, 466, 287, 5, CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (624, 468, 240, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (625, 468, 243, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (626, 468, 92, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (627, 468, 105, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (628, 468, 52, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (629, 468, 53, 5, CAST(0.003000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.003000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.003000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (630, 468, 286, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (642, 471, 240, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (643, 471, 243, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (644, 471, 92, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (645, 471, 104, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (646, 471, 98, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (647, 471, 52, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (648, 471, 9, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (657, 473, 104, 5, CAST(0.016000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.016000 AS Decimal(10, 6)), CAST(6.25 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (658, 473, 72, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (659, 473, 13, 5, CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)), CAST(14.29 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (660, 473, 288, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (661, 473, 286, 5, CAST(0.150000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.150000 AS Decimal(10, 6)), CAST(3.33 AS Decimal(10, 2)), CAST(0.145000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (662, 473, 243, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (663, 473, 49, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (664, 473, 273, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (666, 476, 34, 5, CAST(0.140000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.140000 AS Decimal(10, 6)), CAST(14.29 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (667, 476, 240, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (668, 476, 243, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (669, 476, 92, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (670, 476, 127, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (671, 476, 286, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (687, 486, 139, 5, CAST(0.150000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.150000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.150000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (688, 486, 15, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (706, 499, 172, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (707, 499, 49, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (708, 499, 288, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (709, 499, 286, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (734, 528, 88, 5, CAST(0.800000 AS Decimal(10, 6)), CAST(54.00 AS Decimal(10, 2)), CAST(0.368000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.368000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (769, 541, 200, 5, CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)), CAST(2.00 AS Decimal(10, 2)), CAST(0.059000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (770, 541, 166, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (771, 542, 41, 5, CAST(0.310000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.310000 AS Decimal(10, 6)), CAST(3.23 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (772, 542, 53, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (773, 543, 209, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (774, 543, 168, 5, CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.006000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (775, 545, 287, 5, CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)), CAST(25.00 AS Decimal(10, 2)), CAST(0.045000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (776, 545, 144, 5, CAST(0.120000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.120000 AS Decimal(10, 6)), CAST(8.33 AS Decimal(10, 2)), CAST(0.110000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (777, 545, 49, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (778, 545, 45, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (789, 550, 274, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (790, 551, 50, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (791, 551, 49, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (792, 554, 240, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (793, 554, 243, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (794, 554, 145, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (795, 554, 92, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (796, 554, 51, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (797, 554, 286, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (805, 556, 240, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (806, 556, 243, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (807, 556, 105, 5, CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.035000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (808, 556, 79, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (809, 556, 132, 5, CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (810, 556, 64, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (821, 560, 39, 5, CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.300000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (822, 560, 74, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (823, 560, 53, 5, CAST(0.002000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.002000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.002000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (829, 563, 47, 5, CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (830, 563, 44, 5, CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.030000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (863, 576, 69, 5, CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)), CAST(25.00 AS Decimal(10, 2)), CAST(0.045000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (864, 576, 154, 5, CAST(0.080000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)), CAST(6.25 AS Decimal(10, 2)), CAST(0.075000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (865, 576, 240, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
GO
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (866, 576, 243, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (867, 576, 105, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (868, 576, 92, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (869, 576, 52, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (870, 576, 82, 5, CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.010000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (871, 576, 137, 5, CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (881, 594, 172, 5, CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (882, 594, 227, 5, CAST(0.080000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (883, 594, 170, 5, CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.040000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (884, 594, 180, 5, CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.050000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (885, 595, 228, 5, CAST(0.080000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.080000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (886, 595, 47, 5, CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (887, 595, 45, 5, CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.070000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (888, 596, 228, 5, CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.100000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (889, 596, 173, 5, CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.015000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (890, 596, 166, 5, CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.020000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (891, 596, 161, 5, CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.060000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (911, 434, 286, 3, CAST(170.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(170.000000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(170.000000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (912, 75, 27, 5, CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.005000 AS Decimal(10, 6)))
INSERT [dbo].[TechnicalCardSemiFinishedComposition] ([Id], [TechnicalCardId], [SemiFinishedId], [UnitOfMeasureId], [GrossWeight], [ColdLossPercent], [NetWeight], [HotLossPercent], [OutputWeight]) VALUES (915, 476, 294, 5, CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)), CAST(0.00 AS Decimal(10, 2)), CAST(0.025000 AS Decimal(10, 6)))
SET IDENTITY_INSERT [dbo].[TechnicalCardSemiFinishedComposition] OFF
GO
SET IDENTITY_INSERT [dbo].[ToppingCategories] ON 

INSERT [dbo].[ToppingCategories] ([id], [name]) VALUES (1, N'К блюдам')
INSERT [dbo].[ToppingCategories] ([id], [name]) VALUES (2, N'К напиткам')
INSERT [dbo].[ToppingCategories] ([id], [name]) VALUES (4, N'Неактивные')
SET IDENTITY_INSERT [dbo].[ToppingCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[ToppingsAndSyrups] ON 

INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (1, N'Авокадо крем', CAST(30.00 AS Decimal(8, 2)), 2, CAST(26.28 AS Decimal(10, 2)), CAST(177.78 AS Decimal(10, 2)), CAST(69.00 AS Decimal(10, 2)), CAST(36.00 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (2, N'Апельсин', CAST(40.00 AS Decimal(8, 2)), 2, CAST(6.99 AS Decimal(10, 2)), CAST(186.12 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), CAST(34.95 AS Decimal(5, 2)), NULL, CAST(0.10 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(65.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (3, N'Апельсиновый сироп', CAST(20.00 AS Decimal(8, 2)), 2, CAST(4.98 AS Decimal(10, 2)), CAST(683.13 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), CAST(12.77 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 2, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (4, N'Вяленые томаты', CAST(15.00 AS Decimal(8, 2)), 2, CAST(32.08 AS Decimal(10, 2)), CAST(87.03 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(53.47 AS Decimal(5, 2)), NULL, CAST(0.50 AS Decimal(8, 2)), CAST(2.00 AS Decimal(8, 2)), CAST(6.50 AS Decimal(8, 2)), CAST(40.00 AS Decimal(8, 2)), CAST(160.00 AS Decimal(8, 2)), 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (5, N'Имбирь корень', CAST(20.00 AS Decimal(8, 2)), 2, CAST(10.62 AS Decimal(10, 2)), CAST(88.32 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), CAST(53.10 AS Decimal(5, 2)), NULL, CAST(0.20 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(15.00 AS Decimal(8, 2)), CAST(70.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (6, N'Индейка су-вид', CAST(30.00 AS Decimal(8, 2)), 2, CAST(21.09 AS Decimal(10, 2)), CAST(416.83 AS Decimal(10, 2)), CAST(109.00 AS Decimal(10, 2)), CAST(19.35 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (7, N'Кешью сметана', CAST(30.00 AS Decimal(8, 2)), 2, CAST(15.15 AS Decimal(10, 2)), CAST(230.03 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(30.30 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (8, N'Кокосовая пена', CAST(30.00 AS Decimal(8, 2)), 2, CAST(19.48 AS Decimal(10, 2)), CAST(202.87 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(33.02 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (10, N'Лимон', CAST(20.00 AS Decimal(8, 2)), 2, CAST(4.93 AS Decimal(10, 2)), CAST(204.26 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), CAST(32.87 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 2, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (11, N'Масло', CAST(30.00 AS Decimal(8, 2)), 2, CAST(29.63 AS Decimal(10, 2)), CAST(267.87 AS Decimal(10, 2)), CAST(109.00 AS Decimal(10, 2)), CAST(27.18 AS Decimal(5, 2)), NULL, CAST(22.00 AS Decimal(8, 2)), CAST(0.30 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(200.00 AS Decimal(8, 2)), CAST(830.00 AS Decimal(8, 2)), 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (12, N'Мед', CAST(30.00 AS Decimal(8, 2)), 2, CAST(7.88 AS Decimal(10, 2)), CAST(534.52 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(15.76 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.20 AS Decimal(8, 2)), CAST(41.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), CAST(690.00 AS Decimal(8, 2)), 2, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (13, N'Мята', CAST(5.00 AS Decimal(8, 2)), 2, CAST(6.16 AS Decimal(10, 2)), CAST(62.34 AS Decimal(10, 2)), CAST(19.00 AS Decimal(10, 2)), CAST(61.60 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.20 AS Decimal(8, 2)), CAST(0.30 AS Decimal(8, 2)), CAST(3.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (14, N'Ростбиф', CAST(30.00 AS Decimal(8, 2)), 2, CAST(36.21 AS Decimal(10, 2)), CAST(256.26 AS Decimal(10, 2)), CAST(129.00 AS Decimal(10, 2)), CAST(28.07 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (15, N'Сироп Ваниль', CAST(22.00 AS Decimal(8, 2)), 2, CAST(5.51 AS Decimal(10, 2)), CAST(789.29 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(11.24 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(13.00 AS Decimal(8, 2)), CAST(55.00 AS Decimal(8, 2)), CAST(230.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (16, N'Сироп Кокос', CAST(25.00 AS Decimal(8, 2)), 2, CAST(10.09 AS Decimal(10, 2)), CAST(286.52 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), CAST(25.87 AS Decimal(5, 2)), NULL, CAST(3.50 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(3.50 AS Decimal(8, 2)), CAST(50.00 AS Decimal(8, 2)), CAST(210.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (17, N'Сироп топинамбура', CAST(20.00 AS Decimal(8, 2)), 3, CAST(5.51 AS Decimal(10, 2)), CAST(426.32 AS Decimal(10, 2)), CAST(29.00 AS Decimal(10, 2)), CAST(19.00 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 2, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (18, N'Соленая Карамель', CAST(30.00 AS Decimal(8, 2)), 2, CAST(15.28 AS Decimal(10, 2)), CAST(220.68 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(31.18 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (19, N'Хлеб', CAST(50.00 AS Decimal(8, 2)), 2, CAST(20.57 AS Decimal(10, 2)), CAST(138.21 AS Decimal(10, 2)), CAST(49.00 AS Decimal(10, 2)), CAST(41.98 AS Decimal(5, 2)), NULL, CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(0.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), CAST(1.00 AS Decimal(8, 2)), 1, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (20, N'Шоколадный сироп', CAST(20.00 AS Decimal(8, 2)), 2, CAST(7.01 AS Decimal(10, 2)), CAST(456.35 AS Decimal(10, 2)), CAST(39.00 AS Decimal(10, 2)), CAST(17.97 AS Decimal(5, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (21, N'Шот Какао', CAST(10.00 AS Decimal(8, 2)), 2, CAST(14.17 AS Decimal(10, 2)), CAST(323.43 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(23.62 AS Decimal(5, 2)), NULL, CAST(1.00 AS Decimal(8, 2)), CAST(2.50 AS Decimal(8, 2)), CAST(5.50 AS Decimal(8, 2)), CAST(40.00 AS Decimal(8, 2)), CAST(170.00 AS Decimal(8, 2)), 4, 1)
INSERT [dbo].[ToppingsAndSyrups] ([Id], [Name], [Quantity], [UnitOfMeasureId], [CostRub], [MarkupPercent], [PriceRub], [CostPercent], [TechnicalCardId], [FatsG], [ProteinsG], [CarbsG], [CaloriesKcal], [Kilojoules], [CategoryID], [IsAvailable]) VALUES (22, N'Яйцо пашот', CAST(1.00 AS Decimal(8, 2)), 4, CAST(9.76 AS Decimal(10, 2)), CAST(504.51 AS Decimal(10, 2)), CAST(59.00 AS Decimal(10, 2)), CAST(16.54 AS Decimal(5, 2)), NULL, CAST(5.50 AS Decimal(8, 2)), CAST(7.50 AS Decimal(8, 2)), CAST(0.40 AS Decimal(8, 2)), CAST(85.00 AS Decimal(8, 2)), CAST(360.00 AS Decimal(8, 2)), 1, 1)
SET IDENTITY_INSERT [dbo].[ToppingsAndSyrups] OFF
GO
SET IDENTITY_INSERT [dbo].[UnitsOfMeasure] ON 

INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (1, N'Порция')
INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (2, N'Граммы')
INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (3, N'Миллилитры')
INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (4, N'Штуки')
INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (5, N'Килограммы')
INSERT [dbo].[UnitsOfMeasure] ([Id], [Name]) VALUES (6, N'Литры')
SET IDENTITY_INSERT [dbo].[UnitsOfMeasure] OFF
GO
SET IDENTITY_INSERT [dbo].[WriteOffActs] ON 

INSERT [dbo].[WriteOffActs] ([Id], [Date], [Comment], [StaffId]) VALUES (1, CAST(N'2026-04-27T00:00:00.0000000' AS DateTime2), N'сожрали все гады', 1)
INSERT [dbo].[WriteOffActs] ([Id], [Date], [Comment], [StaffId]) VALUES (2, CAST(N'2026-04-29T00:00:00.0000000' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[WriteOffActs] OFF
GO
SET IDENTITY_INSERT [dbo].[WriteOffTypes] ON 

INSERT [dbo].[WriteOffTypes] ([Id], [Name]) VALUES (1, N'Порча')
INSERT [dbo].[WriteOffTypes] ([Id], [Name]) VALUES (2, N'Питание персонала')
INSERT [dbo].[WriteOffTypes] ([Id], [Name]) VALUES (3, N'Брокераж')
SET IDENTITY_INSERT [dbo].[WriteOffTypes] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Clients_PhoneNumber]    Script Date: 07.05.2026 22:11:02 ******/
ALTER TABLE [dbo].[Clients] ADD  CONSTRAINT [UQ_Clients_PhoneNumber] UNIQUE NONCLUSTERED 
(
	[PhoneNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientSupplyActItems_IngredientId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientSupplyActItems_IngredientId] ON [dbo].[IngredientSupplyActItems]
(
	[IngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientSupplyActItems_SupplyActId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientSupplyActItems_SupplyActId] ON [dbo].[IngredientSupplyActItems]
(
	[SupplyActId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientSupplyActItems_UnitOfMeasureId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientSupplyActItems_UnitOfMeasureId] ON [dbo].[IngredientSupplyActItems]
(
	[UnitOfMeasureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientSupplyActs_Date]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientSupplyActs_Date] ON [dbo].[IngredientSupplyActs]
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientWriteOffActItems_IngredientId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientWriteOffActItems_IngredientId] ON [dbo].[IngredientWriteOffActItems]
(
	[IngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientWriteOffActItems_UnitOfMeasureId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientWriteOffActItems_UnitOfMeasureId] ON [dbo].[IngredientWriteOffActItems]
(
	[UnitOfMeasureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientWriteOffActItems_WriteOffActId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientWriteOffActItems_WriteOffActId] ON [dbo].[IngredientWriteOffActItems]
(
	[WriteOffActId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientWriteOffActItems_WriteOffTypeId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_IngredientWriteOffActItems_WriteOffTypeId] ON [dbo].[IngredientWriteOffActItems]
(
	[WriteOffTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_MenuItemPortionLimits_ItemType_ItemId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_MenuItemPortionLimits_ItemType_ItemId] ON [dbo].[MenuItemPortionLimits]
(
	[ItemType] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDrinkItemModifiers_CoffeeIngredientId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_OrderDrinkItemModifiers_CoffeeIngredientId] ON [dbo].[OrderDrinkItemModifiers]
(
	[CoffeeIngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDrinkItemModifiers_MilkIngredientId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_OrderDrinkItemModifiers_MilkIngredientId] ON [dbo].[OrderDrinkItemModifiers]
(
	[MilkIngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_OrderDrinkItemModifiers_OrderDrinkItemId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_OrderDrinkItemModifiers_OrderDrinkItemId] ON [dbo].[OrderDrinkItemModifiers]
(
	[OrderDrinkItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_StatusID]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_StatusID] ON [dbo].[Orders]
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PreparationTasks_SemiFinishedId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_PreparationTasks_SemiFinishedId] ON [dbo].[PreparationTasks]
(
	[SemiFinishedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SemiFinishedWriteOffActItems_SemiFinishedId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_SemiFinishedWriteOffActItems_SemiFinishedId] ON [dbo].[SemiFinishedWriteOffActItems]
(
	[SemiFinishedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SemiFinishedWriteOffActItems_UnitOfMeasureId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_SemiFinishedWriteOffActItems_UnitOfMeasureId] ON [dbo].[SemiFinishedWriteOffActItems]
(
	[UnitOfMeasureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SemiFinishedWriteOffActItems_WriteOffActId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_SemiFinishedWriteOffActItems_WriteOffActId] ON [dbo].[SemiFinishedWriteOffActItems]
(
	[WriteOffActId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SemiFinishedWriteOffActItems_WriteOffTypeId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_SemiFinishedWriteOffActItems_WriteOffTypeId] ON [dbo].[SemiFinishedWriteOffActItems]
(
	[WriteOffTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Staff_Login]    Script Date: 07.05.2026 22:11:02 ******/
ALTER TABLE [dbo].[Staff] ADD  CONSTRAINT [UQ_Staff_Login] UNIQUE NONCLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffActs_Date]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffActs_Date] ON [dbo].[WriteOffActs]
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WriteOffActs_StaffId]    Script Date: 07.05.2026 22:11:02 ******/
CREATE NONCLUSTERED INDEX [IX_WriteOffActs_StaffId] ON [dbo].[WriteOffActs]
(
	[StaffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[Drinks] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[OrderDishItems] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[OrderDrinkItems] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[OrderToppingItems] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[ToppingsAndSyrups] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_Clients_ClientCategories] FOREIGN KEY([ClientCategoryId])
REFERENCES [dbo].[ClientCategories] ([Id])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_Clients_ClientCategories]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_DishCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[DishCategories] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_DishCategories]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_TechnicalCards]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[DishToppings]  WITH CHECK ADD  CONSTRAINT [FK_DishToppings_OrderDishItems] FOREIGN KEY([OrderDishItemId])
REFERENCES [dbo].[OrderDishItems] ([Id])
GO
ALTER TABLE [dbo].[DishToppings] CHECK CONSTRAINT [FK_DishToppings_OrderDishItems]
GO
ALTER TABLE [dbo].[DishToppings]  WITH CHECK ADD  CONSTRAINT [FK_DishToppings_Toppings] FOREIGN KEY([ToppingId])
REFERENCES [dbo].[ToppingsAndSyrups] ([Id])
GO
ALTER TABLE [dbo].[DishToppings] CHECK CONSTRAINT [FK_DishToppings_Toppings]
GO
ALTER TABLE [dbo].[Drinks]  WITH CHECK ADD  CONSTRAINT [FK_Drinks_DrinkCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[DrinkCategories] ([Id])
GO
ALTER TABLE [dbo].[Drinks] CHECK CONSTRAINT [FK_Drinks_DrinkCategories]
GO
ALTER TABLE [dbo].[Drinks]  WITH CHECK ADD  CONSTRAINT [FK_Drinks_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[Drinks] CHECK CONSTRAINT [FK_Drinks_TechnicalCards]
GO
ALTER TABLE [dbo].[Drinks]  WITH CHECK ADD  CONSTRAINT [FK_Drinks_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[Drinks] CHECK CONSTRAINT [FK_Drinks_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[DrinkToppings]  WITH CHECK ADD  CONSTRAINT [FK_DrinkToppings_OrderDrinkItems] FOREIGN KEY([OrderDrinkItemId])
REFERENCES [dbo].[OrderDrinkItems] ([Id])
GO
ALTER TABLE [dbo].[DrinkToppings] CHECK CONSTRAINT [FK_DrinkToppings_OrderDrinkItems]
GO
ALTER TABLE [dbo].[DrinkToppings]  WITH CHECK ADD  CONSTRAINT [FK_DrinkToppings_Toppings] FOREIGN KEY([ToppingId])
REFERENCES [dbo].[ToppingsAndSyrups] ([Id])
GO
ALTER TABLE [dbo].[DrinkToppings] CHECK CONSTRAINT [FK_DrinkToppings_Toppings]
GO
ALTER TABLE [dbo].[Ingredients]  WITH CHECK ADD  CONSTRAINT [FK_Ingredients_IngredientCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[IngredientCategories] ([Id])
GO
ALTER TABLE [dbo].[Ingredients] CHECK CONSTRAINT [FK_Ingredients_IngredientCategories]
GO
ALTER TABLE [dbo].[Ingredients]  WITH CHECK ADD  CONSTRAINT [FK_Ingredients_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[Ingredients] CHECK CONSTRAINT [FK_Ingredients_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[IngredientSupplyActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientSupplyActItems_Ingredients] FOREIGN KEY([IngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[IngredientSupplyActItems] CHECK CONSTRAINT [FK_IngredientSupplyActItems_Ingredients]
GO
ALTER TABLE [dbo].[IngredientSupplyActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientSupplyActItems_IngredientSupplyActs] FOREIGN KEY([SupplyActId])
REFERENCES [dbo].[IngredientSupplyActs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IngredientSupplyActItems] CHECK CONSTRAINT [FK_IngredientSupplyActItems_IngredientSupplyActs]
GO
ALTER TABLE [dbo].[IngredientSupplyActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientSupplyActItems_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[IngredientSupplyActItems] CHECK CONSTRAINT [FK_IngredientSupplyActItems_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientWriteOffActItems_Ingredients] FOREIGN KEY([IngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems] CHECK CONSTRAINT [FK_IngredientWriteOffActItems_Ingredients]
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientWriteOffActItems_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems] CHECK CONSTRAINT [FK_IngredientWriteOffActItems_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientWriteOffActItems_WriteOffActs] FOREIGN KEY([WriteOffActId])
REFERENCES [dbo].[WriteOffActs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems] CHECK CONSTRAINT [FK_IngredientWriteOffActItems_WriteOffActs]
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_IngredientWriteOffActItems_WriteOffTypes] FOREIGN KEY([WriteOffTypeId])
REFERENCES [dbo].[WriteOffTypes] ([Id])
GO
ALTER TABLE [dbo].[IngredientWriteOffActItems] CHECK CONSTRAINT [FK_IngredientWriteOffActItems_WriteOffTypes]
GO
ALTER TABLE [dbo].[OrderDishItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderDishItems_Dishes] FOREIGN KEY([DishId])
REFERENCES [dbo].[Dishes] ([Id])
GO
ALTER TABLE [dbo].[OrderDishItems] CHECK CONSTRAINT [FK_OrderDishItems_Dishes]
GO
ALTER TABLE [dbo].[OrderDishItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderDishItems_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDishItems] CHECK CONSTRAINT [FK_OrderDishItems_Orders]
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers]  WITH CHECK ADD  CONSTRAINT [FK_OrderDrinkItemModifiers_Ingredients_Coffee] FOREIGN KEY([CoffeeIngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers] CHECK CONSTRAINT [FK_OrderDrinkItemModifiers_Ingredients_Coffee]
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers]  WITH CHECK ADD  CONSTRAINT [FK_OrderDrinkItemModifiers_Ingredients_Milk] FOREIGN KEY([MilkIngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers] CHECK CONSTRAINT [FK_OrderDrinkItemModifiers_Ingredients_Milk]
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers]  WITH CHECK ADD  CONSTRAINT [FK_OrderDrinkItemModifiers_OrderDrinkItems] FOREIGN KEY([OrderDrinkItemId])
REFERENCES [dbo].[OrderDrinkItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDrinkItemModifiers] CHECK CONSTRAINT [FK_OrderDrinkItemModifiers_OrderDrinkItems]
GO
ALTER TABLE [dbo].[OrderDrinkItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderDrinkItems_Drinks] FOREIGN KEY([DrinkId])
REFERENCES [dbo].[Drinks] ([Id])
GO
ALTER TABLE [dbo].[OrderDrinkItems] CHECK CONSTRAINT [FK_OrderDrinkItems_Drinks]
GO
ALTER TABLE [dbo].[OrderDrinkItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderDrinkItems_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDrinkItems] CHECK CONSTRAINT [FK_OrderDrinkItems_Orders]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Clients] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Clients]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Discounts] FOREIGN KEY([DiscountId])
REFERENCES [dbo].[Discounts] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Discounts]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrderStatuses] FOREIGN KEY([StatusID])
REFERENCES [dbo].[OrderStatuses] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_OrderStatuses]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrderTypes] FOREIGN KEY([OrderTypeId])
REFERENCES [dbo].[OrderTypes] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_OrderTypes]
GO
ALTER TABLE [dbo].[OrderToppingItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderToppingItems_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderToppingItems] CHECK CONSTRAINT [FK_OrderToppingItems_Orders]
GO
ALTER TABLE [dbo].[OrderToppingItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderToppingItems_Toppings] FOREIGN KEY([ToppingId])
REFERENCES [dbo].[ToppingsAndSyrups] ([Id])
GO
ALTER TABLE [dbo].[OrderToppingItems] CHECK CONSTRAINT [FK_OrderToppingItems_Toppings]
GO
ALTER TABLE [dbo].[Preparations]  WITH CHECK ADD  CONSTRAINT [FK_Preparations_SemiFinished] FOREIGN KEY([SemiFinishedId])
REFERENCES [dbo].[SemiFinished] ([Id])
GO
ALTER TABLE [dbo].[Preparations] CHECK CONSTRAINT [FK_Preparations_SemiFinished]
GO
ALTER TABLE [dbo].[PreparationTasks]  WITH CHECK ADD  CONSTRAINT [FK_PreparationTasks_SemiFinished] FOREIGN KEY([SemiFinishedId])
REFERENCES [dbo].[SemiFinished] ([Id])
GO
ALTER TABLE [dbo].[PreparationTasks] CHECK CONSTRAINT [FK_PreparationTasks_SemiFinished]
GO
ALTER TABLE [dbo].[SemiFinished]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinished_SemiFinishedCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[SemiFinishedCategories] ([Id])
GO
ALTER TABLE [dbo].[SemiFinished] CHECK CONSTRAINT [FK_SemiFinished_SemiFinishedCategories]
GO
ALTER TABLE [dbo].[SemiFinished]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinished_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[SemiFinished] CHECK CONSTRAINT [FK_SemiFinished_TechnicalCards]
GO
ALTER TABLE [dbo].[SemiFinished]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinished_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[SemiFinished] CHECK CONSTRAINT [FK_SemiFinished_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinishedWriteOffActItems_SemiFinished] FOREIGN KEY([SemiFinishedId])
REFERENCES [dbo].[SemiFinished] ([Id])
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems] CHECK CONSTRAINT [FK_SemiFinishedWriteOffActItems_SemiFinished]
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinishedWriteOffActItems_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems] CHECK CONSTRAINT [FK_SemiFinishedWriteOffActItems_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinishedWriteOffActItems_WriteOffActs] FOREIGN KEY([WriteOffActId])
REFERENCES [dbo].[WriteOffActs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems] CHECK CONSTRAINT [FK_SemiFinishedWriteOffActItems_WriteOffActs]
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems]  WITH CHECK ADD  CONSTRAINT [FK_SemiFinishedWriteOffActItems_WriteOffTypes] FOREIGN KEY([WriteOffTypeId])
REFERENCES [dbo].[WriteOffTypes] ([Id])
GO
ALTER TABLE [dbo].[SemiFinishedWriteOffActItems] CHECK CONSTRAINT [FK_SemiFinishedWriteOffActItems_WriteOffTypes]
GO
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_StaffRoles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[StaffRoles] ([Id])
GO
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_StaffRoles]
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardIngComp_Ingredients] FOREIGN KEY([IngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition] CHECK CONSTRAINT [FK_TechCardIngComp_Ingredients]
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardIngComp_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition] CHECK CONSTRAINT [FK_TechCardIngComp_TechnicalCards]
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardIngComp_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardIngredientComposition] CHECK CONSTRAINT [FK_TechCardIngComp_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardSemiComp_SemiFinished] FOREIGN KEY([SemiFinishedId])
REFERENCES [dbo].[SemiFinished] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition] CHECK CONSTRAINT [FK_TechCardSemiComp_SemiFinished]
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardSemiComp_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition] CHECK CONSTRAINT [FK_TechCardSemiComp_TechnicalCards]
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition]  WITH CHECK ADD  CONSTRAINT [FK_TechCardSemiComp_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[TechnicalCardSemiFinishedComposition] CHECK CONSTRAINT [FK_TechCardSemiComp_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[ToppingsAndSyrups]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ToppingCategories] ([id])
GO
ALTER TABLE [dbo].[ToppingsAndSyrups]  WITH CHECK ADD  CONSTRAINT [FK_Toppings_TechnicalCards] FOREIGN KEY([TechnicalCardId])
REFERENCES [dbo].[TechnicalCards] ([Id])
GO
ALTER TABLE [dbo].[ToppingsAndSyrups] CHECK CONSTRAINT [FK_Toppings_TechnicalCards]
GO
ALTER TABLE [dbo].[ToppingsAndSyrups]  WITH CHECK ADD  CONSTRAINT [FK_Toppings_UnitsOfMeasure] FOREIGN KEY([UnitOfMeasureId])
REFERENCES [dbo].[UnitsOfMeasure] ([Id])
GO
ALTER TABLE [dbo].[ToppingsAndSyrups] CHECK CONSTRAINT [FK_Toppings_UnitsOfMeasure]
GO
ALTER TABLE [dbo].[WriteOffActs]  WITH CHECK ADD  CONSTRAINT [FK_WriteOffActs_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[WriteOffActs] CHECK CONSTRAINT [FK_WriteOffActs_Staff]
GO
USE [master]
GO
ALTER DATABASE [db_demo2] SET  READ_WRITE 
GO

USE [db_demo2]
GO

/* Пополнение остатков для доступности активного меню.
   Активное меню: блюда, напитки и топпинги/сиропы, категория которых не "Неактивные".
   Цель: доступность активного меню, запас заготовок примерно на 20 порций
   и разумные минимумы ингредиентов. */
SET NOCOUNT ON;

DECLARE @TargetPortions decimal(10, 2) = 20.00;
DECLARE @Today date = CONVERT(date, GETDATE());
DECLARE @Now datetime2(7) = SYSDATETIME();

IF OBJECT_ID('tempdb..#ActiveMenuItems') IS NOT NULL DROP TABLE #ActiveMenuItems;
CREATE TABLE #ActiveMenuItems
(
    ItemType nvarchar(16) NOT NULL,
    ItemId int NOT NULL,
    TechnicalCardId int NULL
);

INSERT INTO #ActiveMenuItems (ItemType, ItemId, TechnicalCardId)
SELECT N'dish', d.Id, d.TechnicalCardId
FROM [dbo].[Dishes] d
LEFT JOIN [dbo].[DishCategories] c ON c.Id = d.CategoryId
WHERE ISNULL(c.Name, N'') <> N'Неактивные';

INSERT INTO #ActiveMenuItems (ItemType, ItemId, TechnicalCardId)
SELECT N'drink', d.Id, d.TechnicalCardId
FROM [dbo].[Drinks] d
LEFT JOIN [dbo].[DrinkCategories] c ON c.Id = d.CategoryId
WHERE ISNULL(c.Name, N'') <> N'Неактивные';

INSERT INTO #ActiveMenuItems (ItemType, ItemId, TechnicalCardId)
SELECT N'topping', t.Id, t.TechnicalCardId
FROM [dbo].[ToppingsAndSyrups] t
LEFT JOIN [dbo].[ToppingCategories] c ON c.id = t.CategoryID
WHERE ISNULL(c.name, N'') <> N'Неактивные';

IF OBJECT_ID('tempdb..#SemiTargets') IS NOT NULL DROP TABLE #SemiTargets;
SELECT
    r.SemiFinishedId,
    TargetStockGrams = CAST(ROUND(MAX(r.RequiredBase) * @TargetPortions, 2) AS decimal(10, 2))
INTO #SemiTargets
FROM
(
    SELECT
        c.SemiFinishedId,
        RequiredBase =
            CASE c.UnitOfMeasureId
                WHEN 5 THEN COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0) * 1000
                WHEN 6 THEN COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0) * 1000
                ELSE COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0)
            END
    FROM #ActiveMenuItems m
    INNER JOIN [dbo].[TechnicalCardSemiFinishedComposition] c ON c.TechnicalCardId = m.TechnicalCardId
    WHERE m.TechnicalCardId IS NOT NULL
      AND c.SemiFinishedId IS NOT NULL
) r
WHERE r.RequiredBase > 0
GROUP BY r.SemiFinishedId;

INSERT INTO [dbo].[Preparations] ([Name], [SemiFinishedId], [StockGrams], [ProductionDate])
SELECT
    sf.Name,
    st.SemiFinishedId,
    CAST(ROUND(st.TargetStockGrams - ISNULL(p.CurrentStockGrams, 0), 2) AS decimal(10, 2)),
    @Today
FROM #SemiTargets st
INNER JOIN [dbo].[SemiFinished] sf ON sf.Id = st.SemiFinishedId
OUTER APPLY
(
    SELECT CurrentStockGrams = SUM(CASE WHEN StockGrams > 0 THEN StockGrams ELSE 0 END)
    FROM [dbo].[Preparations] p
    WHERE p.SemiFinishedId = st.SemiFinishedId
) p
WHERE st.TargetStockGrams > ISNULL(p.CurrentStockGrams, 0);

IF OBJECT_ID('tempdb..#IngredientTargets') IS NOT NULL DROP TABLE #IngredientTargets;
CREATE TABLE #IngredientTargets
(
    IngredientId int NOT NULL PRIMARY KEY,
    TargetStock decimal(10, 2) NOT NULL
);

INSERT INTO #IngredientTargets (IngredientId, TargetStock)
SELECT DISTINCT
    i.IngredientId,
    CAST(
        CASE ing.UnitOfMeasureId
            WHEN 5 THEN 5.00
            WHEN 6 THEN 5.00
            WHEN 2 THEN 5000.00
            WHEN 3 THEN 5000.00
            WHEN 4 THEN 50.00
            ELSE 50.00
        END AS decimal(10, 2)) AS TargetStock
FROM
(
    SELECT c.IngredientId
    FROM #ActiveMenuItems m
    INNER JOIN [dbo].[TechnicalCardIngredientComposition] c ON c.TechnicalCardId = m.TechnicalCardId
    WHERE m.TechnicalCardId IS NOT NULL
      AND c.IngredientId IS NOT NULL

    UNION

    SELECT c.IngredientId
    FROM #SemiTargets st
    INNER JOIN [dbo].[SemiFinished] sf ON sf.Id = st.SemiFinishedId
    INNER JOIN [dbo].[TechnicalCardIngredientComposition] c ON c.TechnicalCardId = sf.TechnicalCardId
    WHERE sf.TechnicalCardId IS NOT NULL
      AND c.IngredientId IS NOT NULL
) i
INNER JOIN [dbo].[Ingredients] ing ON ing.Id = i.IngredientId;

UPDATE ing
SET ing.Stock = it.TargetStock
FROM [dbo].[Ingredients] ing
INNER JOIN #IngredientTargets it ON it.IngredientId = ing.Id
WHERE ISNULL(ing.Stock, 0) < it.TargetStock;

UPDATE mpl
SET
    mpl.RemainingPortions = @TargetPortions,
    mpl.UpdatedAt = @Now
FROM [dbo].[MenuItemPortionLimits] mpl
INNER JOIN #ActiveMenuItems m
    ON LOWER(mpl.ItemType) = m.ItemType
   AND mpl.ItemId = m.ItemId
WHERE mpl.RemainingPortions < @TargetPortions;

IF OBJECT_ID('tempdb..#CardAvailability') IS NOT NULL DROP TABLE #CardAvailability;
SELECT
    cr.TechnicalCardId,
    IsAvailable = CAST(MIN(CASE WHEN ISNULL(ps.StockGrams, 0) + 0.000001 >= cr.RequiredBase THEN 1 ELSE 0 END) AS bit)
INTO #CardAvailability
FROM
(
    SELECT
        c.TechnicalCardId,
        c.SemiFinishedId,
        RequiredBase = SUM(
            CASE c.UnitOfMeasureId
                WHEN 5 THEN COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0) * 1000
                WHEN 6 THEN COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0) * 1000
                ELSE COALESCE(NULLIF(c.OutputWeight, 0), NULLIF(c.NetWeight, 0), NULLIF(c.GrossWeight, 0), 0)
            END)
    FROM [dbo].[TechnicalCardSemiFinishedComposition] c
    WHERE c.TechnicalCardId IS NOT NULL
      AND c.SemiFinishedId IS NOT NULL
    GROUP BY c.TechnicalCardId, c.SemiFinishedId
) cr
OUTER APPLY
(
    SELECT StockGrams = SUM(CASE WHEN StockGrams > 0 THEN StockGrams ELSE 0 END)
    FROM [dbo].[Preparations] p
    WHERE p.SemiFinishedId = cr.SemiFinishedId
) ps
WHERE cr.RequiredBase > 0
GROUP BY cr.TechnicalCardId;

UPDATE d
SET d.IsAvailable =
    CASE
        WHEN ISNULL(dc.Name, N'') = N'Неактивные' THEN 0
        WHEN mpl.Id IS NOT NULL AND mpl.RemainingPortions <= 0 THEN 0
        WHEN d.TechnicalCardId IS NULL THEN 1
        ELSE ISNULL(ca.IsAvailable, 1)
    END
FROM [dbo].[Dishes] d
LEFT JOIN [dbo].[DishCategories] dc ON dc.Id = d.CategoryId
LEFT JOIN #CardAvailability ca ON ca.TechnicalCardId = d.TechnicalCardId
LEFT JOIN [dbo].[MenuItemPortionLimits] mpl ON LOWER(mpl.ItemType) = N'dish' AND mpl.ItemId = d.Id;

UPDATE d
SET d.IsAvailable =
    CASE
        WHEN ISNULL(dc.Name, N'') = N'Неактивные' THEN 0
        WHEN mpl.Id IS NOT NULL AND mpl.RemainingPortions <= 0 THEN 0
        WHEN d.TechnicalCardId IS NULL THEN 1
        ELSE ISNULL(ca.IsAvailable, 1)
    END
FROM [dbo].[Drinks] d
LEFT JOIN [dbo].[DrinkCategories] dc ON dc.Id = d.CategoryId
LEFT JOIN #CardAvailability ca ON ca.TechnicalCardId = d.TechnicalCardId
LEFT JOIN [dbo].[MenuItemPortionLimits] mpl ON LOWER(mpl.ItemType) = N'drink' AND mpl.ItemId = d.Id;

UPDATE t
SET t.IsAvailable =
    CASE
        WHEN ISNULL(tc.name, N'') = N'Неактивные' THEN 0
        WHEN mpl.Id IS NOT NULL AND mpl.RemainingPortions <= 0 THEN 0
        WHEN t.TechnicalCardId IS NULL THEN 1
        ELSE ISNULL(ca.IsAvailable, 1)
    END
FROM [dbo].[ToppingsAndSyrups] t
LEFT JOIN [dbo].[ToppingCategories] tc ON tc.id = t.CategoryID
LEFT JOIN #CardAvailability ca ON ca.TechnicalCardId = t.TechnicalCardId
LEFT JOIN [dbo].[MenuItemPortionLimits] mpl ON LOWER(mpl.ItemType) = N'topping' AND mpl.ItemId = t.Id;

DROP TABLE IF EXISTS #CardAvailability;
DROP TABLE IF EXISTS #IngredientTargets;
DROP TABLE IF EXISTS #SemiTargets;
DROP TABLE IF EXISTS #ActiveMenuItems;
GO
