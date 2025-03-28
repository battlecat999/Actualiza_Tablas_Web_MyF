/****** Object:  Table [dbo].[uni_Clientes]    Script Date: 26/9/2019 10:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[uni_Clientes](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Agente] [char](10) NULL,
	[Sucursal] [char](10) NULL,
	[Cliente] [char](10) NOT NULL,
	[Abreviatura] [char](10) NULL,
	[Razon_Social] [varchar](50) NULL,
	[Nombre_Fantasia] [varchar](50) NULL,
	[Domicilio] [varchar](50) NULL,
	[Localidad] [varchar](50) NULL,
	[Codigo_Postal] [char](10) NULL,
	[Pais] [char](10) NULL,
	[Provincia] [char](10) NULL,
	[Registro_Fiscal] [varchar](15) NULL,
	[Telefonos] [varchar](100) NULL,
	[e_mail] [varchar](50) NULL,
	[Condicion_IVA] [tinyint] NULL,
	[Ultimo_Precio] [tinyint] NULL,
	[Saldo_Favor_Proxima_Factura] [tinyint] NULL,
	[Observaciones] [nvarchar](1999) NULL,
	[Controla_Credito] [tinyint] NULL,
	[Descuento] [decimal](12, 3) NULL,
	[Tipo_DNI] [smallint] NULL,
	[Actualizacion_Web] [smalldatetime] NULL,
	[Clave_Web] [varchar](15) NULL,
	[Activo] [tinyint] NULL,
	[Tipo_Cliente] [smallint] NULL,
	[Percibir_IIBB] [tinyint] NULL,
	[Vendedor] [smallint] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[uni_Proveedores]    Script Date: 26/9/2019 10:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[uni_Proveedores](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Proveedor] [char](10) NOT NULL,
	[Abreviatura] [char](10) NULL,
	[Razon_Social] [varchar](50) NULL,
	[Nombre_Fantasia] [varchar](50) NULL,
	[Domicilio] [varchar](50) NULL,
	[Localidad] [varchar](50) NULL,
	[Codigo_Postal] [char](10) NULL,
	[Provincia] [char](10) NULL,
	[Registro_Fiscal] [varchar](15) NULL,
	[Telefonos] [varchar](100) NULL,
	[e_mail] [varchar](50) NULL,
	[Condicion_IVA] [tinyint] NULL,
	[Saldo_Favor_Proxima_Factura] [tinyint] NULL,
	[Actualizacion_Web] [smalldatetime] NULL,
 CONSTRAINT [PK_uni_Proveedores] PRIMARY KEY CLUSTERED 
(
	[Proveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
