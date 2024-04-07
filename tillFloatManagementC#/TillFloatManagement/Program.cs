
// ===========================================================
// Author: Mamotsoko Precious Maphutha
// Date: 06/04/2024
// Project: Till Float Management
// ===========================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Transaction
{
    public List<string> Items { get; set; }
    public List<int> PaidAmounts { get; set; }

    public Transaction()
    {
        Items = new List<string>();
        PaidAmounts = new List<int>();
    }
}

class Program
{
    static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines("input.txt");

        List<Transaction> transactions = new List<Transaction>();

        string itemPattern = @"(?<description>.+?)\s*R(?<amount>\d+)";
        string paidAmountPattern = @"R(\d+)";

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');

            if (parts.Length != 2)
            {
                
                Console.WriteLine($"Error: Line '{line}' does not contain proper format. Skipping.");
                continue; 
            }

            string itemsString = parts[0];
            string paidAmountString = parts[1];

            MatchCollection itemMatches = Regex.Matches(itemsString, itemPattern);
            Transaction transaction = new Transaction();

            foreach (Match match in itemMatches)
            {
                string description = match.Groups["description"].Value.Trim();
                string amount = match.Groups["amount"].Value;
                transaction.Items.Add($"{description}: R{amount}");
            }

            MatchCollection paidAmountMatches = Regex.Matches(paidAmountString, paidAmountPattern);
            foreach (Match match in paidAmountMatches)
            {
                string amount = match.Groups[1].Value;
                transaction.PaidAmounts.Add(int.Parse(amount));
            }

            transactions.Add(transaction);
        }

        List<string> results = new List<string>();

        int tillStartAmount = 500; 
        foreach (var transaction in transactions)
        {
        
            Dictionary<int, int> till = new Dictionary<int, int>
            {
                { 50, 5 }, { 20, 5 }, { 10, 6 }, { 5, 12 }, { 2, 10 }, { 1, 10 }
            };

            int totalTransactionCost = transaction.Items.Sum(item => int.Parse(item.Split('R')[1]));
            int totalPaid = transaction.PaidAmounts.Sum();
            int totalChange = totalPaid - totalTransactionCost;

            Dictionary<int, int> changeGiven = new Dictionary<int, int>
            {
                { 50, 0 }, { 20, 0 }, { 10, 0 }, { 5, 0 }, { 2, 0 }, { 1, 0 }
            };

            foreach (var denomination in till.Keys.OrderByDescending(d => d))
            {
                while (totalChange >= denomination && till[denomination] > 0)
                {
                    till[denomination]--;
                    totalChange -= denomination;
                    changeGiven[denomination]++;
                }
            }

            string changeBreakdown = string.Join("-", changeGiven
                .Where(kv => kv.Value > 0)
                .SelectMany(kv => Enumerable.Repeat($"R{kv.Key}", kv.Value)));

            string transactionResult = $"R{tillStartAmount}, R{totalTransactionCost}, R{totalPaid}, R{totalPaid - totalTransactionCost}, {changeBreakdown}";
            results.Add(transactionResult);

            tillStartAmount += totalTransactionCost;
        }

        results.Add($"R{tillStartAmount}");

        string header = "Till Start, Transaction Total, Paid, Change Total, Change Breakdown";
        results.Insert(0, header);

        File.WriteAllLines("output.txt", results);
    }
}