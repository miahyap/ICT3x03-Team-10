-- MySQL dump 10.13  Distrib 8.0.22, for Win64 (x86_64)
--
-- Host: localhost    Database: forumx
-- ------------------------------------------------------
-- Server version	5.5.5-10.4.15-MariaDB-1:10.4.15+maria~bionic

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `activity_logs`
--

DROP TABLE IF EXISTS `activity_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activity_logs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` int(11) NOT NULL,
  `source` varchar(50) NOT NULL,
  `activity` text NOT NULL,
  `time` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `activity_logs_users_id_fk` (`user`),
  CONSTRAINT `activity_logs_users_id_fk` FOREIGN KEY (`user`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=327 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activity_logs`
--

LOCK TABLES `activity_logs` WRITE;
/*!40000 ALTER TABLE `activity_logs` DISABLE KEYS */;
INSERT INTO `activity_logs` VALUES (138,19,'1','1','2020-11-03 20:01:49'),(139,19,'1','1','2020-11-03 20:01:49'),(140,19,'1','1','2020-11-03 20:01:49'),(141,19,'1','1','2020-11-03 20:01:49'),(150,19,'1','1','2020-11-03 20:01:49'),(151,19,'1','1','2020-11-03 20:01:49'),(152,19,'1','1','2020-11-03 20:01:49'),(153,19,'1','1','2020-11-03 20:01:49'),(154,19,'1','1','2020-11-03 20:01:49'),(155,19,'1','1','2020-11-03 20:01:49'),(156,19,'1','1','2020-11-03 20:01:49'),(157,19,'1','1','2020-11-03 20:01:49'),(174,19,'1','1','2020-11-03 20:01:49'),(176,19,'1','1','2020-11-03 20:01:49'),(177,19,'1','1','2020-11-03 20:01:49'),(178,19,'1','1','2020-11-03 20:01:49'),(179,19,'1','1','2020-11-03 20:01:49'),(180,19,'1','1','2020-11-03 20:01:49'),(181,19,'1','1','2020-11-03 20:01:49'),(182,19,'1','1','2020-11-03 20:01:49'),(183,19,'1','1','2020-11-03 20:01:49'),(184,19,'1','1','2020-11-03 20:01:49'),(185,19,'1','1','2020-11-03 20:01:49'),(186,19,'1','1','2020-11-03 20:01:49'),(187,19,'1','1','2020-11-03 20:01:49'),(188,19,'1','1','2020-11-03 20:01:49'),(189,19,'1','1','2020-11-03 20:01:49'),(190,19,'1','1','2020-11-03 20:01:49'),(223,19,'1','1','2020-11-03 20:01:49'),(224,19,'1','1','2020-11-03 20:01:49'),(225,19,'1','1','2020-11-03 20:01:49'),(226,19,'1','1','2020-11-03 20:01:49'),(259,19,'1','1','2020-11-03 20:02:49'),(260,19,'1','1','2020-11-03 20:02:49'),(261,19,'1','1','2020-11-03 20:02:49'),(262,19,'1','1','2020-11-03 20:02:49'),(263,19,'1','1','2020-11-03 20:02:49'),(264,19,'1','1','2020-11-03 20:02:49'),(265,19,'1','1','2020-11-03 20:02:49'),(266,19,'1','1','2020-11-03 20:02:49'),(267,19,'1','1','2020-11-03 20:02:49'),(268,19,'1','1','2020-11-03 20:02:49'),(269,19,'1','1','2020-11-03 20:02:49'),(270,19,'1','1','2020-11-03 20:02:49'),(271,19,'1','1','2020-11-03 20:02:49'),(272,19,'1','1','2020-11-03 20:02:49'),(273,19,'1','1','2020-11-03 20:02:49'),(274,19,'1','1','2020-11-03 20:02:49'),(275,19,'1','1','2020-11-03 20:02:49'),(276,19,'1','1','2020-11-03 20:02:49'),(277,19,'1','1','2020-11-03 20:02:49'),(278,19,'1','1','2020-11-03 20:02:49'),(279,19,'1','1','2020-11-03 20:02:49'),(280,19,'1','1','2020-11-03 20:02:49'),(281,19,'1','1','2020-11-03 20:02:49'),(282,19,'1','1','2020-11-03 20:02:49'),(283,19,'1','1','2020-11-03 20:02:49'),(284,19,'1','1','2020-11-03 20:02:49'),(285,19,'1','1','2020-11-03 20:02:49'),(286,19,'1','1','2020-11-03 20:02:49'),(287,19,'1','1','2020-11-03 20:02:49'),(288,19,'1','1','2020-11-03 20:02:49'),(289,19,'1','1','2020-11-03 20:02:49'),(290,19,'1','1','2020-11-03 20:02:49'),(291,19,'1','1','2020-11-03 20:02:49'),(292,19,'1','1','2020-11-03 20:02:49'),(293,19,'1','1','2020-11-03 20:02:49'),(294,19,'1','1','2020-11-03 20:02:49'),(295,19,'1','1','2020-11-03 20:02:49'),(296,19,'1','1','2020-11-03 20:02:49'),(297,19,'1','1','2020-11-03 20:02:49'),(298,19,'1','1','2020-11-03 20:02:49'),(299,19,'1','1','2020-11-03 20:02:49'),(300,19,'1','1','2020-11-03 20:02:49'),(301,19,'1','1','2020-11-03 20:02:49'),(302,19,'1','1','2020-11-03 20:02:49'),(303,19,'1','1','2020-11-03 20:02:49'),(304,19,'1','1','2020-11-03 20:02:49'),(305,19,'1','1','2020-11-03 20:02:49'),(306,19,'1','1','2020-11-03 20:02:49'),(307,19,'1','1','2020-11-03 20:02:49'),(308,19,'1','1','2020-11-03 20:02:49'),(309,19,'1','1','2020-11-03 20:02:49'),(310,19,'1','1','2020-11-03 20:02:49'),(311,19,'1','1','2020-11-03 20:02:49'),(312,19,'1','1','2020-11-03 20:02:49'),(313,19,'1','1','2020-11-03 20:02:49'),(314,19,'1','1','2020-11-03 20:02:49'),(315,19,'1','1','2020-11-03 20:02:49'),(316,19,'1','1','2020-11-03 20:02:49'),(317,19,'1','1','2020-11-03 20:02:49'),(318,19,'1','1','2020-11-03 20:02:49'),(319,19,'1','1','2020-11-03 20:02:49'),(320,19,'1','1','2020-11-03 20:02:49'),(321,19,'1','1','2020-11-03 20:02:49'),(322,19,'1','1','2020-11-03 20:02:49'),(323,19,'1','1','2020-11-03 20:02:49'),(324,19,'1','1','2020-11-03 20:02:49'),(325,19,'1','1','2020-11-03 20:02:49'),(326,19,'1','1','2020-11-03 20:02:49');
/*!40000 ALTER TABLE `activity_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comments`
--

DROP TABLE IF EXISTS `comments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uuid` binary(16) NOT NULL,
  `post` int(11) NOT NULL,
  `user` int(11) NOT NULL,
  `content` text NOT NULL,
  `timePosted` datetime NOT NULL,
  `edited` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `comments_users_id_fk` (`user`),
  KEY `comments_posts_id_fk` (`post`),
  CONSTRAINT `comments_posts_id_fk` FOREIGN KEY (`post`) REFERENCES `posts` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `comments_users_id_fk` FOREIGN KEY (`user`) REFERENCES `users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comments`
--

LOCK TABLES `comments` WRITE;
/*!40000 ALTER TABLE `comments` DISABLE KEYS */;
INSERT INTO `comments` VALUES (2,_binary '░R¥\╔\╬L\"▓BA\­×',6,20,'Post 1 topic 1 comment 1 by test','2020-10-31 16:16:04',0);
/*!40000 ALTER TABLE `comments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `posts`
--

DROP TABLE IF EXISTS `posts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `posts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uuid` binary(16) NOT NULL,
  `user` int(11) NOT NULL,
  `topic` int(11) NOT NULL,
  `content` text NOT NULL,
  `timePosted` datetime NOT NULL,
  `edited` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `posts_topics_id_fk` (`topic`),
  KEY `posts_users_id_fk` (`user`),
  CONSTRAINT `posts_topics_id_fk` FOREIGN KEY (`topic`) REFERENCES `topics` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `posts_users_id_fk` FOREIGN KEY (`user`) REFERENCES `users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `posts`
--

LOCK TABLES `posts` WRITE;
/*!40000 ALTER TABLE `posts` DISABLE KEYS */;
INSERT INTO `posts` VALUES (6,_binary '░R¥\╔\╬L!▓B@\█\­ƒ',19,1,'womp in topic 1 by test user','2020-10-27 14:22:27',0),(8,_binary '\ıp\ß\±W`BÆ\Ù[\´pñnm',19,2,'testcontent111','2020-10-27 14:58:09',0),(9,_binary 'âaüø5ç\╠Fì#╠©D╝`',19,2,'testcontent11144441','2020-10-27 14:59:54',0);
/*!40000 ALTER TABLE `posts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `topics`
--

DROP TABLE IF EXISTS `topics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `topics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uuid` binary(16) NOT NULL,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `topics_name_uindex` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `topics`
--

LOCK TABLES `topics` WRITE;
/*!40000 ALTER TABLE `topics` DISABLE KEYS */;
INSERT INTO `topics` VALUES (1,_binary '╬▒Ç¡X~MïÖ°ê`ÉE\µ','Test Topic 1'),(2,_binary 'a│@┤▒\ÒH\Òê\Ý\Þ\ÃL\Ý\þ','Test Topic 2'),(3,_binary '\¤>\Ïl\ÎI├¥╚£ \¶öÄ╗','Test Topic 3'),(4,_binary '£¢TojOÄÁ\¤Ù╣ì░T\┼','Test Topic 4'),(5,_binary 'ìK(5ù\¾B─Àâ\ð├ô:','Test Topic 5');
/*!40000 ALTER TABLE `topics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uuid` binary(16) NOT NULL,
  `login` varchar(20) NOT NULL,
  `passHash` binary(48) NOT NULL,
  `totp` binary(32) NOT NULL,
  `name` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `users_login_uindex` (`login`),
  UNIQUE KEY `users_email_uindex` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (19,_binary 'b°2Åså\ıCá´×│QéY\r','test',_binary 'z\n¼èä227&4ä,J\n\Ò┴\ÝîQu\Ô┼É\¯4v\ð\ßLm╩╣Q·a\╩\╬`\ÒuxÅî',_binary '@7e▓iÖ,¥NÃ¿$øEÉö\¾\‗¼©XE7\┬\ßƒ\╬Eó','NATHANIEL CHAN JIE LE','1800987@sit.singaporetech.edu.sg'),(20,_binary '░R¥\0\0L\"▓B@\█\­ƒ','baduser',_binary '\Ï\ð\Õ&Á▒\╚\¸·B¥é\╦¹éöh\0¼▄ø£ü¢Àh|ï·R:½\ÎB\╠PY*\Ù\ð\Ê. \¾º',_binary '@7e▓iÖ,¥NÃ¿$øEÉö\¾\‗¼©XE7\┬\ßƒ\╬Eó','INVALID','invalid@sit.singaporetech.edu.sg');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'forumx'
--
/*!50003 DROP FUNCTION IF EXISTS `uuid_from_bin` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`%` FUNCTION `uuid_from_bin`(b BINARY(16)) RETURNS char(36) CHARSET utf8
    DETERMINISTIC
BEGIN

  DECLARE h CHAR(32);

  SET h=HEX(b);

  RETURN CONCAT(SUBSTRING(h,7,2), SUBSTRING(h,5,2), SUBSTRING(h,3,2), SUBSTRING(h,1,2),

    '-', SUBSTRING(h,11,2), SUBSTRING(h,9,2),

    '-', SUBSTRING(h,15,2), SUBSTRING(h,13,2),

    '-', SUBSTRING(h,17,4),

    '-', SUBSTRING(h,21,12));

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `addUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `addUser`(IN in_uuid binary(16), IN in_username varchar(20),
                                                      IN in_passHash binary(48), IN in_totp binary(32),
                                                      IN in_email varchar(100), IN in_actName varchar(100))
begin
    insert into forumx.users(uuid, login, passHash, totp, name, email) values (in_uuid, in_username, in_passHash, in_totp, in_actName, in_email);

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `changePassword` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `changePassword`(IN userUUID binary(16), IN newPassHash binary(48))
begin

    update users set passHash = newPassHash where uuid = userUUID;

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `deleteComment` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `deleteComment`(IN commentUUID binary(16), IN userUUID binary(16))
begin

    DECLARE commentUserID int;

    select u.id into commentUserID from users u where u.uuid = userUUID;



    delete from posts

    where posts.uuid = commentUUID and posts.user = commentUserID;

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `deletePost` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `deletePost`(IN postUUID binary(16), IN userUUID binary(16))
begin
    DECLARE postUserID int;
    select u.id into postUserID from users u where u.uuid = userUUID;
    
    delete from posts
    where posts.uuid = postUUID and posts.user = postUserID;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `editComment` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `editComment`(IN commentUUID binary(16), IN userUUID binary(16),
                                                          IN updatedContent text)
begin
    DECLARE commentUserID int;
    select u.id into commentUserID from users u where u.uuid = userUUID;
    update comments c set c.content = updatedContent, timePosted = current_time, edited = true
    where c.uuid = commentUUID and commentUUID = c.user;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `editPost` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `editPost`(IN postUUID binary(16), IN userUUID binary(16),
                                                       IN updatedContent text)
begin
    DECLARE postUserID int;
    select u.id into postUserID from users u where u.uuid = userUUID;
    update posts p set p.content = updatedContent, p.timePosted = current_time, edited = true where p.uuid = postUUID and postUserID = p.user;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getActivityLogs` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getActivityLogs`(IN userUUID binary(16))
begin

    select source, activity, time from activity_logs al where al.user = (select id from users where uuid = userUUID);

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getCommentsByPost` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getCommentsByPost`(IN postUUID binary(16))
begin
    select c.uuid, c.content, u.login, c.timePosted, c.edited from posts p, comments c, users u
    where postUUID = p.uuid and p.id = c.post and u.id = c.user;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getPostInfo` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getPostInfo`(IN postUUID binary(16))
begin
    select t.name as 'topic', u.login as 'user', p.content, p.timePosted, p.edited
           from posts p, users u, topics t
    where postUUID = p.uuid and t.id = p.topic and u.id = p.user;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getPostsByTopic` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getPostsByTopic`(IN topicUUID binary(16))
begin
    select p.uuid, p.content, u.login, p.timePosted, p.edited from topics t, posts p, users u
    where t.uuid = topicUUID and t.id = p.topic and u.id = p.user;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getTopics` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getTopics`(IN topicUUID binary(16))
begin
    
    IF (topicUUID is NULL) THEN
        select t.uuid, t.name from topics t;
    else
        select t.uuid, t.name from topics t where t.uuid = topicUUID;
    end if;
    
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getUserByGuid` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getUserByGuid`(IN in_userGuid binary(16))
begin

    select u.uuid, u.login, u.passHash, u.totp, u.name, u.email from forumx.users as u where uuid = in_userGuid;

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `getUserByLogin` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `getUserByLogin`(IN in_username varchar(20))
begin
    select u.uuid, u.login, u.passHash, u.totp, u.name, u.email from forumx.users as u where login = in_username;
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `newActivity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `newActivity`(IN userUUID binary(16), IN inSource varchar(50), IN inActivity TEXT)
begin

    insert into activity_logs (user, source, activity, time) VALUES

    (

     (select u.id from users u where u.uuid = userUUID),

     inSource,

     inActivity,

     current_time

    );

delete from activity_logs where user = ((select id from users u where u.uuid = userUUID))

                            and id not in

                                (select * from

                                        (select id from activity_logs al

                                        where al.user =

                                              (select id from users u where u.uuid = userUUID)

                                        order by al.time desc limit 100

                                        ) as ai);

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `newComment` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `newComment`(IN postUUID binary(16), IN userUUID binary(16),

                                                      IN commentUUID binary(16), IN commentContent text)
begin



    insert into comments (uuid, post, user, content, timePosted)

    value (commentUUID,

          (select p.id from posts p where p.uuid = postUUID),

          (select u.id from users u where u.uuid = userUUID),

          commentContent,

          current_time);

end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `newPost` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_unicode_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'IGNORE_SPACE,STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`forumx-db-admin`@`%` PROCEDURE `newPost`(IN postUUID binary(16), IN userUUID binary(16),
                                                      IN topicUUID binary(16), IN postContent text)
begin
    insert into posts (uuid, user, topic, content, timePosted)
     VALUES (postUUID,
             (select u.id from users u where u.uuid = userUUID),
             (select t.id from topics t where t.uuid = topicUUID),
             postContent,
             current_time);
end ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-11-03 22:32:20
