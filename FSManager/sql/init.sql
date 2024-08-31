GO
DROP FUNCTION IF EXISTS getDefaultCardImageCollectionKey;
GO
CREATE FUNCTION getDefaultCardImageCollectionKey()
RETURNS VARCHAR(MAX)
BEGIN
    RETURN 'Default'
END;

GO
DROP PROCEDURE IF EXISTS ensureDefaultCollectionCreated;

GO
CREATE PROCEDURE ensureDefaultCollectionCreatedw
AS BEGIN
    DECLARE @default VARCHAR(MAX)
    SELECT @default = dbo.getDefaultCardImageCollectionKey()
    DECLARE @throwAway VARCHAR(MAX)
    SELECT @throwAway = [Key] FROM CardImageCollections WHERE [Key] = @default
    IF @@ROWCOUNT = 0 INSERT INTO CardImageCollections([Key]) VALUES (@default)
END;

GO
DROP PROCEDURE IF EXISTS createCard; 
GO
CREATE PROCEDURE createCard(
    @key VARCHAR(max),
    @name VARCHAR(max),
    @type VARCHAR(max),
    @health int,
    @attack int,
    @evasion int,
    @text VARCHAR(max),
    @script VARCHAR(max),
    @soul_value int,
    @collectionKey VARCHAR(max),
    @default_image_src VARCHAR(max)
)
AS BEGIN
    EXEC ensureDefaultCollectionCreated
    DECLARE @imageColKey VARCHAR(max)
    SELECT @imageColKey = dbo.getDefaultCardImageCollectionKey()

    IF NOT EXISTS(SELECT * FROM CardCollections WHERE [Key] = @collectionKey)
    BEGIN
        INSERT INTO CardCollections([Key]) VALUES (@collectionKey);
    END

    -- create card
    BEGIN TRY
        BEGIN TRAN
        INSERT INTO Cards(
            [Key], 
            [Name], 
            [Type],
            [Health],
            [Attack],
            [Evasion],
            [Text], 
            [Script], 
            [SoulValue], 
            [CollectionKey]) 
        VALUES (
            @key, 
            @name, 
            @type, 
            @health, 
            @attack, 
            @evasion, 
            @text, 
            @script, 
            @soul_value, 
            @collectionKey
        );
        INSERT INTO CardImages([CardKey], [CollectionKey], [Source]) VALUES (@key, @imageColKey, @default_image_src);
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN
    END CATCH
END;