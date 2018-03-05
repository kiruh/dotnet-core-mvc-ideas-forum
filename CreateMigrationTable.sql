CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` text NOT NULL,
  `ProductVersion` text NOT NULL,
  PRIMARY KEY (`MigrationId`(255)));

-- DROP DATABASE
DROP TABLE `Comment`;
DROP TABLE `Idea`;
DROP TABLE `Token`;
DROP TABLE `User`;
DELETE FROM `__EFMigrationsHistory`;