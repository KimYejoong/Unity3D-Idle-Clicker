using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public static partial class StringToCurrency
{
    private const string Zero = "0";

    static readonly string[] CurrencyUnits = new string[]
    {
        "",
         "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
        "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
        "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX"
    };


    public static string ToCurrencyString(this double number)
    {

        if (-1d < number && number < 1d)
        {
            return Zero;
        }

        if (true == double.IsInfinity(number))
        {
            return "Infinity";
        }

        string significant = (number < 0) ? "-" : string.Empty;
        string showNumber = string.Empty;
        string unitString = string.Empty;

        string[] partsSplit = number.ToString("E").Split('+');

        if (partsSplit.Length < 2)
        {
            UnityEngine.Debug.LogWarning(string.Format("Failed - ToCurrencyString({0})", number));
            return Zero;
        }

        if (false == int.TryParse(partsSplit[1], out int exponent))
        {
            UnityEngine.Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}", number, partsSplit[1]));
            return Zero;
        }
        int quotient = exponent / 3;
        int remainder = exponent % 3;

        if (exponent < 3)
        {
            showNumber = Mathf.FloorToInt((float)number).ToString(); // To be fixed
        }
        else
        {
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * Mathf.Pow(10, remainder);
            showNumber = temp.ToString("F").Replace(".00", "");
        }

        unitString = CurrencyUnits[quotient];

        return $"{significant}{showNumber}{unitString}";
    }
}



