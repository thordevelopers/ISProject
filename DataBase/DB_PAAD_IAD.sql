USE [master]
GO
/****** Object:  Database [DB_PAAD_IAD]    Script Date: 04/06/2020 08:01:34 p. m. ******/
CREATE DATABASE [DB_PAAD_IAD]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DB_PAAD_IAAD', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DB_PAAD_IAAD.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DB_PAAD_IAAD_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\DB_PAAD_IAAD_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [DB_PAAD_IAD] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DB_PAAD_IAD].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DB_PAAD_IAD] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET ARITHABORT OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DB_PAAD_IAD] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DB_PAAD_IAD] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DB_PAAD_IAD] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DB_PAAD_IAD] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET RECOVERY FULL 
GO
ALTER DATABASE [DB_PAAD_IAD] SET  MULTI_USER 
GO
ALTER DATABASE [DB_PAAD_IAD] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DB_PAAD_IAD] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DB_PAAD_IAD] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DB_PAAD_IAD] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DB_PAAD_IAD] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'DB_PAAD_IAD', N'ON'
GO
ALTER DATABASE [DB_PAAD_IAD] SET QUERY_STORE = OFF
GO
USE [DB_PAAD_IAD]
GO
/****** Object:  Table [dbo].[Actividades]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Actividades](
	[id_actividad] [int] IDENTITY(1,1) NOT NULL,
	[actividad] [nvarchar](150) NOT NULL,
	[produccion] [nvarchar](300) NOT NULL,
	[lugar] [nvarchar](150) NOT NULL,
	[porcentaje_inicial] [int] NOT NULL,
	[porcentaje_final] [int] NOT NULL,
	[cacei] [bit] NOT NULL,
	[cuerpo_academico] [bit] NOT NULL,
	[iso] [bit] NOT NULL,
	[id_paad] [int] NULL,
	[id_iad] [int] NULL,
 CONSTRAINT [PK_Actividades] PRIMARY KEY CLUSTERED 
(
	[id_actividad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Administrativos]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Administrativos](
	[id_administrativo] [int] IDENTITY(1,1) NOT NULL,
	[carrera] [int] NOT NULL,
	[rol] [int] NOT NULL,
	[docente] [int] NULL,
	[nombre] [nvarchar](150) NOT NULL,
	[correo] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Administrativos] PRIMARY KEY CLUSTERED 
(
	[id_administrativo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cargos]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cargos](
	[id_cargo] [int] IDENTITY(1,1) NOT NULL,
	[cargo] [nvarchar](50) NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_Cargos] PRIMARY KEY CLUSTERED 
(
	[id_cargo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Carreras]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Carreras](
	[id_carrera] [int] IDENTITY(1,1) NOT NULL,
	[carrera] [nvarchar](50) NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_Carreras] PRIMARY KEY CLUSTERED 
(
	[id_carrera] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categorias]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categorias](
	[id_categoria] [int] IDENTITY(1,1) NOT NULL,
	[categoria] [nvarchar](50) NOT NULL,
	[visible] [bit] NOT NULL,
 CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED 
(
	[id_categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Docentes]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Docentes](
	[id_docente] [int] IDENTITY(1,1) NOT NULL,
	[carrera] [int] NOT NULL,
	[numero_empleado] [int] NOT NULL,
	[nombre] [nvarchar](150) NOT NULL,
	[correo] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Docentes] PRIMARY KEY CLUSTERED 
(
	[id_docente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Estados]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estados](
	[id_estado] [int] IDENTITY(1,1) NOT NULL,
	[estado] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Estados] PRIMARY KEY CLUSTERED 
(
	[id_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IADs]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IADs](
	[id_iad] [int] IDENTITY(1,1) NOT NULL,
	[estado] [int] NOT NULL,
	[periodo] [int] NOT NULL,
	[carrera] [int] NOT NULL,
	[docente] [int] NOT NULL,
	[categoria_docente] [int] NOT NULL,
	[horas_clase] [int] NOT NULL,
	[horas_investigacion] [int] NOT NULL,
	[horas_gestion] [int] NOT NULL,
	[horas_tutorias] [int] NOT NULL,
	[cargo] [int] NOT NULL,
	[firma_docente] [nvarchar](max) NULL,
	[firma_director] [nvarchar](max) NULL,
	[comentarios] [nvarchar](max) NULL,
	[extemporaneo] [bit] NOT NULL,
 CONSTRAINT [PK_IADs] PRIMARY KEY CLUSTERED 
(
	[id_iad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mensajes]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mensajes](
	[id_mensaje] [int] IDENTITY(1,1) NOT NULL,
	[paad] [int] NULL,
	[iad] [int] NULL,
	[tipo] [int] NOT NULL,
	[mensaje] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Mensajes] PRIMARY KEY CLUSTERED 
(
	[id_mensaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PAADs]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAADs](
	[id_paad] [int] IDENTITY(1,1) NOT NULL,
	[estado] [int] NOT NULL,
	[periodo] [int] NOT NULL,
	[carrera] [int] NOT NULL,
	[docente] [int] NOT NULL,
	[categoria_docente] [int] NOT NULL,
	[horas_clase] [int] NOT NULL,
	[horas_investigacion] [int] NOT NULL,
	[horas_gestion] [int] NOT NULL,
	[horas_tutorias] [int] NOT NULL,
	[cargo] [int] NOT NULL,
	[firma_docente] [nvarchar](max) NULL,
	[firma_director] [nvarchar](max) NULL,
	[extemporaneo] [bit] NOT NULL,
 CONSTRAINT [PK_PAADs] PRIMARY KEY CLUSTERED 
(
	[id_paad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Periodos]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Periodos](
	[id_periodo] [int] IDENTITY(1,1) NOT NULL,
	[periodo] [nvarchar](50) NOT NULL,
	[activo] [bit] NOT NULL,
	[paad_inicio] [date] NULL,
	[paad_fin] [date] NULL,
	[iad_inicio] [date] NULL,
	[iad_fin] [date] NULL,
	[fecha_cierre] [date] NULL,
 CONSTRAINT [PK_Periodos] PRIMARY KEY CLUSTERED 
(
	[id_periodo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[id_rol] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TiposDeMensaje]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposDeMensaje](
	[id_tipo_mensaje] [int] IDENTITY(1,1) NOT NULL,
	[tipo] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_TiposDeMensaje] PRIMARY KEY CLUSTERED 
(
	[id_tipo_mensaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TiposDeUsuarios]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposDeUsuarios](
	[id_tipo_usuario] [int] IDENTITY(1,1) NOT NULL,
	[tipo_de_usuario] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TiposDeUsuarios] PRIMARY KEY CLUSTERED 
(
	[id_tipo_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 04/06/2020 08:01:34 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[password] [nvarchar](50) NOT NULL,
	[tipo_usuario] [int] NOT NULL,
 CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_USERS]    Script Date: 04/06/2020 08:01:34 p. m. ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_USERS] ON [dbo].[Usuarios]
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Actividades]  WITH CHECK ADD  CONSTRAINT [FK_Actividades_IADs] FOREIGN KEY([id_iad])
REFERENCES [dbo].[IADs] ([id_iad])
GO
ALTER TABLE [dbo].[Actividades] CHECK CONSTRAINT [FK_Actividades_IADs]
GO
ALTER TABLE [dbo].[Actividades]  WITH CHECK ADD  CONSTRAINT [FK_Actividades_PAADs] FOREIGN KEY([id_paad])
REFERENCES [dbo].[PAADs] ([id_paad])
GO
ALTER TABLE [dbo].[Actividades] CHECK CONSTRAINT [FK_Actividades_PAADs]
GO
ALTER TABLE [dbo].[Administrativos]  WITH CHECK ADD  CONSTRAINT [FK_Administrativos_Carreras] FOREIGN KEY([carrera])
REFERENCES [dbo].[Carreras] ([id_carrera])
GO
ALTER TABLE [dbo].[Administrativos] CHECK CONSTRAINT [FK_Administrativos_Carreras]
GO
ALTER TABLE [dbo].[Administrativos]  WITH CHECK ADD  CONSTRAINT [FK_Administrativos_Docentes] FOREIGN KEY([docente])
REFERENCES [dbo].[Docentes] ([id_docente])
GO
ALTER TABLE [dbo].[Administrativos] CHECK CONSTRAINT [FK_Administrativos_Docentes]
GO
ALTER TABLE [dbo].[Administrativos]  WITH CHECK ADD  CONSTRAINT [FK_Administrativos_Roles] FOREIGN KEY([rol])
REFERENCES [dbo].[Roles] ([id_rol])
GO
ALTER TABLE [dbo].[Administrativos] CHECK CONSTRAINT [FK_Administrativos_Roles]
GO
ALTER TABLE [dbo].[Docentes]  WITH CHECK ADD  CONSTRAINT [FK_Docentes_Carreras] FOREIGN KEY([carrera])
REFERENCES [dbo].[Carreras] ([id_carrera])
GO
ALTER TABLE [dbo].[Docentes] CHECK CONSTRAINT [FK_Docentes_Carreras]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Cargos] FOREIGN KEY([cargo])
REFERENCES [dbo].[Cargos] ([id_cargo])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Cargos]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Carreras] FOREIGN KEY([carrera])
REFERENCES [dbo].[Carreras] ([id_carrera])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Carreras]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Categorias] FOREIGN KEY([categoria_docente])
REFERENCES [dbo].[Categorias] ([id_categoria])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Categorias]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Docentes] FOREIGN KEY([docente])
REFERENCES [dbo].[Docentes] ([id_docente])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Docentes]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Estados] FOREIGN KEY([estado])
REFERENCES [dbo].[Estados] ([id_estado])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Estados]
GO
ALTER TABLE [dbo].[IADs]  WITH CHECK ADD  CONSTRAINT [FK_IADs_Periodos] FOREIGN KEY([periodo])
REFERENCES [dbo].[Periodos] ([id_periodo])
GO
ALTER TABLE [dbo].[IADs] CHECK CONSTRAINT [FK_IADs_Periodos]
GO
ALTER TABLE [dbo].[Mensajes]  WITH CHECK ADD  CONSTRAINT [FK_Mensajes_IADs] FOREIGN KEY([iad])
REFERENCES [dbo].[IADs] ([id_iad])
GO
ALTER TABLE [dbo].[Mensajes] CHECK CONSTRAINT [FK_Mensajes_IADs]
GO
ALTER TABLE [dbo].[Mensajes]  WITH CHECK ADD  CONSTRAINT [FK_Mensajes_PAADs] FOREIGN KEY([paad])
REFERENCES [dbo].[PAADs] ([id_paad])
GO
ALTER TABLE [dbo].[Mensajes] CHECK CONSTRAINT [FK_Mensajes_PAADs]
GO
ALTER TABLE [dbo].[Mensajes]  WITH CHECK ADD  CONSTRAINT [FK_Mensajes_TiposDeMensaje] FOREIGN KEY([tipo])
REFERENCES [dbo].[TiposDeMensaje] ([id_tipo_mensaje])
GO
ALTER TABLE [dbo].[Mensajes] CHECK CONSTRAINT [FK_Mensajes_TiposDeMensaje]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Cargos] FOREIGN KEY([cargo])
REFERENCES [dbo].[Cargos] ([id_cargo])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Cargos]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Carreras] FOREIGN KEY([carrera])
REFERENCES [dbo].[Carreras] ([id_carrera])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Carreras]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Categorias] FOREIGN KEY([categoria_docente])
REFERENCES [dbo].[Categorias] ([id_categoria])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Categorias]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Docentes] FOREIGN KEY([docente])
REFERENCES [dbo].[Docentes] ([id_docente])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Docentes]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Estados] FOREIGN KEY([estado])
REFERENCES [dbo].[Estados] ([id_estado])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Estados]
GO
ALTER TABLE [dbo].[PAADs]  WITH CHECK ADD  CONSTRAINT [FK_PAADs_Periodos] FOREIGN KEY([periodo])
REFERENCES [dbo].[Periodos] ([id_periodo])
GO
ALTER TABLE [dbo].[PAADs] CHECK CONSTRAINT [FK_PAADs_Periodos]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_TiposDeUsuarios] FOREIGN KEY([tipo_usuario])
REFERENCES [dbo].[TiposDeUsuarios] ([id_tipo_usuario])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_TiposDeUsuarios]
GO
USE [master]
GO
ALTER DATABASE [DB_PAAD_IAD] SET  READ_WRITE 
GO
