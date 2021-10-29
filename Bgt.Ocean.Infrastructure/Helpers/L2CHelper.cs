using System;
using System.ComponentModel;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class L2CHelper
    {
        public static class QuotationType
        {
            public const int Proposal = 1;
            public const int Bid = 2;
            public const int RateRenewal = 3;
            public const int RateRenewalForMass = 4;
        }

        public static class ContractStatus
        {
            public const int Draft = 1;
            public const int ApprovalInProgress = 2;
            public const int Approved = 3;
            public const int Terminated = 4;
            public const int Sent = 5;
            public const int UnderNegotiation = 6;
            public const int Canceled = 7;
            public const int Revised = 8;
            public const int Signed = 9;
        }

        public static class Status
        {
            public const int Draft = 1;
            public const int Published = 2;
        }

        public static bool IsMoney(this string txt)
        {
            decimal n;
            bool isNumeric = decimal.TryParse(txt, out n);
            return isNumeric;
        }

        public enum AdjustRate
        {
            [Description("Increase")]
            Increase = 0,
            [Description("Decrease")]
            Decrease = 1,
        }

        public static string AdjustRateValue(this string value, decimal percentage, decimal amount, int adjustRateTypeID)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            else if (!value.IsMoney())
                return value;

            decimal rate = percentage / 100;
            if (adjustRateTypeID == (int)AdjustRate.Increase)
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = (value.ToDecimal() + amount).ToString();
                }
                else
                {
                    // percent
                    value = IncreaseWithPercentRate(value.ToDecimal(), rate).ToString();
                }
            }
            else
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = (Math.Max(0, value.ToDecimal() - amount)).ToString();
                }
                else
                {
                    // percent
                    value = Math.Max(0, value.ToDecimal() - (value.ToDecimal() * rate)).ToString();
                }
            }
            return value;
        }

        public static decimal? AdjustRateValue(this decimal? value, decimal percentage, decimal amount, int adjustRateTypeID)
        {
            if (!value.HasValue) return null;

            decimal rate = percentage / 100;
            if (adjustRateTypeID == (int)AdjustRate.Increase)
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = value + amount;
                }
                else
                {
                    // percent
                    value = IncreaseWithPercentRate(value.Value, rate);
                }
            }
            else
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = (Math.Max(0, value.Value - amount));
                }
                else
                {
                    // percent
                    value = Math.Max(0, value.Value - (value.Value * rate));
                }
            }
            return value;
        }

        public static decimal AdjustRateValue(this decimal value, decimal percentage, decimal amount, int adjustRateTypeID)
        {
            decimal rate = percentage / 100;
            if (adjustRateTypeID == (int)AdjustRate.Increase)
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = value + amount;
                }
                else
                {
                    // percent
                    value = IncreaseWithPercentRate(value, rate);
                }
            }
            else
            {
                if (percentage == decimal.Zero)
                {
                    // amount
                    value = (Math.Max(0, value - amount));
                }
                else
                {
                    // percent
                    value = Math.Max(0, value - (value * rate));
                }
            }
            return value;
        }

        public static decimal IncreaseWithPercentRate(decimal value, decimal percentRate)
        {
            var result = ((value * (percentRate)) + value);
            return result;
        }
    }
}
