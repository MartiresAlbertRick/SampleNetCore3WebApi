EXEC sp_droprolemember 'db_datareader', 'CaapsApiHallmarc'
GO

EXEC sp_droprolemember 'db_datawriter', 'CaapsApiHallmarc'
GO

EXEC sp_addrolemember 'CaapsApi_RW', 'CaapsApiHallmarc'
GO
