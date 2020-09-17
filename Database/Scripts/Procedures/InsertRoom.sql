CREATE PROCEDURE InsertRoom 
(
	@Wing nvarchar(3),
	@RoomNumber INT, 
	@SubRoom nvarchar(5) = NULL,
	@Floor INT
)
AS 
BEGIN
	INSERT INTO ROOM VALUES (CONCAT(@Wing, @RoomNumber, @SubRoom), null,
	(SELECT FLOOR_ID FROM FLOOR_LEVEL WHERE FLOOR_LEVEL = @Floor AND
	WING_ID = (SELECT WING_ID FROM WING WHERE WING_NAME = @Wing)))
END
GO