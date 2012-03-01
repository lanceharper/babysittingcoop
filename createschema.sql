USE [dbd33fd9a3b7b1440e8f15a0060108aeda]
GO

/****** Object:  Table [dbo].[BabySitter]    Script Date: 02/29/2012 23:12:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BabySitter](
	[BabySitterId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[LegacyId] [int] NULL,
 CONSTRAINT [PK_BabySitter] PRIMARY KEY CLUSTERED 
(
	[BabySitterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [dbd33fd9a3b7b1440e8f15a0060108aeda]
GO

/****** Object:  Table [dbo].[BabySittingTransaction]    Script Date: 02/29/2012 23:12:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BabySittingTransaction](
	[BabySittingTransactionId] [int] IDENTITY(1,1) NOT NULL,
	[ChildrenWatched] [int] NOT NULL,
	[Duration] [bigint] NOT NULL,
	[SittingProviderId] [int] NOT NULL,
	[SittingReceiverId] [int] NOT NULL,
	[StartedAtUtc] [datetimeoffset](0) NOT NULL,
 CONSTRAINT [PK_BabySitterTransaction] PRIMARY KEY CLUSTERED 
(
	[BabySittingTransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [dbd33fd9a3b7b1440e8f15a0060108aeda]
GO

/****** Object:  View [dbo].[vw_BabySittingPoints]    Script Date: 02/29/2012 23:12:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vw_BabySittingPoints] AS

SELECT 
    bs.BabySitterId
   ,Name
   ,COALESCE(providers.ProvidedPoints, 0) AS 'ProvidedPoints'
   ,COALESCE(receiver.ReceiverPoints, 0) AS 'ReceiverPoints'
   ,COALESCE(providers.ProvidedPoints, 0) + COALESCE(receiver.ReceiverPoints, 0) + 5 AS 'TotalPoints'
FROM BabySitter bs
LEFT JOIN 
	(SELECT SittingProviderId, 
			SUM(CONVERT(INTEGER, [Duration]/36000000000) * bst.ChildrenWatched) AS 'ProvidedPoints'
	 FROM BabySittingTransaction bst
	 GROUP BY SittingProviderId) providers ON bs.BabySitterId = providers.SittingProviderId
LEFT JOIN 
	(SELECT SittingReceiverId, 
			SUM(CONVERT(INTEGER, [Duration]/36000000000) * bst.ChildrenWatched) AS 'ReceiverPoints'
	 FROM BabySittingTransaction bst
	 GROUP BY SittingReceiverId) receiver ON bs.BabySitterId = receiver.SittingReceiverId



GO


USE [dbd33fd9a3b7b1440e8f15a0060108aeda]
GO

/****** Object:  StoredProcedure [dbo].[usp_BabySitterRecommendations]    Script Date: 02/29/2012 23:13:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Lance Harper
-- Create date: 2012/02/29
-- Description:	Get the counts of babysitters who have not yet been used.
-- =============================================
CREATE PROCEDURE [dbo].[usp_BabySitterRecommendations]
	-- Add the parameters for the stored procedure here
	@parentId int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT 
    bs.BabySitterId
   ,Name
   ,COALESCE(providers.ProvidedCount, 0) AS 'ProvidedCount'
FROM BabySitter bs
LEFT JOIN 
	(SELECT 
		SittingProviderId
	   ,COUNT(bst.BabySittingTransactionId) AS 'ProvidedCount'
	 FROM BabySittingTransaction bst
	 GROUP BY SittingProviderId) providers ON bs.BabySitterId = providers.SittingProviderId
LEFT JOIN 
	(SELECT DISTINCT SittingProviderId
	 FROM BabySittingTransaction bst
	 WHERE SittingReceiverId = @parentId) priorProviders ON bs.BabySitterId = priorProviders.SittingProviderId
WHERE priorProviders.SittingProviderId IS NULL
END

GO




