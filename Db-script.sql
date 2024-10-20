USE [hudiyo_dev]
GO
/****** Object:  Schema [HangFire]    Script Date: 19-10-2024 05:00:03 PM ******/
CREATE SCHEMA [HangFire]
GO
/****** Object:  Table [dbo].[BuildVersion]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuildVersion](
	[SystemInformationID] [tinyint] IDENTITY(1,1) NOT NULL,
	[Database Version] [nvarchar](25) NOT NULL,
	[VersionDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CachedMessages]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CachedMessages](
	[CacheId] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ErrorLogID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorTime] [datetime] NOT NULL,
	[UserName] [sysname] NOT NULL,
	[ErrorNumber] [int] NOT NULL,
	[ErrorSeverity] [int] NULL,
	[ErrorState] [int] NULL,
	[ErrorProcedure] [nvarchar](126) NULL,
	[ErrorLine] [int] NULL,
	[ErrorMessage] [nvarchar](4000) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileUrls]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileUrls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileUrl] [nvarchar](max) NULL,
	[FileType] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageContent]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageContent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageContent] [nvarchar](max) NULL,
	[TimeStamp] [datetime] NULL,
	[HasFile] [bit] NULL,
	[FileId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OTPs]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OTPs](
	[OTPId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OTPValue] [varchar](10) NOT NULL,
	[ExpirationDateTime] [datetime] NOT NULL,
	[IsUsed] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserConnectionId]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserConnectionId](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[connectionId] [nvarchar](255) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMessage]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SendById] [int] NULL,
	[ToUserId] [int] NULL,
	[Delivery] [bit] NULL,
	[Deleted] [bit] NULL,
	[MessageId] [int] NULL,
	[isCached] [bit] NULL,
	[CacheMessageId] [nvarchar](max) NULL,
	[IsDbCached] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[mobile_number] [nvarchar](15) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[isDisabled] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BuildVersion] ADD  CONSTRAINT [DF_BuildVersion_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[ErrorLog] ADD  CONSTRAINT [DF_ErrorLog_ErrorTime]  DEFAULT (getdate()) FOR [ErrorTime]
GO
ALTER TABLE [dbo].[OTPs] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [isDisabled]
GO
/****** Object:  StoredProcedure [dbo].[usp_CheckOTPandActivateUser]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_CheckOTPandActivateUser]
    @userId INT,
    @OtpNumber INT
AS
BEGIN
    -- Check if the OTP exists for the given user
    IF EXISTS (SELECT 1 FROM dbo.OTPs WHERE [UserId] = @userId AND [OTPValue] = @OtpNumber)
    BEGIN
        -- If OTP matches, update the Users table to activate the user
        UPDATE dbo.Users
        SET [isDisabled] = 0
        WHERE [UID] = @userId;

        -- Delete the OTP entry after successful activation
        DELETE FROM dbo.OTPs
        WHERE [UserId] = @userId AND [OTPValue] = @OtpNumber;

        -- Return success as true
        SELECT 1 AS Success;
    END
    ELSE
    BEGIN
        -- Return success as false if OTP does not match
        SELECT 0 AS Success;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_CreateUserRecord]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[usp_CreateUserRecord]    Script Date: 8/2/2024 1:06:55 PM ******/
CREATE PROCEDURE [dbo].[usp_CreateUserRecord]
    @Name NVARCHAR(100),
    @mobile_number NVARCHAR(15),
    @email NVARCHAR(100),
    @isDisabled BIT = 0,
    @UserId INT OUTPUT
AS
BEGIN
    -- Check if the mobile_number already exists
    IF EXISTS (SELECT 1 FROM Users WHERE mobile_number = @mobile_number)
    BEGIN
        -- Get the existing UserId
        SELECT @UserId = UID FROM Users WHERE mobile_number = @mobile_number;
    END
    ELSE
    BEGIN
        -- Insert a new record
        INSERT INTO Users (Name, mobile_number, email, isDisabled)
        VALUES (@Name, @mobile_number, @email, @isDisabled);
        
        -- Get the new UserId
        SET @UserId = SCOPE_IDENTITY();
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDBCachedMessages]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[usp_GetDBCachedMessages] 
(
@userId INT
) 
AS 
Begin 
Select UM.ToUserId,MC.TimeStamp,MC.MessageContent,UM.CacheMessageId from UserMessage UM 
Left JOIN MessageContent MC on Um.Id =MC.Id where ToUserId=@userId

