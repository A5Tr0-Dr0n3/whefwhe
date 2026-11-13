using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UsernameGenerator
{
	public const int numUsernameDigits = 8;

	public const string defaultUsername = "727108-R1";

	private const string usernamePrefix = "0-HN-";

	public static string GenerateUsername()
	{
		string empty = string.Empty;
		empty += getRandomNumberString(6);
		empty += "-";
		empty += getRandomLetterString(1, capital: true);
		return empty + getRandomNumberString(1);
	}

	public static string GetPrefixedUsername(string coreUsername)
	{
		return "0-HN-" + coreUsername;
	}

	public static string GetRandomDigitedUsername(string baseUsername, List<int> randomizedIndices, string lastGenerated)
	{
		if (string.IsNullOrEmpty(lastGenerated))
		{
			lastGenerated = GenerateUsername();
		}
		StringBuilder stringBuilder = new StringBuilder(lastGenerated);
		for (int i = 0; i < 6; i++)
		{
			stringBuilder[i] = ((!randomizedIndices.Contains(i)) ? baseUsername[i] : getNewRandomNumberChar(lastGenerated[0]));
		}
		stringBuilder[6] = '-';
		stringBuilder[7] = ((!randomizedIndices.Contains(6)) ? baseUsername[7] : getNewRandomLetterChar(lastGenerated[7]));
		stringBuilder[8] = ((!randomizedIndices.Contains(7)) ? baseUsername[8] : getNewRandomNumberChar(lastGenerated[8]));
		return stringBuilder.ToString();
	}

	private static char getNewRandomNumberChar(char oldDigit)
	{
		char c = (char)(48 + (oldDigit - 48 + Random.Range(0, 8)) % 10);
		if (oldDigit == c)
		{
			int num = 0;
			num++;
			num--;
		}
		return c;
	}

	private static char getNewRandomLetterChar(char oldAlpha)
	{
		char c = (char)(65 + (oldAlpha - 65 + Random.Range(0, 24)) % 26);
		if (oldAlpha == c)
		{
			int num = 0;
			num++;
			num--;
		}
		return c;
	}

	private static string getRandomLetterString(int count, bool capital)
	{
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			char c = (char)(97 + Random.Range(0, 26));
			if (capital)
			{
				c = (c + string.Empty).ToUpper()[0];
			}
			text += c;
		}
		return text;
	}

	private static string getRandomNumberString(int count)
	{
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			char c = (char)(48 + Random.Range(0, 10));
			text += c;
		}
		return text;
	}
}
