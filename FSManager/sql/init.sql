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
CREATE PROCEDURE ensureDefaultCollectionCreated
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
    @text VARCHAR(max),
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
        INSERT INTO Cards([Key], [Name], [Text], [CollectionKey]) VALUES (@key, @name, @text, @collectionKey);
        INSERT INTO CardImages([CardKey], [CollectionKey], [Source]) VALUES (@key, @imageColKey, @default_image_src);
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN
    END CATCH
END;
-- DELETE FROM Cards;
-- DELETE FROM CardImages;
GO
EXEC createCard 'the-d6-v2', 'The D6', '{T}: Choose a dice roll. It''s controller rerolls it.\nAt the end of your turn, recharge this.', 'v2', 'https://foursouls.com/wp-content/uploads/2022/01/b2-the_d6.png';