END;
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDbCachedUserIds]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[usp_GetDbCachedUserIds]

AS Begin
	select distinct U.UID, STRING_AGG(UM.CacheMessageId, ',') AS CacheMessageIds from Users U INNER Join UserMessage UM on UM.IsDbCached=1 and UM.ToUserId=U.UID AND UM.isCached=0 GROUP BY 
        U.UID;;
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUserCachedUser]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_GetUserCachedUser]

AS Begin

select DIstinct ToUserId from UserMessage where isCached=1 and IsDbCached=0

End
GO
/****** Object:  StoredProcedure [dbo].[usp_RemoveConnectioId]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_RemoveConnectioId]
	@userId NVARCHAR(15),
    @success BIT OUTPUT

As begin 
SET NOCOUNT ON;
BEGIN TRY
        -- Default output parameter to false
        SET @success = 0;

        -- Check if the mobile number exists in the Users table
        IF EXISTS (SELECT 1 FROM UserConnectionId WHERE userId = @userId)
		begin
			delete from UserConnectionId where userId=@userId
			SET @success = 1;
		end
		else
		BEGIN
            -- Set output parameter to false if mobile number does not exist
            SET @success = 0;
        END
		 END TRY
    BEGIN CATCH
        -- Set output parameter to false in case of an error
        SET @success = 0;
    END CATCH
			

end;
GO
/****** Object:  StoredProcedure [dbo].[usp_Store_Message]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Store_Message]
(
    @CacheMessageId NVARCHAR(MAX),
    @FromUserId INT,
    @ToUserId INT,
    @MessageContent NVARCHAR(MAX),
    @HasFile BIT,
    @FileUrl NVARCHAR(MAX) = NULL,
    @FileType NVARCHAR(100) = NULL,
    @Success BIT OUTPUT -- Output parameter
)
AS
BEGIN
    BEGIN TRY
        DECLARE @MessageContentId INT;
        DECLARE @FileId INT = NULL;

        -- Insert into FileUrls if HasFile is true
        IF @HasFile = 1
        BEGIN
            INSERT INTO dbo.FileUrls (FileUrl, FileType)
            VALUES (@FileUrl, @FileType);

            -- Get the FileId of the inserted file
            SET @FileId = SCOPE_IDENTITY();
        END

        -- Insert into MessageContent
        INSERT INTO dbo.MessageContent (MessageContent, TimeStamp, FileId, HasFile)
        VALUES (@MessageContent, GETDATE(), @FileId, @HasFile);

        -- Get the MessageContentId of the inserted message
        SET @MessageContentId = SCOPE_IDENTITY();

        -- Insert into UserMessage
        INSERT INTO dbo.UserMessage (CacheMessageId, MessageId, SendById, ToUserId, Delivery, Deleted,isCached)
        VALUES (@CacheMessageId, @MessageContentId, @FromUserId, @ToUserId, 0, 0,1);

        -- If everything is successful, set Success to 1
        SET @Success = 1;
    END TRY
    BEGIN CATCH
        -- If there is an error, set Success to 0
        SET @Success = 0;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_StoreCachedMessages]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_StoreCachedMessages]
(
    @MessageIds NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Success BIT = 1;

    BEGIN TRY
        CREATE TABLE #TempMessageIds (MessageId NVARCHAR(255));
        INSERT INTO #TempMessageIds (MessageId)
        SELECT value
        FROM STRING_SPLIT(@MessageIds, ',');

        INSERT INTO CachedMessages (MessageId, UserId)
        SELECT 
            UM.MessageId, 
            UM.ToUserId
        FROM 
            UserMessage UM 
        INNER JOIN 
            #TempMessageIds T ON UM.CacheMessageId = T.MessageId;
		UPDATE UM
		SET UM.IsDbCached = 1 , UM.isCached=0
		FROM UserMessage UM
		INNER JOIN #TempMessageIds T ON UM.CacheMessageId = T.MessageId;
        DROP TABLE #TempMessageIds;
    END TRY
    BEGIN CATCH
        SET @Success = 0;
    END CATCH

    SELECT @Success AS Success;
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_storeOTPs]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_storeOTPs]
    @UserId INT,
    @OTPValue VARCHAR(10),
    @ExpirationDateTime DATETIME
AS
BEGIN
    INSERT INTO OTPs (UserId, OTPValue, ExpirationDateTime, IsUsed)
    VALUES (@UserId, @OTPValue, @ExpirationDateTime, 0);
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_StoreUserConnectionId]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_StoreUserConnectionId]
    @userId NVARCHAR(15),
    @connectionId NVARCHAR(255),
    @success BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Default output parameter to false
        SET @success = 0;

        -- Check if the mobile number exists in the Users table
        IF EXISTS (SELECT 1 FROM Users WHERE UID = @userId)
        BEGIN
            -- Insert into UserConnectionId if mobile number exists
            INSERT INTO UserConnectionId (userId, connectionId)
            Values(@userId,@connectionId)
            
            -- Set output parameter to true on successful insert
            SET @success = 1;
        END
        ELSE
        BEGIN
            -- Set output parameter to false if mobile number does not exist
            SET @success = 0;
        END

    END TRY
    BEGIN CATCH
        -- Set output parameter to false in case of an error
        SET @success = 0;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[uspLogError]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspLogError logs error information in the ErrorLog table about the
-- error that caused execution to jump to the CATCH block of a
-- TRY...CATCH construct. This should be executed from within the scope
-- of a CATCH block otherwise it will return without inserting error
-- information.
CREATE PROCEDURE [dbo].[uspLogError]
    @ErrorLogID int = 0 OUTPUT -- contains the ErrorLogID of the row inserted
AS                             -- by uspLogError in the ErrorLog table
BEGIN
    SET NOCOUNT ON;

    -- Output parameter value of 0 indicates that error
    -- information was not logged
    SET @ErrorLogID = 0;

    BEGIN TRY
        -- Return if there is no error information to log
        IF ERROR_NUMBER() IS NULL
            RETURN;

        -- Return if inside an uncommittable transaction.
        -- Data insertion/modification is not allowed when
        -- a transaction is in an uncommittable state.
        IF XACT_STATE() = -1
        BEGIN
            PRINT 'Cannot log error since the current transaction is in an uncommittable state. '
                + 'Rollback the transaction before executing uspLogError in order to successfully log error information.';
            RETURN;
        END

        INSERT [dbo].[ErrorLog]
            (
            [UserName],
            [ErrorNumber],
            [ErrorSeverity],
            [ErrorState],
            [ErrorProcedure],
            [ErrorLine],
            [ErrorMessage]
            )
        VALUES
            (
            CONVERT(sysname, CURRENT_USER),
            ERROR_NUMBER(),
            ERROR_SEVERITY(),
            ERROR_STATE(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE()
            );

        -- Pass back the ErrorLogID of the row inserted
        SET @ErrorLogID = @@IDENTITY;
    END TRY
    BEGIN CATCH
        PRINT 'An error occurred in stored procedure uspLogError: ';
        EXECUTE [dbo].[uspPrintError];
        RETURN -1;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[uspPrintError]    Script Date: 19-10-2024 05:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspPrintError prints error information about the error that caused
-- execution to jump to the CATCH block of a TRY...CATCH construct.
-- Should be executed from within the scope of a CATCH block otherwise
-- it will return without printing any error information.
CREATE PROCEDURE [dbo].[uspPrintError]
AS
BEGIN
    SET NOCOUNT ON;

    -- Print error information.
    PRINT 'Error ' + CONVERT(varchar(50), ERROR_NUMBER()) +
          ', Severity ' + CONVERT(varchar(5), ERROR_SEVERITY()) +
          ', State ' + CONVERT(varchar(5), ERROR_STATE()) +
          ', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') +
          ', Line ' + CONVERT(varchar(5), ERROR_LINE());
    PRINT ERROR_MESSAGE();
END;
GO
