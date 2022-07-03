using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AdoNetExcircises
{
    public class Program
    {
        static void Main(string[] args)
        {

            using SqlConnection sqlConnection =
                new SqlConnection(Config.ConnectionString);

            sqlConnection.Open();

            string output = increaseAgeStoredProcedure(sqlConnection);
            Console.WriteLine(output);

            sqlConnection.Close();
        }


        /// <summary>
        /// Write a program that prints on the console all villains'
        /// names and their number of minions of those who have more than 
        /// 3 minions ordered descending by the number of minions.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 2
        private static string VillainNames(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            string querry = @"SELECT v.[Name],
	                   COUNT(mv.MinionId) AS MinionCount
                        FROM [Villains] AS [v]
                   LEFT JOIN [MinionsVillains] AS [mv]
	                      ON [v].Id = mv.VillainId
	                GROUP BY v.Id, v.Name	
                      HAVING COUNT(mv.MinionId) > 3
	                ORDER BY v.Id";

            SqlCommand cmd = new SqlCommand(querry, sqlConnection);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                output.AppendLine($"{reader["Name"]} - {reader["MinionCount"]}");
            }

            reader.Close();

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that prints on the console 
        /// all minion names and ages for a given villain id, ordered by name alphabetically.
        /// If there is no villain with the given ID, print 
        /// "No villain with ID <VillainId> exists in the database.".
        ///If the selected villain has no minions, print "(no minions)" on the second row.
        /// </summary>
        /// <param name="villainId"></param>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 3
        private static string MinionNames(int villainId, SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            string findVilianQuery = @"SELECT [Name] FROM Villains WHERE Id = @villainId";

            SqlCommand findVillianCmd = new SqlCommand(findVilianQuery, sqlConnection);
            findVillianCmd.Parameters.AddWithValue("@villainId", villainId);

            string villianName = (string)findVillianCmd.ExecuteScalar();

            if (villianName == null)
                return $"No villain with ID {villainId} exists in the database.";

            output.AppendLine($"Villain: {villianName}");


            string findMinionsQuery = @"SELECT v.[Name] AS [Villain Name],
		                             m.[Name] AS [Minion Name],
                                     m.Age
		                        FROM Villains AS v
                           LEFT JOIN [MinionsVillains] AS [mv]
	                              ON [v].Id = mv.VillainId
	                            JOIN Minions AS m
		                          ON m.Id = mv.MinionId
			                   WHERE v.Id = @villainId
                            ORDER BY m.Name";

            SqlCommand findMinionsCmd = new SqlCommand(findMinionsQuery, sqlConnection);
            findMinionsCmd.Parameters.AddWithValue("@villainId", villainId);

            using SqlDataReader reader = findMinionsCmd.ExecuteReader();

            if (reader.HasRows)
            {
                int count = 0;
                while (reader.Read())
                {


                    output.AppendLine($"{++count}. {reader["Minion Name"]} {reader["Age"]}");

                    // count++;
                }
            }
            else
            {
                output.AppendLine($"(no minions)");
            }

            reader.Close();

            return output.ToString().Trim();
        }


        /// <summary>
        /// Write a program that reads information about 
        /// a minion and its villain and adds it to the database.
        /// In case the town of the minion is not in the database, 
        /// insert it as well. In case the villain is not present in the database,
        /// add him too with a default evilness factor of "evil".
        /// Finally set the new minion to be a servant of the villain.
        /// Print appropriate messages after each operation.
        /// </summary>
        /// <param name="minionInfo"></param>
        /// <param name="villainInfo"></param>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 4
        private static string AddMinion(string[] minionInfo,
            string[] villainInfo, SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            // Minion's info
            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string minionTown = minionInfo[3];

            //Villain's info
            string villainName = villainInfo[1];

            string findVillainQuerry =
                @"SELECT [Id]
			        FROM Villains
			       WHERE [Name] = @villainName";

            string insertVillainQuerry =
                @"INSERT INTO Villains ([Name], EvilnessFactorId)
				  VALUES (@villainName, 4)";

            string insertTownQuerry =
                @"INSERT INTO Towns ([Name])
				  VALUES (@minionTown)";

            string addMinionQuerry =
                @"INSERT INTO Minions (Name, Age, TownId)
                  VALUES (@name, @age, @townId)";

            string findTownQuerry =
                @"SELECT [Id]
					FROM [Towns]
				   WHERE [Name] = @minionTown";

            string slaveMinionQuerry =
                @"INSERT INTO MinionsVillains (MinionId, VillainId)
                  VALUES (@minionId, @villainId)";

            string findMinionIdQuerry =
                @"SELECT Id FROM Minions WHERE Name = @Name";

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                SqlCommand findTownCmd = new SqlCommand(findTownQuerry, sqlConnection, sqlTransaction);
                findTownCmd.Parameters.AddWithValue("@minionTown", minionTown);

                object townId = findTownCmd.ExecuteScalar();

                if (townId == null)
                {
                    SqlCommand addTownCmd = new SqlCommand(insertTownQuerry, sqlConnection, sqlTransaction);
                    addTownCmd.Parameters.AddWithValue("@minionTown", minionTown);
                    addTownCmd.ExecuteNonQuery();
                    townId = findTownCmd.ExecuteScalar();

                    output.AppendLine($"Town {minionTown} was added to the database.");
                }

                //TODO add minion and the villain

                SqlCommand findVillianCmd = new SqlCommand(findVillainQuerry, sqlConnection, sqlTransaction);
                findVillianCmd.Parameters.AddWithValue("@villainName", villainName);

                object villianId = findVillianCmd.ExecuteScalar();

                if (villianId == null)
                {
                    SqlCommand addVillain = new SqlCommand(insertVillainQuerry, sqlConnection, sqlTransaction);
                    addVillain.Parameters.AddWithValue("@villainName", villainName);
                    addVillain.ExecuteNonQuery();
                    villianId = findVillianCmd.ExecuteScalar();

                    output.AppendLine($"Villain {villainName} was added to the database.");
                }

                SqlCommand addMinion = new SqlCommand(addMinionQuerry, sqlConnection, sqlTransaction);
                addMinion.Parameters.AddWithValue("@name", minionName);
                addMinion.Parameters.AddWithValue("@age", minionAge);
                addMinion.Parameters.AddWithValue("@townId", townId); // warning
                addMinion.ExecuteNonQuery();

                SqlCommand findMinionIdCmd = new SqlCommand(findMinionIdQuerry, sqlConnection, sqlTransaction);
                findMinionIdCmd.Parameters.AddWithValue("@Name", minionName);
                int minionId = (int)findMinionIdCmd.ExecuteScalar();

                SqlCommand addSlave = new SqlCommand(slaveMinionQuerry, sqlConnection, sqlTransaction);
                addSlave.Parameters.AddWithValue("@villainId", villianId);
                addSlave.Parameters.AddWithValue("@minionId", minionId);
                addSlave.ExecuteNonQuery();

                output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

                sqlTransaction.Commit();
            }
            catch (Exception e)
            {

                sqlTransaction.Rollback();
                return e.ToString();
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that changes all town names to uppercase for a given country.
        /// You will receive one line of input with the name of the country.
        /// Print the number of towns that were changed in the format
        /// "<ChangedTownsCount> town names were affected.". On a
        /// second line, print the names that were changed, separated by a comma and a space.
        /// If no towns were affected(the country does not exist in the database
        /// or has no cities connected to it), print "No town names were affected.".
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 5
        private static string changeTownNamesCasing(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            List<string> selectedTowns = new List<string>();

            string countryName = Console.ReadLine();

            string CTNQuerry = @"UPDATE [Towns]
					                SET [Name] = UPPER([Name])
					              WHERE [CountryCode] = 
					            (SELECT [Countries].[Id] FROM [Countries]
                                  WHERE [Name] = @countryName )";

            string selectAllTowns = @"SELECT [t].[Name]
						                FROM [Towns] AS [t]
						                JOIN [Countries] AS c
						                  ON [c].Id = [t].CountryCode
						               WHERE [t].[CountryCode] =
                                     (SELECT [Id] 
                                        FROM [Countries] 
                                       WHERE [Name] = @countryName) ";

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                SqlCommand changeTownNamecmd = new SqlCommand(CTNQuerry, sqlConnection, sqlTransaction);
                changeTownNamecmd.Parameters.AddWithValue("@countryName", countryName);

                int changedTownsCount = (int)changeTownNamecmd.ExecuteNonQuery();

                if (changedTownsCount == 0) return $"No town names were affected.";

                output.AppendLine($"{changedTownsCount} town names were affected.");

                sqlTransaction.Commit();

                SqlCommand selectAllTownsCmd = new SqlCommand(selectAllTowns, sqlConnection, sqlTransaction);
                selectAllTownsCmd.Parameters.AddWithValue("@countryName", countryName);

                using SqlDataReader townReader = selectAllTownsCmd.ExecuteReader();

                while (townReader.Read())
                {
                    selectedTowns.Add(townReader.GetString(0));
                }

                output.AppendLine($"[{string.Join(", ", selectedTowns)}]");

            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that prints all minion names from the minions table in the following order:
        /// first record, last record, first + 1, last - 1, first + 2, last - 2 … first + n, last - n. 
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 7
        private static string printAllMinionNames(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            List<string> minions = new List<string>();

            string getMinionsQuerry = @"SELECT [Name] FROM [Minions]";

            SqlCommand getAllMinionsCmd = new SqlCommand(getMinionsQuerry, sqlConnection);

            using SqlDataReader minionReader = getAllMinionsCmd.ExecuteReader();

            while (minionReader.Read())
            {
                minions.Add(minionReader.GetString(0));
            }

            foreach (string item in minions)
            {
                output.AppendLine(item.ToUpper());
            }
            output.AppendLine();

            string[] minionsArr = minions.ToArray();

            for (int i = 0; i < minionsArr.Length / 2; i++)
            {
                output.AppendLine(minionsArr[i]);
                output.AppendLine(minionsArr[minionsArr.Length - i - 1]);
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Read from the console minion IDs separated by space. 
        /// Increment the age of those minions by 1 and make the title of their name case. 
        /// Finally, print the name and the age of all minions in the database,
        /// each on a new row in the format "<Name> <Age>".
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 8
        private static string increaseMinionAge(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            int[] ids = Console.ReadLine().Split().Select(int.Parse).ToArray();

            string updateAgeQuerry = @"UPDATE [Minions]
							              SET [Age] += 1
						                 FROM [Minions]
							            WHERE Id IN( @ids)";

            string selectMinionsQuerry = @" SELECT [Name], [Age] FROM Minions";

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand updateAgeCommand = new SqlCommand(updateAgeQuerry, sqlConnection, sqlTransaction);

                    updateAgeCommand.Parameters.AddWithValue("@ids", ids[i]);

                    updateAgeCommand.ExecuteNonQuery();
                }

                sqlTransaction.Commit();
            }
            catch (Exception e)
            {

                sqlTransaction.Rollback();
                return e.ToString();
            }

            SqlCommand selectMinionsCmd = new SqlCommand(selectMinionsQuerry, sqlConnection);

            using SqlDataReader minionsReader = selectMinionsCmd.ExecuteReader();

            while (minionsReader.Read())
            {
                output.AppendLine($"{minionsReader["Name"]} {minionsReader["Age"]}");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Create stored procedure usp_GetOlder 
        /// (directly in the database using Management Studio or any other similar tool) 
        /// that receives MinionId and increases that minion's age by 1. 
        /// Write a program that uses that stored procedure to increase 
        /// the age of a minion whose id will be given as input from the console.
        /// After that print the name and the age of that minion.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        // Problem 9
        private static string increaseAgeStoredProcedure(SqlConnection sqlConnection)
        {
            StringBuilder output = new StringBuilder();

            int id = int.Parse(Console.ReadLine());

            string execIncreaseAgeQuerry = @"EXEC[dbo].[usp_GetOlder] @MinionId";

            string selectMinionQuerry = @"SELECT [Name], [Age]
							                FROM [Minions]
							               WHERE Id = @MinionId";

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                SqlCommand execProCmd = new SqlCommand(execIncreaseAgeQuerry, sqlConnection, sqlTransaction);
                execProCmd.Parameters.AddWithValue("@MinionId", id);
                execProCmd.ExecuteNonQuery();

                sqlTransaction.Commit();
            }
            catch (Exception e)
            {
                sqlTransaction.Rollback();
                return e.ToString();
            }

            SqlCommand showMinionCmd = new SqlCommand(selectMinionQuerry, sqlConnection);
            showMinionCmd.Parameters.AddWithValue("@MinionId", id);

            using SqlDataReader minionReader = showMinionCmd.ExecuteReader();

            while (minionReader.Read())
            {
                output.AppendLine($"{minionReader["Name"]} - {minionReader["Age"]} years old");
            }

            return output.ToString();
        }
    }
}
