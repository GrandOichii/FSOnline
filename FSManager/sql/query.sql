CREATE OR REPLACE PROCEDURE createCard(
    key VARCHAR,
    name VARCHAR,
    type VARCHAR,
    health int,
    attack int,
    evasion int,
    text VARCHAR,
    script VARCHAR,
    soul_value int,
    collectionKey VARCHAR,
    default_image_src VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS(SELECT * FROM "CardCollections" WHERE "Key" = collectionKey)
    THEN
        INSERT INTO "CardCollections"("Key") VALUES (collectionKey);
	END IF;

    -- create card
	INSERT INTO "Cards"(
		"Key", 
		"Name", 
		"Type",
		"Health",
		"Attack",
		"Evasion",
		"Text", 
		"Script", 
		"SoulValue", 
		"CollectionKey") 
	VALUES (
		key, 
		name, 
		type, 
		health, 
		attack, 
		evasion, 
		text, 
		script, 
		soul_value, 
		collectionKey
	);
	commit;
END; $$;