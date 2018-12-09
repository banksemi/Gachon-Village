/*
MySQL Data Transfer
Source Host: 127.0.0.1
Source Database: gachon_village
Target Host: 127.0.0.1
Target Database: gachon_village
Date: 2018-12-09 오전 9:03:40
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for account
-- ----------------------------
CREATE TABLE `account` (
  `id` char(20) NOT NULL,
  `studentnumber` char(11) NOT NULL,
  `name` char(20) NOT NULL,
  `department` char(20) NOT NULL,
  `phone` char(15) NOT NULL,
  `email` varchar(60) DEFAULT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for article
-- ----------------------------
CREATE TABLE `article` (
  `course_no` char(16) NOT NULL,
  `board_name` char(30) NOT NULL,
  `no` int(11) NOT NULL,
  `sitetype` char(20) NOT NULL,
  `siteid` char(20) NOT NULL,
  `category` int(4) NOT NULL,
  `publisher` char(40) NOT NULL,
  `title` char(255) NOT NULL,
  `date` datetime NOT NULL,
  `content` text,
  `url` varchar(500) NOT NULL,
  `modify_date` datetime DEFAULT NULL,
  PRIMARY KEY (`course_no`,`board_name`,`no`,`sitetype`,`siteid`),
  KEY `course_no` (`course_no`,`no`),
  CONSTRAINT `article_ibfk_1` FOREIGN KEY (`course_no`) REFERENCES `course` (`no`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for course
-- ----------------------------
CREATE TABLE `course` (
  `no` char(16) NOT NULL,
  `type` char(10) NOT NULL,
  `name` char(255) NOT NULL,
  `eclass` char(20) DEFAULT NULL,
  `gcafe` char(20) DEFAULT NULL,
  `cyber` char(20) DEFAULT NULL,
  `navercafe` char(20) DEFAULT NULL,
  PRIMARY KEY (`no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for file
-- ----------------------------
CREATE TABLE `file` (
  `file_no` int(4) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `size` bigint(20) NOT NULL,
  `path` varchar(255) NOT NULL,
  `owner` char(20) DEFAULT NULL,
  `date` datetime NOT NULL,
  `last_modifier` char(20) DEFAULT NULL,
  `modification_date` datetime DEFAULT NULL,
  PRIMARY KEY (`file_no`),
  KEY `owner` (`owner`),
  KEY `last_modifier` (`last_modifier`),
  CONSTRAINT `file_ibfk_1` FOREIGN KEY (`owner`) REFERENCES `account` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `file_ibfk_2` FOREIGN KEY (`last_modifier`) REFERENCES `account` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=338 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for group
-- ----------------------------
CREATE TABLE `group` (
  `group_name` char(16) NOT NULL,
  `master` char(20) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `q` float NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`group_name`),
  KEY `master` (`master`),
  CONSTRAINT `group_ibfk_2` FOREIGN KEY (`group_name`) REFERENCES `course` (`no`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `group_ibfk_3` FOREIGN KEY (`master`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for group_chat
-- ----------------------------
CREATE TABLE `group_chat` (
  `no` int(11) NOT NULL AUTO_INCREMENT,
  `group_name` char(16) NOT NULL,
  `student_id` char(20) NOT NULL,
  `data` text NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`no`),
  KEY `group_name` (`group_name`),
  KEY `student_id` (`student_id`),
  CONSTRAINT `group_chat_ibfk_1` FOREIGN KEY (`group_name`) REFERENCES `group` (`group_name`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `group_chat_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `account` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=920 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for group_file
-- ----------------------------
CREATE TABLE `group_file` (
  `group_name` char(16) NOT NULL,
  `file_no` int(11) NOT NULL,
  `upload_user` char(20) NOT NULL,
  `upload_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY `group_name` (`group_name`),
  KEY `file_no` (`file_no`),
  KEY `upload_user` (`upload_user`),
  CONSTRAINT `group_file_ibfk_1` FOREIGN KEY (`group_name`) REFERENCES `course` (`no`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `group_file_ibfk_2` FOREIGN KEY (`file_no`) REFERENCES `file` (`file_no`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `group_file_ibfk_3` FOREIGN KEY (`upload_user`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for homework
-- ----------------------------
CREATE TABLE `homework` (
  `course_no` char(16) NOT NULL,
  `article_no` int(11) NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  PRIMARY KEY (`course_no`,`article_no`),
  CONSTRAINT `homework_ibfk_2` FOREIGN KEY (`course_no`, `article_no`) REFERENCES `article` (`course_no`, `no`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for inventory
-- ----------------------------
CREATE TABLE `inventory` (
  `student_id` char(20) NOT NULL,
  `file_no` int(11) NOT NULL,
  PRIMARY KEY (`student_id`,`file_no`),
  KEY `file_no` (`file_no`),
  KEY `student_id` (`student_id`),
  CONSTRAINT `inventory_ibfk_2` FOREIGN KEY (`file_no`) REFERENCES `file` (`file_no`) ON UPDATE CASCADE,
  CONSTRAINT `inventory_ibfk_3` FOREIGN KEY (`student_id`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for keyword
-- ----------------------------
CREATE TABLE `keyword` (
  `student_id` char(20) NOT NULL,
  `keyword` char(255) NOT NULL,
  PRIMARY KEY (`student_id`,`keyword`),
  CONSTRAINT `keyword_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for npc
-- ----------------------------
CREATE TABLE `npc` (
  `npc_no` int(4) NOT NULL AUTO_INCREMENT,
  `name` char(20) NOT NULL,
  `skin` char(20) NOT NULL,
  `function` varchar(30) DEFAULT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `q` float NOT NULL,
  PRIMARY KEY (`npc_no`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for npc_message
-- ----------------------------
CREATE TABLE `npc_message` (
  `npc_no` int(4) NOT NULL,
  `message` varchar(255) NOT NULL,
  PRIMARY KEY (`npc_no`,`message`),
  CONSTRAINT `npc_message_ibfk_2` FOREIGN KEY (`npc_no`) REFERENCES `npc` (`npc_no`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for post
-- ----------------------------
CREATE TABLE `post` (
  `no` int(11) NOT NULL AUTO_INCREMENT,
  `title` char(255) NOT NULL,
  `content` text NOT NULL,
  `date` datetime NOT NULL,
  `file` int(11) DEFAULT NULL,
  `sender` char(20) DEFAULT NULL,
  `receiver` char(20) NOT NULL,
  `read` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`no`),
  KEY `file` (`file`),
  KEY `sender` (`sender`),
  KEY `receiver` (`receiver`),
  KEY `read` (`read`,`receiver`),
  CONSTRAINT `post_ibfk_1` FOREIGN KEY (`file`) REFERENCES `file` (`file_no`) ON UPDATE CASCADE,
  CONSTRAINT `post_ibfk_2` FOREIGN KEY (`sender`) REFERENCES `account` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `post_ibfk_3` FOREIGN KEY (`receiver`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14446 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for takes_course
-- ----------------------------
CREATE TABLE `takes_course` (
  `student_id` char(20) NOT NULL,
  `course_no` char(16) NOT NULL,
  `level` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`student_id`,`course_no`,`level`),
  KEY `course_no` (`course_no`),
  CONSTRAINT `takes_course_ibfk_2` FOREIGN KEY (`course_no`) REFERENCES `course` (`no`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `takes_course_ibfk_3` FOREIGN KEY (`student_id`) REFERENCES `account` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- View structure for group_chat_report
-- ----------------------------
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `group_chat_report` AS select `group_chat`.`no` AS `no`,`group_chat`.`group_name` AS `group_name`,concat(`account`.`name`,' (',`group_chat`.`student_id`,')') AS `who`,`group_chat`.`date` AS `date`,`group_chat`.`data` AS `data` from (`group_chat` join `account` on((`account`.`id` = `group_chat`.`student_id`))) order by `group_chat`.`no`;

-- ----------------------------
-- View structure for group_file_info
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `group_file_info` AS select `A`.`group_name` AS `group_name`,`A`.`file_no` AS `file_no`,`A`.`upload_user` AS `upload_user`,`C`.`name` AS `upload_user_name`,`A`.`upload_date` AS `upload_date`,`B`.`name` AS `name`,`B`.`size` AS `size`,`B`.`owner` AS `owner`,`D`.`name` AS `owner_name`,`B`.`date` AS `date` from (((`group_file` `A` join `file` `B`) join `account` `C`) join `account` `D` on(((`A`.`file_no` = `B`.`file_no`) and (`A`.`upload_user` = `C`.`id`) and (`B`.`owner` = `D`.`id`))));

-- ----------------------------
-- View structure for group_info
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `group_info` AS select `group`.`group_name` AS `group_name`,`account`.`name` AS `master_name`,`group`.`master` AS `master_id`,`group`.`date` AS `start_date`,(select count(0) from `takes_course` where ((`takes_course`.`course_no` = `group`.`group_name`) and (`takes_course`.`level` > 0))) AS `member` from (`group` join `account` on((`group`.`master` = `account`.`id`)));

-- ----------------------------
-- View structure for group_member
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `group_member` AS select `takes_course`.`course_no` AS `group_id`,`takes_course`.`level` AS `level`,`takes_course`.`student_id` AS `student_id`,`account`.`studentnumber` AS `studentnumber`,`account`.`name` AS `name`,`account`.`department` AS `department`,`account`.`phone` AS `phone`,`account`.`email` AS `email` from (`takes_course` join `account` on((`account`.`id` = `takes_course`.`student_id`)));

-- ----------------------------
-- View structure for homework_list
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `homework_list` AS select `article`.`course_no` AS `course_no`,`article`.`title` AS `title`,`homework`.`start_date` AS `start_date`,`homework`.`end_date` AS `end_date` from (`article` join `homework` on(((`article`.`course_no` = `homework`.`course_no`) and (`article`.`no` = `homework`.`article_no`))));

-- ----------------------------
-- View structure for inventory_items
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `inventory_items` AS select `inventory`.`student_id` AS `student_id`,`file`.`file_no` AS `file_no`,`file`.`name` AS `name`,`file`.`size` AS `size`,`account`.`name` AS `owner`,`file`.`date` AS `date` from ((`inventory` join `file`) join `account` on(((`file`.`file_no` = `inventory`.`file_no`) and (`file`.`owner` = `account`.`id`))));

-- ----------------------------
-- View structure for post_name
-- ----------------------------
CREATE ALGORITHM=UNDEFINED VIEW `post_name` AS select `post`.`no` AS `no`,`post`.`title` AS `title`,`post`.`content` AS `content`,`post`.`date` AS `date`,`post`.`file` AS `file`,`post`.`sender` AS `sender`,`post`.`receiver` AS `receiver`,`post`.`read` AS `read`,`account`.`name` AS `sender_name` from (`post` join `account` on((`post`.`sender` = `account`.`id`)));

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `account` VALUES ('admin_group', '2', '스터디 알림', '', '', null, '0', '0', '0');
INSERT INTO `account` VALUES ('admin_keyword', '0', '키워드 알림', '', '', null, '0', '0', '0');
INSERT INTO `account` VALUES ('admin_postsystem', '1', '시스템', '시스템', '', null, '0', '0', '0');
INSERT INTO `course` VALUES ('201809970002', 'Class', '컴퓨터네트워크 및 실습', null, '135687', '37984', null);
INSERT INTO `course` VALUES ('소프트웨어학과', 'Dept', '소프트웨어학과', null, null, null, 'gachon2010');
INSERT INTO `npc` VALUES ('1', '가천 게시판', 'Board', '가천 소식 듣기', '-0.08', '3.8', '40.85', '-134.993');
INSERT INTO `npc` VALUES ('2', '우편함', 'PostBox', '우편함 열기', '-40.7761', '3.06066', '23.6428', '20.86');
INSERT INTO `npc` VALUES ('3', '키워드 알리미', 'Eve', '키워드 알림 설정', '15.76', '0.02', '36.3916', '177.964');
INSERT INTO `npc` VALUES ('4', '우편함', 'PostBox', '우편함 열기', '-50.5', '3.06066', '-46.4', '173.114');
INSERT INTO `npc` VALUES ('5', '우편함', 'PostBox', '우편함 열기', '94.5546', '3.5', '7.75307', '-114.335');
INSERT INTO `npc` VALUES ('6', '늑대', 'Wolf', null, '-55.8123', '-0.0118368', '27.2315', '2.29979');
INSERT INTO `npc_message` VALUES ('1', '오늘은 학교에 무슨 일들이 일어났을까요?');
INSERT INTO `npc_message` VALUES ('1', '저를 클릭한다면 유용한 정보들을 알 수 있어요.');
INSERT INTO `npc_message` VALUES ('2', '새로운 소식을 확인하세요!!');
INSERT INTO `npc_message` VALUES ('2', '새로운 소식이 오면 알려드립니다');
INSERT INTO `npc_message` VALUES ('2', '우편함을 통해 파일을 공유할 수 있어요.');
INSERT INTO `npc_message` VALUES ('2', '우편함의 내용은 스마트폰을 통해서도 확인할 수 있습니다.');
INSERT INTO `npc_message` VALUES ('2', '키워드 알림을 통해 원하는 소식을 받아보세요.');
INSERT INTO `npc_message` VALUES ('3', '원하는 키워드를 설정하시면 새로운 게시글이 등록됬을때 알려드릴게요.');
INSERT INTO `npc_message` VALUES ('3', '저는 여러분들께 필요한 정보를 전해드릴수 있어요!');
INSERT INTO `npc_message` VALUES ('3', '제가 알려드리는 정보는 가천 게시판에서 자세히 확인할 수 있어요!');
INSERT INTO `npc_message` VALUES ('3', '할일이 많아....');
INSERT INTO `npc_message` VALUES ('6', 'A-oo-oo-oo-ooo!');
INSERT INTO `npc_message` VALUES ('6', 'Hatee-hatee-hatee-ho!');
INSERT INTO `npc_message` VALUES ('6', 'Ring-ding-ding-ding-dingeringeding!');
INSERT INTO `npc_message` VALUES ('6', 'Wa-pa-pa-pa-pa-pa-pow!');
INSERT INTO `npc_message` VALUES ('6', 'What does the fox say?');
INSERT INTO `npc_message` VALUES ('6', '으르릉...');
