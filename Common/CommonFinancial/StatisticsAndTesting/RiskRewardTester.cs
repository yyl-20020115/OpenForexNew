using System;

namespace CommonFinancial
{
	/// <summary>
	/// 
	/// </summary>
    public class RiskRewardTester
	{
        double _startingCapital = 10000;
        /// <summary>
        /// The starting capital.
        /// </summary>
        public double StartingCapital
        {
            get { return _startingCapital; }
            set { _startingCapital = value; }
        }

        double _betSizePercentage = 2;
        /// <summary>
        /// How much of the account to risk on each bet.
        /// </summary>
        public double BetSizePercentage
        {
            get { return _betSizePercentage; }
            set { _betSizePercentage = value; }
        }
        
        int _winningChancePercentage = 55;
        /// <summary>
        /// What is the chance to win each bet.
        /// </summary>
        public int WinningChancePercentage
        {
            get { return _winningChancePercentage; }
            set { _winningChancePercentage = value; }
        }

        int _rounds = 100;
        public int Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        bool _useVariableBetSize = false;
        /// <summary>
        /// Should the bet size depend on current account size or be fixed as to account size in the beggining.
        /// </summary>
        public bool UseVariableBetSize
        {
            get { return _useVariableBetSize; }
            set { _useVariableBetSize = value; }
        }


        double _minimumCapital = 10;
        
        /// <summary>
        /// When capital drops below this boundary, account is considered busted.
        /// </summary>
        public double MinimumCapital
        {
            get { return _minimumCapital; }
            set { _minimumCapital = value; }
        }

        System.Random _random = new Random();
        
        /// <summary>
        /// 
        /// </summary>
        public RiskRewardTester()
		{
		}

        /// <summary>
        /// 
        /// </summary>
        double CalculateBetSize(double capital)
        {
            if (UseVariableBetSize)
            {
                return capital * BetSizePercentage / 100;
            }
            else
            {
                return StartingCapital * BetSizePercentage / 100;
            }
        }


        /// <summary>
        /// 
        /// </summary>
		public double RunTest()
		{
			double capital = StartingCapital;

			for(int i=0; i<Rounds && capital > 0; i++)
            {
                int value = _random.Next(1, 101);
                if (WinningChancePercentage >= value)
				{
                    capital += CalculateBetSize(capital);
				}
				else
				{
                    capital -= CalculateBetSize(capital);
				}

                if (capital < MinimumCapital)
                {
                    capital = 0;
                    break;
                }
			}

			return capital;
		}


	}
}
