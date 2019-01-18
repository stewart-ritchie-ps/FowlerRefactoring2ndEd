using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Xunit;

namespace FowlerRefactoring2ndEd
{
    [Trait("Chapter", "1. One")]
    public class The_starting_point
    {
        public string Statement(dynamic invoice, dynamic plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = $"Statement for {invoice.Customer}\r\n";

            Func<int, string> format = value => value.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayId];
                var thisAmount = 0;

                switch (play.Type)
                {
                    case "tragedy":
                        thisAmount = 40000;
                        if (perf.Audience > 30)
                        {
                            thisAmount += 1000 * (perf.Audience - 30);
                        }
                        break;
                    case "comedy":
                        thisAmount = 30000;
                        if (perf.Audience > 20)
                        {
                            thisAmount += 10000 + 500 * (perf.Audience - 20);
                        }
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception($"unknown type: {play.Type}");
                }

                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);

                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order
                result += $"  {play.Name}: {format(thisAmount / 100)} ({perf.Audience} seats)\r\n";
                totalAmount += thisAmount;
            }

            result += $"Amount owed is {format(totalAmount / 100)}\r\n";
            result += $"You earned {volumeCredits} credits\r\n";

            return result;
        }

        [Fact(DisplayName = nameof(The_starting_point) + " > " + nameof(BigCo_statement))]
        public void BigCo_statement()
        {
            Statement(Invoices[0], Plays)
                .Should()
                .Be(@"Statement for BigCo
  Hamlet: $650.00 (55 seats)
  As You Like It: $580.00 (35 seats)
  Othello: $500.00 (40 seats)
Amount owed is $1,730.00
You earned 47 credits
");
        }

        private static readonly IDictionary<string, dynamic> Plays = new Dictionary<string, dynamic>
        {
            { "hamlet", new { Name = "Hamlet", Type = "tragedy" } },
            { "as-like", new { Name = "As You Like It", Type = "comedy" } },
            { "othello", new { Name = "Othello", Type = "tragedy" } }
        };

        private static readonly object[] Invoices = 
        {
            new
            {
                Customer = "BigCo",
                Performances = new[]
                {
                    new
                    {
                        PlayId = "hamlet",
                        Audience = 55
                    },
                    new
                    {
                        PlayId = "as-like",
                        Audience = 35
                    },
                    new
                    {
                        PlayId = "othello",
                        Audience = 40
                    },
                }
            }
        };
    }
}
