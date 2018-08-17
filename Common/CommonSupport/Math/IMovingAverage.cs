using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSupport
{
	public interface IMovingAverage
	{
		float Average { get;}

		void AddSample(float val);
		void ClearSamples();
		void InitializeSamples(float val);
	}
}
