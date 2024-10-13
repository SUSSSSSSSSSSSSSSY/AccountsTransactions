namespace AccountsTransactions
{
    using System;
    using System.Data;
    using Microsoft.Data.SqlClient;

    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-9PK656A\\SQLEXPRESS;Database=AccountsDB;Trusted_Connection=True;TrustServerCertificate=True";
            int fromAccountId = 10000;
            int toAccountId = 10001;
            decimal transferAmount = 1000.00m;
            string moneyAmmountfrom = "";
            string moneyAmmountto = "";


            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            moneyAmmountfrom = Convert.ToString(GetBalance(fromAccountId, conn, transaction));
            moneyAmmountto = Convert.ToString(GetBalance(toAccountId, conn, transaction));

            transaction.Commit();

            Console.WriteLine("Source account money: " + moneyAmmountfrom + "\n");
            Console.WriteLine("Target account money: " + moneyAmmountto + "\n");



            Console.WriteLine("\n");
            TransferFunds(fromAccountId, toAccountId, transferAmount, connectionString);
            Console.WriteLine("\n");



            transaction = conn.BeginTransaction();

            moneyAmmountfrom = Convert.ToString(GetBalance(fromAccountId, conn, transaction));
            moneyAmmountto = Convert.ToString(GetBalance(toAccountId, conn, transaction));

            transaction.Commit();
            conn.Close();

            Console.WriteLine("Source account money: " + moneyAmmountfrom + "\n");
            Console.WriteLine("Target account money: " + moneyAmmountto + "\n");
        }

        static void TransferFunds(int fromAccountId, int toAccountId, decimal amount, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    decimal fromBalance = GetBalance(fromAccountId, conn, transaction);
                    if (fromBalance < amount)
                    {
                        throw new InvalidOperationException("Недостаточно средств для перевода.");
                    }

                    UpdateBalance(fromAccountId, -amount, conn, transaction);


                    UpdateBalance(toAccountId, amount, conn, transaction);

                    transaction.Commit();
                    Console.WriteLine("Перевод выполнен успешно.\n");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка: {ex.Message}. Транзакция отменена.");
                }

                conn.Close();
            }
        }

        static decimal GetBalance(int accountId, SqlConnection conn, SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT Balance FROM Accounts WHERE AccountID = @AccountID", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AccountID", accountId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }

        static void UpdateBalance(int accountId, decimal amount, SqlConnection conn, SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountID = @AccountID", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@AccountID", accountId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
