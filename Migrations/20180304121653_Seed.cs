using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ideas.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string query = @"
                INSERT INTO User (FirstName, LastName, Login, Password, IsAdmin) VALUES
                (""System"", ""Admin"", ""admin"", md5(""admin""), b'1'),
                (""Dan"", ""Abramov"", ""dan"", md5(""abramov""), b'0'),
                (""Linus"", ""Torvalds"", ""linus"", md5(""torvalds""), b'0'),
                (""Elon"", ""Musk"", ""elon"", md5(""musk""), b'0'),
                (""Steve"", ""Jobs"", ""steve"", md5(""steve""), b'0');

                INSERT INTO Idea (Approved, Text, UserId, CreatedAt) VALUES
                (
                    b'1',
                    ""What do you think about launching a rocket with a car on board?"",
                    (SELECT Id FROM User WHERE Login = ""elon""),
                    ""2014-02-23 04:58:00""
                ),
                (
                    b'1',
                    ""Let's make a new phone but it will cost to much so no \
                    one would be able to pay for it but everyone is gonna \
                    want it. And we will steal ideas from those who have done \
                    it already!"",
                    (SELECT Id FROM User WHERE Login = ""steve""),
                    ""2003-01-12 04:58:00""
                ),
                (
                    b'1',
                    ""Hey guys, maybe we should create some library for \
                    managing state in react applications! It's gonna \
                    have immutable state and developers are gonna be able \
                    to access it from any place in their programs."",
                    (SELECT Id FROM User WHERE Login = ""dan""),
                    ""2015-03-03 04:58:00""
                ),
                (
                    b'1',
                    ""What do you think about flamethrowers? I think about \
                    selling one."",
                    (SELECT Id FROM User WHERE Login = ""elon""),
                    ""2017-12-21 04:58:00""
                ),
                (
                    b'1',
                    ""I'm gonna make money selling hats!"",
                    (SELECT Id FROM User WHERE Login = ""elon""),
                    ""2017-12-23 04:58:00""
                ),
                (
                    b'1',
                    ""Hey, guys. I think, maybe I should open source OS I've created. \
                    Do you think people will like it?"",
                    (SELECT Id FROM User WHERE Login = ""linus""),
                    ""1993-05-17 04:58:00""
                );

            INSERT INTO Comment(IdeaId, UserId, CreatedAt, Text) VALUES
            (
               (SELECT Id FROM Idea WHERE Text LIKE ""%launching a rocket with a car%"" LIMIT 1),
                    (SELECT Id FROM User WHERE Login = ""linus""),
                    ""2018-01-22 04:58:00"",
                    ""You are genius!""
                ),
                (
                    (SELECT Id FROM Idea WHERE Text LIKE ""%launching a rocket with a car%"" LIMIT 1),
                    (SELECT Id FROM User WHERE Login = ""dan""),
                    ""2018-01-23 04:58:00"",
                    ""Linus is right! You are fucking genius :)""
                );
            ";
            migrationBuilder.Sql(query);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string query = @"DELETE FROM User";
            migrationBuilder.Sql(query);
        }
    }
}
