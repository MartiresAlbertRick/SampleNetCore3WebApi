IF DATABASE_PRINCIPAL_ID('CaapsApi_RW') IS NULL
BEGIN
    --create new database role for caaps api
    CREATE ROLE CaapsApi_RW AUTHORIZATION [dbo] 
    
	--deny permissions on Flux
    DENY SELECT, INSERT, UPDATE, DELETE, ALTER  ON SCHEMA::Flux TO CaapsApi_RW 
    
    --deny alter on dbo schema
    DENY ALTER  ON SCHEMA::dbo TO CaapsApi_RW 
    
    GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO CaapsApi_RW 
END
