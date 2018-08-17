



// Class supports exporting stopwatch results for external analysis and calculation.
class ExternalStopwatch
{
public:
	
	// Constructor.
	ExternalStopwatch()
	{
	}

	// Destructor.
	~ExternalStopwatch()
	{
	}

	static void SendMeasure()
	{
		LARGE_INTEGER output, output2;
		//__int64 freq = 2666700000;

		QueryPerformanceCounter((LARGE_INTEGER*)&output);
		
		//double outputProccessed = output / freq;
		SendMessage((HWND)3278846, 9999, output.HighPart, output.LowPart);

		//sprintf(text, "%I64d", output);
		//x.append(text);
		//x.append("\r\n");
		//OutputDebugString(x.c_str());

		QueryPerformanceCounter((LARGE_INTEGER*)&output2);
		SendMessage((HWND)3278846, 9998, output2.HighPart, output2.LowPart);

		//x = ">> ";
		//sprintf(text, "%I64d", diff);
		//x.append(text);
		//x.append("\r\n");
		//OutputDebugString(x.c_str());

		//if (QueryPerformanceCounter(&output))
		//{
		//	CHAR text[512] = { 0 };
		//	sprintf(text, "%I64d", output); // or
		//	std::string x = ">> ";
		//	x.append(text);
		//	x.append("\r\n");
		//	OutputDebugString(x.c_str());
		//}
	}
};