create database Sonic
go

use Sonic
go

create table Player
(
	playerID char(6) primary key,
	name varchar(20),
	pswd varchar(40),
	goal int
)

create table Play
(
	playerID char(6),
	ord int,
	goal int,
	primary key(PlayerID, ord)
)

create table Own
(
	playerID char(6),
	propID char(2),
	num int,
	primary key(playerID, propID)
)