using System;
using System.IO;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using ConsoleTables;

namespace RPSLS
{
    class Program
    {
        static void Main(string[] moves)
        {
            string res = inputCheck(moves);
            if (res != "1")
            {
                Console.WriteLine(res);
                return;
            }
            string key = keyGeneration.getKey();
            int compMove = generateComputerMove(moves.Length - 1);
            string hmac = keyGeneration.calculationOfHmac(moves[compMove], key);
            Console.WriteLine("HMAC key:{0}", hmac);
            printMenu(moves.Length, moves);
            string userMove = userChoise();
            string winner;
            if (userMove == "?")
            {
                Table.table(moves);
                userMove = userChoise();
                winner = determiningTheWinner.whoIsWinner(int.Parse(userMove) - 1, compMove, moves.Length, (moves.Length) / 2);
                Console.WriteLine("Your move: {2} \nComputer move was: {0} \n{1}", moves[compMove], winner, moves[int.Parse(userMove) - 1]);
            }

            else
            {
                if ((int.Parse(userMove) - 1) == 0)
                    return;

                else if (((int.Parse(userMove) - 1) > 0) && ((int.Parse(userMove) - 1) <= moves.Length))
                {
                    winner = determiningTheWinner.whoIsWinner(int.Parse(userMove) - 1, compMove, moves.Length, (moves.Length) / 2);
                    Console.WriteLine("Your move: {2} \nComputer move was: {0} \n{1}", moves[compMove], winner, moves[int.Parse(userMove) - 1]);
                }
            }
            Console.WriteLine("Key: {0}", key);
        }

        static string inputCheck(string[] input)
        {
            string result = "1";
            if (((input.Length) % 2 != 1) | (input.Length <= 1))
            {
                result = "Error! The number of moves must be >1 and be odd.";
            }
            else for (int i = 0; i < input.Length; i++)
                    for (int j = i + 1; j < input.Length; j++)
                        if (input[i] == input[j])
                            result = "Error! Moves must not be repeated. Try again";
            return result;
        }

        public static int generateComputerMove(int maxVal, int minVal = 1)
        {
            var rnd = new byte[4];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);
            var i = Math.Abs(BitConverter.ToInt32(rnd, 0));
            return Convert.ToInt32(i % (maxVal - minVal + 1) + minVal);
        }

        static string userChoise()
        {
            Console.WriteLine("Enter the number corresponding to your choice of move from the ones suggested above:");
            string usMove = Console.ReadLine();
            return usMove;

        }
        static void printMenu(int countOfMoves, string[] words)
        {
            for (int i = 0; i < countOfMoves; i++)
                Console.WriteLine("{0} - {1}", i + 1, words[i]);
            Console.WriteLine("0 - Exit");
            Console.WriteLine("? - Help");
        }

    }

    class Table
    {
        static int win(int a, int b, int n, int kol)
        {
            int res;
            if (a == b)
                res = 0;
            else res = (((((b - a) <= kol) && ((b - a) > 0)) | (((n - a + b) <= kol) && ((n - a + b) <= n))) ? res = b : res = a);
            return res;
        }
        static string[] gettingRow(string[] moves, int a)
        {
            string str = moves[a];
            for (int j = 1; j <= moves.Length - 1; j++)
                str = str + " " + moves[win(a, j, moves.Length - 1, (moves.Length - 1) / 2)];
            string[] st = str.Split(' '); ;
            return st;
        }
        public static void table(string[] moves)
        {
            moves = getString(moves);
            var table = new ConsoleTable(moves);
            for (int i = 1; i <= moves.Length - 1; i++)
                table.AddRow(gettingRow(moves, i));
            table.Write();

        }

        public static string[] getString(string[] moves)
        {
            string m = string.Join(" ", moves);
            m = "X " + m;
            moves = m.Split(" ");
            return moves;
        }
    }
}

class determiningTheWinner
{
    public static string whoIsWinner(int a, int b, int n, int kol)
    {
        string res;
        if (a == b)
            res = "Tie";
        else res = (((((b - a) <= kol) && ((b - a) > 0)) | (((n - a + b) <= kol) && ((n - a + b) <= n))) ? "Computer is winner" : "You are winner!");
        return res;
    }
}

class keyGeneration
{
    public static string getKey()
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] data = new byte[16];
            rng.GetBytes(data);
            string key = BitConverter.ToString(data);
            return key;
        }
    }

    public static string calculationOfHmac(string str, string key)
    {
        byte[] bkey = Encoding.Default.GetBytes(key);
        using (var hmac = new HMACSHA256(bkey))
        {
            byte[] bstr = Encoding.Default.GetBytes(str);
            var bhash = hmac.ComputeHash(bstr);
            return BitConverter.ToString(bhash).Replace("-", string.Empty).ToLower();
        }

    }
}


